using backend.Commands;

public abstract class Command
{
    public abstract string CommandName { get; }
    public abstract string Description { get; }
    public abstract Dictionary<string, CommandArgument>? Args { get; }
    public abstract Task<CommandResult> Handle(Dictionary<string, object> args);
    public CommandArgsResult ValidateArguments(Dictionary<string, string> args)
    {
        var result = new CommandArgsResult();

        if (Args == null)
        {
            return result;
        }

        foreach (var arg in Args)
        {
            if (arg.Value.IsRequired && !args.ContainsKey(arg.Key))
            {
                result.AddError(arg.Key, $"Argument {arg.Value.Name} is required");

                continue;
            }

            result.AddArg(arg.Key, args[arg.Key]);
        }

        return result;
    }
}