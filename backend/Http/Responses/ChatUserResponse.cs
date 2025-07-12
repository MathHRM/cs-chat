namespace backend.Http.Responses;

public class ChatUserResponse
{
    public ChatResponse Chat { get; set; }
    public UserResponse User { get; set; }
    public List<UserResponse> Users { get; set; }
}