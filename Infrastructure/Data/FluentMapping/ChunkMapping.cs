using Domain.Entities.Core.Documents;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Pgvector;

namespace Infrastructure.Data.FluentMapping;

internal class ChunkMapping : IEntityTypeConfiguration<Chunk>
{
    public void Configure(EntityTypeBuilder<Chunk> builder)
    {
        builder.ToTable("Chunks");
        builder.HasKey(x => x.Id);

        builder.Property(p => p.CreatedDate)
            .HasColumnType("timestamptz")
            .IsRequired();

        builder.Property(p => p.UpdatedDate)
            .HasColumnType("timestamptz")
            .IsRequired();

        builder.Property(p => p.DeletedDate)
            .HasColumnType("timestamptz")
            .IsRequired(false);

        builder.Property(x => x.DocumentId)
            .HasColumnType("uuid")
            .IsRequired();

        builder.Property(x => x.PositionIndex)
            .IsRequired();

        builder.Property(x => x.Content)
            .IsRequired();

        builder.Property(x => x.Metadata)
            .HasColumnType("jsonb")
            .IsRequired(false);

        builder.Property(x => x.Embedding)
            .HasColumnType("vector(1536)")
            .IsRequired(false);

        builder.HasIndex(x => x.DocumentId);
        builder.HasIndex(x => x.DeletedDate);
    }
}
