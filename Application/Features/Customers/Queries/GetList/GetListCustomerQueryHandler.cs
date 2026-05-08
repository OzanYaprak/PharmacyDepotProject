using Application.Common.Responses;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Persistence.Paging;
using Persistence.Repositories.Customer;

namespace Application.Features.Customers.Queries.GetList;

public class GetListCustomerQueryHandler : IRequestHandler<GetListCustomerQuery, GetListResponse<GetListCustomerListItemDto>>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IMapper _mapper;

    public GetListCustomerQueryHandler(ICustomerRepository customerRepository, IMapper mapper)
    {
        _customerRepository = customerRepository;
        _mapper = mapper;
    }

    public async Task<GetListResponse<GetListCustomerListItemDto>> Handle(GetListCustomerQuery request, CancellationToken cancellationToken)
    {
        Paginate<Customer> customers = await _customerRepository.GetListAsync(
            pageNumber: request.PageRequest?.PageNumber ?? 0,
            pageSize: request.PageRequest?.PageSize ?? 10,
            cancellationToken: cancellationToken);

        return _mapper.Map<GetListResponse<GetListCustomerListItemDto>>(customers);
    }
}
