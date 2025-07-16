using System.Text;
using backend.Commands.Results;
using backend.Commands.Enums;
using backend.Http.Responses;
using Microsoft.AspNetCore.SignalR;

namespace backend.Commands.Commands;

public class Logout : Command
{
    public override string CommandName => "logout";
    public override string Description => "Logout from the chat";
    public override Dictionary<string, CommandArgument>? Args => null;
    public override bool RequiresAuthentication => true;

    public override async Task<CommandResult> Handle(Dictionary<string, object?> args)
    {
        var connection = args["connection"] as HubCallerContext;

        if (connection == null || !(connection.User.Identity?.IsAuthenticated ?? false))
        {
            return CommandResult.FailureResult("You are not logged in", CommandName);
        }

        return CommandResult.SuccessResult("You have been logged out", CommandName);
    }
}