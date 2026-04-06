using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.PlanId).HasColumnName("plan_id").IsRequired();
        builder.Property(x => x.DailyMessageCount).HasColumnName("daily_message_count").IsRequired();
        builder.Property(x => x.LastMessageDate).HasColumnName("last_message_date").IsRequired();
        builder.Property(x => x.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at");
        builder.Property(x => x.DeletedAt).HasColumnName("deleted_at");

        // Value Objects
        builder.OwnsOne(x => x.FirebaseUid, vo =>
        {
            vo.Property(v => v.Value).HasColumnName("firebase_uid").HasMaxLength(200).IsRequired();
            vo.Ignore("Notifications");
            vo.Ignore("IsValid");
        });

        builder.OwnsOne(x => x.Email, vo =>
        {
            vo.Property(v => v.Address).HasColumnName("email").HasMaxLength(300).IsRequired();
            vo.Ignore("Notifications");
            vo.Ignore("IsValid");
        });

        builder.HasIndex("FirebaseUid_Value").IsUnique().HasDatabaseName("ix_users_firebase_uid");

        builder.HasOne(x => x.Plan)
               .WithMany()
               .HasForeignKey(x => x.PlanId)
               .OnDelete(DeleteBehavior.Restrict);

        // Ignore Flunt properties
        builder.Ignore("Notifications");
        builder.Ignore("IsValid");
    }
}
