using Application.Features.Customers.Constants;
using Application.Rules;
using CrossCuttingConcerns.Exceptions.Types;
using Domain.Entities;
using Persistence.Repositories.Customer;

namespace Application.Features.Customers.Rules;

public class CustomerBusinessRules : BaseBusinessRules
{
    private readonly ICustomerRepository _customerRepository;

    public CustomerBusinessRules(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task LicenseNumberCannotBeDuplicatedWhenInserted(string licenseNumber)
    {
        Customer? customer = await _customerRepository.GetAsync(predicate: c => c.LicenseNumber.ToLower().Replace(" ", "").Replace("-", "") == licenseNumber.ToLower().Replace(" ", "").Replace("-", ""));

        if (customer != null)
        {
            throw new BusinessException(CustomerMessages.LicenseNumberExists);
        }
    }
}
