using DoQuest.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DoQuest.Infrastructure.Persistence.Configurations;

public sealed class PlanConfiguration : IEntityTypeConfiguration<Plan>
{
    public void Configure(EntityTypeBuilder<Plan> builder)
    {
        builder.ToTable("plans");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.Type).HasColumnName("type").IsRequired();
        builder.Property(x => x.Name).HasColumnName("name").HasMaxLength(100).IsRequired();
        builder.Property(x => x.DailyMessageLimit).HasColumnName("daily_message_limit").IsRequired();
        builder.Property(x => x.DailyTokenLimit).HasColumnName("daily_token_limit").IsRequired();
        builder.Property(x => x.StripeProductId).HasColumnName("stripe_product_id").HasMaxLength(200).IsRequired();

        // Ignore Flunt properties
        builder.Ignore("Notifications");
        builder.Ignore("IsValid");
    }
}
