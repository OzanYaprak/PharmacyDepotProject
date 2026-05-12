using Application.Features.Stocks.Rules;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Persistence.Repositories.Stock;

namespace Application.Features.Stocks.Commands.Update;

public class UpdateStockCommandHandler : IRequestHandler<UpdateStockCommand, UpdatedStockResponse>
{
    private readonly IStockRepository _stockRepository;
    private readonly IMapper _mapper;
    private readonly StockBusinessRules _stockBusinessRules;

    public UpdateStockCommandHandler(IStockRepository stockRepository, IMapper mapper, StockBusinessRules stockBusinessRules)
    {
        _stockRepository = stockRepository;
        _mapper = mapper;
        _stockBusinessRules = stockBusinessRules;
    }

    public async Task<UpdatedStockResponse> Handle(UpdateStockCommand request, CancellationToken cancellationToken)
    {
        if (request.Quantity.HasValue)
            await _stockBusinessRules.QuantityMustBeNonNegative(request.Quantity.Value);

        if (request.UnitPrice.HasValue)
            await _stockBusinessRules.UnitPriceMustBePositive(request.UnitPrice.Value);

        Stock? stock = await _stockRepository.GetAsync(
            predicate: s => s.Id == request.Id,
            cancellationToken: cancellationToken);

        if (stock is null)
            throw new KeyNotFoundException($"Stock with id '{request.Id}' was not found.");

        _mapper.Map(request, stock);

        var result = await _stockRepository.UpdateAsync(stock, cancellationToken);
        return _mapper.Map<UpdatedStockResponse>(result);
    }
}
