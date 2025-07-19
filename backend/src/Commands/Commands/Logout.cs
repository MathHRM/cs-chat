using Microsoft.AspNetCore.SignalR;
using backend.Services;

namespace backend.Commands;

public class Logout : Command
{
    public override string CommandName => "logout";
    public override string Description => "Logout from the chat";
    public override Dictionary<string, CommandArgument>? Args => null;
    public override bool ForAuthenticatedUsers => true;
    public override bool ForGuestUsers => false;

    private readonly UserService _userService;

    public Logout(UserService userService)
    {
        _userService = userService;
    }

    public override async Task<CommandResult> Handle(Dictionary<string, string?> args)
    {
        if (HubCallerContext == null || !(HubCallerContext.User.Identity?.IsAuthenticated ?? false))
        {
            return CommandResult.FailureResult("You are not logged in", CommandName);
        }

        var user = await _userService.GetUserByUsernameAsync(HubCallerContext.User.Identity.Name);

        if (user == null)
        {
            return CommandResult.FailureResult("User not found", CommandName);
        }

        await _userService.DisconnectUserFromAllChatsAsync(user, HubCallerContext, HubGroups);

        return CommandResult.SuccessResult("You have been logged out", CommandName);
    }
}