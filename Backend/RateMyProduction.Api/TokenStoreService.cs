namespace RateMyProduction.Api
{
    using Microsoft.Extensions.Caching.Memory;
    using System;

    public class TokenStoreService
    {
        private readonly IMemoryCache _cache;
        private readonly TimeSpan _tokenIdExpiration = TimeSpan.FromMinutes(5);

        public TokenStoreService(IMemoryCache cache)
        {
            _cache = cache;
        }

        public string StoreTokens(string accessToken, string refreshToken, string username)
        {
            var tokenId = Guid.NewGuid().ToString();
            var tokenData = new TokenData
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                Username = username
            };

            _cache.Set(tokenId, tokenData, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = _tokenIdExpiration
            });

            return tokenId;
        }

        public TokenData GetAndInvalidateToken(string tokenId)
        {
            if (_cache.TryGetValue(tokenId, out TokenData tokenData))
            {
                _cache.Remove(tokenId); // Invalidate after retrieval
                return tokenData;
            }
            return null;
        }
    }

    public class TokenData
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public string Username { get; set; }
    }
}
