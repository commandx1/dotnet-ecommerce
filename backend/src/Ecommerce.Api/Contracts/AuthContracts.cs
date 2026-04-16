namespace Ecommerce.Api.Contracts;

public sealed record RegisterRequest(string Email, string Password, string Role);
public sealed record LoginRequest(string Email, string Password);
public sealed record RefreshRequest(string RefreshToken);
public sealed record LogoutRequest(string? RefreshToken, bool RevokeAllSessions = false);
public sealed record AuthResponse(string AccessToken, string RefreshToken, DateTimeOffset RefreshTokenExpiresAt);
