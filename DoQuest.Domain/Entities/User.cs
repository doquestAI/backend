using Flunt.Notifications;
using Flunt.Validations;

namespace DoQuest.Domain.Entities;

public class User : Notifiable<Notification>
{
    private User() { }

    public Guid Id { get; private set; }
    public string FirebaseUid { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public Guid PlanId { get; private set; }
    public int DailyMessageCount { get; private set; }
    public DateOnly LastMessageDate { get; private set; }
    public DateTime CreatedAt { get; private set; }

    // Navigation property (set by EF Core)
    public Plan Plan { get; private set; } = null!;

    public static User Create(string firebaseUid, string email, Guid planId)
    {
        var user = new User();

        user.AddNotifications(new Contract<User>()
            .Requires()
            .IsNotNullOrEmpty(firebaseUid, nameof(FirebaseUid), "Firebase UID é obrigatório")
            .IsEmail(email, nameof(Email), "Email inválido")
            .AreNotEquals(planId, Guid.Empty, nameof(PlanId), "PlanId inválido"));

        if (user.IsValid)
        {
            user.Id = Guid.NewGuid();
            user.FirebaseUid = firebaseUid;
            user.Email = email;
            user.PlanId = planId;
            user.DailyMessageCount = 0;
            user.LastMessageDate = DateOnly.FromDateTime(DateTime.UtcNow);
            user.CreatedAt = DateTime.UtcNow;
        }

        return user;
    }

    /// <summary>
    /// Attempts to consume one message slot for today.
    /// Resets the counter if the last message was on a different day.
    /// Adds a notification if the plan limit has been reached.
    /// </summary>
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

    /// <summary>Updates the user's plan.</summary>
    public void ChangePlan(Guid newPlanId)
    {
        AddNotifications(new Contract<User>()
            .Requires()
            .AreNotEquals(newPlanId, Guid.Empty, nameof(PlanId), "PlanId inválido"));

        if (IsValid)
            PlanId = newPlanId;
    }
}
