namespace backend.Commands;

public class CommandArgsResult
{
    public Dictionary<string, string> Errors { get; set; } = new Dictionary<string, string>();
    public Dictionary<string, object?> Args { get; set; } = new Dictionary<string, object?>();

    public bool Validated()
    {
        return Errors.Count == 0;
    }

    public void AddError(string arg, string error)
    {
        Errors.Add(arg, error);
    }

    public void AddArg(string arg, object? value)
    {
        Args.Add(arg, value);
    }
}