using System.ComponentModel.DataAnnotations;

namespace RateMyProduction.Core.DTOs.Requests;

public record RegisterRequest(
    [Required(ErrorMessage = "Username is required")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters")]
    string Username,

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Please enter a valid email address")]
    [StringLength(100, ErrorMessage = "Email must be 100 characters or less")]
    string Email,

    [Required(ErrorMessage = "Password is required")]
    [MinLength(8, ErrorMessage = "Password must be at least 8 characters")]
    string Password,

    [StringLength(100, ErrorMessage = "Display name must be 100 characters or less")]
    string? DisplayName = null);

public record LoginRequest(
    [Required(ErrorMessage = "Username or email is required")]
    string UsernameOrEmail,
    [Required(ErrorMessage = "Password is required")]
    [MinLength(8, ErrorMessage = "Password must be at least 8 characters long")]
    string Password);

public record ExchangeTokenRequest(
    [Required(ErrorMessage = "Token ID is required")]
    string TokenId
);