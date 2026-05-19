using Application.Pipelines.Caching.Remove;
using Application.Pipelines.Transaction;
using MediatR;

namespace Application.Features.Drugs.Commands.Delete;

public class DeleteDrugCommand : IRequest<DeleteDrugResponse>, ITransactionalRequest, ICacheRemoverRequest
{
    public Guid Id { get; set; }


    public string? CacheKey => null;
    public string? CacheGroupKey => "GetDrugsQuery";
    public bool BypassCache => false;
}
