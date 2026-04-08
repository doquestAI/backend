using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

internal sealed class ChatSessionConfiguration : IEntityTypeConfiguration<ChatSession>
{
    public void Configure(EntityTypeBuilder<ChatSession> builder)
    {
        builder.ToTable("chat_sessions");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.UserId).HasColumnName("user_id").IsRequired();
        builder.Property(x => x.VestibularId).HasColumnName("vestibular_id");
        builder.Property(x => x.Title).HasColumnName("title").HasMaxLength(500).IsRequired();
        builder.Property(x => x.SerializedAgentSession).HasColumnName("serialized_agent_session").HasColumnType("text");
        builder.Property(x => x.LastMessageAt).HasColumnName("last_message_at");
        builder.Property(x => x.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at");
        builder.Property(x => x.DeletedAt).HasColumnName("deleted_at");

        builder.HasOne(x => x.User)
               .WithMany()
               .HasForeignKey(x => x.UserId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Vestibular)
               .WithMany()
               .HasForeignKey(x => x.VestibularId)
               .OnDelete(DeleteBehavior.SetNull)
               .IsRequired(false);

        builder.HasMany(x => x.Messages)
               .WithOne(x => x.ChatSession)
               .HasForeignKey(x => x.ChatSessionId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(x => x.Messages).HasField("_messages").UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasIndex(x => x.UserId).HasDatabaseName("ix_chat_sessions_user_id");
        builder.HasIndex(x => x.VestibularId).HasDatabaseName("ix_chat_sessions_vestibular_id");
        builder.HasIndex(x => x.LastMessageAt).HasDatabaseName("ix_chat_sessions_last_message_at");

        builder.Ignore("Notifications");
        builder.Ignore("IsValid");
    }
}