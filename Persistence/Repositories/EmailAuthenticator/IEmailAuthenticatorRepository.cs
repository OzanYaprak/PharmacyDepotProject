using Persistence.Repositories.Interfaces;

namespace Persistence.Repositories.Email;

public interface IEmailAuthenticatorRepository  : IAsyncRepository<Security.Entities.EmailAuthenticator, int>
{
}
