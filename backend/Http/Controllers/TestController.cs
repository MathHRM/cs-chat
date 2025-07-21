using backend.Commands;
using Microsoft.AspNetCore.Mvc;
using System.CommandLine;
using System.CommandLine.Parsing;
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
    public async Task<IActionResult> Get([FromQuery] string commandLine)
    {
        commandLine = "/command --option a --b";

        var argument = new Argument<string?>("command") {
            DefaultValueFactory = (result) => null,
        };
        var option = new Option<string>("--option");
        var option2 = new Option<bool>("--b");
        var rootCommand = new RootCommand("Command executor");

        var command = new Command("/command") {
            argument,
            option,
            option2
        };
        rootCommand.Add(command);

        var parseResult = rootCommand.Parse(commandLine);

        var systemCommandLineResult = new
        {
            CommandName = parseResult.CommandResult.Command.Name,
            Arguments = parseResult.CommandResult.Tokens.Select(t => new {
                Value = t.Value,
                Type = t.Type.ToString()
            }).ToList(),
            Errors = parseResult.Errors.Select(e => e.Message).ToList(),
            HasErrors = parseResult.Errors.Any()
        };

        var optionValue = parseResult.GetValue(option);
        var option2Value = parseResult.GetValue(option2);
        var argumentValue = parseResult.GetValue(argument);

        return Ok(new
        {
            SystemCommandLine = systemCommandLineResult,
            OptionValue = optionValue,
            Option2Value = option2Value,
            ArgumentValue = argumentValue,
            Tokens = parseResult.Tokens,
            UnmatchedTokens = parseResult.UnmatchedTokens
        });
    }
}
