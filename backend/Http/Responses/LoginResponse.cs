namespace backend.Http.Responses;

public class LoginResponse
{
    public string Token { get; set; }
    public UserResponse User { get; set; }
    public ChatResponse Chat { get; set; }
}