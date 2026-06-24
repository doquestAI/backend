using Microsoft.OpenApi;
using Newtonsoft.Json.Linq;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Linq;

namespace Presentation.Common.Filters;

/// <summary>
/// Filtro que mescla documentação Swagger de serviços externos no documento principal.
/// Busca o Swagger do serviço RAG e adiciona seus endpoints com prefixo /api/rag
/// </summary>
internal class ExternalSwaggerDocumentFilter(
    IHttpClientFactory httpClientFactory,
    IConfiguration configuration,
    ILogger<ExternalSwaggerDocumentFilter> logger) : IDocumentFilter
{
    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
    private readonly IConfiguration _configuration = configuration;
    private readonly ILogger<ExternalSwaggerDocumentFilter> _logger = logger;

    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        var ragServiceUrl = _configuration["ReverseProxy:Clusters:rag-service:Destinations:destination1:Address"];

        if (string.IsNullOrEmpty(ragServiceUrl))
        {
            _logger.LogWarning("URL do serviço RAG não configurada em ReverseProxy:Clusters:rag-service");
            return;
        }

        try
        {
            var client = _httpClientFactory.CreateClient();
            client.Timeout = TimeSpan.FromSeconds(5);

            var externalSwaggerUrl = $"{ragServiceUrl}/api/docs-json";
            _logger.LogInformation("Buscando Swagger externo de: {Url}", externalSwaggerUrl);

            var response = client.GetAsync(externalSwaggerUrl).Result;

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Não foi possível obter Swagger externo. Status: {StatusCode}", response.StatusCode);
                return;
            }

            var content = response.Content.ReadAsStringAsync().Result;
            var externalSwaggerJson = JObject.Parse(content);

            MergePaths(swaggerDoc, externalSwaggerJson);
            MergeSchemas(swaggerDoc, externalSwaggerJson);
            MergeTags(swaggerDoc, externalSwaggerJson);

            _logger.LogInformation("Swagger externo mesclado com sucesso");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao mesclar Swagger externo: {Message}", ex.Message);
        }
    }

    private void MergePaths(OpenApiDocument swaggerDoc, JObject externalSwagger)
    {
        var paths = externalSwagger["paths"] as JObject;
        if (paths == null)
        {
            _logger.LogWarning("Nenhum path encontrado no Swagger externo");
            return;
        }

        _logger.LogInformation("Encontrados {Count} paths no Swagger externo", paths.Count);

        foreach (var pathItem in paths)
        {
            var originalPath = pathItem.Key;
            var newPath = $"/api/rag{originalPath}";

            _logger.LogDebug("Processando path: {OriginalPath} -> {NewPath}", originalPath, newPath);

            if (swaggerDoc.Paths.ContainsKey(newPath))
            {
                _logger.LogWarning("Path {Path} já existe, ignorando", newPath);
                continue;
            }

            try
            {
                // Converter JObject para OpenApiPathItem usando serialização/desserialização
                var pathJson = pathItem.Value.ToString();

                _logger.LogDebug("JSON do path {Path}: {Json}", originalPath, pathJson.Length > 200 ? pathJson.Substring(0, 200) + "..." : pathJson);

                var openApiPath = ConvertJsonToOpenApiPathItem(pathJson);

                if (openApiPath != null)
                {
                    swaggerDoc.Paths.Add(newPath, openApiPath);
                    _logger.LogInformation("✓ Path adicionado com sucesso: {Path}", newPath);
                }
                else
                {
                    _logger.LogWarning("✗ ConvertJsonToOpenApiPathItem retornou null para {Path}", originalPath);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "✗ Erro ao converter path {Path}: {Message}", originalPath, ex.Message);
            }
        }

        _logger.LogInformation("Total de paths mesclados: {Count}", swaggerDoc.Paths.Count(p => p.Key.StartsWith("/api/rag")));
    }

    private void MergeSchemas(OpenApiDocument swaggerDoc, JObject externalSwagger)
    {
        var schemas = externalSwagger["components"]?["schemas"] as JObject;
        if (schemas == null)
            return;

        foreach (var schema in schemas)
        {
            var schemaName = schema.Key;

            if (swaggerDoc.Components?.Schemas.ContainsKey(schemaName) == true)
            {
                _logger.LogDebug("Schema {Schema} já existe, ignorando", schemaName);
                continue;
            }

            try
            {
                var schemaJObject = schema.Value as JObject;
                var openApiSchema = BuildOpenApiSchema(schemaJObject);

                if (openApiSchema != null)
                {
                    swaggerDoc.Components ??= new OpenApiComponents();
                    swaggerDoc.Components.Schemas ??= new Dictionary<string, IOpenApiSchema>();
                    swaggerDoc.Components.Schemas.Add(schemaName, openApiSchema);
                    _logger.LogDebug("Schema adicionado: {Schema}", schemaName);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Erro ao converter schema {Schema}: {Message}", schemaName, ex.Message);
            }
        }
    }

    private void MergeTags(OpenApiDocument swaggerDoc, JObject externalSwagger)
    {
        var tags = externalSwagger["tags"] as JArray;
        if (tags == null)
            return;

        foreach (var tag in tags)
        {
            var tagName = tag["name"]?.ToString();
            if (string.IsNullOrEmpty(tagName))
                continue;

            if (swaggerDoc.Tags?.Any(t => t.Name == tagName) == true)
            {
                _logger.LogDebug("Tag {Tag} já existe, ignorando", tagName);
                continue;
            }

            var openApiTag = new OpenApiTag
            {
                Name = tagName,
                Description = tag["description"]?.ToString()
            };

            swaggerDoc.Tags ??= new HashSet<OpenApiTag>();
            swaggerDoc.Tags.Add(openApiTag);
            _logger.LogDebug("Tag adicionada: {Tag}", tagName);
        }
    }

    private OpenApiPathItem? ConvertJsonToOpenApiPathItem(string json)
    {
        try
        {
            _logger.LogDebug("Iniciando conversão de PathItem");

            // Parse JSON manualmente para ter mais controle
            var pathJson = JObject.Parse(json);
            var pathItem = new OpenApiPathItem
            {
                // Inicializar o dicionário de operações
                Operations = new Dictionary<System.Net.Http.HttpMethod, OpenApiOperation>()
            };

            // Processar cada operação HTTP (get, post, put, delete, patch)
            var httpMethods = new[] { "get", "post", "put", "delete", "patch", "options", "head", "trace" };

            foreach (var method in httpMethods)
            {
                var operationJson = pathJson[method] as JObject;
                if (operationJson == null)
                    continue;

                var operation = BuildOpenApiOperation(operationJson);
                if (operation != null)
                {
                    // Criar HttpMethod para adicionar ao pathItem
                    var httpMethod = method.ToLowerInvariant() switch
                    {
                        "get" => System.Net.Http.HttpMethod.Get,
                        "post" => System.Net.Http.HttpMethod.Post,
                        "put" => System.Net.Http.HttpMethod.Put,
                        "delete" => System.Net.Http.HttpMethod.Delete,
                        "patch" => System.Net.Http.HttpMethod.Patch,
                        "options" => System.Net.Http.HttpMethod.Options,
                        "head" => System.Net.Http.HttpMethod.Head,
                        "trace" => System.Net.Http.HttpMethod.Trace,
                        _ => null
                    };

                    if (httpMethod != null && !pathItem.Operations.ContainsKey(httpMethod))
                    {
                        pathItem.Operations[httpMethod] = operation;
                        _logger.LogDebug("Operação {Method} adicionada: {Summary}", method.ToUpper(), operation.Summary);
                    }
                }
            }

            return pathItem;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao converter PathItem: {Message}", ex.Message);
            return null;
        }
    }

    private OpenApiOperation? BuildOpenApiOperation(JObject operationJson)
    {
        try
        {
            var operation = new OpenApiOperation
            {
                Summary = operationJson["summary"]?.ToString(),
                Description = operationJson["description"]?.ToString(),
                OperationId = operationJson["operationId"]?.ToString(),
                // Inicializar coleções para evitar NullReferenceException
                Tags = new HashSet<OpenApiTagReference>(),
                Parameters = new List<IOpenApiParameter>(),
                Responses = new OpenApiResponses()
            };

            // Tags
            if (operationJson["tags"] is JArray tags)
            {
                foreach (var tag in tags)
                {
                    var tagName = tag.ToString();
                    var tagRef = new OpenApiTagReference(tagName, null, null);
                    operation.Tags.Add(tagRef);
                }
            }

            // Parameters
            if (operationJson["parameters"] is JArray parameters)
            {
                foreach (var param in parameters)
                {
                    var parameter = BuildOpenApiParameter(param as JObject);
                    if (parameter != null)
                    {
                        operation.Parameters.Add(parameter);
                    }
                }
            }

            // Request Body
            if (operationJson["requestBody"] is JObject requestBody)
            {
                operation.RequestBody = BuildOpenApiRequestBody(requestBody);
            }

            // Responses
            if (operationJson["responses"] is JObject responses)
            {
                foreach (var response in responses)
                {
                    var resp = BuildOpenApiResponse(response.Value as JObject);
                    if (resp != null)
                    {
                        operation.Responses.Add(response.Key, resp);
                    }
                }
            }

            return operation;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Erro ao construir OpenApiOperation");
            return null;
        }
    }

    private OpenApiParameter? BuildOpenApiParameter(JObject? paramJson)
    {
        if (paramJson == null)
            return null;

        var parameter = new OpenApiParameter();
        parameter.Name = paramJson["name"]?.ToString() ?? "";
        parameter.In = ParseParameterLocation(paramJson["in"]?.ToString());
        parameter.Description = paramJson["description"]?.ToString();
        parameter.Required = paramJson["required"]?.ToObject<bool>() ?? false;

        // Schema do parâmetro
        var schemaJson = paramJson["schema"] as JObject;
        if (schemaJson != null)
        {
            parameter.Schema = BuildOpenApiSchema(schemaJson);
        }

        return parameter;
    }

    private OpenApiRequestBody? BuildOpenApiRequestBody(JObject? requestBodyJson)
    {
        if (requestBodyJson == null)
            return null;

        var reqBody = new OpenApiRequestBody
        {
            Required = requestBodyJson["required"]?.ToObject<bool>() ?? false,
            Description = requestBodyJson["description"]?.ToString(),
            Content = new Dictionary<string, OpenApiMediaType>()
        };

        // Content
        if (requestBodyJson["content"] is JObject contentJson)
        {
            foreach (var contentItem in contentJson)
            {
                var mediaType = BuildOpenApiMediaType(contentItem.Value as JObject);
                if (mediaType != null)
                {
                    reqBody.Content.Add(contentItem.Key, mediaType);
                }
            }
        }

        return reqBody;
    }

    private OpenApiResponse? BuildOpenApiResponse(JObject? responseJson)
    {
        if (responseJson == null)
            return null;

        var response = new OpenApiResponse
        {
            Description = responseJson["description"]?.ToString() ?? "Success",
            Content = new Dictionary<string, OpenApiMediaType>()
        };

        // Content
        if (responseJson["content"] is JObject contentJson)
        {
            foreach (var contentItem in contentJson)
            {
                var mediaType = BuildOpenApiMediaType(contentItem.Value as JObject);
                if (mediaType != null)
                {
                    response.Content.Add(contentItem.Key, mediaType);
                }
            }
        }

        return response;
    }

    private OpenApiMediaType? BuildOpenApiMediaType(JObject? mediaTypeJson)
    {
        if (mediaTypeJson == null)
            return null;

        var mediaType = new OpenApiMediaType();

        var schemaJson = mediaTypeJson["schema"] as JObject;
        if (schemaJson != null)
        {
            mediaType.Schema = BuildOpenApiSchema(schemaJson);
        }

        return mediaType;
    }

    private static IOpenApiSchema? BuildOpenApiSchema(JObject? schemaJson)
    {
        if (schemaJson == null)
            return null;

        // Se for uma referência ($ref), criar OpenApiSchemaReference
        var refValue = schemaJson["$ref"]?.ToString();
        if (!string.IsNullOrEmpty(refValue))
        {
            // Extrair o nome do schema da referência: "#/components/schemas/QueryRAGDto" -> "QueryRAGDto"
            var schemaName = refValue.Split('/').Last();
            return new OpenApiSchemaReference(schemaName, null);
        }

        var schema = new OpenApiSchema
        {
            // Inicializar coleções para evitar NullReferenceException
            Required = new HashSet<string>(),
            Enum = new List<System.Text.Json.Nodes.JsonNode>(),
            Properties = new Dictionary<string, IOpenApiSchema>(),
            AllOf = new List<IOpenApiSchema>()
        };

        // Type
        var typeValue = schemaJson["type"]?.ToString();
        if (!string.IsNullOrEmpty(typeValue))
        {
            schema.Type = ParseJsonSchemaType(typeValue);
        }

        // Format
        schema.Format = schemaJson["format"]?.ToString();

        // Description
        schema.Description = schemaJson["description"]?.ToString();

        // Example - usar JsonNode para representar valores JSON
        var exampleValue = schemaJson["example"];
        if (exampleValue != null && exampleValue.Type != JTokenType.Null)
        {
            schema.Example = ConvertJTokenToJsonNode(exampleValue);
        }

        // Default
        var defaultValue = schemaJson["default"];
        if (defaultValue != null && defaultValue.Type != JTokenType.Null)
        {
            schema.Default = ConvertJTokenToJsonNode(defaultValue);
        }

        // Enum
        if (schemaJson["enum"] is JArray enumValues)
        {
            foreach (var enumValue in enumValues)
            {
                var jsonNode = ConvertJTokenToJsonNode(enumValue);
                if (jsonNode != null)
                {
                    schema.Enum.Add(jsonNode);
                }
            }
        }

        // Required (lista de campos obrigatórios)
        if (schemaJson["required"] is JArray requiredFields)
        {
            foreach (var field in requiredFields)
            {
                schema.Required.Add(field.ToString());
            }
        }

        // Minimum/Maximum - são strings no OpenAPI 2.x
        if (schemaJson["minimum"] != null)
            schema.Minimum = schemaJson["minimum"]?.ToString();
        if (schemaJson["maximum"] != null)
            schema.Maximum = schemaJson["maximum"]?.ToString();

        // MinLength/MaxLength
        if (schemaJson["minLength"] != null)
            schema.MinLength = schemaJson["minLength"]?.ToObject<int>();
        if (schemaJson["maxLength"] != null)
            schema.MaxLength = schemaJson["maxLength"]?.ToObject<int>();

        // MinItems/MaxItems (para arrays)
        if (schemaJson["minItems"] != null)
            schema.MinItems = schemaJson["minItems"]?.ToObject<int>();
        if (schemaJson["maxItems"] != null)
            schema.MaxItems = schemaJson["maxItems"]?.ToObject<int>();

        // Properties (para objetos)
        if (schemaJson["properties"] is JObject propertiesJson)
        {
            foreach (var prop in propertiesJson)
            {
                var propSchema = BuildOpenApiSchema(prop.Value as JObject);
                if (propSchema != null)
                {
                    schema.Properties.Add(prop.Key, propSchema);
                }
            }
        }

        // Items (para arrays)
        if (schemaJson["items"] is JObject itemsJson)
        {
            schema.Items = BuildOpenApiSchema(itemsJson);
        }

        // AllOf
        if (schemaJson["allOf"] is JArray allOfJson)
        {
            foreach (var item in allOfJson)
            {
                var itemSchema = BuildOpenApiSchema(item as JObject);
                if (itemSchema != null)
                {
                    schema.AllOf.Add(itemSchema);
                }
            }
        }

        // AdditionalProperties
        var additionalPropsJson = schemaJson["additionalProperties"];
        if (additionalPropsJson != null && additionalPropsJson.Type == JTokenType.Object)
        {
            schema.AdditionalProperties = BuildOpenApiSchema(additionalPropsJson as JObject);
        }

        return schema;
    }

    private static System.Text.Json.Nodes.JsonNode? ConvertJTokenToJsonNode(JToken token)
    {
        if (token == null || token.Type == JTokenType.Null)
            return null;

        var jsonString = token.ToString(Newtonsoft.Json.Formatting.None);
        return System.Text.Json.Nodes.JsonNode.Parse(jsonString);
    }

    private static JsonSchemaType? ParseJsonSchemaType(string? type)
    {
        return type?.ToLowerInvariant() switch
        {
            "string" => JsonSchemaType.String,
            "number" => JsonSchemaType.Number,
            "integer" => JsonSchemaType.Integer,
            "boolean" => JsonSchemaType.Boolean,
            "array" => JsonSchemaType.Array,
            "object" => JsonSchemaType.Object,
            "null" => JsonSchemaType.Null,
            _ => null
        };
    }

    private static Microsoft.OpenApi.ParameterLocation? ParseParameterLocation(string? location)
    {
        return location?.ToLowerInvariant() switch
        {
            "query" => Microsoft.OpenApi.ParameterLocation.Query,
            "header" => Microsoft.OpenApi.ParameterLocation.Header,
            "path" => Microsoft.OpenApi.ParameterLocation.Path,
            "cookie" => Microsoft.OpenApi.ParameterLocation.Cookie,
            _ => null
        };
    }

}