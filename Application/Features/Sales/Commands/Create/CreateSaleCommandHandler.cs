using Application.Features.Sales.Rules;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Persistence.Repositories.Sale;

namespace Application.Features.Sales.Commands.Create;

public class CreateSaleCommandHandler : IRequestHandler<CreateSaleCommand, CreatedSaleResponse>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly SaleBusinessRules _saleBusinessRules;

    public CreateSaleCommandHandler(ISaleRepository saleRepository, IMapper mapper, SaleBusinessRules saleBusinessRules)
    {
        _saleRepository = saleRepository;
        _mapper = mapper;
        _saleBusinessRules = saleBusinessRules;
    }

    public async Task<CreatedSaleResponse> Handle(CreateSaleCommand request, CancellationToken cancellationToken)
    {
        await _saleBusinessRules.SaleDateCannotBeInTheFuture(request.SaleDate);
        await _saleBusinessRules.TotalAmountMustBePositive(request.TotalAmount);

        Sale sale = _mapper.Map<Sale>(request);
        sale.Id = Guid.NewGuid();

        var result = await _saleRepository.AddAsync(sale, cancellationToken);
        return _mapper.Map<CreatedSaleResponse>(result);
    }
}
