using Application.Pipelines.Caching.Remove;
using Application.Pipelines.Transaction;
using MediatR;

namespace Application.Features.Drugs.Commands.Update;

public class UpdateDrugCommand : IRequest<UpdateDrugResponse>, ITransactionalRequest, ICacheRemoverRequest
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? GTIN { get; set; }
    public string? SN { get; set; }
    public string? BN { get; set; }
    public DateTime? ExpireDate { get; set; }


    public string? CacheKey => null;
    public string? CacheGroupKey => "GetDrugsQuery";
    public bool BypassCache => false;
}
