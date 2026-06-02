using Persistence.Repositories.Interfaces;

namespace Persistence.Repositories.RefreshToken;

public interface IRefreshTokenRepository : IAsyncRepository<Security.Entities.RefreshToken, int>
{
    Task<List<Security.Entities.RefreshToken>> GetOldRefreshTokenAsync(int userId, int refreshTokenTTL);
}
