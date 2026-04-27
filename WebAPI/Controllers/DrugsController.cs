using Application.Features.Drugs.Commands.Create;
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
}
