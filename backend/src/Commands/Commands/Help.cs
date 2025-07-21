using System.Text;
using System.CommandLine;
using backend.Http.Responses;

namespace backend.Commands;

public class Help : CommandBase
{
    private readonly ICommandResolver _commandResolver;

    public override string CommandName => "help";
    public override string Description => "Show help for all commands";
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

            helpMessage.AppendLine($"/{commandInstance.Name} - {commandInstance.Description}");

            foreach (var argument in commandInstance.Arguments)
            {
                helpMessage.AppendLine($"    [{argument.Name}] - {argument.Description}");
            }

            foreach (var option in commandInstance.Options)
            {
                string alias = $" | {string.Join(", ", option.Aliases)}";
                helpMessage.AppendLine($"    {option.Name}{alias} - {option.Description}");
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