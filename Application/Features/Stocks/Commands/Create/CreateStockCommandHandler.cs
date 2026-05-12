using Application.Features.Stocks.Rules;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Persistence.Repositories.Stock;

namespace Application.Features.Stocks.Commands.Create;

public class CreateStockCommandHandler : IRequestHandler<CreateStockCommand, CreatedStockResponse>
{
    private readonly IStockRepository _stockRepository;
    private readonly IMapper _mapper;
    private readonly StockBusinessRules _stockBusinessRules;

    public CreateStockCommandHandler(IStockRepository stockRepository, IMapper mapper, StockBusinessRules stockBusinessRules)
    {
        _stockRepository = stockRepository;
        _mapper = mapper;
        _stockBusinessRules = stockBusinessRules;
    }

    public async Task<CreatedStockResponse> Handle(CreateStockCommand request, CancellationToken cancellationToken)
    {
        await _stockBusinessRules.StockCannotBeDuplicatedForSameDrugAndWarehouseWhenInserted(request.DrugId, request.WarehouseId);
        await _stockBusinessRules.QuantityMustBeNonNegative(request.Quantity);
        await _stockBusinessRules.UnitPriceMustBePositive(request.UnitPrice);

        Stock stock = _mapper.Map<Stock>(request);
        stock.Id = Guid.NewGuid();

        var result = await _stockRepository.AddAsync(stock, cancellationToken);
        return _mapper.Map<CreatedStockResponse>(result);
    }
}
