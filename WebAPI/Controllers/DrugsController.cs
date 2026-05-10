using Application.Common.Requests;
using Application.Common.Responses;
using Application.Features.Drugs.Commands.Create;
using Application.Features.Drugs.Commands.Delete;
using Application.Features.Drugs.Commands.Update;
using Application.Features.Drugs.Queries.GetById;
using Application.Features.Drugs.Queries.GetList;
using Application.Features.Drugs.Queries.GetListByDynamic;
using Microsoft.AspNetCore.Mvc;
namespace WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DrugsController : BaseController
{
    [HttpPost]
    public async Task<IActionResult> CreateDrug([FromBody] CreateDrugCommand createDrugCommand)
    {
        CreatedDrugResponse response = await Mediator.Send(createDrugCommand);
        return Ok(response);
    }

    [HttpGet]
    public async Task<IActionResult> GetDrugs([FromQuery] PageRequest pageRequest)
    {
        GetListDrugQuery query = new GetListDrugQuery { PageRequest = pageRequest };
        GetListResponse<GetListDrugListItemDto> response = await Mediator.Send(query);
        return Ok(response);
    }

    [HttpGet("GetDrugsByDynamic")]
    public async Task<IActionResult> GetDrugsByDynamic([FromQuery] PageRequest pageRequest)
    {
        GetListByDynamicDrugQuery query = new GetListByDynamicDrugQuery { PageRequest = pageRequest };
        GetListResponse<GetListByDynamicDrugListItemDto> response = await Mediator.Send(query);
        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetDrugById([FromRoute] Guid id)
    {
        GetByIdDrugQuery query = new GetByIdDrugQuery { Id = id };
        GetByIdDrugResponse response = await Mediator.Send(query);
        return Ok(response);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateDrug([FromBody] UpdateDrugCommand updateDrugCommand)
    {
        UpdateDrugResponse response = await Mediator.Send(updateDrugCommand);
        return Ok(response);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteDrug([FromRoute] Guid id)
    {
        DeleteDrugCommand command = new DeleteDrugCommand { Id = id };
        DeleteDrugResponse response = await Mediator.Send(command);
        return Ok(response);
    }
}