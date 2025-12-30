using Azure;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using RateMyProduction.Core.Entities;

namespace RateMyProduction.Infrastructure.Data;

public class RateMyProductionContext : IdentityDbContext<User, IdentityRole<int>, int> //DbContext
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

        modelBuilder.Entity<User>()
            .HasIndex(u => u.UserName)
            .IsUnique();

        modelBuilder.Entity<RefreshToken>()
            .HasIndex(t => t.TokenHash);

        modelBuilder.Entity<RefreshToken>()
            .HasIndex(t => t.ExpiresAt);

        modelBuilder.Entity<Production>()
            .HasIndex(p => p.Title);

        modelBuilder.Entity<Production>()
            .HasIndex(p => p.Studio);

        modelBuilder.Entity<Review>()
            .HasIndex(r => new { r.ProductionID, r.DatePosted });

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

        // FIX FOR TRIGGER
        modelBuilder.Entity<Review>()
            .ToTable(tb => tb.UseSqlOutputClause(false));

        // removed to fix duplicates when inheritying from IdentityDbContext
        //modelBuilder.Entity<User>(entity =>
        //{
        //    entity.ToTable("Users"); // Use your existing table name
        //    entity.Property(u => u.Id).HasColumnName("UserID");
        //    entity.Property(u => u.UserName).HasColumnName("Username");
        //    // Add more mappings if needed (e.g., EmailConfirmed → IsEmailVerified)
        //});

        //modelBuilder.Entity<IdentityRole<int>>().ToTable("Roles");
        // You can map other Identity tables if you want custom names

        // Use your existing "Users" table for user data (instead of creating AspNetUsers)
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("Users");
            entity.Property(u => u.Id).HasColumnName("UserID"); // Map Id → UserID
            entity.Property(u => u.UserName).HasColumnName("Username"); // Map UserName → Username
            // Optional: map EmailConfirmed to your IsEmailVerified if you want
            // entity.Property(u => u.EmailConfirmed).HasColumnName("IsEmailVerified");
        });

        // Map the other Identity tables to custom names (they will be created if they don't exist)
        modelBuilder.Entity<IdentityRole<int>>().ToTable("Roles");
        modelBuilder.Entity<IdentityUserRole<int>>().ToTable("UserRoles");
        modelBuilder.Entity<IdentityUserClaim<int>>().ToTable("UserClaims");
        modelBuilder.Entity<IdentityUserLogin<int>>().ToTable("UserLogins");
        modelBuilder.Entity<IdentityUserToken<int>>().ToTable("UserTokens");
        modelBuilder.Entity<IdentityRoleClaim<int>>().ToTable("RoleClaims");
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