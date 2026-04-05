namespace DoQuest.Application.Abstractions;

public interface IDocumentChunker
{
    /// <summary>
    /// Splits raw text into overlapping chunks suitable for embedding.
    /// Each chunk has at most ModelConstants.MaxChunkTokens tokens with ModelConstants.ChunkOverlapTokens overlap.
    /// </summary>
    IReadOnlyList<string> Chunk(string rawText);
}
