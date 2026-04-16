using Ecommerce.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Ecommerce.Infrastructure.Persistence;

public sealed class DbInitializer : IDbInitializer
{
    public static readonly string[] Roles = ["Buyer", "Vendor"];

    private readonly ApplicationDbContext _dbContext;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<DbInitializer> _logger;

    public DbInitializer(
        ApplicationDbContext dbContext,
        RoleManager<IdentityRole<Guid>> roleManager,
        UserManager<ApplicationUser> userManager,
        ILogger<DbInitializer> logger)
    {
        _dbContext = dbContext;
        _roleManager = roleManager;
        _userManager = userManager;
        _logger = logger;
    }

    public async Task MigrateAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Applying database migrations...");
        await _dbContext.Database.MigrateAsync(cancellationToken);

        foreach (var role in Roles)
        {
            if (await _roleManager.RoleExistsAsync(role))
            {
                continue;
            }

            await _roleManager.CreateAsync(new IdentityRole<Guid>(role));
        }

        await EnsureSeedUsersAsync();
    }

    private async Task EnsureSeedUsersAsync()
    {
        await EnsureSeedUserAsync("buyer@local.dev", "Buyer");
        await EnsureSeedUserAsync("vendor@local.dev", "Vendor");
    }

    private async Task EnsureSeedUserAsync(string email, string role)
    {
        var user = await _userManager.Users.FirstOrDefaultAsync(x => x.Email == email);
        if (user is not null)
        {
            return;
        }

        var newUser = new ApplicationUser
        {
            UserName = email,
            Email = email,
            EmailConfirmed = true
        };

        var createResult = await _userManager.CreateAsync(newUser, "Passw0rd!");
        if (!createResult.Succeeded)
        {
            var errors = string.Join(", ", createResult.Errors.Select(x => x.Description));
            throw new InvalidOperationException($"Seed user creation failed: {errors}");
        }

        var roleResult = await _userManager.AddToRoleAsync(newUser, role);
        if (!roleResult.Succeeded)
        {
            var errors = string.Join(", ", roleResult.Errors.Select(x => x.Description));
            throw new InvalidOperationException($"Seed role assignment failed: {errors}");
        }
    }
}
