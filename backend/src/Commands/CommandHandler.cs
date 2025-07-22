using System.Text.RegularExpressions;
using Microsoft.AspNetCore.SignalR;

namespace backend.Commands;

public class CommandHandler
{
    private readonly ICommandResolver _commandResolver;

    public CommandHandler(ICommandResolver commandResolver)
    {
        _commandResolver = commandResolver;
    }

    public async Task<CommandResult> HandleCommand(
        string commandInput,
        HubCallerContext? connection = null,
        IGroupManager? groups = null,
        HttpContext? httpContext = null
    )
    {
        var matches = Regex.Matches(commandInput.Substring(1), @"[\""].+?[\""]|[^ ]+");
        var commandArgs = matches.Select(m => m.Value.Trim('"')).ToArray();
        var commandName = commandArgs.FirstOrDefault();

        if (string.IsNullOrWhiteSpace(commandName))
        {
            return CommandResult.FailureResult("Invalid command", null, null, Error.CommandNotFound);
        }

        var command = _commandResolver.GetCommand(commandName);

        if (command == null)
        {
            return CommandResult.FailureResult("Command not found", commandName, null, Error.CommandNotFound);
        }

        if (!UserCanUseCommand(command, connection, httpContext))
        {
            return CommandResult.FailureResult("Invalid command", command.CommandName, null, Error.Unauthorized);
        }

        var commandInstance = command.GetCommandInstance();
        var validationResult = commandInstance.Parse(commandArgs);

        if (validationResult.Errors.Any())
        {
            return CommandResult.FailureResult(
                "Invalid arguments",
                command.CommandName,
                validationResult.Errors.ToDictionary(e => e.SymbolResult.ToString(), e => e.Message),
                Error.UnknownError
            );
        }

        command.HubCallerContext = connection;
        command.HubGroups = groups;
        command.HttpContext = httpContext;
        var result = await command.Handle(validationResult);

        return result;
    }

    public bool IsCommand(string input)
    {
        return !string.IsNullOrWhiteSpace(input) && input.Trim().StartsWith("/");
    }

    public bool UserIsAuthenticated(HubCallerContext? hubCallerContext, HttpContext? httpContext)
    {
        if (hubCallerContext != null && hubCallerContext.User?.Identity?.IsAuthenticated == true)
        {
            return true;
        }

        if (httpContext != null && httpContext.User?.Identity?.IsAuthenticated == true)
        {
            return true;
        }

        return false;
    }

    public bool UserCanUseCommand(CommandBase command, HubCallerContext? hubCallerContext, HttpContext? httpContext)
    {
        if (command.ForAuthenticatedUsers && !UserIsAuthenticated(hubCallerContext, httpContext) && !command.ForGuestUsers)
        {
            return false;
        }

        if (command.ForGuestUsers && UserIsAuthenticated(hubCallerContext, httpContext) && !command.ForAuthenticatedUsers)
        {
            return false;
        }

        return true;
    }
}
