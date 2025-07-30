namespace backend.Http.Responses;

public class ArgumentResponse
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public bool IsRequired { get; set; }
    public List<string>? Aliases { get; set; }
}