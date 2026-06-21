using MediatR;

namespace Application.UseCases.Commands.Subscription.DeactivateUser;

internal record Request(string EntraUserId) : IRequest<Response>;
