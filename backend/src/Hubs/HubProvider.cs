using backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace backend.src.Hubs;

public class HubProvider : Hub<IHubProvider>
{
    [Authorize]
    public async Task SendMessage(Message message)
    {
        await Clients.All.ReceivedMessage(message);
    }
}
