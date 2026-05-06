using AutoMapper;
using Domain.Entities;
using MediatR;
using Persistence.Repositories.Drug;

namespace Application.Features.Drugs.Queries.GetById;

public class GetByIdDrugQueryHandler : IRequestHandler<GetByIdDrugQuery, GetByIdDrugResponse>
{
    #region Constructor Injection

    private readonly IDrugRepository _drugRepository;
    private readonly IMapper _mapper;
    public GetByIdDrugQueryHandler(IDrugRepository drugRepository, IMapper mapper)
    {
        _drugRepository = drugRepository;
        _mapper = mapper;
    }

    #endregion

    public async Task<GetByIdDrugResponse> Handle(GetByIdDrugQuery request, CancellationToken cancellationToken)
    {
        Drug? drug = await _drugRepository
           .GetAsync(
            withDeleted: true,
            predicate: d => d.Id == request.Id,
            cancellationToken: cancellationToken);

        GetByIdDrugResponse response = _mapper.Map<GetByIdDrugResponse>(drug);

        return response;
    }
}
