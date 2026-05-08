using AutoMapper;
using Domain.Entities;
using MediatR;
using Persistence.Repositories.Stock;

namespace Application.Features.Stocks.Commands.Update;

public class UpdateStockCommandHandler : IRequestHandler<UpdateStockCommand, UpdatedStockResponse>
{
    private readonly IStockRepository _stockRepository;
    private readonly IMapper _mapper;

    public UpdateStockCommandHandler(IStockRepository stockRepository, IMapper mapper)
    {
        _stockRepository = stockRepository;
        _mapper = mapper;
    }

    public async Task<UpdatedStockResponse> Handle(UpdateStockCommand request, CancellationToken cancellationToken)
    {
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
