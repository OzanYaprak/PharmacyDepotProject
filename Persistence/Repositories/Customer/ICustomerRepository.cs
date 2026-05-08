using Persistence.Repositories.Interfaces;

namespace Persistence.Repositories.Customer;

/// <summary>
/// Customer entity'si için özel repository arayüzü.
/// </summary>
public interface ICustomerRepository : IAsyncRepository<Domain.Entities.Customer, Guid>
{
}
