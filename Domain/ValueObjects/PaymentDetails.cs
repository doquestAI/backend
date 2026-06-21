using Flunt.Notifications;
using Flunt.Validations;

namespace Domain.ValueObjects;

internal class PaymentDetails : BaseValueObject
{
    public string PaymentGateway { get; private set; }
    public string TransactionId { get; private set; }

    private PaymentDetails() { }
    public PaymentDetails(string gateway, string transactionId)
    {
        PaymentGateway = gateway;
        TransactionId = transactionId;

        AddNotifications(new Contract<Notification>()
            .Requires()
            .IsNotNullOrEmpty(PaymentGateway, "PaymentDetails.PaymentGateway", "Gateway de pagamento é obrigatório")
            .IsNotNullOrEmpty(TransactionId, "PaymentDetails.TransactionId", "ID da transação é obrigatório")
        );
    }
}