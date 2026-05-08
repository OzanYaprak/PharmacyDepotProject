using AutoMapper;
using Domain.Entities;
using MediatR;
using Persistence.Repositories.Sale;

namespace Application.Features.Sales.Commands.Create;

public class CreateSaleCommandHandler : IRequestHandler<CreateSaleCommand, CreatedSaleResponse>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;

    public CreateSaleCommandHandler(ISaleRepository saleRepository, IMapper mapper)
    {
        _saleRepository = saleRepository;
        _mapper = mapper;
    }

    public async Task<CreatedSaleResponse> Handle(CreateSaleCommand request, CancellationToken cancellationToken)
    {
        Sale sale = _mapper.Map<Sale>(request);
        sale.Id = Guid.NewGuid();

        var result = await _saleRepository.AddAsync(sale, cancellationToken);
        return _mapper.Map<CreatedSaleResponse>(result);
    }
}
