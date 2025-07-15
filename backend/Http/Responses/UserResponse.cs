namespace backend.Http.Responses;

public class UserResponse
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string? CurrentChatId { get; set; }

    public UserResponse()
    {
        Id = 0;
        Username = "System";
    }
}