using Domain.Entities.Abstracts;
using Domain.Entities.Payments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.FluentMapping;

internal class SubscriptionMapping : IEntityTypeConfiguration<Subscription>
{
    public void Configure(EntityTypeBuilder<Subscription> builder)
    {
        builder.ToTable("Subscriptions");

        builder.HasKey(s => s.Id).HasName("PK_Subscriptions");

        builder.Property(s => s.Id)
            .HasColumnType("uuid")
            .IsRequired()
            ;

        builder.Property(s => s.CreatedDate).HasColumnType("timestamptz").IsRequired();
        builder.Property(s => s.UpdatedDate).HasColumnType("timestamptz").IsRequired();
        builder.Property(s => s.DeletedDate).HasColumnType("timestamptz").IsRequired(false);


        // Value Object: SubscriptionPeriod
        builder.OwnsOne(s => s.Period, period =>
        {
            period.Property(p => p.StartDate)
                .HasColumnName("StartDate")
                .HasColumnType("timestamptz")
                .IsRequired();

            period.Property(p => p.EndDate)
                .HasColumnName("EndDate")
                .HasColumnType("timestamptz")
                .IsRequired();
        });

        // Value Object: Payment (nullable — apenas Premium usa)
        builder.OwnsOne(s => s.Payment, pay =>
        {
            pay.Property(p => p.PaymentGateway)
                .HasColumnName("PaymentProvider")
                .HasColumnType("varchar")
                .HasMaxLength(50)
                .IsRequired(false);

            pay.Property(p => p.TransactionId)
                .HasColumnName("TransactionId")
                .HasColumnType("varchar")
                .IsRequired(false);
        });

        // Discriminator para TPH
        builder
            .HasDiscriminator<string>("SubscriptionType")
            .HasValue<FreeSubscription>("Free")
            .HasValue<PremiumSubscription>("Premium");

        // Campos específicos de PremiumSubscription
        builder.Property<decimal?>("Price")
                .HasColumnType("decimal")
                .HasPrecision(10, 2)
                .IsRequired(false);

        builder.Property<string>("StripeSubscriptionId")
        .HasColumnType("varchar")
        .HasMaxLength(255)
        .IsRequired(false);

        builder.Property<string>("StripeCustomerId")
            .HasColumnType("varchar")
            .HasMaxLength(255)
            .IsRequired(false);

        builder.Property<DateTime?>("StartDate")
            .HasColumnType("timestamptz")
            .IsRequired(false);

        builder.Property<DateTime?>("EndDate")
            .HasColumnType("timestamptz")
            .IsRequired(false);

    }
}