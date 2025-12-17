using System;
using System.Collections.Generic;
using System.Text;

namespace RateMyProduction.Core.DTOs.Responses
{
    public record UserDTOs(
        int UserID,
        string Username,
        string Email,
        string? DisplayName,
        string? PrimaryRole,
        bool IsEmailVerified,
        bool IsActive,
        DateTime DateJoined,
        DateTime? LastLogin);
}
