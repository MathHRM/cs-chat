using backend.Commands;
using backend.Http.Responses;

namespace backend.src.Hubs;

public interface IHubProvider
{
    Task ReceivedMessage(MessageResponse message);
    Task ReceivedCommand(CommandResult result);
}
