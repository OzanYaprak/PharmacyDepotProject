using AutoMapper;
using Domain.Entities;
using MediatR;
using Persistence.Repositories.Stock;

namespace Application.Features.Stocks.Commands.Create;

public class CreateStockCommandHandler : IRequestHandler<CreateStockCommand, CreatedStockResponse>
{
    private readonly IStockRepository _stockRepository;
    private readonly IMapper _mapper;

    public CreateStockCommandHandler(IStockRepository stockRepository, IMapper mapper)
    {
        _stockRepository = stockRepository;
        _mapper = mapper;
    }

    public async Task<CreatedStockResponse> Handle(CreateStockCommand request, CancellationToken cancellationToken)
    {
        Stock stock = _mapper.Map<Stock>(request);
        stock.Id = Guid.NewGuid();

        var result = await _stockRepository.AddAsync(stock, cancellationToken);
        return _mapper.Map<CreatedStockResponse>(result);
    }
}
