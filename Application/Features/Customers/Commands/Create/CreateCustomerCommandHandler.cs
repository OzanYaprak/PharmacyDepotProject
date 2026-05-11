using Application.Features.Customers.Rules;
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
        await _customerBusinessRules.LicenseNumberCannotBeDuplicatedWhenInserted(request.LicenseNumber);

        Customer customer = _mapper.Map<Customer>(request);
        customer.Id = Guid.NewGuid();

        var result = await _customerRepository.AddAsync(customer, cancellationToken);

        return _mapper.Map<CreatedCustomerResponse>(result);
    }
}
