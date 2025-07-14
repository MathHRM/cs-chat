namespace backend.Commands.Commands;

public class Login : Command
{
    public override string CommandName => "login";

    public override string Description => "Login to the system";

    public override Dictionary<string, CommandArgument>? Args => new Dictionary<string, CommandArgument>
    {
        { "username", new CommandArgument { Name = "username", IsRequired = true, Description = "The username to login with" } },
        { "password", new CommandArgument { Name = "password", IsRequired = true, Description = "The password to login with" } }
    };

    public override Task<CommandResult> Handle(Dictionary<string, object> args)
    {
        return Task.FromResult(new CommandResult { Success = true, Message = $"Login command with args: {string.Join(", ", args.Select(kvp => $"{kvp.Key}: {kvp.Value}"))}" });
    }
}