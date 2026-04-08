using Domain.Entities.Abstracts;
using Domain.ValueObjects;
using Flunt.Notifications;
using Flunt.Validations;

namespace Domain.Entities;

internal class User : Entity
{
    private User() { }

    public FirebaseUid FirebaseUid { get; private set; } = null!;
    public Email Email { get; private set; } = null!;
    public Guid PlanId { get; private set; }
    public int DailyMessageCount { get; private set; }
    public DateOnly LastMessageDate { get; private set; }

    // Navigation property (set by EF Core)
    public Plan Plan { get; private set; } = null!;

    public static User Create(string firebaseUid, string email, Guid planId)
    {
        var user = new User();

        var firebaseUidVo = new FirebaseUid(firebaseUid);
        var emailVo = new Email(email);

        user.AddNotificationsFromValueObjects(firebaseUidVo, emailVo);

        user.AddNotifications(new Contract<User>()
            .Requires()
            .AreNotEquals(planId, Guid.Empty, nameof(PlanId), "PlanId inválido"));

        if (user.IsValid)
        {
            user.FirebaseUid = firebaseUidVo;
            user.Email = emailVo;
            user.PlanId = planId;
            user.DailyMessageCount = 0;
            user.LastMessageDate = DateOnly.FromDateTime(DateTime.UtcNow);
        }

        return user;
    }

    public void ConsumeMessage()
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        if (LastMessageDate != today)
        {
            DailyMessageCount = 0;
            LastMessageDate = today;
        }

        if (!Plan.CanSendMessage(DailyMessageCount))
        {
            AddNotification(new Notification(
                nameof(DailyMessageCount),
                $"Limite diário de mensagens atingido para o plano {Plan.Name}. Faça upgrade para continuar."));
            return;
        }

        DailyMessageCount++;
    }

    public void ChangePlan(Guid newPlanId)
    {
        AddNotifications(new Contract<User>()
            .Requires()
            .AreNotEquals(newPlanId, Guid.Empty, nameof(PlanId), "PlanId inválido"));

        if (IsValid)
        {
            PlanId = newPlanId;
            MarkAsUpdated();
        }
    }
}