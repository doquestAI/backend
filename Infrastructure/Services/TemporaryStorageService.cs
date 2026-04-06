
using Domain.Interfaces.Services;

namespace Infrastructure.Services;

internal class TemporaryStorageService : ITemporaryStorageService
{
    public async Task<string> SaveAsync(string pathBase, Guid id, Stream file, string extension, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(pathBase))
            throw new ArgumentException("Path base is required.", nameof(pathBase));

        if (string.IsNullOrWhiteSpace(extension))
            throw new ArgumentException("Extension is required.", nameof(extension));

        if (!Directory.Exists(pathBase))
            Directory.CreateDirectory(pathBase);

        var fullPath = Path.Combine(pathBase, $"{id}{extension}");

        await using var fileStream = new FileStream(fullPath, FileMode.Create, FileAccess.Write);
        await file.CopyToAsync(fileStream, cancellationToken);

        return fullPath;
    }

    public Task DeleteAsync(string filePath, CancellationToken cancellationToken)
    {
        if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
        {
            File.Delete(filePath);
        }
        return Task.CompletedTask;
    }
}