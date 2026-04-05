using DoQuest.Domain.Constants;
using DoQuest.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pgvector;

namespace DoQuest.Infrastructure.Persistence.Configurations;

public sealed class DocumentChunkConfiguration : IEntityTypeConfiguration<DocumentChunk>
{
    public void Configure(EntityTypeBuilder<DocumentChunk> builder)
    {
        builder.ToTable("document_chunks");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.DocumentId).HasColumnName("document_id").IsRequired();
        builder.Property(x => x.Text).HasColumnName("text").IsRequired();
        builder.Property(x => x.ChunkIndex).HasColumnName("chunk_index").IsRequired();

        // Map float[] to pgvector column using value conversion
        builder.Property(x => x.Embedding)
               .HasColumnName("embedding")
               .HasColumnType($"vector({ModelConstants.EmbeddingDimension})")
               .HasConversion(
                   v => new Vector(v),
                   v => v.ToArray())
               .IsRequired();

        builder.HasIndex(x => x.DocumentId).HasDatabaseName("ix_document_chunks_document_id");

        // Ignore Flunt properties
        builder.Ignore("Notifications");
        builder.Ignore("IsValid");
    }
}
