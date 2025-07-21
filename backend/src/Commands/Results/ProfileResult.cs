using backend.Http.Responses;

namespace backend.Commands;

public class ProfileResult : CommandResult
{
    public UserResponse? Response { get; set; }
}