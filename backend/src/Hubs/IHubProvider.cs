using backend.Http.Responses;

namespace backend.src.Hubs;

public interface IHubProvider
{
    Task ReceivedMessage(MessageResource message);
}
