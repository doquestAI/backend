using Flunt.Br;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.ValueObjects;

internal class AppFile : BaseValueObject
{
    [NotMapped]
    public Stream File { get; private set; } = null!;
    public string FileName { get; private set; } = null!;
    public long FileSize { get; private set; }

    private AppFile() { }
    public AppFile(Stream file, string fileName)
    {
        AddNotifications(
            new Contract()
                .Requires()
                .IsNotNull(file, Key, "File cannot be null")
                .IsLowerThan(file.Length, 10_000_000, Key, "File size must be less than 10MB")
                .IsNotNullOrEmpty(fileName, Key, "File name cannot be null or empty")
        );

        if (!IsValid)
            return;
        File = file;
        FileName = fileName;
        FileSize = file.Length;
    }
}