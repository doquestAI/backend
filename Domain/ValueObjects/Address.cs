using Flunt.Br;
using Flunt.Br.Extensions;

namespace Domain.ValueObjects;

internal class Address : BaseValueObject
{
    public string? Road { get; private set; }
    public string? NeighBordHood { get; private set; }
    public long? Number { get; private set; }
    public string? CEP { get; private set; }
    public string? Complement { get; private set; }

    private Address() { }
    public Address(long? number, string? neighBordHood, string? road,
     string? complement, string? cep)
    {
        var contract = new Contract()
            .IsCep(cep, Key, "CEP is required")
            .Requires()
            .IsNotNull(number, Key, "Number cannot be null")
            .IsGreaterThan(number != null ? (double)number : double.MinValue, 0.0, Key, "Number must be greater than 0")
            .IsNotNullOrEmpty(road ?? string.Empty, Key, "Road is required")
            .IsNotNullOrEmpty(neighBordHood ?? string.Empty, Key, "Neighborhood is required");

        if (!string.IsNullOrEmpty(complement))
            contract.IsLowerThan(complement.Length, 100, Key, "Complement must not exceed 100 characters");

        AddNotifications(contract);

        Number = number;
        NeighBordHood = neighBordHood;
        Road = road;
        Complement = complement;
        CEP = cep;
    }
}