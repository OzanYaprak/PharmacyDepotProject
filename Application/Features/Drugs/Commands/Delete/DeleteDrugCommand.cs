using Application.Pipelines.Transaction;
using MediatR;

namespace Application.Features.Drugs.Commands.Delete;

public class DeleteDrugCommand : IRequest<DeleteDrugResponse>, ITransactionalRequest
{
    public Guid Id { get; set; }
}
