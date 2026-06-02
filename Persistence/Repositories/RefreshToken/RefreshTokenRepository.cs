using Microsoft.EntityFrameworkCore;
using Persistence.Contexts;

namespace Persistence.Repositories.RefreshToken;

public class RefreshTokenRepository : EntityFrameworkRepositoryBase<Security.Entities.RefreshToken, int, BaseDbContext>, IRefreshTokenRepository
{
    public RefreshTokenRepository(BaseDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<List<Security.Entities.RefreshToken>> GetOldRefreshTokenAsync(int userId, int refreshTokenTTL)
    {
        List<Security.Entities.RefreshToken> oldRefreshTokens = await Query()
            .AsNoTracking()
            .Where(
            rt => 
                rt.UserId == userId 
                && 
                rt.Revoked == null
                &&
                rt.Expires >= DateTime.UtcNow
                &&
                rt.CreatedDate.AddDays(refreshTokenTTL) <= DateTime.UtcNow
                )
            .ToListAsync();

        return oldRefreshTokens;
    }
}