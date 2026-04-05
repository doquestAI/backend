using DoQuest.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DoQuest.Infrastructure.Persistence.Configurations;

public sealed class DocumentConfiguration : IEntityTypeConfiguration<Document>
{
    public void Configure(EntityTypeBuilder<Document> builder)
    {
        builder.ToTable("documents");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.Title).HasColumnName("title").HasMaxLength(500).IsRequired();
        builder.Property(x => x.SourceUrl).HasColumnName("source_url").HasMaxLength(2000).IsRequired();
        builder.Property(x => x.VestibularId).HasColumnName("vestibular_id").IsRequired();
        builder.Property(x => x.CreatedAt).HasColumnName("created_at").IsRequired();

        builder.HasOne(x => x.Vestibular)
               .WithMany()
               .HasForeignKey(x => x.VestibularId)
               .OnDelete(DeleteBehavior.Restrict);

        // Map the private _chunks backing field
        builder.HasMany(x => x.Chunks)
               .WithOne(x => x.Document)
               .HasForeignKey(x => x.DocumentId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(x => x.Chunks).HasField("_chunks").UsePropertyAccessMode(PropertyAccessMode.Field);

        // Ignore Flunt properties
        builder.Ignore("Notifications");
        builder.Ignore("IsValid");
    }
}
