using AutoMapper;
using Domain.Entities;
using MediatR;
using Persistence.Repositories.Sale;

namespace Application.Features.Sales.Commands.Delete;

public class DeleteSaleCommandHandler : IRequestHandler<DeleteSaleCommand, DeletedSaleResponse>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;

    public DeleteSaleCommandHandler(ISaleRepository saleRepository, IMapper mapper)
    {
        _saleRepository = saleRepository;
        _mapper = mapper;
    }

    public async Task<DeletedSaleResponse> Handle(DeleteSaleCommand request, CancellationToken cancellationToken)
    {
        Sale? sale = await _saleRepository.GetAsync(
            predicate: s => s.Id == request.Id,
            cancellationToken: cancellationToken);

        if (sale is null)
            throw new KeyNotFoundException($"Sale with id '{request.Id}' was not found.");

        await _saleRepository.DeleteAsync(sale, permanent: false, cancellationToken: cancellationToken);
        return _mapper.Map<DeletedSaleResponse>(sale);
    }
}
