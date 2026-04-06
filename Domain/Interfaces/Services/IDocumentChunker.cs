namespace Domain.Interfaces.Services;

internal interface IDocumentChunker
{
    IReadOnlyList<string> Chunk(string rawText);
}
