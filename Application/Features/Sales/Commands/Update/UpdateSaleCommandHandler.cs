using AutoMapper;
using Domain.Entities;
using MediatR;
using Persistence.Repositories.Sale;

namespace Application.Features.Sales.Commands.Update;

public class UpdateSaleCommandHandler : IRequestHandler<UpdateSaleCommand, UpdatedSaleResponse>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;

    public UpdateSaleCommandHandler(ISaleRepository saleRepository, IMapper mapper)
    {
        _saleRepository = saleRepository;
        _mapper = mapper;
    }

    public async Task<UpdatedSaleResponse> Handle(UpdateSaleCommand request, CancellationToken cancellationToken)
    {
        Sale? sale = await _saleRepository.GetAsync(
            predicate: s => s.Id == request.Id,
            cancellationToken: cancellationToken);

        if (sale is null)
            throw new KeyNotFoundException($"Sale with id '{request.Id}' was not found.");

        _mapper.Map(request, sale);

        var result = await _saleRepository.UpdateAsync(sale, cancellationToken);
        return _mapper.Map<UpdatedSaleResponse>(result);
    }
}
