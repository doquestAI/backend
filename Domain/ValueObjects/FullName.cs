using Flunt.Br;

namespace Domain.ValueObjects;

internal class FullName : BaseValueObject
{
    public string FirstName { get; private set; }
    public string LastName { get; private set; }

    public FullName(string firstName, string lastName)
    {
        AddNotifications(
            new Contract()
                .Requires()
                .IsNotNullOrEmpty(firstName, Key, "First name cannot be null or empty")
                .IsNotNullOrWhiteSpace(firstName, Key, "First name cannot be whitespace only")
                .IsNotNullOrEmpty(lastName, Key, "Last name cannot be null or empty")
                .IsNotNullOrWhiteSpace(lastName, Key, "Last name cannot be whitespace only")
                .IsLowerThan(firstName?.Length ?? 0, 100.0, Key, "First name cannot be longer than 100 characters")
                .IsLowerThan(lastName?.Length ?? 0, 100.0, Key, "Last name cannot be longer than 100 characters")
        );
        FirstName = firstName;
        LastName = lastName;
    }
    private FullName() { }

    public override string ToString() => $"{FirstName} {LastName}";
}