using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

internal sealed class PlanConfiguration : IEntityTypeConfiguration<Plan>
{
    public void Configure(EntityTypeBuilder<Plan> builder)
    {
        builder.ToTable("plans");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.Type).HasColumnName("type").IsRequired();
        builder.Property(x => x.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at");
        builder.Property(x => x.DeletedAt).HasColumnName("deleted_at");

        // Value Objects
        builder.OwnsOne(x => x.Name, vo =>
        {
            vo.Property(v => v.Value).HasColumnName("name").HasMaxLength(100).IsRequired();
            vo.Ignore("Notifications");
            vo.Ignore("IsValid");
        });

        builder.OwnsOne(x => x.DailyMessageLimit, vo =>
        {
            vo.Property(v => v.Value).HasColumnName("daily_message_limit").IsRequired();
            vo.Ignore("Notifications");
            vo.Ignore("IsValid");
        });

        builder.OwnsOne(x => x.DailyTokenLimit, vo =>
        {
            vo.Property(v => v.Value).HasColumnName("daily_token_limit").IsRequired();
            vo.Ignore("Notifications");
            vo.Ignore("IsValid");
        });

        builder.OwnsOne(x => x.StripeProductId, vo =>
        {
            vo.Property(v => v.Value).HasColumnName("stripe_product_id").HasMaxLength(200).IsRequired();
            vo.Ignore("Notifications");
            vo.Ignore("IsValid");
        });

        // Ignore Flunt properties
        builder.Ignore("Notifications");
        builder.Ignore("IsValid");
    }
}