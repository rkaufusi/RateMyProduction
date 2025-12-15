using System;

namespace RateMyProduction.Core.Interfaces
{
    public interface IUserService
    {
        Task<UserDto?> GetByIdAsync(int id);
        Task<IReadOnlyList<UserDto>> GetAllAsync();
        Task<PagedResult<UserDto>> GetPagedAsync(int page = 1, int pageSize = 20);
        Task<UserDto?> GetByUsernameAsync(string username);
        Task<UserDto?> GetByEmailAsync(string email);
        Task<bool> UsernameExistsAsync(string username);
        Task<bool> EmailExistsAsync(string email);
    }

    // TODO: move to separate DTO file later
    public record UserDto(
        int UserID,
        string Username,
        string Email,
        string? DisplayName,
        string? PrimaryRole,
        bool IsEmailVerified,
        bool IsActive,
        DateTime DateJoined,
        DateTime? LastLogin
    );

}