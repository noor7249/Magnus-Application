using Magnus.API.DTOs.Auth;
using Magnus.API.Helpers;
using Magnus.API.Middleware;
using Magnus.API.Models;
using Magnus.API.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Security.Claims;

namespace Magnus.API.Services.Implementations;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ITokenService _tokenService;
    private readonly IAuditService _auditService;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        ITokenService tokenService,
        IAuditService auditService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenService = tokenService;
        _auditService = auditService;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request, CancellationToken cancellationToken = default)
    {
        const string role = RoleConstants.Employee;

        var existingUser = await _userManager.FindByEmailAsync(request.Email);
        if (existingUser is not null)
        {
            throw new AppException("A user with the same email already exists.", HttpStatusCode.Conflict);
        }

        var user = new ApplicationUser
        {
            FullName = request.FullName,
            Email = request.Email,
            UserName = request.Email,
            IsActive = true
        };

        var createResult = await _userManager.CreateAsync(user, request.Password);
        if (!createResult.Succeeded)
        {
            throw new AppException("User registration failed.", HttpStatusCode.BadRequest, createResult.Errors.Select(x => x.Description));
        }

        await _userManager.AddToRoleAsync(user, role);
        await _auditService.LogAsync("Register", nameof(ApplicationUser), user.Id, $"User registered with default role {role}.", user.Id, cancellationToken);

        return await _tokenService.CreateTokenAsync(user);
    }

    public async Task<AuthResponseDto> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(request.Email)
            ?? throw new AppException("Invalid email or password.", HttpStatusCode.Unauthorized);

        if (!user.IsActive)
        {
            throw new AppException("This user account is inactive.", HttpStatusCode.Forbidden);
        }

        var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
        if (!result.Succeeded)
        {
            throw new AppException("Invalid email or password.", HttpStatusCode.Unauthorized);
        }

        await _auditService.LogAsync("Login", nameof(ApplicationUser), user.Id, "User logged in successfully.", user.Id, cancellationToken);
        return await _tokenService.CreateTokenAsync(user);
    }

    public async Task<AuthResponseDto> RefreshTokenAsync(RefreshTokenRequestDto request, CancellationToken cancellationToken = default)
    {
        var principal = _tokenService.GetPrincipalFromExpiredToken(request.AccessToken);
        var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new AppException("Invalid access token.", HttpStatusCode.Unauthorized);

        var user = await _userManager.FindByIdAsync(userId)
            ?? throw new AppException("User not found.", HttpStatusCode.NotFound);

        var refreshTokenHash = _tokenService.HashRefreshToken(request.RefreshToken);
        var matchesCurrentHash = user.RefreshToken == refreshTokenHash;
        var matchesLegacyRawToken = user.RefreshToken == request.RefreshToken;

        if ((!matchesCurrentHash && !matchesLegacyRawToken) || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
        {
            throw new AppException("Invalid refresh token.", HttpStatusCode.Unauthorized);
        }

        await _auditService.LogAsync("RefreshToken", nameof(ApplicationUser), user.Id, "JWT token refreshed.", user.Id, cancellationToken);
        return await _tokenService.CreateTokenAsync(user);
    }

    public async Task<AuthResponseDto> RefreshTokenFromCookieAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        var refreshTokenHash = _tokenService.HashRefreshToken(refreshToken);
        var user = await _userManager.Users.FirstOrDefaultAsync(
            x => x.RefreshToken == refreshTokenHash || x.RefreshToken == refreshToken,
            cancellationToken)
            ?? throw new AppException("Invalid refresh token.", HttpStatusCode.Unauthorized);

        if (!user.IsActive || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
        {
            throw new AppException("Invalid refresh token.", HttpStatusCode.Unauthorized);
        }

        await _auditService.LogAsync("RefreshToken", nameof(ApplicationUser), user.Id, "JWT token refreshed from HttpOnly cookie.", user.Id, cancellationToken);
        return await _tokenService.CreateTokenAsync(user);
    }

    public async Task LogoutAsync(string? refreshToken, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            return;
        }

        var refreshTokenHash = _tokenService.HashRefreshToken(refreshToken);
        var user = await _userManager.Users.FirstOrDefaultAsync(
            x => x.RefreshToken == refreshTokenHash || x.RefreshToken == refreshToken,
            cancellationToken);

        if (user is null)
        {
            return;
        }

        user.RefreshToken = null;
        user.RefreshTokenExpiryTime = null;
        await _userManager.UpdateAsync(user);
        await _auditService.LogAsync("Logout", nameof(ApplicationUser), user.Id, "Refresh token revoked.", user.Id, cancellationToken);
    }
}
