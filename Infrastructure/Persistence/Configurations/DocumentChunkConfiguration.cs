using Domain.Constants;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pgvector;

namespace Infrastructure.Persistence.Configurations;

internal sealed class DocumentChunkConfiguration : IEntityTypeConfiguration<DocumentChunk>
{
    public void Configure(EntityTypeBuilder<DocumentChunk> builder)
    {
        builder.ToTable("document_chunks");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.DocumentId).HasColumnName("document_id").IsRequired();
        builder.Property(x => x.ChunkIndex).HasColumnName("chunk_index").IsRequired();
        builder.Property(x => x.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at");
        builder.Property(x => x.DeletedAt).HasColumnName("deleted_at");

        // Value Objects
        builder.OwnsOne(x => x.Text, vo =>
        {
            vo.Property(v => v.Value).HasColumnName("text").IsRequired();
            vo.Ignore("Notifications");
            vo.Ignore("IsValid");
        });

        builder.OwnsOne(x => x.Embedding, vo =>
        {
            vo.Property(v => v.Values)
               .HasColumnName("embedding")
               .HasColumnType($"vector({ModelConstants.EmbeddingDimension})")
               .HasConversion(
                   v => new Vector(v),
                   v => v.ToArray())
               .IsRequired();
            vo.Ignore("Notifications");
            vo.Ignore("IsValid");
        });

        builder.HasIndex(x => x.DocumentId).HasDatabaseName("ix_document_chunks_document_id");

        // Ignore Flunt properties
        builder.Ignore("Notifications");
        builder.Ignore("IsValid");
    }
}
