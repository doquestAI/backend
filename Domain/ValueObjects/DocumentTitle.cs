using Flunt.Validations;

namespace Domain.ValueObjects;

internal class DocumentTitle : BaseValueObject
{
    public string Value { get; private set; } = string.Empty;

    protected DocumentTitle() { }

    public DocumentTitle(string value)
    {
        AddNotifications(new Contract<DocumentTitle>()
            .Requires()
            .IsNotNullOrEmpty(value, nameof(Value), "Título do documento é obrigatório")
            .IsLowerOrEqualsThan(value?.Length ?? 0, 500, nameof(Value), "Título do documento deve ter no máximo 500 caracteres"));

        if (IsValid)
            Value = value!;
    }

    public override string ToString() => Value;
}