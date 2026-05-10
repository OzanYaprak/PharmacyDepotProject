using AutoMapper;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence.Repositories.Sale;

namespace Application.Features.Sales.Queries.GetById;

public class GetByIdSaleQueryHandler : IRequestHandler<GetByIdSaleQuery, GetByIdSaleResponse>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;

    public GetByIdSaleQueryHandler(ISaleRepository saleRepository, IMapper mapper)
    {
        _saleRepository = saleRepository;
        _mapper = mapper;
    }

    public async Task<GetByIdSaleResponse> Handle(GetByIdSaleQuery request, CancellationToken cancellationToken)
    {
        Sale? sale = await _saleRepository.GetAsync(
            include: s => s.Include(s => s.Customer),
            predicate: s => s.Id == request.Id,
            cancellationToken: cancellationToken);

        if (sale is null)
            throw new KeyNotFoundException($"Sale with id '{request.Id}' was not found.");

        return _mapper.Map<GetByIdSaleResponse>(sale);
    }
}
