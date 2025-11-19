using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace RateMyProduction.Infrastructure.Data;

public class RateMyProductionContext : DbContext
{
    public RateMyProductionContext(DbContextOptions<RateMyProductionContext> options)
        : base(options)
    {
    }

    // We'll add DbSets here very soon
    // public DbSet<User> Users => Set<User>();
    // ADD THIS AT THE VERY BOTTOM OF RateMyProductionContext.cs
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<RateMyProductionContext>
    {
        public RateMyProductionContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.Development.json", optional: true)
                .AddJsonFile("appsettings.json", optional: true)
                .Build();

            var connectionString = configuration.GetConnectionString("RateMyProductionDb")
                ?? throw new InvalidOperationException(
                    "Connection string 'RateMyProductionDb' not found. " +
                    "Add it to appsettings.Development.json (which is git-ignored) in the RateMyProduction.Api project.");

            var optionsBuilder = new DbContextOptionsBuilder<RateMyProductionContext>();
            optionsBuilder.UseSqlServer(connectionString);

            return new RateMyProductionContext(optionsBuilder.Options);
        }
    }
}