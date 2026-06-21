using MediatR;

namespace Application.UseCases.Commands.Subscription.ActivateUser;

internal record Request(string EntraUserId) : IRequest<Response>;
