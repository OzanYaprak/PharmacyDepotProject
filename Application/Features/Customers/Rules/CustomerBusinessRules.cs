using Application.Features.Customers.Constants;
using Application.Rules;
using Azure.Core;
using CrossCuttingConcerns.Exceptions.Types;
using Domain.Entities;
using Persistence.Repositories.Customer;

namespace Application.Features.Customers.Rules;

public class CustomerBusinessRules : BaseBusinessRules
{
    #region Constructor And Fields

    private readonly ICustomerRepository _customerRepository;
    public CustomerBusinessRules(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    #endregion

    /// <summary>
    /// Insert sırasında aynı Lisans numarasına sahip müşteri olmamalıdır.
    /// </summary>
    public async Task LicenseNumberCannotBeDuplicatedWhenInserted(string licenseNumber)
    {
        Customer? customer = await _customerRepository.GetAsync(predicate: c => c.LicenseNumber.ToLower().Replace(" ", "").Replace("-", "") == licenseNumber.ToLower().Replace(" ", "").Replace("-", ""));

        if (customer != null)
        {
            throw new BusinessException(CustomerMessages.LicenseNumberExists);
        }
    }

    /// <summary>
    /// Insert sırasında aynı Telefon numarasına sahip müşteri olmamalıdır.
    /// </summary>
    public async Task PhoneNumberCannotBeDuplicatedWhenInserted(string phoneNumber)
    {
        Customer? customer = await _customerRepository.GetAsync(predicate: c => c.Phone.Replace(" ", "").Replace("-", "") == phoneNumber.Replace(" ", "").Replace("-", "").Replace("+", ""));

        if (customer != null)
        {
            throw new BusinessException(CustomerMessages.PhoneNumberExists);
        }
    }

    /// <summary>
    /// Update sırasında başka bir müşteride aynı Lisans numarası olamaz.
    /// </summary>
    public async Task LicenseNumberCannotBeDuplicatedWhenUpdated(Guid id, string licenseNumber)
    {
        var dbDataList = await _customerRepository.GetListAsync(predicate: x => x.Id != id);
        List<Customer>? list = dbDataList.DataList?.ToList();
        if (list?.Any(
            predicate: x => x.LicenseNumber.ToLower().Replace(" ", "").Replace("-", "")
            ==
            licenseNumber.ToLower().Replace(" ", "").Replace("-", ""))
            == true)
        {
            throw new BusinessException(CustomerMessages.LicenseNumberExists);
        }
    }

    /// <summary>
    /// Update sırasında başka bir müşteride aynı Telefon numarası olamaz.
    /// </summary>
    public async Task PhoneNumberCannotBeDuplicatedWhenUpdated(Guid id, string phoneNumber)
    {
        var dbDataList = await _customerRepository.GetListAsync(predicate: x => x.Id != id);
        List<Customer>? list = dbDataList.DataList?.ToList();

        if (list?.Any(x => x.Phone.Replace(" ", "").Replace("-", "")
            == phoneNumber.Replace(" ", "").Replace("-", "").Replace("+", ""))
            == true)
        {
            throw new BusinessException(CustomerMessages.PhoneNumberExists);
        }
    }
}
