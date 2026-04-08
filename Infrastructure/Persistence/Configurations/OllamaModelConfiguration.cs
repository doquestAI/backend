using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

internal sealed class OllamaModelConfiguration : IEntityTypeConfiguration<OllamaModel>
{
    public void Configure(EntityTypeBuilder<OllamaModel> builder)
    {
        builder.ToTable("ollama_models");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.IsEmbeddingModel).HasColumnName("is_embedding_model").IsRequired();
        builder.Property(x => x.IsDefaultChat).HasColumnName("is_default_chat").IsRequired();
        builder.Property(x => x.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at");
        builder.Property(x => x.DeletedAt).HasColumnName("deleted_at");

        // Value Objects
        builder.OwnsOne(x => x.ModelId, vo =>
        {
            vo.Property(v => v.Value).HasColumnName("model_id").HasMaxLength(200).IsRequired();
            vo.Ignore("Notifications");
            vo.Ignore("IsValid");
        });

        builder.OwnsOne(x => x.DisplayName, vo =>
        {
            vo.Property(v => v.Value).HasColumnName("display_name").HasMaxLength(200).IsRequired();
            vo.Ignore("Notifications");
            vo.Ignore("IsValid");
        });

        builder.OwnsOne(x => x.ContextLength, vo =>
        {
            vo.Property(v => v.Value).HasColumnName("context_length").IsRequired();
            vo.Ignore("Notifications");
            vo.Ignore("IsValid");
        });

        builder.HasIndex("ModelId_Value").IsUnique().HasDatabaseName("ix_ollama_models_model_id");

        // Ignore Flunt properties
        builder.Ignore("Notifications");
        builder.Ignore("IsValid");
    }
}