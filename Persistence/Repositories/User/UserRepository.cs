using Persistence.Contexts;

namespace Persistence.Repositories.User;

public class UserRepository : EntityFrameworkRepositoryBase<Security.Entities.User, int, BaseDbContext>, IUserRepository
{
    public UserRepository(BaseDbContext dbContext) : base(dbContext)
    {
    }
}
