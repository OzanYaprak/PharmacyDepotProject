using Persistence.Repositories.Interfaces;

namespace Persistence.Repositories.Stock;

/// <summary>
/// Stock entity'si için özel repository arayüzü.
/// </summary>
public interface IStockRepository : IAsyncRepository<Domain.Entities.Stock, Guid>
{
}
