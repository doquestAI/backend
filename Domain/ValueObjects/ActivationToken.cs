using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography;
using System.Text;

namespace Domain.ValueObjects;

internal sealed class ActivationToken : BaseValueObject
{
    private const int KeySize = 32;
    private const int IvSize = 16;

    public string EncryptedValue { get; private set; }

    [NotMapped]
    public string PlainValue { get; private set; }

    public ActivationToken() { }

    private ActivationToken(string encryptedValue, string plainValue)
    {
        EncryptedValue = encryptedValue;
        PlainValue = plainValue;
    }

    /// <summary>
    /// Gera um novo token de ativação com criptografia
    /// </summary>
    public static ActivationToken Create(string encryptionKey)
    {
        if (string.IsNullOrWhiteSpace(encryptionKey))
            throw new ArgumentException("Encryption key cannot be null or empty", nameof(encryptionKey));

        if (encryptionKey.Length < KeySize * 2)
            throw new ArgumentException("Encryption key must be at least 64 characters long (32 bytes in hex)", nameof(encryptionKey));

        var plainToken = Guid.NewGuid().ToString("N").Substring(0, 32);
        var encryptedToken = EncryptToken(plainToken, encryptionKey);

        return new ActivationToken(encryptedToken, plainToken);
    }

    /// <summary>
    /// Valida se um token em texto plano corresponde ao token criptografado armazenado
    /// </summary>
    public bool Validate(string plainTokenToVerify, string encryptionKey)
    {
        if (string.IsNullOrWhiteSpace(plainTokenToVerify) || string.IsNullOrWhiteSpace(encryptionKey))
            return false;

        try
        {
            var decryptedToken = DecryptToken(EncryptedValue, encryptionKey);
            return decryptedToken == plainTokenToVerify;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Criptografa um token em texto plano usando a chave fornecida
    /// </summary>
    private static string EncryptToken(string plainToken, string encryptionKey)
    {
        // Converter chave hex para bytes (primeiros 32 bytes = 64 caracteres hex)
        var keyHex = encryptionKey.Substring(0, KeySize * 2);
        var keyBytes = Convert.FromHexString(keyHex);

        using var aes = Aes.Create();
        aes.Key = keyBytes;
        aes.GenerateIV();

        var iv = aes.IV;

        using var encryptor = aes.CreateEncryptor(aes.Key, iv);
        using var ms = new MemoryStream();

        // Escrever IV no início do stream
        ms.Write(iv, 0, iv.Length);

        using var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write);
        using var sw = new StreamWriter(cs);
        sw.Write(plainToken);
        sw.Flush();
        cs.FlushFinalBlock();

        var encryptedBytes = ms.ToArray();
        return Convert.ToBase64String(encryptedBytes, Base64FormattingOptions.None);  // ← Base64, não Hex!
    }


    /// <summary>
    /// Descriptografa um token usando a chave fornecida
    /// </summary>
    private static string DecryptToken(string encryptedToken, string encryptionKey)
    {
        // Converter chave hex para bytes (primeiros 32 bytes = 64 caracteres hex)
        var keyHex = encryptionKey.Substring(0, KeySize * 2);
        var keyBytes = Convert.FromHexString(keyHex);
        var encryptedBytes = Convert.FromBase64String(encryptedToken);

        using var aes = Aes.Create();
        aes.Key = keyBytes;

        var iv = new byte[IvSize];
        Array.Copy(encryptedBytes, 0, iv, 0, IvSize);
        aes.IV = iv;

        using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
        using var ms = new MemoryStream(encryptedBytes, IvSize, encryptedBytes.Length - IvSize);
        using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
        using var sr = new StreamReader(cs);

        return sr.ReadToEnd();
    }
}
