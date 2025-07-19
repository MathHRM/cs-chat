namespace backend.Http.Requests;

public class ChatRequest
{
    public int? UserId { get; set; } = null;
    public string Name { get; set; }
    public string? Description { get; set; } = null;
    public string? Password { get; set; } = null;
}