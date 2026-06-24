using MediatR;

namespace Application.UseCases.Queries.Subscription.GetSubscriptionStatus;

internal record Request(string EntraUserId) : IRequest<Response>;