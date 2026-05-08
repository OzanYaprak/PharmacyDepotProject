using Application.Common.Requests;
using Application.Common.Responses;
using Application.Features.Stocks.Commands.Create;
using Application.Features.Stocks.Commands.Delete;
using Application.Features.Stocks.Commands.Update;
using Application.Features.Stocks.Queries.GetById;
using Application.Features.Stocks.Queries.GetList;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class StocksController : BaseController
{
    [HttpPost]
    public async Task<IActionResult> CreateStock([FromBody] CreateStockCommand command, CancellationToken ct)
    {
        CreatedStockResponse response = await Mediator.Send(command, ct);
        return CreatedAtAction(nameof(GetStockById), new { id = response.Id }, response);
    }

    [HttpGet]
    public async Task<IActionResult> GetStocks([FromQuery] PageRequest pageRequest, CancellationToken ct)
    {
        GetListStockQuery query = new GetListStockQuery { PageRequest = pageRequest };
        GetListResponse<GetListStockListItemDto> response = await Mediator.Send(query, ct);
        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetStockById([FromRoute] Guid id, CancellationToken ct)
    {
        GetByIdStockQuery query = new GetByIdStockQuery { Id = id };
        GetByIdStockResponse response = await Mediator.Send(query, ct);
        return Ok(response);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateStock([FromBody] UpdateStockCommand command, CancellationToken ct)
    {
        UpdatedStockResponse response = await Mediator.Send(command, ct);
        return Ok(response);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteStock([FromRoute] Guid id, CancellationToken ct)
    {
        DeleteStockCommand command = new DeleteStockCommand { Id = id };
        DeletedStockResponse response = await Mediator.Send(command, ct);
        return Ok(response);
    }
}
