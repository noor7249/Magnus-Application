using Magnus.API.DTOs.Auth;
using Magnus.API.Models;
using System.Security.Claims;

namespace Magnus.API.Services.Interfaces;

public interface ITokenService
{
    Task<AuthResponseDto> CreateTokenAsync(ApplicationUser user);
    ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    string GenerateRefreshToken();
}
