using AutoMapper;
using Domain.Entities;
using MediatR;
using Persistence.Repositories.Drug;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.Drugs.Commands.Delete;

public class DeleteDrugCommandHandler : IRequestHandler<DeleteDrugCommand, DeleteDrugResponse>
{
    #region Constructor Injection

    private readonly IDrugRepository _drugRepository;
    private readonly IMapper _mapper;

    public DeleteDrugCommandHandler(IDrugRepository drugRepository, IMapper mapper)
    {
        _drugRepository = drugRepository;
        _mapper = mapper;
    }

    #endregion

    public async Task<DeleteDrugResponse> Handle(DeleteDrugCommand request, CancellationToken cancellationToken)
    {
        Drug? drug = await _drugRepository.GetAsync(
            predicate: d => d.Id == request.Id,
            cancellationToken: cancellationToken);

        if (drug is null)
        {
            throw new KeyNotFoundException($"Drug with id '{request.Id}' was not found.");
        }

        Drug deletedDrug = _mapper.Map(request, drug);
        
        await _drugRepository.DeleteAsync(
            entity: deletedDrug, 
            cancellationToken:cancellationToken);

        DeleteDrugResponse response = _mapper.Map<DeleteDrugResponse>(deletedDrug);

        return response;
    }
}
