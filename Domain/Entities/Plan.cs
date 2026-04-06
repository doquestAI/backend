using Domain.Entities.Abstracts;
using Domain.Enums;
using Domain.ValueObjects;

namespace Domain.Entities;

internal class Plan : Entity
{
    private Plan() { }

    public PlanType Type { get; private set; }
    public PlanName Name { get; private set; } = null!;
    public DailyLimit DailyMessageLimit { get; private set; } = null!;
    public DailyLimit DailyTokenLimit { get; private set; } = null!;
    public StripeProductId StripeProductId { get; private set; } = null!;

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

        var nameVo = new PlanName(name);
        var messageLimitVo = new DailyLimit(dailyMessageLimit, nameof(DailyMessageLimit));
        var tokenLimitVo = new DailyLimit(dailyTokenLimit, nameof(DailyTokenLimit));
        var stripeVo = new StripeProductId(stripeProductId);

        plan.AddNotificationsFromValueObjects(nameVo, messageLimitVo, tokenLimitVo, stripeVo);

        if (plan.IsValid)
        {
            plan.Id = id;
            plan.Type = type;
            plan.Name = nameVo;
            plan.DailyMessageLimit = messageLimitVo;
            plan.DailyTokenLimit = tokenLimitVo;
            plan.StripeProductId = stripeVo;
        }

        return plan;
    }

    public bool CanSendMessage(int currentDailyCount) =>
        DailyMessageLimit.IsWithinLimit(currentDailyCount);
}
