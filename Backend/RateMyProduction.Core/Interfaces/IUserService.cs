using System;
using RateMyProduction.Core.DTOs.Responses;

namespace RateMyProduction.Core.Interfaces
{
    public interface IUserService
    {
        Task<UserDTOs?> GetByIdAsync(int id);
        Task<IReadOnlyList<UserDTOs>> GetAllAsync();
        Task<PagedResult<UserDTOs>> GetPagedAsync(int page = 1, int pageSize = 20);
        Task<UserDTOs?> GetByUsernameAsync(string username);
        Task<UserDTOs?> GetByEmailAsync(string email);
        Task<bool> UsernameExistsAsync(string username);
        Task<bool> EmailExistsAsync(string email);
    }

    //// TODO: move to separate DTO file later
    //public record UserDto(
    //    int UserID,
    //    string Username,
    //    string Email,
    //    string? DisplayName,
    //    string? PrimaryRole,
    //    bool IsEmailVerified,
    //    bool IsActive,
    //    DateTime DateJoined,
    //    DateTime? LastLogin
    //);

}