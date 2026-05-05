using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.Drugs.Queries.GetById;

public class GetByIdDrugQuery : IRequest<GetByIdDrugResponse>
{
    public Guid Id { get; set; }
}
