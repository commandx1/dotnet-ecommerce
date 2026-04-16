using Ecommerce.Api.Common;
using Ecommerce.Api.Contracts;
using Ecommerce.Application.Abstractions.Auth;
using Ecommerce.Application.Abstractions.Persistence;
using Ecommerce.Domain.Auth;
using Ecommerce.Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class AuthController : ControllerBase
{
    private static readonly string[] AllowedRoles = ["Buyer", "Vendor"];

    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AuthController(
        UserManager<ApplicationUser> userManager,
        IJwtTokenService jwtTokenService,
        IRefreshTokenRepository refreshTokenRepository,
        IUnitOfWork unitOfWork)
    {
        _userManager = userManager;
        _jwtTokenService = jwtTokenService;
        _refreshTokenRepository = refreshTokenRepository;
        _unitOfWork = unitOfWork;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponse>> Register(RegisterRequest request, CancellationToken cancellationToken)
    {
        var normalizedRole = request.Role.Trim();
        if (!AllowedRoles.Contains(normalizedRole, StringComparer.OrdinalIgnoreCase))
        {
            return BadRequest("Role must be Buyer or Vendor.");
        }

        var user = new ApplicationUser
        {
            Email = request.Email.Trim(),
            UserName = request.Email.Trim()
        };

        var createResult = await _userManager.CreateAsync(user, request.Password);
        if (!createResult.Succeeded)
        {
            return BadRequest(createResult.Errors);
        }

        var roleResult = await _userManager.AddToRoleAsync(user, normalizedRole);
        if (!roleResult.Succeeded)
        {
            return BadRequest(roleResult.Errors);
        }

        return await IssueTokensAsync(user, cancellationToken);
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request, CancellationToken cancellationToken)
    {
        var user = await _userManager.Users.FirstOrDefaultAsync(x => x.Email == request.Email.Trim(), cancellationToken);
        if (user is null)
        {
            return Unauthorized();
        }

        var validPassword = await _userManager.CheckPasswordAsync(user, request.Password);
        if (!validPassword)
        {
            return Unauthorized();
        }

        return await IssueTokensAsync(user, cancellationToken);
    }

    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponse>> Refresh(RefreshRequest request, CancellationToken cancellationToken)
    {
        var incomingHash = _jwtTokenService.HashRefreshToken(request.RefreshToken);
        var existing = await _refreshTokenRepository.GetByTokenHashAsync(incomingHash, cancellationToken);

        if (existing is null || !existing.IsActive(DateTimeOffset.UtcNow))
        {
            return Unauthorized();
        }

        var user = await _userManager.FindByIdAsync(existing.UserId.ToString());
        if (user is null)
        {
            return Unauthorized();
        }

        var roles = await _userManager.GetRolesAsync(user);
        var tokenPair = _jwtTokenService.GenerateTokenPair(user.Id, user.Email!, roles);
        var replacementHash = _jwtTokenService.HashRefreshToken(tokenPair.RefreshToken);

        existing.RevokedAt = DateTimeOffset.UtcNow;
        existing.ReplacedByTokenHash = replacementHash;
        _refreshTokenRepository.Update(existing);

        await _refreshTokenRepository.AddAsync(new RefreshToken
        {
            UserId = user.Id,
            TokenHash = replacementHash,
            ExpiresAt = tokenPair.RefreshTokenExpiresAt
        }, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Ok(new AuthResponse(tokenPair.AccessToken, tokenPair.RefreshToken, tokenPair.RefreshTokenExpiresAt));
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout(LogoutRequest request, CancellationToken cancellationToken)
    {
        var userId = User.GetRequiredUserId();
        var now = DateTimeOffset.UtcNow;

        if (request.RevokeAllSessions || string.IsNullOrWhiteSpace(request.RefreshToken))
        {
            var activeTokens = await _refreshTokenRepository.GetActiveByUserIdAsync(userId, cancellationToken);
            foreach (var token in activeTokens)
            {
                token.RevokedAt = now;
                _refreshTokenRepository.Update(token);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return NoContent();
        }

        var refreshTokenHash = _jwtTokenService.HashRefreshToken(request.RefreshToken);
        var tokenEntity = await _refreshTokenRepository.GetByTokenHashAsync(refreshTokenHash, cancellationToken);
        if (tokenEntity is null || tokenEntity.UserId != userId)
        {
            return NoContent();
        }

        if (tokenEntity.RevokedAt is null)
        {
            tokenEntity.RevokedAt = now;
            _refreshTokenRepository.Update(tokenEntity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        return NoContent();
    }

    private async Task<ActionResult<AuthResponse>> IssueTokensAsync(ApplicationUser user, CancellationToken cancellationToken)
    {
        var roles = await _userManager.GetRolesAsync(user);
        var tokenPair = _jwtTokenService.GenerateTokenPair(user.Id, user.Email!, roles);
        var refreshHash = _jwtTokenService.HashRefreshToken(tokenPair.RefreshToken);

        await _refreshTokenRepository.AddAsync(new RefreshToken
        {
            UserId = user.Id,
            TokenHash = refreshHash,
            ExpiresAt = tokenPair.RefreshTokenExpiresAt
        }, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Ok(new AuthResponse(tokenPair.AccessToken, tokenPair.RefreshToken, tokenPair.RefreshTokenExpiresAt));
    }
}
