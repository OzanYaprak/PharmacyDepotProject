using Persistence.Repositories.Interfaces;

namespace Persistence.Repositories.SaleItem;


/// <summary>
/// SaleItem entity'si için özel repository arayüzü.
/// </summary>
public interface ISaleItemRepository : IAsyncRepository<Domain.Entities.SaleItem, Guid>
{
}
