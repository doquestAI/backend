using DoQuest.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DoQuest.Infrastructure.Persistence.Configurations;

public sealed class OllamaModelConfiguration : IEntityTypeConfiguration<OllamaModel>
{
    public void Configure(EntityTypeBuilder<OllamaModel> builder)
    {
        builder.ToTable("ollama_models");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.ModelId).HasColumnName("model_id").HasMaxLength(200).IsRequired();
        builder.Property(x => x.DisplayName).HasColumnName("display_name").HasMaxLength(200).IsRequired();
        builder.Property(x => x.IsEmbeddingModel).HasColumnName("is_embedding_model").IsRequired();
        builder.Property(x => x.IsDefaultChat).HasColumnName("is_default_chat").IsRequired();
        builder.Property(x => x.ContextLength).HasColumnName("context_length").IsRequired();

        builder.HasIndex(x => x.ModelId).IsUnique().HasDatabaseName("ix_ollama_models_model_id");

        // Ignore Flunt properties
        builder.Ignore("Notifications");
        builder.Ignore("IsValid");
    }
}
