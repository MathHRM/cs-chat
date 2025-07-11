using backend.Models;

namespace backend.src.Hubs;

public interface IHubProvider
{
    Task ReceivedMessage(Message message);
}
