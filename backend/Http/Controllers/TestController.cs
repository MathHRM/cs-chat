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
        var result = await CommandHandler.HandleCommand("/help --chatId=123 aaaaa -c=2323 --dcdcdc -c -d=\"aa aa   aa\" bbbbbb");
        return Ok(result);
    }

}