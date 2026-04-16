namespace Ecommerce.Application.Abstractions.Auth;

public sealed record TokenPair(string AccessToken, string RefreshToken, DateTimeOffset RefreshTokenExpiresAt);

public interface IJwtTokenService
{
    TokenPair GenerateTokenPair(Guid userId, string email, IEnumerable<string> roles);
    string HashRefreshToken(string refreshToken);
}
