using Application.Features.Customers.Rules;
using Application.Pipelines.Transaction;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Persistence.Repositories.Customer;

namespace Application.Features.Customers.Commands.Create;

public class CreateCustomerCommandHandler : IRequestHandler<CreateCustomerCommand, CreatedCustomerResponse>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IMapper _mapper;
    private readonly CustomerBusinessRules _customerBusinessRules;

    public CreateCustomerCommandHandler(ICustomerRepository customerRepository, IMapper mapper, CustomerBusinessRules customerBusinessRules)
    {
        _customerRepository = customerRepository;
        _mapper = mapper;
        _customerBusinessRules = customerBusinessRules;
    }

    public async Task<CreatedCustomerResponse> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
    {
        await _customerBusinessRules.LicenseNumberCannotBeDuplicatedWhenInserted(licenseNumber: request.LicenseNumber);
        await _customerBusinessRules.PhoneNumberCannotBeDuplicatedWhenInserted(phoneNumber: request.Phone);

        Customer customer = _mapper.Map<Customer>(request);
        customer.Id = Guid.NewGuid();

        // TransactionScopeBehavior will ensure that if any of the following operations fail, the entire transaction will be rolled back.
        //Customer customer2 = _mapper.Map<Customer>(request);
        //customer2.Id = Guid.NewGuid();

        var result = await _customerRepository.AddAsync(customer, cancellationToken);
        //var result2 = await _customerRepository.AddAsync(customer2, cancellationToken);

        return _mapper.Map<CreatedCustomerResponse>(result);
    }
}
