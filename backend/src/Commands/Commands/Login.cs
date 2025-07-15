using backend.Services;
using backend.Commands.Results;
using backend.Http.Responses;

namespace backend.Commands.Commands;

public class Login : Command
{
    private readonly UserService _userService;
    private readonly TokenService _tokenService;

    public Login(UserService userService, TokenService tokenService)
    {
        _userService = userService;
        _tokenService = tokenService;
    }

    public override string CommandName => "login";

    public override string Description => "Login to the system";

    public override Dictionary<string, CommandArgument>? Args => new Dictionary<string, CommandArgument>
    {
        { "username", new CommandArgument { Name = "username", IsRequired = true, Description = "The username to login with" } },
        { "password", new CommandArgument { Name = "password", IsRequired = true, Description = "The password to login with" } }
    };

    public override async Task<CommandResult> Handle(Dictionary<string, object> args)
    {
        var user = await _userService.ValidateUserCredentialsAsync(args["username"] as string, args["password"] as string);
        if (user == null)
        {
            return CommandResult.Failure("Invalid email or password", CommandName);
        }

        var token = _tokenService.GenerateToken(user);

        return new LoginResult
        {
            Response = new LoginResponse
            {
                Token = token,
                User = new UserResponse
                {
                    Id = user.Id,
                    Username = user.Username,
                    CurrentChatId = user.CurrentChatId ?? "general"
                }
            },
            Command = CommandName,
            Success = true,
            Message = "Login successful"
        };
    }
}