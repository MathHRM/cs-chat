using backend.Http.Responses;

namespace backend.Commands.Results;

public class LoginResult : CommandResult
{
    public LoginResponse? Response { get; set; }
}