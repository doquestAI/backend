
namespace Domain.Interfaces.Services.DocumentLoaders;

internal interface IDocumentLoader
{
    Task<string> LoadPdfAsync(string filePath);
}
