using Domain.Entities.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.FluentMapping;

internal class DocumentMapping : IEntityTypeConfiguration<Document>
{
    public void Configure(EntityTypeBuilder<Document> builder)
    {
        builder.ToTable("Documents");
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

        builder.OwnsOne(x => x.FileName, fn =>
        {
            fn.Property(p => p.Body)
                .HasColumnName("FileName")
                .HasMaxLength(500)
                .IsRequired();
        });

        builder.OwnsOne(x => x.CloudStorageKey, csk =>
        {
            csk.Property(p => p.Body)
                .HasColumnName("CloudStorageKey")
                .HasMaxLength(1000)
                .IsRequired(false);
        });

        builder.Property(x => x.ContainerName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.ContentType)
            .IsRequired();

        builder.Property(x => x.FileSizeBytes)
            .IsRequired();

        builder.Property(x => x.Status)
            .IsRequired();

        builder.Property(x => x.UploadedByUserId)
            .HasColumnType("uuid")
            .IsRequired();

        builder.OwnsOne(x => x.SignedUrl, su =>
        {
            su.Property(p => p.Endereco)
                .HasColumnName("SignedUrl")
                .HasMaxLength(2000)
                .IsRequired(false);
        });

        builder.Property(x => x.SignedUrlExpiration)
            .HasColumnType("timestamptz")
            .IsRequired(false);

        builder.OwnsOne(x => x.LocalTemporaryPath, ltp =>
        {
            ltp.Property(p => p.Body)
                .HasColumnName("LocalTemporaryPath")
                .HasMaxLength(1000)
                .IsRequired(false);
        });

        builder.Property(x => x.IsEmbedding)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(x => x.EmbeddingProcessingStatus)
            .IsRequired(false);

        builder.Property(x => x.ChunksGenerated)
            .IsRequired(false);

        builder.Property(x => x.EmbeddingCompletedAt)
            .HasColumnType("timestamptz")
            .IsRequired(false);

        builder.Property(x => x.EmbeddingErrorMessage)
            .HasMaxLength(2000)
            .IsRequired(false);

        builder.Property(x => x.EmbeddingModel)
            .HasMaxLength(100)
            .IsRequired(false);

        builder.Property(x => x.DeletionRequestedAt)
            .HasColumnType("timestamptz")
            .IsRequired(false);

        builder.Property(x => x.DeletionRequestedBy)
            .HasColumnType("uuid")
            .IsRequired(false);

        builder.Property(x => x.Metadata)
            .HasColumnType("jsonb")
            .IsRequired(false);

        builder.HasIndex(x => x.Status);
        builder.HasIndex(x => x.UploadedByUserId);
        builder.HasIndex(x => x.IsEmbedding);
    }
}