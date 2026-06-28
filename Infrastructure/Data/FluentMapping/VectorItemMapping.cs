using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.FluentMapping;

internal class VectorItemMapping : IEntityTypeConfiguration<VectorItem>
{
    public void Configure(EntityTypeBuilder<VectorItem> builder)
    {
        builder.ToTable("VectorItems");

        builder.HasKey(v => v.DbId).HasName("PK_VectorItems");

        builder.Property(v => v.DbId)
            .HasColumnName("DbId")
            .HasColumnType("uuid")
            .IsRequired();

        builder.Property(v => v.ItemId)
            .HasColumnName("ItemId")
            .HasColumnType("varchar")
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(v => v.Collection)
            .HasColumnName("Collection")
            .HasColumnType("varchar")
            .HasMaxLength(200)
            .IsRequired();

        builder.HasIndex(v => new { v.Collection, v.ItemId })
            .IsUnique()
            .HasDatabaseName("IX_VectorItems_Collection_ItemId");

        builder.HasIndex(v => v.Collection)
            .HasDatabaseName("IX_VectorItems_Collection");

        builder.Property(v => v.Content)
            .HasColumnName("Content")
            .HasColumnType("text")
            .IsRequired();

        builder.Property(v => v.Embedding)
            .HasColumnName("Embedding")
            .HasColumnType("vector(1536)")
            .IsRequired();

        builder.Property(v => v.MetadataJson)
            .HasColumnName("MetadataJson")
            .HasColumnType("jsonb")
            .IsRequired();

        builder.Property(v => v.CreatedAt)
            .HasColumnName("CreatedAt")
            .HasColumnType("timestamptz")
            .IsRequired();
    }
}
