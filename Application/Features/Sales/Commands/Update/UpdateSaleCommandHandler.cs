using Application.Features.Sales.Rules;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Persistence.Repositories.Sale;

namespace Application.Features.Sales.Commands.Update;

public class UpdateSaleCommandHandler : IRequestHandler<UpdateSaleCommand, UpdatedSaleResponse>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly SaleBusinessRules _saleBusinessRules;

    public UpdateSaleCommandHandler(ISaleRepository saleRepository, IMapper mapper, SaleBusinessRules saleBusinessRules)
    {
        _saleRepository = saleRepository;
        _mapper = mapper;
        _saleBusinessRules = saleBusinessRules;
    }

    public async Task<UpdatedSaleResponse> Handle(UpdateSaleCommand request, CancellationToken cancellationToken)
    {
        if (request.TotalAmount.HasValue)
            await _saleBusinessRules.TotalAmountMustBePositive(request.TotalAmount.Value);

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
