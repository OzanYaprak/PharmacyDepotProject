using Persistence.Repositories.Interfaces;

namespace Persistence.Repositories.OtpAuthenticator;

public interface IOtpAuthenticatorRepository : IAsyncRepository<Security.Entities.OtpAuthenticator, int>
{
}
