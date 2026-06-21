
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Infrastructure.Data.FluentMapping;

internal class PictureMap : IEntityTypeConfiguration<Picture>
{
    public void Configure(EntityTypeBuilder<Picture> builder)
    {
        builder.ToTable("Pictures");

        builder.HasKey(p => p.Id).HasName("PK_Pictures");

        builder.Property(p => p.Id)
            .HasColumnType("uuid")
            .IsRequired();

        builder.Property(p => p.CreatedDate)
            .HasColumnType("timestamptz")
            .IsRequired();

        builder.Property(p => p.UpdatedDate)
            .HasColumnType("timestamptz")
            .IsRequired();

        builder.Property(p => p.DeletedDate)
            .HasColumnType("timestamptz")
            .IsRequired(false);

        builder.Property(p => p.IsActive)
            .HasColumnName("IsActive")
            .HasColumnType("boolean")
            .IsRequired();

        builder.Property(p => p.ContainerName)
            .HasColumnName("ContainerName")
            .HasColumnType("varchar")
            .HasMaxLength(255);

        builder.Property(p => p.SignedUrlExpiration)
            .HasColumnName("SignedUrlExpiration")
            .HasColumnType("timestamptz");

        builder.OwnsOne(p => p.CloudStorageKey, storageKey =>
        {
            storageKey.Property(a => a.Body)
                .HasColumnName("CloudStorageKey")
                .HasColumnType("varchar")
                .HasMaxLength(255)
                .IsRequired();
        });

        builder.Property(p => p.ContentType)
         .HasColumnName("ContentType")
         .HasColumnType("varchar")
         .HasConversion<string>()
         .IsRequired(false);

        builder.OwnsOne(v => v.LocalTemporaryPath, tp =>
        {
            tp.Property(a => a.Body)
                .HasColumnName("LocalTemporaryPath")
                .HasColumnType("varchar")
                .HasMaxLength(255)
                .IsRequired();
        });

        builder.OwnsOne(p => p.SignedUrl, url =>
        {
            url.Property(u => u.Endereco)
                .HasColumnName("SignedUrl")
                .HasColumnType("varchar")
                .HasMaxLength(255)
                .IsRequired(false);
        });
    }
}