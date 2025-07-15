using backend.Commands.Enums;

namespace backend.Http.Responses;

public class MessageResource
{
    public UserResponse User { get; set; } = new();
    public ChatResponse Chat { get; set; }
    public string Content { get; set; }
    public MessageType Type { get; set; }
    public DateTime CreatedAt { get; set; }
}