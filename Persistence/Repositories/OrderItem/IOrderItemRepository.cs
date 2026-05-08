using Persistence.Repositories.Interfaces;

namespace Persistence.Repositories.OrderItem;

/// <summary>
/// OrderItem entity'si için özel repository arayüzü.
/// </summary>
public interface IOrderItemRepository : IAsyncRepository<Domain.Entities.OrderItem, Guid>
{
}
