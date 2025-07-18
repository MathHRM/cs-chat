using System.Text;
using backend.Http.Responses;
using Microsoft.AspNetCore.SignalR;

namespace backend.Commands;

public class Help : Command
{
    private readonly ICommandResolver _commandResolver;

    public override string CommandName => "help";
    public override string Description => "Show help for all commands";
    public override Dictionary<string, CommandArgument>? Args => null;
    public override bool RequiresAuthentication => false;

    public Help(ICommandResolver commandResolver)
    {
        _commandResolver = commandResolver;
    }

    public override async Task<CommandResult> Handle(Dictionary<string, string?> args)
    {
        var helpMessage = new StringBuilder("Available commands:\n\n");
        var commands = _commandResolver.GetAllCommands();

        if (HubCallerContext == null || !(HubCallerContext.User.Identity?.IsAuthenticated ?? false))
        {
            commands = commands.Where(c => !c.RequiresAuthentication).ToList();
        }

        foreach (var command in commands)
        {
            helpMessage.AppendLine($"/{command.CommandName} - {command.Description}");

            if (command.Args == null || !command.Args.Any())
            {
                helpMessage.AppendLine("    No arguments required");
                helpMessage.AppendLine();

                continue;
            }

            foreach (var argument in command.Args.Values)
            {
                if (argument.ByPosition)
                {
                    helpMessage.AppendLine($"    [{argument.Name}] - {argument.Description}");

                    continue;
                }

                helpMessage.AppendLine($"    --{argument.Name} | -{argument.Name[0]} - {argument.Description}");
            }

            helpMessage.AppendLine();
        }

        return new GenericResult
        {
            Result = CommandResultEnum.Success,
            Response = new MessageResponse
            {
                Content = helpMessage.ToString(),
                CreatedAt = DateTime.UtcNow,
                Type = MessageType.Text,
                User = new UserResponse
                {
                    Id = 0,
                    Username = "System",
                    CurrentChatId = null
                },
            },
            Command = CommandName
        };
    }
}