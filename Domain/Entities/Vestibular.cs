using Domain.Entities.Abstracts;
using Domain.Enums;
using Domain.ValueObjects;

namespace Domain.Entities;

internal class Vestibular : Entity
{
    private Vestibular() { }

    public VestibularName Name { get; private set; } = null!;
    public VestibularType Type { get; private set; }
    public VestibularYear Year { get; private set; } = null!;
    public Description Description { get; private set; } = null!;

    public static Vestibular Create(string name, VestibularType type, int year, string description = "")
    {
        var vestibular = new Vestibular();

        var nameVo = new VestibularName(name);
        var yearVo = new VestibularYear(year);
        var descriptionVo = new Description(description);

        vestibular.AddNotificationsFromValueObjects(nameVo, yearVo, descriptionVo);

        if (vestibular.IsValid)
        {
            vestibular.Name = nameVo;
            vestibular.Type = type;
            vestibular.Year = yearVo;
            vestibular.Description = descriptionVo;
        }

        return vestibular;
    }
}