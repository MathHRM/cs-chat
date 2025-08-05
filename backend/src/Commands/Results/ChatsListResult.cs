using backend.Http.Responses;

namespace backend.Commands;

public class ChatsListResult : CommandResult
{
    public List<ChatResponse>? Chats { get; set; } = new();
}