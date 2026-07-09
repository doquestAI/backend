using Domain.Entities.Core.Chat;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.FluentMapping;

internal class ChatSessionMapping : IEntityTypeConfiguration<ChatSession>
{
    public void Configure(EntityTypeBuilder<ChatSession> builder)
    {
        builder.ToTable("ChatSessions");
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

        builder.Property(x => x.Title)
            .HasMaxLength(500)
            .IsRequired(false);

        builder.Property(x => x.Description)
            .HasMaxLength(2000)
            .IsRequired(false);

        builder.Property(x => x.ContextUserId)
            .HasColumnType("uuid")
            .IsRequired();

        builder.Property(x => x.EndedAt)
            .HasColumnType("timestamptz")
            .IsRequired(false);

        builder.HasMany(x => x.Messages)
            .WithOne()
            .HasForeignKey(m => m.ChatSessionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.ContextUserId);
        builder.HasIndex(x => x.DeletedDate);
    }
}
