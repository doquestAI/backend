using DoQuest.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DoQuest.Infrastructure.Persistence.Configurations;

public sealed class VestibularConfiguration : IEntityTypeConfiguration<Vestibular>
{
    public void Configure(EntityTypeBuilder<Vestibular> builder)
    {
        builder.ToTable("vestibulares");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.Name).HasColumnName("name").HasMaxLength(200).IsRequired();
        builder.Property(x => x.Type).HasColumnName("type").IsRequired();
        builder.Property(x => x.Year).HasColumnName("year").IsRequired();
        builder.Property(x => x.Description).HasColumnName("description").HasMaxLength(1000);

        // Ignore Flunt properties
        builder.Ignore("Notifications");
        builder.Ignore("IsValid");
    }
}
