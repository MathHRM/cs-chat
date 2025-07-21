using backend.Services;
using backend.Commands;
using backend.Http.Responses;
using AutoMapper;
using System.CommandLine;
namespace backend.Commands;

public class Login : CommandBase
{
    private readonly UserService _userService;
    private readonly TokenService _tokenService;
    private readonly ChatService _chatService;
    private readonly IMapper _mapper;
    public override string CommandName => "login";

    public override string Description => "Login to the system";

    public override bool ForAuthenticatedUsers => false;
    public override bool ForGuestUsers => true;

    // Arguments
    private readonly Option<string> _username = new Option<string>("--username", "-u") {
        Description = "The username to login with",
        Required = true,
    };
    private readonly Option<string> _password = new Option<string>("--password", "-pass") {
        Description = "The password to login with",
        Required = true,
    };

    public Login(UserService userService, TokenService tokenService, ChatService chatService, IMapper mapper)
    {
        _userService = userService;
        _tokenService = tokenService;
        _chatService = chatService;
        _mapper = mapper;
    }

    public override async Task<CommandResult> Handle(ParseResult parseResult)
    {
        var username = parseResult.GetValue<string>(_username);
        var password = parseResult.GetValue<string>(_password);

        var user = await _userService.ValidateUserCredentialsAsync(username, password);
        if (user == null)
        {
            return CommandResult.FailureResult("Invalid email or password", CommandName);
        }

        var token = _tokenService.GenerateToken(user);

        var chat = await _chatService.GetChatByIdAsync(user.CurrentChatId!);

        return new LoginResult
        {
            Response = new LoginResponse
            {
                Token = token,
                User = _mapper.Map<UserResponse>(user),
                Chat = _mapper.Map<ChatResponse>(chat)
            },
            Command = CommandName,
            Result = CommandResultEnum.Success,
            Message = "Login successful"
        };
    }

    public override Command GetCommandInstance()
    {
        var command = new Command(CommandName, Description)
        {
            _username,
            _password
        };
        command.TreatUnmatchedTokensAsErrors = false;
        return command;
    }
}