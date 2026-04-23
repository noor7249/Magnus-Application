using Magnus.API.DTOs.Auth;
using Magnus.API.Helpers;
using Magnus.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Magnus.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private const string RefreshTokenCookieName = "__Host-magnus-refresh";
    private const string CsrfCookieName = "XSRF-TOKEN";
    private const string CsrfHeaderName = "X-XSRF-TOKEN";
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Register([FromBody] RegisterRequestDto request, CancellationToken cancellationToken)
    {
        var result = await _authService.RegisterAsync(request, cancellationToken);
        SetAuthCookies(result.RefreshToken);
        SuppressRefreshTokenForBrowserClients(result);
        return Ok(ApiResponse<AuthResponseDto>.SuccessResponse(result, "User registered successfully."));
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Login([FromBody] LoginRequestDto request, CancellationToken cancellationToken)
    {
        var result = await _authService.LoginAsync(request, cancellationToken);
        SetAuthCookies(result.RefreshToken);
        SuppressRefreshTokenForBrowserClients(result);
        return Ok(ApiResponse<AuthResponseDto>.SuccessResponse(result, "Login successful."));
    }

    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Refresh(CancellationToken cancellationToken)
    {
        ValidateCsrfToken();

        if (!Request.Cookies.TryGetValue(RefreshTokenCookieName, out var refreshToken) || string.IsNullOrWhiteSpace(refreshToken))
        {
            return Unauthorized(ApiResponse<object>.FailureResponse("Refresh token cookie is missing.", traceId: HttpContext.TraceIdentifier));
        }

        var result = await _authService.RefreshTokenFromCookieAsync(refreshToken, cancellationToken);
        SetAuthCookies(result.RefreshToken);
        result.RefreshToken = string.Empty;
        return Ok(ApiResponse<AuthResponseDto>.SuccessResponse(result, "Token refreshed successfully."));
    }

    [HttpPost("refresh-token")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<AuthResponseDto>>> RefreshToken([FromBody] RefreshTokenRequestDto request, CancellationToken cancellationToken)
    {
        var result = await _authService.RefreshTokenAsync(request, cancellationToken);
        return Ok(ApiResponse<AuthResponseDto>.SuccessResponse(result, "Token refreshed successfully."));
    }

    [HttpPost("logout")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<object>>> Logout(CancellationToken cancellationToken)
    {
        ValidateCsrfToken();
        Request.Cookies.TryGetValue(RefreshTokenCookieName, out var refreshToken);
        await _authService.LogoutAsync(refreshToken, cancellationToken);
        ClearAuthCookies();
        return Ok(ApiResponse<object>.SuccessResponse(null, "Logout successful."));
    }

    private void SetAuthCookies(string refreshToken)
    {
        Response.Cookies.Append(RefreshTokenCookieName, refreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Lax,
            Path = "/",
            Expires = DateTimeOffset.UtcNow.AddDays(7)
        });

        Response.Cookies.Append(CsrfCookieName, Guid.NewGuid().ToString("N"), new CookieOptions
        {
            HttpOnly = false,
            Secure = true,
            SameSite = SameSiteMode.Lax,
            Path = "/",
            Expires = DateTimeOffset.UtcNow.AddDays(7)
        });
    }

    private void ClearAuthCookies()
    {
        Response.Cookies.Delete(RefreshTokenCookieName, new CookieOptions
        {
            Secure = true,
            SameSite = SameSiteMode.Lax,
            Path = "/"
        });

        Response.Cookies.Delete(CsrfCookieName, new CookieOptions
        {
            Secure = true,
            SameSite = SameSiteMode.Lax,
            Path = "/"
        });
    }

    private void ValidateCsrfToken()
    {
        var cookieToken = Request.Cookies[CsrfCookieName];
        var headerToken = Request.Headers[CsrfHeaderName].FirstOrDefault();

        if (string.IsNullOrWhiteSpace(cookieToken) ||
            string.IsNullOrWhiteSpace(headerToken) ||
            !string.Equals(cookieToken, headerToken, StringComparison.Ordinal))
        {
            throw new UnauthorizedAccessException("Invalid CSRF token.");
        }
    }

    private void SuppressRefreshTokenForBrowserClients(AuthResponseDto result)
    {
        if (Request.Headers.ContainsKey("Origin"))
        {
            result.RefreshToken = string.Empty;
        }
    }
}
