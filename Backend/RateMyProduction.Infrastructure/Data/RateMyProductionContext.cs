using Azure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using RateMyProduction.Core.Entities;

namespace RateMyProduction.Infrastructure.Data;

public class RateMyProductionContext : DbContext
{
    public RateMyProductionContext(DbContextOptions<RateMyProductionContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<Production> Productions => Set<Production>();
    public DbSet<Review> Reviews => Set<Review>();
    public DbSet<Tag> Tags => Set<Tag>();
    public DbSet<ReviewTag> ReviewTags => Set<ReviewTag>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Users - Unique Email
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        // RefreshToken indexes
        modelBuilder.Entity<RefreshToken>()
            .HasIndex(t => t.TokenHash);

        modelBuilder.Entity<RefreshToken>()
            .HasIndex(t => t.ExpiresAt);

        // Productions indexes
        modelBuilder.Entity<Production>()
            .HasIndex(p => p.Title);

        modelBuilder.Entity<Production>()
            .HasIndex(p => p.Studio);

        // Reviews - One review per user per production
        modelBuilder.Entity<Review>()
            .HasIndex(r => new { r.UserID, r.ProductionID })
            .IsUnique();

        // Reviews - Optimized query for latest reviews per production
        modelBuilder.Entity<Review>()
            .HasIndex(r => new { r.ProductionID, r.DatePosted });

        // Tags - Unique name
        modelBuilder.Entity<Tag>()
            .HasIndex(t => t.TagName)
            .IsUnique();

        // ReviewTag - Composite primary key + relationships
        modelBuilder.Entity<ReviewTag>()
            .HasKey(rt => new { rt.ReviewID, rt.TagID });

        modelBuilder.Entity<ReviewTag>()
            .HasOne(rt => rt.Review)
            .WithMany(r => r.ReviewTags)
            .HasForeignKey(rt => rt.ReviewID)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ReviewTag>()
            .HasOne(rt => rt.Tag)
            .WithMany(t => t.ReviewTags)
            .HasForeignKey(rt => rt.TagID)
            .OnDelete(DeleteBehavior.NoAction);
    }

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