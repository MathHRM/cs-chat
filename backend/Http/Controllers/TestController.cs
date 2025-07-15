using Microsoft.AspNetCore.Mvc;
using backend.Commands;

namespace backend.Http.Controllers;

[ApiController]
[Route("api/test")]
public class TestController : ControllerBase
{
    private readonly CommandHandler _commandHandler;

    public TestController(CommandHandler commandHandler)
    {
        _commandHandler = commandHandler;
    }

    [HttpGet]
    [Route("testando")]
    public async Task<IActionResult> Get([FromQuery] string command)
    {
        var result = await _commandHandler.HandleCommand(command);
        return Ok(result);
    }

}