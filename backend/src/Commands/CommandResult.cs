namespace backend.Commands;

public class CommandResult
{
    public bool Success { get; set; }
    public string? Message { get; set; }

    public static CommandResult Failure(string message)
    {
        return new CommandResult
        {
            Success = false,
            Message = message
        };
    }
}