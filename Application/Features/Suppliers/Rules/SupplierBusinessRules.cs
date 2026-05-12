using Application.Features.Suppliers.Constants;
using Application.Rules;
using CrossCuttingConcerns.Exceptions.Types;
using Domain.Entities;
using Persistence.Repositories.Supplier;

namespace Application.Features.Suppliers.Rules;

public class SupplierBusinessRules : BaseBusinessRules
{
    #region Constructor And Fields

    private readonly ISupplierRepository _supplierRepository;
    public SupplierBusinessRules(ISupplierRepository supplierRepository)
    {
        _supplierRepository = supplierRepository;
    }

    #endregion

    /// <summary>
    /// Insert sırasında aynı telefon numarasına sahip tedarikçi olmamalıdır.
    /// </summary>
    public async Task PhoneNumberCannotBeDuplicatedWhenInserted(string phone)
    {
        Supplier? supplier = await _supplierRepository.GetAsync(
            predicate: s => s.Phone.Replace(" ", "").Replace("-", "") == phone.Replace(" ", "").Replace("-", "").Replace("+", ""));

        if (supplier != null)
            throw new BusinessException(SupplierMessages.PhoneNumberExists);
    }

    /// <summary>
    /// Update sırasında başka bir tedarikçide aynı telefon numarası olamaz.
    /// </summary>
    public async Task PhoneNumberCannotBeDuplicatedWhenUpdated(Guid id, string phone)
    {
        var dbList = await _supplierRepository.GetListAsync(predicate: s => s.Id != id);
        List<Supplier>? list = dbList.DataList?.ToList();

        if (list?.Any(s => s.Phone.Replace(" ", "").Replace("-", "") == phone.Replace(" ", "").Replace("-", "").Replace("+", "")) == true)
            throw new BusinessException(SupplierMessages.PhoneNumberExists);
    }

    /// <summary>
    /// Insert sırasında aynı e-posta adresine sahip tedarikçi olmamalıdır.
    /// </summary>
    public async Task EmailCannotBeDuplicatedWhenInserted(string email)
    {
        Supplier? supplier = await _supplierRepository.GetAsync(
            predicate: s => s.Email.ToLower() == email.ToLower());

        if (supplier != null)
            throw new BusinessException(SupplierMessages.EmailExists);
    }

    /// <summary>
    /// Update sırasında başka bir tedarikçide aynı e-posta adresi olamaz.
    /// </summary>
    public async Task EmailCannotBeDuplicatedWhenUpdated(Guid id, string email)
    {
        var dbList = await _supplierRepository.GetListAsync(predicate: s => s.Id != id);
        List<Supplier>? list = dbList.DataList?.ToList();

        if (list?.Any(s => s.Email.ToLower() == email.ToLower()) == true)
            throw new BusinessException(SupplierMessages.EmailExists);
    }
}
