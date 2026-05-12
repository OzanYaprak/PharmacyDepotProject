using Application.Features.Drugs.Rules;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Persistence.Repositories.Drug;

namespace Application.Features.Drugs.Commands.Create;

public class CreateDrugCommandHandler : IRequestHandler<CreateDrugCommand, CreatedDrugResponse>
{
    #region Constructor Injection

    private readonly IDrugRepository _drugRepository;
    private readonly IMapper _mapper;
    private readonly DrugBusinessRules _drugBusinessRules;

    public CreateDrugCommandHandler(IDrugRepository drugRepository, IMapper mapper, DrugBusinessRules drugBusinessRules)
    {
        _drugRepository = drugRepository;
        _mapper = mapper;
        _drugBusinessRules = drugBusinessRules;
    }

    #endregion

    public async Task<CreatedDrugResponse> Handle(CreateDrugCommand request, CancellationToken cancellationToken)
    {
        await _drugBusinessRules.GtinCannotBeDuplicatedWhenInserted(request.GTIN);
        await _drugBusinessRules.SerialNumberCannotBeDuplicatedWhenInserted(request.SN);
        await _drugBusinessRules.ExpireDateCannotBeInThePast(request.ExpireDate);

        Drug drug = _mapper.Map<Drug>(request);

        drug.Id = Guid.NewGuid();
        drug.ExpireDate = request.ExpireDate.AddYears(3);

        var result = await _drugRepository.AddAsync(drug);

        CreatedDrugResponse createdDrugResponse = _mapper.Map<CreatedDrugResponse>(result);
        return createdDrugResponse;
    }
}