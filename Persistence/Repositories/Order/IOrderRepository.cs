using Persistence.Repositories.Interfaces;

namespace Persistence.Repositories.Order;

/// <summary>
/// Order entity'si için özel repository arayüzü.
/// </summary>
public interface IOrderRepository : IAsyncRepository<Domain.Entities.Order, Guid>
{
}
