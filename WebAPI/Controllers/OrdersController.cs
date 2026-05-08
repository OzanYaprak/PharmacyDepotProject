using Application.Common.Requests;
using Application.Common.Responses;
using Application.Features.Orders.Commands.Create;
using Application.Features.Orders.Commands.Delete;
using Application.Features.Orders.Commands.Update;
using Application.Features.Orders.Queries.GetById;
using Application.Features.Orders.Queries.GetList;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OrdersController : BaseController
{
    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderCommand command, CancellationToken ct)
    {
        CreatedOrderResponse response = await Mediator.Send(command, ct);
        return CreatedAtAction(nameof(GetOrderById), new { id = response.Id }, response);
    }

    [HttpGet]
    public async Task<IActionResult> GetOrders([FromQuery] PageRequest pageRequest, CancellationToken ct)
    {
        GetListOrderQuery query = new GetListOrderQuery { PageRequest = pageRequest };
        GetListResponse<GetListOrderListItemDto> response = await Mediator.Send(query, ct);
        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetOrderById([FromRoute] Guid id, CancellationToken ct)
    {
        GetByIdOrderQuery query = new GetByIdOrderQuery { Id = id };
        GetByIdOrderResponse response = await Mediator.Send(query, ct);
        return Ok(response);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateOrder([FromBody] UpdateOrderCommand command, CancellationToken ct)
    {
        UpdatedOrderResponse response = await Mediator.Send(command, ct);
        return Ok(response);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteOrder([FromRoute] Guid id, CancellationToken ct)
    {
        DeleteOrderCommand command = new DeleteOrderCommand { Id = id };
        DeletedOrderResponse response = await Mediator.Send(command, ct);
        return Ok(response);
    }
}
