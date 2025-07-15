using backend.Commands.Results;

namespace backend.Commands.Commands;

public class Help : Command
{
    public override string CommandName => "help";

    public override string Description => "Show help for all commands";

    public override Dictionary<string, CommandArgument>? Args => null;

    public override async Task<CommandResult> Handle(Dictionary<string, object> args)
    {
        return new GenericResult
        {
            Success = true,
            Message = "Help command",
            Command = CommandName
        };
    }
}