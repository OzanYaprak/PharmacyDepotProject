using MediatR;

namespace Application.Features.Drugs.Commands.Create;

public class CreateDrugCommandHandler : IRequestHandler<CreateDrugCommand, CreatedDrugResponse>
{
    public Task<CreatedDrugResponse> Handle(CreateDrugCommand request, CancellationToken cancellationToken)
    {
        CreatedDrugResponse createdDrugResponse = new CreatedDrugResponse
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            BN = request.BN,
            GTIN = request.GTIN,
            ExpireDate = request.ExpireDate.AddYears(3),
            SN = request.SN,
        };

        return Task.FromResult(createdDrugResponse);
    }
}
