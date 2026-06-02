using Persistence.Repositories.Interfaces;

namespace Persistence.Repositories.User;

public interface IUserRepository : IAsyncRepository<Security.Entities.User, int>
{
}
