using backend.Services;
using backend.Commands;
using backend.Http.Responses;
using AutoMapper;

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

    public Login(UserService userService, TokenService tokenService, ChatService chatService, IMapper mapper)
    {
        _userService = userService;
        _tokenService = tokenService;
        _chatService = chatService;
        _mapper = mapper;
    }

    public override Dictionary<string, CommandArgument>? Args => new Dictionary<string, CommandArgument>
    {
        {
            "username",
            new CommandArgument {
                Name = "username",
                IsRequired = true,
                Description = "The username to login with",
                Alias = "u",
            }
        },
        {
            "password",
            new CommandArgument {
                Name = "password",
                IsRequired = true,
                Description = "The password to login with",
                Alias = "pass",
            }
        }
    };

    public override async Task<CommandResult> Handle(Dictionary<string, string?> args)
    {
        var user = await _userService.ValidateUserCredentialsAsync(args["username"] as string, args["password"] as string);
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
}