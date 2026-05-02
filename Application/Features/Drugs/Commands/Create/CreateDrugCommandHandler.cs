using AutoMapper;
using Domain.Entities;
using MediatR;
using Persistence.Repositories.Drug;

namespace Application.Features.Drugs.Commands.Create;

public class CreateDrugCommandHandler : IRequestHandler<CreateDrugCommand, CreatedDrugResponse>
{
    private readonly IDrugRepository _drugRepository;
    private readonly IMapper _mapper;

    public CreateDrugCommandHandler(IDrugRepository drugRepository, IMapper mapper)
    {
        _drugRepository = drugRepository;
        _mapper = mapper;
    }

    public async Task<CreatedDrugResponse> Handle(CreateDrugCommand request, CancellationToken cancellationToken)
    {
        Drug drug = _mapper.Map<Drug>(request);

        drug.Id = Guid.NewGuid();
        drug.ExpireDate = request.ExpireDate.AddYears(3);

        var result = await _drugRepository.AddAsync(drug);

        CreatedDrugResponse createdDrugResponse = _mapper.Map<CreatedDrugResponse>(result);
        return createdDrugResponse;
    }
}

