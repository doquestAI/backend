using MediatR;

namespace Application.UseCases.Queries.Capabilities.GetCapabilities;

public record Request : IRequest<Response>;
