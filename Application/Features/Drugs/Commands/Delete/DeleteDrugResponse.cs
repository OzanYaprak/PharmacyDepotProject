using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.Drugs.Commands.Delete;

public class DeleteDrugResponse
{
    public Guid Id { get; set; }
    public DateTime DeletedDate { get; set; }
}
