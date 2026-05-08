using Persistence.Repositories.Interfaces;

namespace Persistence.Repositories.Supplier;

/// <summary>
/// Supplier entity'si için özel repository arayüzü.
/// </summary>
public interface ISupplierRepository : IAsyncRepository<Domain.Entities.Supplier, Guid>
{
}
