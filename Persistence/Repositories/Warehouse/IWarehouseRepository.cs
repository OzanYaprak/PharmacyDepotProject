using Persistence.Repositories.Interfaces;

namespace Persistence.Repositories.Warehouse;

/// <summary>
/// Warehouse entity'si için özel repository arayüzü.
/// </summary>
public interface IWarehouseRepository : IAsyncRepository<Domain.Entities.Warehouse, Guid>
{
}
