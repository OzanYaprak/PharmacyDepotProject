using Persistence.Contexts;

namespace Persistence.Repositories.OtpAuthenticator;

public class OtpAuthenticatorRepository : EntityFrameworkRepositoryBase<Security.Entities.OtpAuthenticator, int, BaseDbContext>, IOtpAuthenticatorRepository
{
    public OtpAuthenticatorRepository(BaseDbContext dbContext) : base(dbContext)
    {
    }
}
