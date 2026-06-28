namespace Domain.Capabilities;

/// <summary>Transporte usado pelo cliente MCP para falar com o servidor.</summary>
public enum McpTransport
{
    StdIo = 0,
    Sse = 1,
    Http = 2,
}
