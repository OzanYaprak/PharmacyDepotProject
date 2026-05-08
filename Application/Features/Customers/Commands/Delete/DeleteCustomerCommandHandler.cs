using AutoMapper;
using Domain.Entities;
using MediatR;
using Persistence.Repositories.Customer;

namespace Application.Features.Customers.Commands.Delete;

public class DeleteCustomerCommandHandler : IRequestHandler<DeleteCustomerCommand, DeletedCustomerResponse>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IMapper _mapper;

    public DeleteCustomerCommandHandler(ICustomerRepository customerRepository, IMapper mapper)
    {
        _customerRepository = customerRepository;
        _mapper = mapper;
    }

    public async Task<DeletedCustomerResponse> Handle(DeleteCustomerCommand request, CancellationToken cancellationToken)
    {
        Customer? customer = await _customerRepository.GetAsync(
            predicate: c => c.Id == request.Id,
            cancellationToken: cancellationToken);

        if (customer is null)
            throw new KeyNotFoundException($"Customer with id '{request.Id}' was not found.");

        await _customerRepository.DeleteAsync(customer, permanent: false, cancellationToken: cancellationToken);
        return _mapper.Map<DeletedCustomerResponse>(customer);
    }
}
