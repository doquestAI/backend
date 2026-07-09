using Domain.Entities.Core.Chat;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.FluentMapping;

internal class ChatMessageMapping : IEntityTypeConfiguration<ChatMessage>
{
    public void Configure(EntityTypeBuilder<ChatMessage> builder)
    {
        builder.ToTable("ChatMessages");
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

        builder.Property(x => x.ChatSessionId)
            .HasColumnType("uuid")
            .IsRequired();

        builder.Property(x => x.Role)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.Content)
            .IsRequired();

        builder.HasIndex(x => x.ChatSessionId);
        builder.HasIndex(x => new { x.ChatSessionId, x.CreatedDate });
        builder.HasIndex(x => x.DeletedDate);
    }
}
