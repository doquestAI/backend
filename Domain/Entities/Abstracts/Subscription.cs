using Domain.ValueObjects;
using Flunt.Validations;

namespace Domain.Entities.Abstracts;

internal abstract class Subscription : Entity
{
    public Guid StudentId { get; private set; }
    public SubscriptionPeriod Period { get; private set; }
    public PaymentDetails? Payment { get; private set; }

    protected Subscription(Guid studentId, SubscriptionPeriod period, PaymentDetails? payment = null)
    {
        StudentId = studentId;
        Period = period;
        Payment = payment;

        AddNotificationsFromValueObjects(period, payment);
        AddNotifications(new Contract<Subscription>()
            .IsNotNull(studentId, "StudentId", "Estudante obrigatório")
            .IsNotNull(Period, "Period", "Período inválido")
        );
    }
}