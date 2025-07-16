using backend.Http.Responses;

namespace backend.Commands;

public class LoginResult : CommandResult
{
    public LoginResponse? Response { get; set; }
}