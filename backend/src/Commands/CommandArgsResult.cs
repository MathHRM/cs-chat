namespace backend.Commands;

public class CommandArgsResult
{
    public Dictionary<string, string> Errors { get; set; } = new Dictionary<string, string>();
    public Dictionary<string, string?> Args { get; set; } = new Dictionary<string, string?>();

    public bool Validated()
    {
        return Errors.Count == 0;
    }

    public void AddError(string arg, string error)
    {
        Errors.Add(arg, error);
    }

    public void AddArg(string arg, string? value)
    {
        Args.Add(arg, value);
    }
}