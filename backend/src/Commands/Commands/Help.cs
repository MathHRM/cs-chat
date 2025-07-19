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
        var commands = CommandsForUser();
        var helpMessage = new StringBuilder("Available commands:\n\n")
            .AppendLine("    For multiple word names, use quotes.")
            .AppendLine();

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

                string alias = argument.Alias != null ? $"| -{argument.Alias}" : "";

                helpMessage.AppendLine($"    --{argument.Name} {alias} - {argument.Description}");
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

    private List<Command> CommandsForUser()
    {
        if (UserIsAuthenticated)
        {
            return _commandResolver.GetAllCommands().Where(c => c.RequiresAuthentication).ToList();
        }

        return _commandResolver.GetAllCommands().Where(c => !c.RequiresAuthentication).ToList();
    }
}