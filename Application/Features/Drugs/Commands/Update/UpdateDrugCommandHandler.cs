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

    public UpdateDrugCommandHandler(IDrugRepository drugRepository, IMapper mapper)
    {
        _drugRepository = drugRepository;
        _mapper = mapper;
    }

    #endregion

    public async Task<UpdateDrugResponse> Handle(UpdateDrugCommand request, CancellationToken cancellationToken)
    {
        Drug? drug = await _drugRepository.GetAsync(predicate: d => d.Id == request.Id, cancellationToken: cancellationToken);

        if (drug is null)
        {
            throw new KeyNotFoundException($"Drug with id '{request.Id}' was not found.");
        }

        Drug updatedDrug = _mapper.Map(request, drug);

        var result = await _drugRepository.UpdateAsync(updatedDrug);   

        UpdateDrugResponse response = _mapper.Map<UpdateDrugResponse>(result);

        return response;
    }
}
