using MediatR;

namespace Application.UseCases.Commands.Capabilities.RegisterPlugin;

/// <summary>
/// Registra um plugin lógico no <c>PluginRegistry</c>. As funções concretas
/// (<c>AIFunction</c>) precisam ser provisionadas via DI na camada de Composition Root —
/// este use case apenas reconhece o plugin como habilitado/desabilitado.
/// </summary>
internal record Request(string Name, string Description, bool Enabled = true) : IRequest<Response>;
