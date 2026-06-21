using Domain.Entities.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.FluentMapping;

internal class UserMapping : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");
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

        builder.OwnsOne(x => x.Password, pw =>
        {
            pw.Property(p => p.Hash)
                .HasColumnName("PasswordHash")
                .HasMaxLength(256)
                .IsRequired(false);

            pw.Property(p => p.Salt)
                .HasColumnName("PasswordSalt")
                .HasMaxLength(256)
                .IsRequired(false);
        });

        builder.OwnsOne(x => x.FullName, fn =>
        {
            fn.Property(p => p.FirstName)
                .HasColumnName("FirstName")
                .HasMaxLength(100);

            fn.Property(p => p.LastName)
                .HasColumnName("LastName")
                .HasMaxLength(100);
        });

        builder.OwnsOne(x => x.Email, email =>
        {
            email.Property(p => p.Address)
                .HasColumnName("Email")
                .HasMaxLength(150);
        });

        builder.OwnsOne(x => x.Address,
            address =>
            {
                address.Property(p => p.Road)
                    .HasColumnName("Road")
                    .HasMaxLength(200);

                address.Property(p => p.Complement)
                    .HasColumnName("Complement")
                    .HasMaxLength(100);

                address.Property(p => p.NeighBordHood)
                    .HasColumnName("Neighborhood")
                    .HasMaxLength(100);

                address.Property(p => p.CEP)
                    .HasColumnName("Cep")
                    .HasMaxLength(20);

                address.Property(p => p.Number)
                    .HasColumnName("Number")
                    .HasMaxLength(20);
            }
        );

        builder.Property(x => x.Active)
            .HasColumnType("boolean");

        builder.Property(x => x.IsActiveCredentials)
            .HasColumnType("boolean");

        builder.OwnsOne(x => x.TokenActivate, token =>
        {
            token.Property(p => p.EncryptedValue)
                .HasColumnName("TokenActivateEncrypted")
                .HasMaxLength(1000)
                .IsRequired(false);
        });

        builder.HasMany(x => x.UserRoles)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
