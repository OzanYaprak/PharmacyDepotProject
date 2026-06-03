using Application.Features.Suppliers.Constants;
using Application.Rules;
using CrossCuttingConcerns.Exceptions.Types;
using Domain.Entities;
using Persistence.Repositories.Supplier;

namespace Application.Features.Suppliers.Rules;

public class SupplierBusinessRules(ISupplierRepository supplierRepository) : BaseBusinessRules
{
    /// <summary>
    /// Insert sırasında aynı telefon numarasına sahip tedarikçi olmamalıdır.
    /// </summary>
    public async Task PhoneNumberCannotBeDuplicatedWhenInserted(string phone)
    {
        Supplier? supplier = await supplierRepository.GetAsync(
            predicate: s => s.Phone != null && s.Phone.Replace(" ", "").Replace("-", "") == phone.Replace(" ", "").Replace("-", "").Replace("+", ""));

        if (supplier != null)
            throw new BusinessException(SupplierMessages.PhoneNumberExists);
    }

    /// <summary>
    /// Update sırasında başka bir tedarikçide aynı telefon numarası olamaz.
    /// </summary>
    public async Task PhoneNumberCannotBeDuplicatedWhenUpdated(Guid id, string phone)
    {
        var dbList = await supplierRepository.GetListAsync(predicate: s => s.Id != id);
        List<Supplier>? list = dbList.DataList?.ToList();

        if (list?.Any(s => s.Phone != null && s.Phone.Replace(" ", "").Replace("-", "") == phone.Replace(" ", "").Replace("-", "").Replace("+", "")) is true)
            throw new BusinessException(SupplierMessages.PhoneNumberExists);
    }

    /// <summary>
    /// Insert sırasında aynı e-posta adresine sahip tedarikçi olmamalıdır.
    /// </summary>
    public async Task EmailCannotBeDuplicatedWhenInserted(string email)
    {
        Supplier? supplier = await supplierRepository.GetAsync(
            predicate: s => s.Email != null && string.Equals(s.Email, email, StringComparison.OrdinalIgnoreCase));

        if (supplier != null)
            throw new BusinessException(SupplierMessages.EmailExists);
    }

    /// <summary>
    /// Update sırasında başka bir tedarikçide aynı e-posta adresi olamaz.
    /// </summary>
    public async Task EmailCannotBeDuplicatedWhenUpdated(Guid id, string email)
    {
        var dbList = await supplierRepository.GetListAsync(predicate: s => s.Id != id);
        List<Supplier>? list = dbList.DataList?.ToList();

        if (list?.Any(s => s.Email != null && string.Equals(s.Email, email, StringComparison.OrdinalIgnoreCase)) is true)
            throw new BusinessException(SupplierMessages.EmailExists);
    }
}
