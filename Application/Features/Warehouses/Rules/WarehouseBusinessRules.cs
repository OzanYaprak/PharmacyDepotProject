using Application.Features.Warehouses.Constants;
using Application.Rules;
using CrossCuttingConcerns.Exceptions.Types;
using Domain.Entities;
using Persistence.Repositories.Warehouse;

namespace Application.Features.Warehouses.Rules;

public class WarehouseBusinessRules : BaseBusinessRules
{
    #region Constructor And Fields

    private readonly IWarehouseRepository _warehouseRepository;

    public WarehouseBusinessRules(IWarehouseRepository warehouseRepository)
    {
        _warehouseRepository = warehouseRepository;
    }

    #endregion

    /// <summary>
    /// Insert sırasında aynı isimde depo olmamalıdır.
    /// </summary>
    public async Task NameCannotBeDuplicatedWhenInserted(string name)
    {
        Warehouse? warehouse = await _warehouseRepository.GetAsync(
            predicate: w => w.Name.ToLower().Trim() == name.ToLower().Trim());

        if (warehouse != null)
            throw new BusinessException(WarehouseMessages.NameExists);
    }

    /// <summary>
    /// Update sırasında başka bir depoda aynı isim olamaz.
    /// </summary>
    public async Task NameCannotBeDuplicatedWhenUpdated(Guid id, string name)
    {
        var dbList = await _warehouseRepository.GetListAsync(predicate: w => w.Id != id);
        List<Warehouse>? list = dbList.DataList?.ToList();

        if (list?.Any(w => w.Name.ToLower().Trim() == name.ToLower().Trim()) == true)
            throw new BusinessException(WarehouseMessages.NameExists);
    }

    /// <summary>
    /// Depo kapasitesi sıfırdan büyük olmalıdır.
    /// </summary>
    public Task CapacityMustBePositive(int capacity)
    {
        if (capacity <= 0)
            throw new BusinessException(WarehouseMessages.CapacityMustBePositive);

        return Task.CompletedTask;
    }
}
