using Domain.Entities.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.FluentMapping;

internal class RefreshTokenMapping : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("RefreshTokens");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.UserId)
            .HasColumnType("uuid")
            .IsRequired();

        builder.Property(x => x.TokenHash)
            .HasMaxLength(512)
            .IsRequired();

        builder.Property(x => x.JwtId)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.ExpiresAt)
            .HasColumnType("timestamptz")
            .IsRequired();

        builder.Property(x => x.DeviceInfo)
            .HasMaxLength(500)
            .IsRequired(false);

        builder.Property(x => x.IpAddress)
            .HasMaxLength(50)
            .IsRequired(false);

        builder.Property(x => x.IsRevoked)
            .IsRequired();

        builder.Property(x => x.RevokedAt)
            .HasColumnType("timestamptz")
            .IsRequired(false);

        builder.Property(x => x.CreatedDate)
            .HasColumnType("timestamptz")
            .IsRequired();

        builder.Property(x => x.UpdatedDate)
            .HasColumnType("timestamptz")
            .IsRequired();

        builder.Property(x => x.DeletedDate)
            .HasColumnType("timestamptz")
            .IsRequired(false);

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.JwtId)
            .IsUnique();

        builder.HasIndex(x => x.UserId);

        builder.HasIndex(x => new { x.UserId, x.IsRevoked, x.ExpiresAt });
    }
}