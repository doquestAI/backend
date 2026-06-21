using Flunt.Br;
using System;

namespace Domain.ValueObjects;

internal class BigString : BaseValueObject
{
    public string? Body { get; private set; }
    public BigString(string text)
    {
        if (text is not null)
        {
            AddNotifications(
                new Contract()
                .IsLowerThan(text.Length, 5000, Key, "Body cannot be longer than 5000 characters")
            );
            Body = text;
        }
    }
    private BigString() { }
}