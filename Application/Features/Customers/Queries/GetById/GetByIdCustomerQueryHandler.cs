using AutoMapper;
using Domain.Entities;
using MediatR;
using Persistence.Repositories.Customer;

namespace Application.Features.Customers.Queries.GetById;

public class GetByIdCustomerQueryHandler : IRequestHandler<GetByIdCustomerQuery, GetByIdCustomerResponse>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IMapper _mapper;

    public GetByIdCustomerQueryHandler(ICustomerRepository customerRepository, IMapper mapper)
    {
        _customerRepository = customerRepository;
        _mapper = mapper;
    }

    public async Task<GetByIdCustomerResponse> Handle(GetByIdCustomerQuery request, CancellationToken cancellationToken)
    {
        Customer? customer = await _customerRepository.GetAsync(
            predicate: c => c.Id == request.Id,
            cancellationToken: cancellationToken);

        if (customer is null)
            throw new KeyNotFoundException($"Customer with id '{request.Id}' was not found.");

        return _mapper.Map<GetByIdCustomerResponse>(customer);
    }
}
