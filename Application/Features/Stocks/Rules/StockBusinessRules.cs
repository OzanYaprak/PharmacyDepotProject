using Application.Features.Stocks.Constants;
using Application.Rules;
using CrossCuttingConcerns.Exceptions.Types;
using Domain.Entities;
using Persistence.Repositories.Stock;

namespace Application.Features.Stocks.Rules;

public class StockBusinessRules : BaseBusinessRules
{
    #region Constructor And Fields

    private readonly IStockRepository _stockRepository;

    public StockBusinessRules(IStockRepository stockRepository)
    {
        _stockRepository = stockRepository;
    }

    #endregion

    /// <summary>
    /// Insert sırasında aynı ilaç-depo kombinasyonuna ait stok kaydı olmamalıdır.
    /// </summary>
    public async Task StockCannotBeDuplicatedForSameDrugAndWarehouseWhenInserted(Guid drugId, Guid warehouseId)
    {
        Stock? stock = await _stockRepository.GetAsync(
            predicate: s => s.DrugId == drugId && s.WarehouseId == warehouseId);

        if (stock != null)
            throw new BusinessException(StockMessages.StockAlreadyExistsForDrugAndWarehouse);
    }

    /// <summary>
    /// Stok miktarı negatif olamaz.
    /// </summary>
    public async Task QuantityMustBeNonNegative(int quantity)
    {
        if (quantity < 0)
            throw new BusinessException(StockMessages.QuantityMustBeNonNegative);

        await Task.CompletedTask;
    }

    /// <summary>
    /// Birim fiyat sıfırdan büyük olmalıdır.
    /// </summary>
    public async Task UnitPriceMustBePositive(decimal unitPrice)
    {
        if (unitPrice <= 0)
            throw new BusinessException(StockMessages.UnitPriceMustBePositive);

        await Task.CompletedTask;
    }
}
