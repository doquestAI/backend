using Domain.Constants;
using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

internal sealed class CoreDbContext(DbContextOptions<CoreDbContext> options) : DbContext(options)
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
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CoreDbContext).Assembly);

        // Seed Plans with well-known GUIDs
        SeedPlans(modelBuilder);
    }

    private static void SeedPlans(ModelBuilder modelBuilder)
    {
        var seedDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        modelBuilder.Entity<Plan>().HasData(
            new
            {
                Id = Plan.FreePlanId,
                Type = PlanType.Free,
                CreatedAt = seedDate
            },
            new
            {
                Id = Plan.ProPlanId,
                Type = PlanType.Pro,
                CreatedAt = seedDate
            },
            new
            {
                Id = Plan.MaxPlanId,
                Type = PlanType.Max,
                CreatedAt = seedDate
            });

        // Seed owned VOs separately
        modelBuilder.Entity<Plan>().OwnsOne(p => p.Name).HasData(
            new { PlanId = Plan.FreePlanId, Value = "Free" },
            new { PlanId = Plan.ProPlanId, Value = "Pro" },
            new { PlanId = Plan.MaxPlanId, Value = "Max" });

        modelBuilder.Entity<Plan>().OwnsOne(p => p.DailyMessageLimit).HasData(
            new { PlanId = Plan.FreePlanId, Value = 10 },
            new { PlanId = Plan.ProPlanId, Value = 100 },
            new { PlanId = Plan.MaxPlanId, Value = -1 });

        modelBuilder.Entity<Plan>().OwnsOne(p => p.DailyTokenLimit).HasData(
            new { PlanId = Plan.FreePlanId, Value = 50_000 },
            new { PlanId = Plan.ProPlanId, Value = 500_000 },
            new { PlanId = Plan.MaxPlanId, Value = -1 });

        modelBuilder.Entity<Plan>().OwnsOne(p => p.StripeProductId).HasData(
            new { PlanId = Plan.FreePlanId, Value = "prod_free" },
            new { PlanId = Plan.ProPlanId, Value = "prod_pro" },
            new { PlanId = Plan.MaxPlanId, Value = "prod_max" });
    }
}
