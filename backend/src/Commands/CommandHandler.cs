using System.Text.RegularExpressions;

namespace backend.Commands;

public class CommandHandler
{
    public static async Task<CommandResult> HandleCommand(string commandInput)
    {
        var args = ParseCommand(commandInput);

        if (args == null)
        {
            return new CommandResult { Success = false, Message = "Invalid command" };
        }

        if (CommandsList.Commands.TryGetValue(args["command"], out Command? command))
        {
            var argsResult = command.ValidateArguments(args);

            if (!argsResult.Validated())
            {
                return new CommandResult { Success = false, Message = string.Join("\n", argsResult.Errors.Select(kvp => $"{kvp.Key}: {kvp.Value}")) };
            }

            var result = await command.Handle(args.ToDictionary(kvp => kvp.Key, kvp => (object)kvp.Value));

            if (!result.Success)
            {
                return new CommandResult { Success = false, Message = result.Message };
            }

            Console.WriteLine(result.Message);

            return result;
        }

        return new CommandResult { Success = false, Message = "Command not found" };
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

                results[argName] = "true";

                continue;
            }

            results[nonNamedArgIndex.ToString()] = part;
            nonNamedArgIndex++;
        }

        return results;
    }
}
