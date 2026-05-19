using Application.Common.Responses;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Persistence.Paging;
using Persistence.Repositories.Customer;

namespace Application.Features.Customers.Queries.GetListByDynamic;

public class GetListByDynamicCustomerQueryHandler : IRequestHandler<GetListByDynamicCustomerQuery, GetListResponse<GetListByDynamicCustomerListItemDto>>
{
    #region Constructor Injection

    private readonly ICustomerRepository _customerRepository;
    private readonly IMapper _mapper;
    public GetListByDynamicCustomerQueryHandler(ICustomerRepository customerRepository, IMapper mapper)
    {
        _customerRepository = customerRepository;
        _mapper = mapper;
    }

    #endregion

    public async Task<GetListResponse<GetListByDynamicCustomerListItemDto>> Handle(GetListByDynamicCustomerQuery request, CancellationToken cancellationToken)
    {
        Paginate<Customer> customers = await _customerRepository
            .GetListByDynamicAsync(
            dynamic: request.DynamicQuery!,
            pageNumber: request.PageRequest?.PageNumber ?? 0,
            pageSize: request.PageRequest?.PageSize ?? 10,
            withDeleted: true,
            cancellationToken: cancellationToken);

        GetListResponse<GetListByDynamicCustomerListItemDto> response = _mapper.Map<GetListResponse<GetListByDynamicCustomerListItemDto>>(customers);
        return response;
    }
}
