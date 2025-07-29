using System.Text;
using System.CommandLine;
using backend.Http.Responses;

namespace backend.Commands;

public class Help : CommandBase
{
    public override string CommandName => "help";
    public override string Description => "Mostra ajuda para todos os comandos";
    public override bool ForAuthenticatedUsers => true;
    public override bool ForGuestUsers => true;

    public override Command GetCommandInstance()
    {
        var command = new Command(CommandName, Description);
        command.TreatUnmatchedTokensAsErrors = false;
        return command;
    }

    private readonly ICommandResolver _commandResolver;

    public Help(ICommandResolver commandResolver)
    {
        _commandResolver = commandResolver;
    }

    public override async Task<CommandResult> Handle(ParseResult parseResult)
    {
        var commands = CommandsForUser();
        var helpMessage = new StringBuilder("Comandos disponíveis:\n\n")
            .AppendLine("    Para nomes com múltiplas palavras, use aspas.")
            .AppendLine();

        foreach (var command in commands)
        {
            var commandInstance = command.GetCommandInstance();
            var commandCopyShortcut = new StringBuilder($"/{commandInstance.Name}");

            helpMessage.AppendLine($"/{commandInstance.Name} - {commandInstance.Description}");

            foreach (var argument in commandInstance.Arguments)
            {
                commandCopyShortcut.Append($" {argument.Name}");
                helpMessage.AppendLine($"    [{argument.Name}] - {argument.Description}");
            }

            foreach (var option in commandInstance.Options)
            {
                commandCopyShortcut.Append($" {option.Name} [valor]");
                string alias = $" | {string.Join(", ", option.Aliases)}";
                helpMessage.AppendLine($"    {option.Name}{alias} - {option.Description}");
            }

            helpMessage.AppendLine();
            helpMessage.AppendLine($"Copie e cole o comando: {commandCopyShortcut}");

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
                    Username = "Sistema",
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
}