using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

internal sealed class ChatMessageConfiguration : IEntityTypeConfiguration<ChatMessage>
{
    public void Configure(EntityTypeBuilder<ChatMessage> builder)
    {
        builder.ToTable("chat_messages");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.ChatSessionId).HasColumnName("chat_session_id").IsRequired();
        builder.Property(x => x.Role).HasColumnName("role").IsRequired()
               .HasConversion<string>();
        builder.Property(x => x.Content).HasColumnName("content").HasColumnType("text").IsRequired();
        builder.Property(x => x.TokenCount).HasColumnName("token_count").IsRequired();
        builder.Property(x => x.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at");
        builder.Property(x => x.DeletedAt).HasColumnName("deleted_at");

        builder.HasIndex(x => x.ChatSessionId).HasDatabaseName("ix_chat_messages_chat_session_id");
        builder.HasIndex(x => new { x.ChatSessionId, x.CreatedAt }).HasDatabaseName("ix_chat_messages_session_created");

        builder.Ignore("Notifications");
        builder.Ignore("IsValid");
    }
}