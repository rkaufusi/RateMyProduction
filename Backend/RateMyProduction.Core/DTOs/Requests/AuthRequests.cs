namespace RateMyProduction.Core.DTOs.Requests;

public record RegisterRequest(
    string Username,
    string Email,
    string Password,
    string? DisplayName = null);

public record LoginRequest(
    string UsernameOrEmail,
    string Password);