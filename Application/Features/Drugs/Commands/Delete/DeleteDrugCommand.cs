using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.Drugs.Commands.Delete;

public class DeleteDrugCommand : IRequest<DeleteDrugResponse>
{
    public Guid Id { get; set; }
}
