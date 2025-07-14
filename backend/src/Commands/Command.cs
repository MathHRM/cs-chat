using backend.Commands;

public abstract class Command
{
    public string Description { get; }
    public Dictionary<string, CommandArgument>? Args { get; }
    public abstract Task Handle(Dictionary<string, object> args);
    public List<string>? Validate(Dictionary<string, object> args)
    {
        if (Args == null)
        {
            return null;
        }

        var errors = new List<string>();

        foreach (var arg in Args)
        {
            if (arg.Value.IsRequired && !args.ContainsKey(arg.Key))
            {
                errors.Add($"Argument {arg.Key} is required");
            }
        }

        return errors;
    }

    private CommandArgsResult GetCommandArgs(string command)
    {
        var result = new CommandArgsResult();

        if (Args == null)
        {
            return result;
        }

        var parts = command.Split(' ').Skip(1).ToList().FindAll(p => !p.StartsWith("-"));

        foreach (var argDefinition in Args)
        {
            var argName = argDefinition.Key;
            var argInfo = argDefinition.Value;

            if (argInfo.ByPosition)
            {
                if (parts.Count > 0)
                {
                    result.AddArg(argName, parts[0]);
                    parts.RemoveAt(0);
                }
                else if (argInfo.IsRequired)
                {
                    result.AddError(argName, "required");
                }
            }
            else // Named argument
            {
                var argIndex = parts.FindIndex(p => p == $"--{argName}" || p == $"-{argName.First()}");
                if (argIndex != -1 && parts.Count > argIndex + 1)
                {
                    result.AddArg(argName, parts[argIndex + 1]);
                    parts.RemoveRange(argIndex, 2);
                }
                else if (argInfo.IsRequired)
                {
                    result.AddError(argName, "required");
                }
            }
        }

        return result;
    }

}