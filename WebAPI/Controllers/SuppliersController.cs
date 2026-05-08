using Application.Common.Requests;
using Application.Common.Responses;
using Application.Features.Suppliers.Commands.Create;
using Application.Features.Suppliers.Commands.Delete;
using Application.Features.Suppliers.Commands.Update;
using Application.Features.Suppliers.Queries.GetById;
using Application.Features.Suppliers.Queries.GetList;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SuppliersController : BaseController
{
    [HttpPost]
    public async Task<IActionResult> CreateSupplier([FromBody] CreateSupplierCommand command, CancellationToken ct)
    {
        CreatedSupplierResponse response = await Mediator.Send(command, ct);
        return CreatedAtAction(nameof(GetSupplierById), new { id = response.Id }, response);
    }

    [HttpGet]
    public async Task<IActionResult> GetSuppliers([FromQuery] PageRequest pageRequest, CancellationToken ct)
    {
        GetListSupplierQuery query = new GetListSupplierQuery { PageRequest = pageRequest };
        GetListResponse<GetListSupplierListItemDto> response = await Mediator.Send(query, ct);
        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetSupplierById([FromRoute] Guid id, CancellationToken ct)
    {
        GetByIdSupplierQuery query = new GetByIdSupplierQuery { Id = id };
        GetByIdSupplierResponse response = await Mediator.Send(query, ct);
        return Ok(response);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateSupplier([FromBody] UpdateSupplierCommand command, CancellationToken ct)
    {
        UpdatedSupplierResponse response = await Mediator.Send(command, ct);
        return Ok(response);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSupplier([FromRoute] Guid id, CancellationToken ct)
    {
        DeleteSupplierCommand command = new DeleteSupplierCommand { Id = id };
        DeletedSupplierResponse response = await Mediator.Send(command, ct);
        return Ok(response);
    }
}
