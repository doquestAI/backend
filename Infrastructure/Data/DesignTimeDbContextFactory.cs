using Domain.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace Infrastructure.Data;

internal class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<CoreDbContext>
{
    public CoreDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("databaseSettings.json", optional: false, reloadOnChange: true)
            .Build();

        var dbSettings = new DatabaseSettings();
        configuration.GetSection("DatabaseSettings").Bind(dbSettings);

        var builder = new NpgsqlConnectionStringBuilder
        {
            Host = dbSettings.Host,
            Port = dbSettings.Port,
            Database = dbSettings.Database,
            Username = dbSettings.Username,
            Password = dbSettings.Password
        };
        var connectionString = builder.ConnectionString;

        if (string.IsNullOrEmpty(connectionString))
            throw new Exception("A connection string must be provided.");

        var optionsBuilder = new DbContextOptionsBuilder<CoreDbContext>();
        optionsBuilder.UseNpgsql(connectionString);

        return new CoreDbContext(optionsBuilder.Options);
    }
}