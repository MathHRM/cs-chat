namespace backend.Commands;

public class CommandArgument
{
    public string Name { get; set; }
    public string Description { get; set; }
    public bool IsRequired { get; set; }
    public bool ByPosition { get; set; }
}