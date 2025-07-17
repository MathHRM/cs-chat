namespace backend.Commands;

public class CommandArgument
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsRequired { get; set; } = false;
    public bool ByPosition { get; set; } = false;
    public int Position { get; set; } = 0;
    public bool IsFlag { get; set; } = false;
}