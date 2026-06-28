namespace Domain.Capabilities;

/// <summary>Origem de uma function call disponível ao Agent.</summary>
public enum FunctionCallSource
{
    Plugin = 0,
    Mcp = 1,
    KernelFunction = 2,
    Native = 3,
}
