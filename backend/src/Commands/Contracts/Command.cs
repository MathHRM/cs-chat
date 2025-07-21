using backend.Commands;
using System.Security.Claims;
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
    public virtual bool ForAuthenticatedUsers => false;
    public virtual bool ForGuestUsers => false;

    public bool UserIsAuthenticated
    {
        get
        {
            if (HubCallerContext != null && HubCallerContext.User?.Identity?.IsAuthenticated == true)
            {
                return true;
            }

            if (HttpContext != null && HttpContext.User?.Identity?.IsAuthenticated == true)
            {
                return true;
            }

            return false;
        }

    }

    public int? AuthenticatedUserId
    {
        get
        {
            if (!UserIsAuthenticated)
            {
                return null;
            }

            var userId = HubCallerContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
            {
                return null;
            }

            return int.Parse(userId);
        }
    }

    public CommandArgsResult ValidateArguments(Dictionary<string, string?> args)
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
            argValue = string.IsNullOrEmpty(argValue) ? null : argValue;

            if (argValue == null && arg.Value.IsRequired)
            {
                result.AddError(argName, $"Argument {argName} is required");
            }

            result.AddArg(argName, argValue);
        }

        return result;
    }

    private string? GetArgValue(CommandArgument arg, Dictionary<string, string?> args)
    {
        if (arg.ByPosition && args.ContainsKey(arg.Position.ToString()) && !arg.IsFlag)
        {
            return args[arg.Position.ToString()];
        }

        if (!arg.ByPosition && ArgContains(arg, args) && !arg.IsFlag)
        {
            return GetByNameOrAlias(arg, args);
        }

        if (arg.IsFlag)
        {
            return ArgContains(arg, args) ? "true" : null;
        }

        return null;
    }

    private bool ArgContains(CommandArgument arg, Dictionary<string, string?> args)
    {
        if (args.ContainsKey(arg.Name))
        {
            return true;
        }

        if (arg.Alias != null && args.ContainsKey(arg.Alias))
        {
            return true;
        }

        return false;
    }

    private string? GetByNameOrAlias(CommandArgument arg, Dictionary<string, string?> args)
    {
        if (args.ContainsKey(arg.Name))
        {
            return args[arg.Name];
        }

        if (arg.Alias != null && args.ContainsKey(arg.Alias))
        {
            return args[arg.Alias];
        }

        return null;
    }
}
