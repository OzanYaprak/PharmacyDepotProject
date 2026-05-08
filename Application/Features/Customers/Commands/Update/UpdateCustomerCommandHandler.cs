using AutoMapper;
using Domain.Entities;
using MediatR;
using Persistence.Repositories.Customer;

namespace Application.Features.Customers.Commands.Update;

public class UpdateCustomerCommandHandler : IRequestHandler<UpdateCustomerCommand, UpdatedCustomerResponse>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IMapper _mapper;

    public UpdateCustomerCommandHandler(ICustomerRepository customerRepository, IMapper mapper)
    {
        _customerRepository = customerRepository;
        _mapper = mapper;
    }

    public async Task<UpdatedCustomerResponse> Handle(UpdateCustomerCommand request, CancellationToken cancellationToken)
    {
        Customer? customer = await _customerRepository.GetAsync(
            predicate: c => c.Id == request.Id,
            cancellationToken: cancellationToken);

        if (customer is null)
            throw new KeyNotFoundException($"Customer with id '{request.Id}' was not found.");

        _mapper.Map(request, customer);

        var result = await _customerRepository.UpdateAsync(customer, cancellationToken);
        return _mapper.Map<UpdatedCustomerResponse>(result);
    }
}
