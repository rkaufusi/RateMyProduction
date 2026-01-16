using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using RateMyProduction.Core.DTOs.Requests;
using RateMyProduction.Core.Entities;
using RateMyProduction.Core.Interfaces;
using RateMyProduction.Core.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace RateMyProduction.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly UserManager<User> _userManager;
    private readonly IConfiguration _configuration;
    private readonly IAuthService _refreshTokenService;

    public AuthController(
        UserManager<User> userManager,
        IConfiguration configuration,
        IAuthService refreshTokenService)
    {
        _userManager = userManager;
        _configuration = configuration;
        _refreshTokenService = refreshTokenService;
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register(RateMyProduction.Core.DTOs.Requests.RegisterRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var user = new User
        {
            UserName = request.Username,
            Email = request.Email,
            DisplayName = request.DisplayName ?? request.Username
        };

        var result = await _userManager.CreateAsync(user, request.Password);

        if (result.Succeeded)
        {
            return Ok(new { Message = "User registered successfully" });
        }

        return BadRequest(result.Errors);
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login(RateMyProduction.Core.DTOs.Requests.LoginRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var user = await _userManager.FindByNameAsync(request.UsernameOrEmail)
                   ?? await _userManager.FindByEmailAsync(request.UsernameOrEmail);

        if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
        {
            return Unauthorized(new { Message = "Invalid username/email or password" });
        }

        var accessToken = GenerateJwtToken(user);
        var refreshToken = GenerateRefreshTokenPlain();
        var hashedRefreshToken = HashRefreshToken(refreshToken);

        var refreshTokenEntity = new RefreshToken
        {
            UserID = user.Id,
            TokenHash = hashedRefreshToken,
            ExpiresAt = DateTime.UtcNow.AddDays(30),
            CreatedAt = DateTime.UtcNow,
            IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
            DeviceInfo = HttpContext.Request.Headers["User-Agent"].ToString()
        };

        await _refreshTokenService.SaveRefreshTokenAsync(refreshTokenEntity);  // ← Only service call

        return Ok(new
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,  // plain for client
            User = new
            {
                user.Id,
                user.UserName,
                user.DisplayName,
                user.Email,
                user.IsEmailVerified
            }
        });
    }

    [AllowAnonymous]
    [HttpGet("google")]
    public IActionResult GoogleLogin()
    {
        var redirectUrl = Url.Action(nameof(GoogleCallback), "Auth", null, Request.Scheme);
        var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
        return Challenge(properties, "Google");
    }

    [AllowAnonymous]
    [HttpGet("google-callback")]
    public async Task<IActionResult> GoogleCallback()
    {
        var authenticateResult = await HttpContext.AuthenticateAsync(IdentityConstants.ExternalScheme);

        if (!authenticateResult.Succeeded)
            return BadRequest(new { Message = "External authentication failed" });

        var externalUser = authenticateResult.Principal;

        var email = externalUser.FindFirst(ClaimTypes.Email)?.Value;
        var name = externalUser.FindFirst(ClaimTypes.Name)?.Value;

        if (email == null)
            return BadRequest(new { Message = "Email not provided by Google" });

        var user = await _userManager.FindByEmailAsync(email);

        if (user == null)
        {
            user = new User
            {
                UserName = email,
                Email = email,
                DisplayName = name ?? email.Split('@')[0]
            };

            var createResult = await _userManager.CreateAsync(user);
            if (!createResult.Succeeded)
                return BadRequest(createResult.Errors);
        }

        var accessToken = GenerateJwtToken(user);
        var refreshToken = GenerateRefreshTokenPlain();
        var hashedRefreshToken = HashRefreshToken(refreshToken);

        var refreshTokenEntity = new RefreshToken
        {
            UserID = user.Id,
            TokenHash = hashedRefreshToken,
            ExpiresAt = DateTime.UtcNow.AddDays(30),
            CreatedAt = DateTime.UtcNow,
            IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
            DeviceInfo = HttpContext.Request.Headers["User-Agent"].ToString()
        };

        await _refreshTokenService.SaveRefreshTokenAsync(refreshTokenEntity);

        return Ok(new
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            User = new
            {
                user.Id,
                user.UserName,
                user.DisplayName,
                user.Email,
                user.IsEmailVerified
            }
        });
    }

    [AllowAnonymous]
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(RefreshRequest request)
    {
        if (string.IsNullOrEmpty(request.RefreshToken))
            return BadRequest(new { Message = "Refresh token required" });

        using var sha256 = SHA256.Create();
        var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(request.RefreshToken));
        var incomingHash = Convert.ToBase64String(hashBytes);

        var storedToken = await _refreshTokenService.GetRefreshTokenByHashAsync(incomingHash);

        if (storedToken == null)
            return Unauthorized(new { Message = "Invalid refresh token" });

        if (storedToken.RevokedAt != null)
            return Unauthorized(new { Message = "Refresh token revoked" });

        if (storedToken.ExpiresAt < DateTime.UtcNow)
            return Unauthorized(new { Message = "Refresh token expired" });

        var user = await _userManager.FindByIdAsync(storedToken.UserID.ToString());
        if (user == null)
            return Unauthorized(new { Message = "User not found" });

        storedToken.RevokedAt = DateTime.UtcNow;

        var newAccessToken = GenerateJwtToken(user);

        var newRefreshToken = GenerateRefreshTokenPlain();
        var newHashedRefreshToken = HashRefreshToken(newRefreshToken);

        var newRefreshTokenEntity = new RefreshToken
        {
            UserID = user.Id,
            TokenHash = newHashedRefreshToken,
            ExpiresAt = DateTime.UtcNow.AddDays(30),
            CreatedAt = DateTime.UtcNow,
            IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
            DeviceInfo = HttpContext.Request.Headers["User-Agent"].ToString(),
            ReplacedByTokenID = storedToken.TokenID
        };

        await _refreshTokenService.SaveRefreshTokenAsync(newRefreshTokenEntity);

        return Ok(new
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken,
            User = new
            {
                user.Id,
                user.UserName,
                user.DisplayName,
                user.Email,
                user.IsEmailVerified
            }
        });
    }

    private string GenerateJwtToken(User user)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.UserName!),
            new Claim(ClaimTypes.Email, user.Email!)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddDays(7),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private string GenerateRefreshTokenPlain()
    {
        var bytes = RandomNumberGenerator.GetBytes(32);
        return Convert.ToBase64String(bytes);
    }

    private string HashRefreshToken(string plainToken)
    {
        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(plainToken);
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }
}