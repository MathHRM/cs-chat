using backend.Commands.Results;

namespace backend.Commands;

public abstract class CommandResult
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public Dictionary<string, string> Errors { get; set; } = new();
    public string? Command { get; set; }

    public static CommandResult FailureResult(string message, string? command = null, Dictionary<string, string>? errors = null)
    {
        return new GenericResult
        {
            Success = false,
            Message = message,
            Command = command,
            Errors = errors ?? new Dictionary<string, string> { { "error", message } }
        };
    }

    public static CommandResult SuccessResult(string message, string? command = null, object? response = null)
    {
        return new GenericResult
        {
            Success = true,
            Message = message,
            Command = command,
            Response = response
        };
    }
}