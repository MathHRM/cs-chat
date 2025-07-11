namespace backend.Http.Responses;

public class MessageResource
{
    public UserResponse User { get; set; }
    public MessageResponse Message { get; set; }
    public ChatResponse Chat { get; set; }
}