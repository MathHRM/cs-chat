using Microsoft.AspNetCore.Mvc;
using backend.Commands;

namespace backend.Http.Controllers;

[ApiController]
[Route("api/test")]
public class TestController : ControllerBase
{
    [HttpGet]
    [Route("testando")]
    public async Task<IActionResult> Get()
    {
        var result = await CommandHandler.HandleCommand("/login --username=admin --password=123456");
        return Ok(result);
    }

}