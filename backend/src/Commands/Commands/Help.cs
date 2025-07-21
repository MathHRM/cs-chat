using System.Text;
using System.CommandLine;
using backend.Http.Responses;

namespace backend.Commands;

public class Help : CommandBase
{
    private readonly ICommandResolver _commandResolver;

    public override string CommandName => "help";
    public override string Description => "Show help for all commands";
    public override Dictionary<string, CommandArgument>? Args => null;
    public override bool ForAuthenticatedUsers => true;
    public override bool ForGuestUsers => true;

    public Help(ICommandResolver commandResolver)
    {
        _commandResolver = commandResolver;
    }

    public override async Task<CommandResult> Handle(ParseResult parseResult)
    {
        var commands = CommandsForUser();
        var helpMessage = new StringBuilder("Available commands:\n\n")
            .AppendLine("    For multiple word names, use quotes.")
            .AppendLine();

        foreach (var command in commands)
        {
            var commandInstance = command.GetCommandInstance();

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

    private List<CommandBase> CommandsForUser()
    {
        if (UserIsAuthenticated)
        {
            return _commandResolver.GetAllCommands().Where(c => c.ForAuthenticatedUsers).ToList();
        }

        return _commandResolver.GetAllCommands().Where(c => c.ForGuestUsers).ToList();
    }

    public override Command GetCommandInstance()
    {
        var command = new Command(CommandName, Description);
        command.TreatUnmatchedTokensAsErrors = false;
        return command;
    }
}