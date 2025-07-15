using backend.Commands.Enums;

namespace backend.Http.Responses;

public class MessageResponse
{
    public string Content { get; set; }
    public MessageType Type { get; set; }
    public DateTime CreatedAt { get; set; }
}