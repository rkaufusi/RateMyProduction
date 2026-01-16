using RateMyProduction.Core.Entities;

namespace RateMyProduction.Core.Interfaces;

public interface IAuthService
{
    Task SaveRefreshTokenAsync(RefreshToken token);
    Task<RefreshToken?> GetRefreshTokenByHashAsync(string refreshTokenHash);
}