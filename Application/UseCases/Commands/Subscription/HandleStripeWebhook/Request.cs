using MediatR;

namespace Application.UseCases.Commands.Subscription.HandleStripeWebhook;

internal record Request(string RawPayload, string StripeSignatureHeader) : IRequest<Response>;
