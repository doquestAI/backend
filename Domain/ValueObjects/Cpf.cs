using Flunt.Br;
using Flunt.Br.Extensions;
using System.Text.RegularExpressions;

namespace Domain.ValueObjects;

internal class Cpf : BaseValueObject
{
    public string Numero { get; private set; }

    protected Cpf() { }

    public Cpf(string numero)
    {
        AddNotifications(
        new Contract()
            .IsCpf(numero, "Numero", "CPF inválido")
            .Requires()
        );
        if (IsValid)
            Numero = Regex.Replace(numero ?? string.Empty, @"[^0-9]", "");
    }

    public override string ToString()
    {
        if (string.IsNullOrEmpty(Numero) || Numero.Length != 11)
            return Numero ?? string.Empty;

        return $"{Numero.Substring(0, 3)}.{Numero.Substring(3, 3)}.{Numero.Substring(6, 3)}-{Numero.Substring(9, 2)}";
    }
}