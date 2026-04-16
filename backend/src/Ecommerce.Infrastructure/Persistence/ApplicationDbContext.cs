using Ecommerce.Domain.Auth;
using Ecommerce.Domain.Catalog;
using Ecommerce.Domain.Orders;
using Ecommerce.Domain.Questions;
using Ecommerce.Domain.Reviews;
using Ecommerce.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Infrastructure.Persistence;

public sealed class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Product> Products => Set<Product>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<Review> Reviews => Set<Review>();
    public DbSet<Question> Questions => Set<Question>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Product>(entity =>
        {
            entity.ToTable("products");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Name).IsRequired().HasMaxLength(150);
            entity.Property(x => x.Description).IsRequired().HasMaxLength(2000);
            entity.Property(x => x.Price).HasPrecision(18, 2);
            entity.Property(x => x.ImageUrl).HasMaxLength(500);
            entity.HasIndex(x => x.VendorId);
            entity.HasQueryFilter(x => !x.IsDeleted);
        });

        builder.Entity<Order>(entity =>
        {
            entity.ToTable("orders");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.TotalAmount).HasPrecision(18, 2);
            entity.HasIndex(x => x.BuyerId);
            entity.HasQueryFilter(x => !x.IsDeleted);
        });

        builder.Entity<OrderItem>(entity =>
        {
            entity.ToTable("order_items");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.UnitPrice).HasPrecision(18, 2);
            entity.HasIndex(x => x.OrderId);
            entity.HasIndex(x => x.ProductId);
            entity.HasOne<Order>()
                .WithMany(x => x.Items)
                .HasForeignKey(x => x.OrderId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasQueryFilter(x => !x.IsDeleted);
        });

        builder.Entity<Review>(entity =>
        {
            entity.ToTable("reviews");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Comment).IsRequired().HasMaxLength(1000);
            entity.Property(x => x.ReplyText).HasMaxLength(1000);
            entity.HasIndex(x => x.ProductId);
            entity.HasIndex(x => x.BuyerId);
            entity.HasQueryFilter(x => !x.IsDeleted);
        });

        builder.Entity<Question>(entity =>
        {
            entity.ToTable("questions");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.QuestionText).IsRequired().HasMaxLength(1000);
            entity.Property(x => x.AnswerText).HasMaxLength(1000);
            entity.HasIndex(x => x.ProductId);
            entity.HasIndex(x => x.BuyerId);
            entity.HasQueryFilter(x => !x.IsDeleted);
        });

        builder.Entity<RefreshToken>(entity =>
        {
            entity.ToTable("refresh_tokens");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.TokenHash).IsRequired().HasMaxLength(128);
            entity.Property(x => x.ReplacedByTokenHash).HasMaxLength(128);
            entity.HasIndex(x => x.TokenHash).IsUnique();
            entity.HasIndex(x => x.UserId);
            entity.HasQueryFilter(x => !x.IsDeleted);
        });

        builder.Entity<ApplicationUser>(entity =>
        {
            entity.ToTable("users");
        });

        builder.Entity<IdentityRole<Guid>>(entity =>
        {
            entity.ToTable("roles");
        });

        builder.Entity<IdentityUserRole<Guid>>(entity =>
        {
            entity.ToTable("user_roles");
        });

        builder.Entity<IdentityUserClaim<Guid>>(entity =>
        {
            entity.ToTable("user_claims");
        });

        builder.Entity<IdentityUserLogin<Guid>>(entity =>
        {
            entity.ToTable("user_logins");
        });

        builder.Entity<IdentityRoleClaim<Guid>>(entity =>
        {
            entity.ToTable("role_claims");
        });

        builder.Entity<IdentityUserToken<Guid>>(entity =>
        {
            entity.ToTable("user_tokens");
        });
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var utcNow = DateTimeOffset.UtcNow;

        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry.State is EntityState.Added)
            {
                if (entry.Properties.Any(x => x.Metadata.Name == nameof(Ecommerce.Domain.Common.BaseAuditableEntity.CreatedAt)))
                {
                    entry.Property(nameof(Ecommerce.Domain.Common.BaseAuditableEntity.CreatedAt)).CurrentValue = utcNow;
                }

                if (entry.Properties.Any(x => x.Metadata.Name == nameof(Ecommerce.Domain.Common.BaseAuditableEntity.UpdatedAt)))
                {
                    entry.Property(nameof(Ecommerce.Domain.Common.BaseAuditableEntity.UpdatedAt)).CurrentValue = utcNow;
                }

                if (entry.Properties.Any(x => x.Metadata.Name == nameof(ApplicationUser.CreatedAt)))
                {
                    entry.Property(nameof(ApplicationUser.CreatedAt)).CurrentValue = utcNow;
                    entry.Property(nameof(ApplicationUser.UpdatedAt)).CurrentValue = utcNow;
                }
            }

            if (entry.State is EntityState.Modified)
            {
                if (entry.Properties.Any(x => x.Metadata.Name == nameof(Ecommerce.Domain.Common.BaseAuditableEntity.UpdatedAt)))
                {
                    entry.Property(nameof(Ecommerce.Domain.Common.BaseAuditableEntity.UpdatedAt)).CurrentValue = utcNow;
                }

                if (entry.Properties.Any(x => x.Metadata.Name == nameof(ApplicationUser.UpdatedAt)))
                {
                    entry.Property(nameof(ApplicationUser.UpdatedAt)).CurrentValue = utcNow;
                }
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}
