namespace Domain.Interfaces.Services;

internal interface ITemporaryStorageService
{
    Task<string> SaveAsync(string pathBase, Guid id, Stream file, string extension, CancellationToken cancellationToken);
    Task DeleteAsync(string filePath, CancellationToken cancellationToken);
}