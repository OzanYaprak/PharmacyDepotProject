using Application.Features.Sales.Constants;
using Application.Rules;
using CrossCuttingConcerns.Exceptions.Types;
using Persistence.Repositories.Sale;

namespace Application.Features.Sales.Rules;

public class SaleBusinessRules : BaseBusinessRules
{
    #region Constructor And Fields
    
    private readonly ISaleRepository _saleRepository;
    public SaleBusinessRules(ISaleRepository saleRepository)
    {
        _saleRepository = saleRepository;
    }

    #endregion

    /// <summary>
    /// Satış toplam tutarı sıfırdan büyük olmalıdır.
    /// </summary>
    public async Task TotalAmountMustBePositive(decimal totalAmount)
    {
        if (totalAmount <= 0)
            throw new BusinessException(SaleMessages.TotalAmountMustBePositive);

        await Task.CompletedTask;
    }

    /// <summary>
    /// Satış tarihi gelecek bir tarih olamaz.
    /// </summary>
    public async Task SaleDateCannotBeInTheFuture(DateTime saleDate)
    {
        if (saleDate > DateTime.UtcNow)
            throw new BusinessException(SaleMessages.SaleDateCannotBeInTheFuture);

        await Task.CompletedTask;
    }
}
