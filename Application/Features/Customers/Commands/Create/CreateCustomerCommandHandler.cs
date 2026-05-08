using AutoMapper;
using Domain.Entities;
using MediatR;
using Persistence.Repositories.Customer;

namespace Application.Features.Customers.Commands.Create;

public class CreateCustomerCommandHandler : IRequestHandler<CreateCustomerCommand, CreatedCustomerResponse>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IMapper _mapper;

    public CreateCustomerCommandHandler(ICustomerRepository customerRepository, IMapper mapper)
    {
        _customerRepository = customerRepository;
        _mapper = mapper;
    }

    public async Task<CreatedCustomerResponse> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
    {
        Customer customer = _mapper.Map<Customer>(request);
        customer.Id = Guid.NewGuid();

        var result = await _customerRepository.AddAsync(customer, cancellationToken);

        return _mapper.Map<CreatedCustomerResponse>(result);
    }
}
