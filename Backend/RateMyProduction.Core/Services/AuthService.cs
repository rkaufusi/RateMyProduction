using RateMyProduction.Core.Entities;
using RateMyProduction.Core.Interfaces;

namespace RateMyProduction.Core.Services;

public class AuthService : IAuthService
{
    private readonly IRepository<RefreshToken> _refreshTokenRepo;

    public AuthService(IRepository<RefreshToken> refreshTokenRepo)
    {
        _refreshTokenRepo = refreshTokenRepo;
    }

    public async Task SaveRefreshTokenAsync(RefreshToken token)
    {
        await _refreshTokenRepo.AddAsync(token);
        await _refreshTokenRepo.SaveChangesAsync();
    }
    public async Task<RefreshToken?> GetRefreshTokenByHashAsync(string hashedToken)
    {
        return await _refreshTokenRepo.FirstOrDefaultAsync(t => t.TokenHash == hashedToken);
    }
}