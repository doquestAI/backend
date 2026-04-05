using DoQuest.Domain.Constants;
using DoQuest.Domain.Entities;
using DoQuest.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace DoQuest.Infrastructure.Persistence;

public sealed class DoQuestDbContext(DbContextOptions<DoQuestDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Plan> Plans => Set<Plan>();
    public DbSet<Vestibular> Vestibulares => Set<Vestibular>();
    public DbSet<Document> Documents => Set<Document>();
    public DbSet<DocumentChunk> DocumentChunks => Set<DocumentChunk>();
    public DbSet<OllamaModel> OllamaModels => Set<OllamaModel>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("vector");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DoQuestDbContext).Assembly);

        // Seed Plans with well-known GUIDs
        SeedPlans(modelBuilder);
    }

    private static void SeedPlans(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Plan>().HasData(
            new
            {
                Id = Plan.FreePlanId,
                Type = PlanType.Free,
                Name = "Free",
                DailyMessageLimit = 10,
                DailyTokenLimit = 50_000,
                StripeProductId = "prod_free"
            },
            new
            {
                Id = Plan.ProPlanId,
                Type = PlanType.Pro,
                Name = "Pro",
                DailyMessageLimit = 100,
                DailyTokenLimit = 500_000,
                StripeProductId = "prod_pro"
            },
            new
            {
                Id = Plan.MaxPlanId,
                Type = PlanType.Max,
                Name = "Max",
                DailyMessageLimit = -1,
                DailyTokenLimit = -1,
                StripeProductId = "prod_max"
            });
    }
}
