using Microsoft.AspNetCore.Mvc;
using backend.Commands;
using backend.Http.Requests;

namespace backend.Http.Controllers;

[ApiController]
[Route("api/commands")]
public class CommandController : ControllerBase
{
    private readonly CommandHandler _commandHandler;

    public CommandController(CommandHandler commandHandler)
    {
        _commandHandler = commandHandler;
    }

    [HttpPost]
    public async Task<ActionResult<CommandResult>> HandleCommand([FromBody] CommandRequest request)
    {
        return Ok(await _commandHandler.HandleCommand(request.Command, httpContext: HttpContext));
    }
}