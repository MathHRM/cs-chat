using backend.Http.Responses;

namespace backend.Commands;

public abstract class CommandResult
{
    public CommandResultEnum Result { get; set; }
    public string? Message { get; set; }
    public Dictionary<string, string> Errors { get; set; } = new();
    public string? Command { get; set; }
    public Error? Error { get; set; }

    public static CommandResult FailureResult(
        string message,
        string? command = null,
        Dictionary<string, string>? errors = null,
        Error? error = null
    )
    {
        return new GenericResult
        {
            Result = CommandResultEnum.Error,
            Message = message,
            Command = command,
            Errors = errors ?? new Dictionary<string, string> { { "error", message } },
            Error = error,
            Response = new MessageResponse
            {
                Content = message,
                Type = MessageType.Error,
                CreatedAt = DateTime.UtcNow
            }
        };
    }

    public static CommandResult SuccessResult(string message, string? command = null, string? response = null)
    {
        return new GenericResult
        {
            Result = CommandResultEnum.Success,
            Message = message,
            Command = command,
            Response = new MessageResponse
            {
                Content = response,
                Type = MessageType.Success,
                CreatedAt = DateTime.UtcNow
            },
        };
    }

    public static CommandResult UnauthorizedResult(string? command = null)
    {
        return new GenericResult
        {
            Result = CommandResultEnum.Unauthorized,
            Message = "Não autorizado",
            Command = command,
            Error = Commands.Error.Unauthorized,
            Response = new MessageResponse
            {
                Content = "Não autorizado",
                Type = MessageType.Error,
                CreatedAt = DateTime.UtcNow
            }
        };
    }
}