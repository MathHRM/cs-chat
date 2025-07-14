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
        // Parse the command input into arguments
        var args = ParseCommand(commandInput);

        if (args == null || !args.TryGetValue("command", out var commandName) || string.IsNullOrWhiteSpace(commandName))
        {
            return CommandResult.Failure("Invalid command");
        }

        // Resolve the command implementation
        var command = _commandResolver.GetCommand(commandName);

        if (command == null)
        {
            return CommandResult.Failure("Command not found");
        }

        // Validate the arguments for the command
        var validationResult = command.ValidateArguments(args);

        if (!validationResult.Validated())
        {
            var errorMessages = validationResult.Errors
                .Select(kvp => $"{kvp.Key}: {kvp.Value}");
            return CommandResult.Failure(string.Join(Environment.NewLine, errorMessages));
        }

        // Prepare arguments for the command handler
        var handlerArgs = args.ToDictionary(kvp => kvp.Key, kvp => (object)kvp.Value);

        // Execute the command
        var result = await command.Handle(handlerArgs);

        if (!result.Success)
        {
            return CommandResult.Failure(result.Message);
        }

        Console.WriteLine(result.Message);

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
