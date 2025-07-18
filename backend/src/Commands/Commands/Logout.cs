using Microsoft.AspNetCore.SignalR;

namespace backend.Commands;

public class Logout : Command
{
    public override string CommandName => "logout";
    public override string Description => "Logout from the chat";
    public override Dictionary<string, CommandArgument>? Args => null;
    public override bool RequiresAuthentication => true;

    public override async Task<CommandResult> Handle(Dictionary<string, string?> args)
    {
        if (HubCallerContext == null || !(HubCallerContext.User.Identity?.IsAuthenticated ?? false))
        {
            return CommandResult.FailureResult("You are not logged in", CommandName);
        }

        return CommandResult.SuccessResult("You have been logged out", CommandName);
    }
}