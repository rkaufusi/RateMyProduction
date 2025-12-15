using RateMyProduction.Core.Entities;
using RateMyProduction.Core.Interfaces;

namespace RateMyProduction.Core.Services
{
    public class UserService : IUserService
    {
        private readonly IRepository<User> _userRepository;

        public UserService(IRepository<User> userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<UserDto?> GetByIdAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            return user is null ? null : ToDto(user);
        }

        public async Task<IReadOnlyList<UserDto>> GetAllAsync()
        {
            var users = await _userRepository.ListAllAsync();
            return users.Select(ToDto).ToList();
        }

        public async Task<PagedResult<UserDto>> GetPagedAsync(int page = 1, int pageSize = 20)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 20;

            var allUsers = await _userRepository.ListAllAsync();

            var totalCount = allUsers.Count;

            var pagedUsers = allUsers
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(ToDto)
                .ToList();

            return new PagedResult<UserDto>(pagedUsers, page, pageSize)
            {
                TotalCount = totalCount
            };
        }

        public async Task<UserDto?> GetByUsernameAsync(string username)
        {
            var user = await _userRepository.FirstOrDefaultAsync(u => u.Username == username);
            return user is null ? null : ToDto(user);
        }

        public async Task<UserDto?> GetByEmailAsync(string email)
        {
            var user = await _userRepository.FirstOrDefaultAsync(u => u.Email == email);
            return user is null ? null : ToDto(user);
        }

        public async Task<bool> UsernameExistsAsync(string username)
            => await _userRepository.AnyAsync(u => u.Username == username);

        public async Task<bool> EmailExistsAsync(string email)
            => await _userRepository.AnyAsync(u => u.Email == email);

        private static UserDto ToDto(User u) => new(
            u.UserID,
            u.Username,
            u.Email,
            u.DisplayName,
            u.PrimaryRole,
            u.IsEmailVerified,
            u.IsActive,
            u.DateJoined,
            u.LastLogin
        );
    }
}