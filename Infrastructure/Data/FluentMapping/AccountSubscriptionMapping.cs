using Domain.Entities.Core.Subscriptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.FluentMapping;

internal class AccountSubscriptionMapping : IEntityTypeConfiguration<AccountSubscription>
{
    public void Configure(EntityTypeBuilder<AccountSubscription> builder)
    {
        builder.ToTable("AccountSubscriptions");

        builder.HasKey(e => e.Id).HasName("PK_AccountSubscriptions");

        builder.Property(e => e.Id)
            .HasColumnType("uuid")
            .IsRequired();

        builder.Property(e => e.CreatedDate)
            .HasColumnType("timestamptz")
            .IsRequired();

        builder.Property(e => e.UpdatedDate)
            .HasColumnType("timestamptz")
            .IsRequired();

        builder.Property(e => e.DeletedDate)
            .HasColumnType("timestamptz")
            .IsRequired(false);

        builder.Property(e => e.PlanId)
            .HasColumnName("PlanId")
            .HasColumnType("varchar")
            .HasMaxLength(200)
            .IsRequired();

        builder.OwnsOne(e => e.EntraUserId, vo =>
        {
            vo.Property(v => v.Value)
                .HasColumnName("EntraUserId")
                .HasColumnType("varchar")
                .HasMaxLength(200)
                .IsRequired();

            vo.HasIndex(v => v.Value)
                .IsUnique()
                .HasDatabaseName("IX_AccountSubscriptions_EntraUserId");
        });

        builder.OwnsOne(e => e.StripeCustomerId, vo =>
        {
            vo.Property(v => v.Value)
                .HasColumnName("StripeCustomerId")
                .HasColumnType("varchar")
                .HasMaxLength(200)
                .IsRequired();

            vo.HasIndex(v => v.Value)
                .IsUnique()
                .HasDatabaseName("IX_AccountSubscriptions_StripeCustomerId");
        });

        builder.OwnsOne(e => e.Status, vo =>
        {
            vo.Property(v => v.Value)
                .HasColumnName("SubscriptionStatus")
                .HasColumnType("varchar")
                .HasMaxLength(50)
                .IsRequired();
        });
    }
}
