using Microsoft.AspNetCore.Mvc;

namespace HelloRinnegan.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TodosController : ControllerBase
{
    [HttpPost]
    public Task<IActionResult> New(
        [FromServices] HelloRinnegan.Services.Areas.Todos.New service,
        [FromBody] HelloRinnegan.Services.Areas.Todos.New.Request request)
    {
        return service.RunAsActionResultAsync(request);
    }

    [HttpGet]
    public Task<IActionResult> List(
        [FromServices] HelloRinnegan.Services.Areas.Todos.List service,
        [FromQuery] HelloRinnegan.Services.Areas.Todos.List.Request request)
    {
        return service.RunAsActionResultAsync(request);
    }
}
