using backend.Commands;

namespace backend.Http.Responses;

public class MessageResponse
{
    public int Id { get; set; }
    public string Content { get; set; }
    public MessageType Type { get; set; }
    public DateTime CreatedAt { get; set; }
    public UserResponse User { get; set; }
    public ChatResponse Chat { get; set; }
}