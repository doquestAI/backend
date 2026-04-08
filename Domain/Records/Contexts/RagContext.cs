using Domain.Entities;

namespace Domain.Records;

internal sealed record RagContext(
    IReadOnlyList<DocumentChunk> Chunks)
{
    public string ToContextString() =>
        Chunks.Count == 0
            ? string.Empty
            : string.Join("\n\n---\n\n", Chunks.Select((c, i) => $"[Fonte {i + 1}] {c.Text.Value}"));
}
