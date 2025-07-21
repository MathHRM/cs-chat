using backend.Commands;
using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;
using System.CommandLine;

public abstract class CommandBase
{
    public abstract string CommandName { get; }
    public abstract string Description { get; }
    public HubCallerContext? HubCallerContext { get; set; }
    public IGroupManager? HubGroups { get; set; }
    public HttpContext? HttpContext { get; set; }
    public abstract Task<CommandResult> Handle(ParseResult parseResult);
    public virtual bool ForAuthenticatedUsers => false;
    public virtual bool ForGuestUsers => false;
    public abstract Command GetCommandInstance();

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
}
