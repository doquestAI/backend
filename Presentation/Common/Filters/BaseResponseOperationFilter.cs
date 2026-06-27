using Domain.Common.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Presentation.Filters;

public class BaseResponseOperationFilter
    : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var hasAttribute = context.MethodInfo.DeclaringType?
            .GetCustomAttributes(typeof(Common.ProducesBaseResponseAttribute), true).Any() ?? false;

        if (!hasAttribute)
            return;

        var hasCustomResponses = context.MethodInfo
            .GetCustomAttributes(typeof(ProducesResponseTypeAttribute), false)
            .Any();

        if (hasCustomResponses)
            return;

        var returnType = context.MethodInfo.ReturnType;
        Type? baseResponseType = null;

        if (returnType.IsGenericType)
        {
            var taskType = returnType.GetGenericArguments().FirstOrDefault();
            if (taskType?.IsGenericType == true)
            {
                var actionResultType = taskType.GetGenericArguments().FirstOrDefault();
                if (actionResultType is not null)
                {
                    if (actionResultType == typeof(BaseResponse))
                    {
                        baseResponseType = actionResultType;
                    }
                    else if (actionResultType.IsGenericType &&
                             actionResultType.GetGenericTypeDefinition() == typeof(BaseResponse))
                    {
                        baseResponseType = actionResultType;
                    }
                }
            }
        }

        if (baseResponseType == null)
            return;

        var successCode = GetSuccessStatusCode(context);

        AddResponse(operation, context, successCode.ToString(), baseResponseType);

        AddResponse(operation, context, "400", typeof(BaseResponse));
        AddResponse(operation, context, "500", typeof(BaseResponse));

        if (context.MethodInfo.Name.Contains("Login", StringComparison.OrdinalIgnoreCase))
        {
            AddResponse(operation, context, "403", typeof(BaseResponse));
        }
    }

    private static void AddResponse(OpenApiOperation operation, OperationFilterContext context,
        string statusCode, Type responseType)
    {
        if (operation.Responses.ContainsKey(statusCode))
            return;

        operation.Responses.Add(statusCode, new OpenApiResponse
        {
            Description = GetDescription(statusCode),
            Content = new Dictionary<string, OpenApiMediaType>
            {
                ["application/json"] = new OpenApiMediaType
                {
                    Schema = context.SchemaGenerator.GenerateSchema(responseType, context.SchemaRepository)
                }
            }
        });
    }

    private static int GetSuccessStatusCode(OperationFilterContext context)
    {
        var httpMethodAttr = context.MethodInfo
            .GetCustomAttributes(typeof(HttpMethodAttribute), false)
            .FirstOrDefault() as HttpMethodAttribute;

        var httpMethod = httpMethodAttr?.HttpMethods.FirstOrDefault();
        var methodName = context.MethodInfo.Name;

        return httpMethod switch
        {
            "POST" when methodName.Contains("Register", StringComparison.OrdinalIgnoreCase) => 200,
            "POST" => 200,
            "PUT" => 200,
            "DELETE" => 204,
            _ => 200
        };
    }

    private static string GetDescription(string code) => code switch
    {
        "200" => "Success",
        "201" => "Created",
        "204" => "No Content",
        "400" => "Bad Request",
        "403" => "Forbidden",
        "404" => "Not Found",
        "500" => "Internal Server Error",
        _ => "Response"
    };
}