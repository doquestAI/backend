using Domain.Entities.Abstracts;
using Domain.Entities.Core;
using Domain.Entities.Payments;
using Flunt.Notifications;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Infrastructure.Data;

internal class CoreDbContext : DbContext
{
    public CoreDbContext(DbContextOptions<CoreDbContext> options) : base(options)
    {
    }

    public DbSet<Role> Roles { get; init; }
    public DbSet<User> Users { get; init; }
    public DbSet<RefreshToken> RefreshTokens { get; init; }
    public DbSet<Document> Documents { get; init; }
    public DbSet<Subscription> Subscriptions { get; init; }
    public DbSet<FreeSubscription> FreeSubscriptions { get; init; }
    public DbSet<PremiumSubscription> PremiumSubscriptions { get; init; }
    public DbSet<StripeWebhookEvent> StripeWebhookEvents { get; init; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Ignore<Notification>();
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
