using backend.Http.Responses;

namespace backend.Commands.Results;

public class JoinResult : CommandResult
{
    public ChatUserResponse? Response { get; set; }
}