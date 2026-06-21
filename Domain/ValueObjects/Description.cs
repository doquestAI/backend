using Flunt.Br;

namespace Domain.ValueObjects;

internal class Description : BaseValueObject
{
    public string Text { get; private set; }

    public Description(string text)
    {
        AddNotifications(
            new Contract()
                .Requires()
                .IsNotNullOrEmpty(text, Key, "Description cannot be null or empty")
                .IsNotNullOrWhiteSpace(text, Key, "Description cannot be whitespace only")
                .IsLowerThan(text?.Length ?? 0, 1000, Key, "Description cannot be longer than 1000 characters")
        );
        Text = text;
    }
    private Description() { }
}