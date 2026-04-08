using Domain.Constants;
using Domain.Entities;
using FluentAssertions;

namespace Tests.Domain;

public sealed class DocumentChunkInvariantTests
{
    private static readonly float[] ValidEmbedding =
        Enumerable.Repeat(0.1f, ModelConstants.EmbeddingDimension).ToArray();

    [Fact]
    public void Create_WithValidTextAndEmbedding_ShouldBeValid()
    {
        var chunk = DocumentChunk.Create(Guid.NewGuid(), "Texto de exemplo", ValidEmbedding, 0);
        chunk.IsValid.Should().BeTrue();
        chunk.Text.Value.Should().Be("Texto de exemplo");
        chunk.Embedding.Values.Should().HaveCount(ModelConstants.EmbeddingDimension);
    }

    [Fact]
    public void Create_WithEmptyText_ShouldBeInvalid()
    {
        var chunk = DocumentChunk.Create(Guid.NewGuid(), "", ValidEmbedding, 0);
        chunk.IsInvalid.Should().BeTrue();
        chunk.Notifications.Should().ContainSingle(n => n.Key == "Value");
    }

    [Fact]
    public void Create_WithNullEmbedding_ShouldBeInvalid()
    {
        var chunk = DocumentChunk.Create(Guid.NewGuid(), "Texto", null!, 0);
        chunk.IsInvalid.Should().BeTrue();
        chunk.Notifications.Should().ContainSingle(n => n.Key == "Values");
    }

    [Fact]
    public void Create_WithWrongEmbeddingDimension_ShouldBeInvalid()
    {
        var wrongEmbedding = new float[100]; // Wrong dimension
        var chunk = DocumentChunk.Create(Guid.NewGuid(), "Texto", wrongEmbedding, 0);
        chunk.IsInvalid.Should().BeTrue();
        chunk.Notifications.Should().ContainSingle(n =>
            n.Key == "Values" && n.Message.Contains(ModelConstants.EmbeddingDimension.ToString()));
    }

    [Fact]
    public void Create_WithNegativeChunkIndex_ShouldBeInvalid()
    {
        var chunk = DocumentChunk.Create(Guid.NewGuid(), "Texto", ValidEmbedding, -1);
        chunk.IsInvalid.Should().BeTrue();
        chunk.Notifications.Should().ContainSingle(n => n.Key == "ChunkIndex");
    }

    [Fact]
    public void Document_AddChunk_WithInvalidEmbedding_ShouldMarkDocumentInvalid()
    {
        var document = Document.Create("Título", "https://example.com", Guid.NewGuid());
        document.IsValid.Should().BeTrue();

        document.AddChunk("Texto", []); // Empty embedding

        document.IsInvalid.Should().BeTrue();
        document.Chunks.Should().BeEmpty("invalid chunk should not be added");
    }
}