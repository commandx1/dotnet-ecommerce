using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Ecommerce.Infrastructure.Persistence;

public sealed class DbInitializer : IDbInitializer
{
    public static readonly string[] Roles = ["Buyer", "Vendor"];

    private readonly ApplicationDbContext _dbContext;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;
    private readonly ILogger<DbInitializer> _logger;

    public DbInitializer(
        ApplicationDbContext dbContext,
        RoleManager<IdentityRole<Guid>> roleManager,
        ILogger<DbInitializer> logger)
    {
        _dbContext = dbContext;
        _roleManager = roleManager;
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
    }
}
