using Domain.Common;
using Flunt.Validations;

namespace Domain.Sessions.ValueObjects;

public sealed class SessionId : ValueObject
{
    public string Value { get; private set; } = string.Empty;

    private SessionId() { }

    public SessionId(string value)
    {
        AddNotifications(
            new Contract<SessionId>()
                .IsNotNullOrEmpty(value, nameof(SessionId), "SessionId cannot be empty")
                .IsLowerOrEqualsThan(value?.Length ?? 0, 256, nameof(SessionId),
                    "SessionId cannot exceed 256 characters"));

        if (IsValid)
            Value = value!;
    }

    public static SessionId New() => new(Guid.NewGuid().ToString("N"));

    public override string ToString() => Value;
}
