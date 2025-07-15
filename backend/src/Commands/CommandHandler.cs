using System.Text.RegularExpressions;

namespace backend.Commands;

public class CommandHandler
{
    private readonly ICommandResolver _commandResolver;

    public CommandHandler(ICommandResolver commandResolver)
    {
        _commandResolver = commandResolver;
    }

    public async Task<CommandResult> HandleCommand(string commandInput)
    {
        var args = ParseCommand(commandInput);

        if (args == null || !args.TryGetValue("command", out var commandName) || string.IsNullOrWhiteSpace(commandName))
        {
            return CommandResult.Failure("Invalid command");
        }

        var command = _commandResolver.GetCommand(commandName);

        if (command == null)
        {
            return CommandResult.Failure("Command not found");
        }

        var validationResult = command.ValidateArguments(args);

        if (!validationResult.Validated())
        {
            return CommandResult.Failure(
                "Invalid arguments",
                command.CommandName,
                validationResult.Errors
            );
        }

        var handlerArgs = args.ToDictionary(kvp => kvp.Key, kvp => (object)kvp.Value);

        var result = await command.Handle(handlerArgs);

        return result;
    }

    public static Dictionary<string, string>? ParseCommand(string input)
    {
        if (string.IsNullOrWhiteSpace(input) || !input.Trim().StartsWith("/"))
        {
            return null;
        }

        var results = new Dictionary<string, string>();

        var parts = Regex.Matches(input, @"[\""].+?[\""]|[^ ]+")
            .Cast<Match>()
            .Select(m => m.Value.Trim('"'))
            .ToList();

        if (parts.Count > 0)
        {
            results["command"] = parts[0].Substring(1);
        }

        int nonNamedArgIndex = 0;

        for (int i = 1; i < parts.Count; i++)
        {
            var part = parts[i];

            if (part.StartsWith("-"))
            {
                string[] argParts = part.Split(new[] { '=' }, 2);
                string argName = argParts[0].TrimStart('-');

                if (argParts.Length == 2)
                {
                    results[argName] = argParts[1];

                    continue;
                }

                continue;
            }

            results[nonNamedArgIndex.ToString()] = part;
            nonNamedArgIndex++;
        }

        return results;
    }
}
