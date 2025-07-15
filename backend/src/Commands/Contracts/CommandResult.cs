using backend.Commands.Results;
using backend.Commands.Enums;
using backend.Http.Responses;

namespace backend.Commands;

public abstract class CommandResult
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public Dictionary<string, string> Errors { get; set; } = new();
    public string? Command { get; set; }
    public MessageType? ResponseType { get; set; } = MessageType.Text;

    public static CommandResult FailureResult(string message, string? command = null, Dictionary<string, string>? errors = null)
    {
        return new GenericResult
        {
            Success = false,
            Message = message,
            Command = command,
            Errors = errors ?? new Dictionary<string, string> { { "error", message } },
            ResponseType = MessageType.Error
        };
    }

    public static CommandResult SuccessResult(string message, string? command = null, string? response = null)
    {
        return new GenericResult
        {
            Success = true,
            Message = message,
            Command = command,
            Response = new MessageResource
            {
                Message = new MessageResponse
                {
                    Content = response,
                    CreatedAt = DateTime.UtcNow
                },
            },
            ResponseType = MessageType.Text
        };
    }

    public static CommandResult UnauthorizedResult(string? command = null)
    {
        return new GenericResult
        {
            Success = false,
            Message = "Unauthorized",
            Command = command,
            ResponseType = MessageType.Error
        };
    }
}