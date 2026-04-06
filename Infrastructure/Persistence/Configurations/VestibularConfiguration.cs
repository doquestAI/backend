using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

internal sealed class VestibularConfiguration : IEntityTypeConfiguration<Vestibular>
{
    public void Configure(EntityTypeBuilder<Vestibular> builder)
    {
        builder.ToTable("vestibulares");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.Type).HasColumnName("type").IsRequired();
        builder.Property(x => x.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at");
        builder.Property(x => x.DeletedAt).HasColumnName("deleted_at");

        // Value Objects
        builder.OwnsOne(x => x.Name, vo =>
        {
            vo.Property(v => v.Value).HasColumnName("name").HasMaxLength(200).IsRequired();
            vo.Ignore("Notifications");
            vo.Ignore("IsValid");
        });

        builder.OwnsOne(x => x.Year, vo =>
        {
            vo.Property(v => v.Value).HasColumnName("year").IsRequired();
            vo.Ignore("Notifications");
            vo.Ignore("IsValid");
        });

        builder.OwnsOne(x => x.Description, vo =>
        {
            vo.Property(v => v.Value).HasColumnName("description").HasMaxLength(1000);
            vo.Ignore("Notifications");
            vo.Ignore("IsValid");
        });

        // Ignore Flunt properties
        builder.Ignore("Notifications");
        builder.Ignore("IsValid");
    }
}
