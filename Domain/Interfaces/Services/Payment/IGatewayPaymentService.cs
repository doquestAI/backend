namespace Domain.Interfaces.Services.Payment;

internal interface IGatewayPaymentService
{
    Task<string> CreateCheckoutSessionAsync(string userEmail, string stripePriceId,
        string successUrl, string cancelUrl, CancellationToken cancellationToken = default);
    Task<string?> GetProductIdFromEventAsync(string webhookPayload,
        string stripeSignature, CancellationToken cancellationToken = default);
}