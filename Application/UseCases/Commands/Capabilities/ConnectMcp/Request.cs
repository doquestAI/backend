using Domain.Capabilities;
using MediatR;

namespace Application.UseCases.Commands.Capabilities.ConnectMcp;

/// <summary>
/// Conecta um servidor MCP. Para StdIo, <paramref name="Endpoint"/> deve ser
/// codificado como <c>cmd|arg1|arg2</c>. Para Sse/Http, deve ser uma URL absoluta.
/// </summary>
public record Request(string Name, McpTransport Transport, string Endpoint) : IRequest<Response>;
