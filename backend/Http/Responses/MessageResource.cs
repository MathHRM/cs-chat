namespace backend.Http.Responses;

public class MessageResource
{
    public UserResponse User { get; set; } = new();
    public MessageResponse Message { get; set; }
    public ChatResponse Chat { get; set; }
}