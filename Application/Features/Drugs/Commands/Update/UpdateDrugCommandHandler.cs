using Application.Features.Drugs.Rules;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Persistence.Repositories.Drug;

namespace Application.Features.Drugs.Commands.Update;

public class UpdateDrugCommandHandler : IRequestHandler<UpdateDrugCommand, UpdateDrugResponse>
{
    #region Constructor Injection

    private readonly IDrugRepository _drugRepository;
    private readonly IMapper _mapper;
    private readonly DrugBusinessRules _drugBusinessRules;

    public UpdateDrugCommandHandler(IDrugRepository drugRepository, IMapper mapper, DrugBusinessRules drugBusinessRules)
    {
        _drugRepository = drugRepository;
        _mapper = mapper;
        _drugBusinessRules = drugBusinessRules;
    }

    #endregion

    public async Task<UpdateDrugResponse> Handle(UpdateDrugCommand request, CancellationToken cancellationToken)
    {
        if (request.GTIN is not null)
            await _drugBusinessRules.GtinCannotBeDuplicatedWhenUpdated(request.Id, request.GTIN);

        if (request.SN is not null)
            await _drugBusinessRules.SerialNumberCannotBeDuplicatedWhenUpdated(request.Id, request.SN);

        if (request.ExpireDate.HasValue)
            await _drugBusinessRules.ExpireDateCannotBeInThePast(request.ExpireDate.Value);

        Drug? drug = await _drugRepository.GetAsync(
            predicate: d => d.Id == request.Id,
            cancellationToken: cancellationToken);

        if (drug is null)
            throw new KeyNotFoundException($"Drug with id '{request.Id}' was not found.");

        Drug updatedDrug = _mapper.Map(request, drug);

        var result = await _drugRepository.UpdateAsync(updatedDrug);

        UpdateDrugResponse response = _mapper.Map<UpdateDrugResponse>(result);

        return response;
    }
}
