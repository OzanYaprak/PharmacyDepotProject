using MediatR;

namespace Application.Features.Drugs.Commands.Create;

public class CreateDrugCommand : IRequest<CreatedDrugResponse>
{
    public string Name { get; set; }
    public string GTIN { get; set; }
    public string SN { get; set; }
    public string BN { get; set; }
    public DateTime ExpireDate { get; set; }
}
