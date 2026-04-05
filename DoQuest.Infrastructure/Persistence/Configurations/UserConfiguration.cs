using DoQuest.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DoQuest.Infrastructure.Persistence.Configurations;

public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.FirebaseUid).HasColumnName("firebase_uid").HasMaxLength(200).IsRequired();
        builder.Property(x => x.Email).HasColumnName("email").HasMaxLength(300).IsRequired();
        builder.Property(x => x.PlanId).HasColumnName("plan_id").IsRequired();
        builder.Property(x => x.DailyMessageCount).HasColumnName("daily_message_count").IsRequired();
        builder.Property(x => x.LastMessageDate).HasColumnName("last_message_date").IsRequired();
        builder.Property(x => x.CreatedAt).HasColumnName("created_at").IsRequired();

        builder.HasIndex(x => x.FirebaseUid).IsUnique().HasDatabaseName("ix_users_firebase_uid");

        builder.HasOne(x => x.Plan)
               .WithMany()
               .HasForeignKey(x => x.PlanId)
               .OnDelete(DeleteBehavior.Restrict);

        // Ignore Flunt properties
        builder.Ignore("Notifications");
        builder.Ignore("IsValid");
    }
}
