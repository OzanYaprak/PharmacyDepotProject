using Persistence.Contexts;
using Persistence.Repositories.Email;

namespace Persistence.Repositories.EmailAuthenticator;

public class EmailAuthenticatorRepository : EntityFrameworkRepositoryBase<Security.Entities.EmailAuthenticator, int, BaseDbContext>, IEmailAuthenticatorRepository
{
    public EmailAuthenticatorRepository(BaseDbContext dbContext) : base(dbContext)
    {
    }
}
