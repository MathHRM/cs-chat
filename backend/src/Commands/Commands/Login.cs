using backend.Services;
using backend.Commands;
using backend.Http.Responses;
using AutoMapper;
using System.CommandLine;
namespace backend.Commands;

public class Login : CommandBase
{
    public override string CommandName => "login";
    public override string Description => "Login no sistema";
    public override bool ForAuthenticatedUsers => false;
    public override bool ForGuestUsers => true;

    private readonly UserService _userService;
    private readonly TokenService _tokenService;
    private readonly ChatService _chatService;
    private readonly IMapper _mapper;

    // Arguments
    private readonly Option<string> _username = new Option<string>("--username", "-u")
    {
        Description = "O nome de usuário para login",
        Required = true,
    };
    private readonly Option<string> _password = new Option<string>("--password", "-pass")
    {
        Description = "A senha para login",
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
        var username = parseResult.GetValue(_username);
        var password = parseResult.GetValue(_password);

        var user = await _userService.ValidateUserCredentialsAsync(username, password);
        if (user == null)
        {
            return CommandResult.FailureResult("Email ou senha inválidos", CommandName);
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
            Message = "Login realizado com sucesso"
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