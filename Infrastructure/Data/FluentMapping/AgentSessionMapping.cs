using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.FluentMapping;

internal class AgentSessionMapping : IEntityTypeConfiguration<AgentSessionRecord>
{
    public void Configure(EntityTypeBuilder<AgentSessionRecord> builder)
    {
        builder.ToTable("AgentSessions");

        builder.HasKey(s => s.Id).HasName("PK_AgentSessions");

        builder.Property(s => s.Id)
            .HasColumnType("uuid")
            .IsRequired();

        builder.Property(s => s.SessionKey)
            .HasColumnName("SessionKey")
            .HasColumnType("varchar")
            .HasMaxLength(500)
            .IsRequired();

        builder.HasIndex(s => s.SessionKey)
            .IsUnique()
            .HasDatabaseName("IX_AgentSessions_SessionKey");

        builder.Property(s => s.AgentName)
            .HasColumnName("AgentName")
            .HasColumnType("varchar")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(s => s.SessionJson)
            .HasColumnName("SessionJson")
            .HasColumnType("jsonb")
            .IsRequired();

        builder.Property(s => s.CreatedAt)
            .HasColumnName("CreatedAt")
            .HasColumnType("timestamptz")
            .IsRequired();

        builder.Property(s => s.UpdatedAt)
            .HasColumnName("UpdatedAt")
            .HasColumnType("timestamptz")
            .IsRequired();
    }
}
