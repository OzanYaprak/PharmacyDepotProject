using AutoMapper;
using Domain.Entities;
using MediatR;
using Persistence.Repositories.Stock;

namespace Application.Features.Stocks.Queries.GetById;

public class GetByIdStockQueryHandler : IRequestHandler<GetByIdStockQuery, GetByIdStockResponse>
{
    private readonly IStockRepository _stockRepository;
    private readonly IMapper _mapper;

    public GetByIdStockQueryHandler(IStockRepository stockRepository, IMapper mapper)
    {
        _stockRepository = stockRepository;
        _mapper = mapper;
    }

    public async Task<GetByIdStockResponse> Handle(GetByIdStockQuery request, CancellationToken cancellationToken)
    {
        Stock? stock = await _stockRepository.GetAsync(
            predicate: s => s.Id == request.Id,
            cancellationToken: cancellationToken);

        if (stock is null)
            throw new KeyNotFoundException($"Stock with id '{request.Id}' was not found.");

        return _mapper.Map<GetByIdStockResponse>(stock);
    }
}
