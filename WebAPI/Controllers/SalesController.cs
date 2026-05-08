using Application.Common.Requests;
using Application.Common.Responses;
using Application.Features.Sales.Commands.Create;
using Application.Features.Sales.Commands.Delete;
using Application.Features.Sales.Commands.Update;
using Application.Features.Sales.Queries.GetById;
using Application.Features.Sales.Queries.GetList;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SalesController : BaseController
{
    [HttpPost]
    public async Task<IActionResult> CreateSale([FromBody] CreateSaleCommand command, CancellationToken ct)
    {
        CreatedSaleResponse response = await Mediator.Send(command, ct);
        return CreatedAtAction(nameof(GetSaleById), new { id = response.Id }, response);
    }

    [HttpGet]
    public async Task<IActionResult> GetSales([FromQuery] PageRequest pageRequest, CancellationToken ct)
    {
        GetListSaleQuery query = new GetListSaleQuery { PageRequest = pageRequest };
        GetListResponse<GetListSaleListItemDto> response = await Mediator.Send(query, ct);
        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetSaleById([FromRoute] Guid id, CancellationToken ct)
    {
        GetByIdSaleQuery query = new GetByIdSaleQuery { Id = id };
        GetByIdSaleResponse response = await Mediator.Send(query, ct);
        return Ok(response);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateSale([FromBody] UpdateSaleCommand command, CancellationToken ct)
    {
        UpdatedSaleResponse response = await Mediator.Send(command, ct);
        return Ok(response);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSale([FromRoute] Guid id, CancellationToken ct)
    {
        DeleteSaleCommand command = new DeleteSaleCommand { Id = id };
        DeletedSaleResponse response = await Mediator.Send(command, ct);
        return Ok(response);
    }
}
