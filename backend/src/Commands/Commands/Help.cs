using System.Text;
using System.CommandLine;
using backend.Http.Responses;
using backend.Commands.Responses;

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

        var commandResponses = new List<CommandResponse>();

        foreach (var command in commands)
        {
            var commandInstance = command.GetCommandInstance();

            commandResponses.Add(new CommandResponse
            {
                Name = command.CommandName,
                Description = command.Description,
                Arguments = commandInstance.Arguments.Select(a => new ArgumentResponse
                {
                    Name = a.Name,
                    Description = a.Description,
                    IsRequired = true,
                }).ToList(),
                Options = commandInstance.Options.Select(o => new ArgumentResponse
                {
                    Name = o.Name,
                    Description = o.Description,
                    IsRequired = o.Required,
                    Aliases = o.Aliases.ToList()
                }).ToList()
            });
        }

        return new HelpResult
        {
            Result = CommandResultEnum.Success,
            Response = commandResponses,
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