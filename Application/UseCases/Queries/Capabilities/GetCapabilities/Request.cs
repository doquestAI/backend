using MediatR;

namespace Application.UseCases.Queries.Capabilities.GetCapabilities;

internal record Request : IRequest<Response>;
