using Microsoft.AspNetCore.Mvc;

namespace HelloRinnegan.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GreetingController : ControllerBase
{
    [HttpGet]
    public Task<IActionResult> Get([FromServices] HelloRinnegan.Services.Areas.Greeting.Index service)
    {
        return service.RunAsActionResultAsync(null, null);
    }
}
