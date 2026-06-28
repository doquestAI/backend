using Pgvector;

namespace Infrastructure.Data;

internal sealed class VectorItem
{
    public Guid DbId { get; set; }
    public string ItemId { get; set; } = default!;
    public string Collection { get; set; } = default!;
    public string Content { get; set; } = default!;
    public Vector Embedding { get; set; } = default!;
    public string MetadataJson { get; set; } = "{}";
    public DateTime CreatedAt { get; set; }
}
