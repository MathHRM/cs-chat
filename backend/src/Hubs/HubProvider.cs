using backend.Models;
using Microsoft.AspNetCore.SignalR;

namespace backend.src.Hubs
{
    public class HubProvider : Hub<IHubProvider>
    {
        public async Task SendMessage(Message message)
        {
            await Clients.All.ReceivedMessage(message);
        }
    }
}
