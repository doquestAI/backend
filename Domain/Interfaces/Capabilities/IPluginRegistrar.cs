namespace Domain.Interfaces.Capabilities;

/// <summary>
/// Lado de escrita do registro de plugins — habilita Use Cases da Application a
/// registrar/desabilitar plugins lógicos sem conhecer a concretude da AI.
/// Funções concretas (<c>AIFunction</c>) são provisionadas via DI no Composition Root.
/// </summary>
internal interface IPluginRegistrar
{
    void RegisterEmpty(string name, string description, bool enabled = true);
}
