using Flunt.Br;
using System;

namespace Domain.ValueObjects;

internal class UniqueName : BaseValueObject
{
    public string Name { get; private set; }
    public UniqueName(string name)
    {
        AddNotifications(
            new Contract()
                .Requires()
                .IsNotNullOrEmpty(name, Key, "Name cannot be null or empty")
                .IsNotNullOrWhiteSpace(name, Key, "Name cannot be whitespace only")
                .IsLowerThan(name?.Length ?? 0, 50.0, Key, "Name cannot be longer than 50 characters")
            );
        Name = name;
    }
    private UniqueName() { }
}