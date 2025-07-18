using backend.Commands;
using Microsoft.AspNetCore.SignalR;

public abstract class Command
{
    public abstract string CommandName { get; }
    public abstract string Description { get; }
    public HubCallerContext? HubCallerContext { get; set; }
    public IGroupManager? HubGroups { get; set; }
    public HttpContext? HttpContext { get; set; }
    public abstract Dictionary<string, CommandArgument>? Args { get; }
    public abstract Task<CommandResult> Handle(Dictionary<string, string?> args);
    public virtual bool RequiresAuthentication => true;

    public CommandArgsResult ValidateArguments(Dictionary<string, string> args)
    {
        var result = new CommandArgsResult();

        if (Args == null)
        {
            return result;
        }

        foreach (var arg in Args)
        {
            var argName = arg.Value.Name;
            var argValue = GetArgValue(arg.Value, args);

            if (argValue == null && arg.Value.IsRequired)
            {
                result.AddError(argName, $"Argument {argName} is required");
            }

            result.AddArg(argName, argValue);
        }

        return result;
    }

    private string? GetArgValue(CommandArgument arg, Dictionary<string, string> args)
    {
        if (arg.ByPosition && args.ContainsKey(arg.Position.ToString()) && !arg.IsFlag)
        {
            return args[arg.Position.ToString()];
        }

        if (!arg.ByPosition && args.ContainsKey(arg.Name) && !arg.IsFlag)
        {
            return args[arg.Name];
        }

        if (arg.IsFlag)
        {
            return args.ContainsKey(arg.Name) ? "true" : "false";
        }

        return null;
    }
}