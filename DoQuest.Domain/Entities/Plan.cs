using Flunt.Notifications;
using Flunt.Validations;
using DoQuest.Domain.Enums;

namespace DoQuest.Domain.Entities;

public class Plan : Notifiable<Notification>
{
    private Plan() { }

    public Guid Id { get; private set; }
    public PlanType Type { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public int DailyMessageLimit { get; private set; } // -1 = unlimited
    public int DailyTokenLimit { get; private set; }   // -1 = unlimited
    public string StripeProductId { get; private set; } = string.Empty;

    // Well-known seeded GUIDs (stable across environments)
    public static readonly Guid FreePlanId = new("a1b2c3d4-0001-0001-0001-000000000001");
    public static readonly Guid ProPlanId  = new("a1b2c3d4-0002-0002-0002-000000000002");
    public static readonly Guid MaxPlanId  = new("a1b2c3d4-0003-0003-0003-000000000003");

    public static Plan Create(
        Guid id,
        PlanType type,
        string name,
        int dailyMessageLimit,
        int dailyTokenLimit,
        string stripeProductId)
    {
        var plan = new Plan();

        plan.AddNotifications(new Contract<Plan>()
            .Requires()
            .IsNotNullOrEmpty(name, nameof(Name), "Nome do plano é obrigatório")
            .IsGreaterOrEqualsThan(dailyMessageLimit, -1, nameof(DailyMessageLimit), "Limite de mensagens inválido (mínimo -1)")
            .IsGreaterOrEqualsThan(dailyTokenLimit, -1, nameof(DailyTokenLimit), "Limite de tokens inválido (mínimo -1)")
            .IsNotNullOrEmpty(stripeProductId, nameof(StripeProductId), "Stripe Product ID é obrigatório"));

        if (plan.IsValid)
        {
            plan.Id = id;
            plan.Type = type;
            plan.Name = name;
            plan.DailyMessageLimit = dailyMessageLimit;
            plan.DailyTokenLimit = dailyTokenLimit;
            plan.StripeProductId = stripeProductId;
        }

        return plan;
    }

    /// <summary>Returns true when the user can send a message given their current daily count.</summary>
    public bool CanSendMessage(int currentDailyCount) =>
        DailyMessageLimit == -1 || currentDailyCount < DailyMessageLimit;
}
