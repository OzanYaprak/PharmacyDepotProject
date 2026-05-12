using Application.Features.Customers.Rules;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Persistence.Repositories.Customer;

namespace Application.Features.Customers.Commands.Update;

public class UpdateCustomerCommandHandler : IRequestHandler<UpdateCustomerCommand, UpdatedCustomerResponse>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IMapper _mapper;
    private readonly CustomerBusinessRules _customerBusinessRules;

    public UpdateCustomerCommandHandler(ICustomerRepository customerRepository, IMapper mapper, CustomerBusinessRules customerBusinessRules)
    {
        _customerRepository = customerRepository;
        _mapper = mapper;
        _customerBusinessRules = customerBusinessRules;
    }

    public async Task<UpdatedCustomerResponse> Handle(UpdateCustomerCommand request, CancellationToken cancellationToken)
    {
        await _customerBusinessRules.LicenseNumberCannotBeDuplicatedWhenUpdated(request.Id, request.LicenseNumber!);
        await _customerBusinessRules.PhoneNumberCannotBeDuplicatedWhenUpdated(request.Id, request.Phone!);

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
