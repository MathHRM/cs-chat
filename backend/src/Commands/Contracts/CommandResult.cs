using backend.Commands.Results;
using backend.Commands.Enums;
using backend.Http.Responses;

namespace backend.Commands;

public abstract class CommandResult
{
    public CommandResultEnum Result { get; set; }
    public string? Message { get; set; }
    public Dictionary<string, string> Errors { get; set; } = new();
    public string? Command { get; set; }

    public static CommandResult FailureResult(string message, string? command = null, Dictionary<string, string>? errors = null)
    {
        return new GenericResult
        {
            Result = CommandResultEnum.Error,
            Message = message,
            Command = command,
            Errors = errors ?? new Dictionary<string, string> { { "error", message } },
        };
    }

    public static CommandResult SuccessResult(string message, string? command = null, string? response = null)
    {
        return new GenericResult
        {
            Result = CommandResultEnum.Success,
            Message = message,
            Command = command,
            Response = new MessageResource
            {
                Message = new MessageResponse
                {
                    Content = response,
                    CreatedAt = DateTime.UtcNow,
                    Type = MessageType.Text
                },
            },
        };
    }

    public static CommandResult UnauthorizedResult(string? command = null)
    {
        return new GenericResult
        {
            Result = CommandResultEnum.Unauthorized,
            Message = "Unauthorized",
            Command = command,
        };
    }
}