namespace backend.Commands.Commands;

public class Help : Command
{
    public string Description = "Show help for all commands";

    public Dictionary<string, CommandArgument>? Args = null;

    public override Task Handle(Dictionary<string, object> args)
    {
        return Task.CompletedTask;
    }
}