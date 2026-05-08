using AutoMapper;
using Domain.Entities;
using MediatR;
using Persistence.Repositories.Stock;

namespace Application.Features.Stocks.Commands.Delete;

public class DeleteStockCommandHandler : IRequestHandler<DeleteStockCommand, DeletedStockResponse>
{
    private readonly IStockRepository _stockRepository;
    private readonly IMapper _mapper;

    public DeleteStockCommandHandler(IStockRepository stockRepository, IMapper mapper)
    {
        _stockRepository = stockRepository;
        _mapper = mapper;
    }

    public async Task<DeletedStockResponse> Handle(DeleteStockCommand request, CancellationToken cancellationToken)
    {
        Stock? stock = await _stockRepository.GetAsync(
            predicate: s => s.Id == request.Id,
            cancellationToken: cancellationToken);

        if (stock is null)
            throw new KeyNotFoundException($"Stock with id '{request.Id}' was not found.");

        await _stockRepository.DeleteAsync(stock, permanent: false, cancellationToken: cancellationToken);
        return _mapper.Map<DeletedStockResponse>(stock);
    }
}
