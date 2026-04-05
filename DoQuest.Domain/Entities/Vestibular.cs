using Flunt.Notifications;
using Flunt.Validations;
using DoQuest.Domain.Enums;

namespace DoQuest.Domain.Entities;

public class Vestibular : Notifiable<Notification>
{
    private Vestibular() { }

    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public VestibularType Type { get; private set; }
    public int Year { get; private set; }
    public string Description { get; private set; } = string.Empty;

    public static Vestibular Create(string name, VestibularType type, int year, string description = "")
    {
        var vestibular = new Vestibular();

        vestibular.AddNotifications(new Contract<Vestibular>()
            .Requires()
            .IsNotNullOrEmpty(name, nameof(Name), "Nome do vestibular é obrigatório")
            .IsGreaterOrEqualsThan(year, 1900, nameof(Year), "Ano do vestibular deve ser >= 1900")
            .IsLowerOrEqualsThan(year, DateTime.UtcNow.Year + 1, nameof(Year), "Ano do vestibular inválido"));

        if (vestibular.IsValid)
        {
            vestibular.Id = Guid.NewGuid();
            vestibular.Name = name;
            vestibular.Type = type;
            vestibular.Year = year;
            vestibular.Description = description;
        }

        return vestibular;
    }
}
