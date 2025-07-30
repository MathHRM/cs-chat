namespace backend.Http.Responses;

public class CommandResponse
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public List<ArgumentResponse>? Arguments { get; set; }
    public List<ArgumentResponse>? Options { get; set; }
}
