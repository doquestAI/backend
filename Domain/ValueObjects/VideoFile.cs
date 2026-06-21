using Flunt.Br;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.ValueObjects;

internal class VideoFile : BaseValueObject
{
    [NotMapped]
    public Stream File { get; private set; } = null!;
    public string FileName { get; private set; } = null!;
    public long FileSize { get; private set; }
    private VideoFile() { }
    public VideoFile(Stream? file, string fileName)
    {
        var contract = new Contract()
            .Requires()
            .IsNotNull(file, Key, "File cannot be null")
            .IsNotNullOrEmpty(fileName, Key, "File name cannot be null or empty");

        if (file != null)
            contract.IsLowerThan(file.Length, 10_000_000, Key, "File size must be less than 10MB");

        AddNotifications(contract);

        if (!IsValid)
            return;
        File = file!;
        FileName = fileName;
        FileSize = file.Length;
    }
}