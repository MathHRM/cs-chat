using backend.Services;
using backend.Http.Responses;
using backend.Models;
using AutoMapper;

namespace backend.Commands;

public class Register : Command
{
    private readonly UserService _userService;
    private readonly TokenService _tokenService;
    private readonly ChatService _chatService;
    private readonly IMapper _mapper;
    public override string CommandName => "register";

    public override string Description => "Register a new user";

    public override bool RequiresAuthentication => false;

    public Register(UserService userService, TokenService tokenService, ChatService chatService, IMapper mapper)
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
                Description = "The username to register with"
            }
        },
        {
            "password",
            new CommandArgument {
                Name = "password",
                IsRequired = true,
                Description = "The password to register with"
            }
        }
    };

    public override async Task<CommandResult> Handle(Dictionary<string, string?> args)
    {
        if (await _userService.UserExistsAsync(args["username"] as string))
        {
            return CommandResult.FailureResult("User with this Username already exists", CommandName);
        }

        var user = new User
        {
            Username = args["username"] as string,
            Password = args["password"] as string,
        };

        var createdUser = await _userService.CreateUserAsync(user);

        var chat = await _chatService.GetChatByIdAsync(createdUser.CurrentChatId!);

        return new LoginResult
        {
            Response = new LoginResponse
            {
                Token = _tokenService.GenerateToken(createdUser),
                User = _mapper.Map<UserResponse>(createdUser),
                Chat = _mapper.Map<ChatResponse>(chat)
            },
            Command = CommandName,
            Result = CommandResultEnum.Success,
            Message = "Register successful"
        };
    }
}