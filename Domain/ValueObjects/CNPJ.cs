using Flunt.Br;
using Flunt.Br.Extensions;

namespace Domain.ValueObjects;

internal class CNPJ : BaseValueObject
{
    public string Number { get; private set; }

    private CNPJ() { }

    public CNPJ(string number)
    {
        var cleanNumber = new string(number.Where(char.IsDigit).ToArray());

        AddNotifications(
         new Contract()
             .IsCnpj(cleanNumber, "Number", "Invalid document")
             .Requires()
     );

        if (IsValid)
            Number = cleanNumber;
    }
}