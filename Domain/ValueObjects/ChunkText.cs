using Flunt.Validations;

namespace Domain.ValueObjects;

internal class ChunkText : BaseValueObject
{
    public string Value { get; private set; } = string.Empty;

    protected ChunkText() { }

    public ChunkText(string value)
    {
        AddNotifications(new Contract<ChunkText>()
            .Requires()
            .IsNotNullOrEmpty(value, nameof(Value), "Texto do chunk é obrigatório"));

        if (IsValid)
            Value = value!;
    }

    public override string ToString() => Value;
}
