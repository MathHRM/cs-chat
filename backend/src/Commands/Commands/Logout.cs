using Microsoft.AspNetCore.SignalR;
using backend.Services;
using System.CommandLine;

namespace backend.Commands;

public class Logout : CommandBase
{
    public override string CommandName => "logout";
    public override string Description => "Logout do chat";
    public override bool ForAuthenticatedUsers => true;
    public override bool ForGuestUsers => false;

    public override Command GetCommandInstance()
    {
        var command = new Command(CommandName, Description);
        command.TreatUnmatchedTokensAsErrors = false;
        return command;
    }

    private readonly UserService _userService;
    private readonly HubConnectionService _hubConnectionService;

    public Logout(UserService userService, HubConnectionService hubConnectionService)
    {
        _userService = userService;
        _hubConnectionService = hubConnectionService;
    }

    public override async Task<CommandResult> Handle(ParseResult parseResult)
    {
        if (HubCallerContext == null || !(HubCallerContext.User.Identity?.IsAuthenticated ?? false))
        {
            return CommandResult.FailureResult("Você não está logado", CommandName);
        }

        var user = await _userService.GetUserByIdAsync(AuthenticatedUserId);

        if (user == null)
        {
            return CommandResult.FailureResult("Usuário não encontrado", CommandName);
        }

        await _hubConnectionService.DisconnectUserFromAllChatsAsync(user.Id, HubCallerContext, HubGroups);

        return CommandResult.SuccessResult("Você foi deslogado com sucesso", CommandName);
    }
}