namespace Ecommerce.Infrastructure.Auth;

public sealed class JwtOptions
{
    public const string SectionName = "Jwt";

    public string Issuer { get; set; } = "Ecommerce.Api";
    public string Audience { get; set; } = "Ecommerce.Client";
    public string Key { get; set; } = "dev-key-change-me-at-least-32-characters";
    public int AccessTokenMinutes { get; set; } = 30;
    public int RefreshTokenDays { get; set; } = 7;
}
