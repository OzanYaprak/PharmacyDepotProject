using Application.Common.Requests;
using Application.Common.Responses;
using Application.Features.Warehouses.Commands.Create;
using Application.Features.Warehouses.Commands.Delete;
using Application.Features.Warehouses.Commands.Update;
using Application.Features.Warehouses.Queries.GetById;
using Application.Features.Warehouses.Queries.GetList;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class WarehousesController : BaseController
{
    [HttpPost]
    public async Task<IActionResult> CreateWarehouse([FromBody] CreateWarehouseCommand command, CancellationToken ct)
    {
        CreatedWarehouseResponse response = await Mediator.Send(command, ct);
        return CreatedAtAction(nameof(GetWarehouseById), new { id = response.Id }, response);
    }

    [HttpGet]
    public async Task<IActionResult> GetWarehouses([FromQuery] PageRequest pageRequest, CancellationToken ct)
    {
        GetListWarehouseQuery query = new GetListWarehouseQuery { PageRequest = pageRequest };
        GetListResponse<GetListWarehouseListItemDto> response = await Mediator.Send(query, ct);
        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetWarehouseById([FromRoute] Guid id, CancellationToken ct)
    {
        GetByIdWarehouseQuery query = new GetByIdWarehouseQuery { Id = id };
        GetByIdWarehouseResponse response = await Mediator.Send(query, ct);
        return Ok(response);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateWarehouse([FromBody] UpdateWarehouseCommand command, CancellationToken ct)
    {
        UpdatedWarehouseResponse response = await Mediator.Send(command, ct);
        return Ok(response);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteWarehouse([FromRoute] Guid id, CancellationToken ct)
    {
        DeleteWarehouseCommand command = new DeleteWarehouseCommand { Id = id };
        DeletedWarehouseResponse response = await Mediator.Send(command, ct);
        return Ok(response);
    }
}
