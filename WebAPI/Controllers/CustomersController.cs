using Application.Common.Requests;
using Application.Common.Responses;
using Application.Features.Customers.Commands.Create;
using Application.Features.Customers.Commands.Delete;
using Application.Features.Customers.Commands.Update;
using Application.Features.Customers.Queries.GetById;
using Application.Features.Customers.Queries.GetList;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CustomersController : BaseController
{
    [HttpPost]
    public async Task<IActionResult> CreateCustomer([FromBody] CreateCustomerCommand command, CancellationToken ct)
    {
        CreatedCustomerResponse response = await Mediator.Send(command, ct);
        return CreatedAtAction(nameof(GetCustomerById), new { id = response.Id }, response);
    }

    [HttpGet]
    public async Task<IActionResult> GetCustomers([FromQuery] PageRequest pageRequest, CancellationToken ct)
    {
        GetListCustomerQuery query = new GetListCustomerQuery { PageRequest = pageRequest };
        GetListResponse<GetListCustomerListItemDto> response = await Mediator.Send(query, ct);
        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetCustomerById([FromRoute] Guid id, CancellationToken ct)
    {
        GetByIdCustomerQuery query = new GetByIdCustomerQuery { Id = id };
        GetByIdCustomerResponse response = await Mediator.Send(query, ct);
        return Ok(response);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateCustomer([FromBody] UpdateCustomerCommand command, CancellationToken ct)
    {
        UpdatedCustomerResponse response = await Mediator.Send(command, ct);
        return Ok(response);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCustomer([FromRoute] Guid id, CancellationToken ct)
    {
        DeleteCustomerCommand command = new DeleteCustomerCommand { Id = id };
        DeletedCustomerResponse response = await Mediator.Send(command, ct);
        return Ok(response);
    }
}
