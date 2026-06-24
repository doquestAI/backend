using Domain.Entities.Abstracts;
using Domain.Entities.Core;
using Domain.Entities.Core.Subscriptions;
using Domain.Entities.Payments;
using Flunt.Notifications;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Infrastructure.Data;

internal class CoreDbContext(DbContextOptions<CoreDbContext> options) : DbContext(options)
{
    public DbSet<Document> Documents { get; init; }
    public DbSet<Subscription> Subscriptions { get; init; }
    public DbSet<FreeSubscription> FreeSubscriptions { get; init; }
    public DbSet<PremiumSubscription> PremiumSubscriptions { get; init; }
    public DbSet<StripeWebhookEvent> StripeWebhookEvents { get; init; }
    public DbSet<AccountSubscription> AccountSubscriptions { get; init; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Ignore<Notification>();
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}