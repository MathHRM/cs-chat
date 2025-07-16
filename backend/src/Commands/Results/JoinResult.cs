using backend.Http.Responses;

namespace backend.Commands;

public class JoinResult : CommandResult
{
    public ChatUserResponse? Response { get; set; }
}