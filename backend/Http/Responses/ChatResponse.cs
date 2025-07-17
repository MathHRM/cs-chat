using backend.Models;

namespace backend.Http.Responses;

public class ChatResponse
{
    public string? Id { get; set; }
    public string? Name { get; set; }
    public bool IsPublic { get; set; }
    public bool IsGroup { get; set; }
}