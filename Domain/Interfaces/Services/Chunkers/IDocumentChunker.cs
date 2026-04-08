namespace Domain.Interfaces.Services.Chunkers;

internal interface IDocumentChunkerService
{
    IReadOnlyList<string> Chunk(string rawText);
}