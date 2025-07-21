using backend.Services;
using backend.Http.Responses;
using backend.Models;
using AutoMapper;
using System.CommandLine;

namespace backend.Commands;

public class Register : CommandBase
{
    public override string CommandName => "register";
    public override string Description => "Register a new user";
    public override bool ForAuthenticatedUsers => false;
    public override bool ForGuestUsers => true;

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

    // Arguments
    private readonly Option<string> _username = new Option<string>("--username", "-u") {
        Description = "The username to register with",
        Required = true,
    };
    private readonly Option<string> _password = new Option<string>("--password", "-pass") {
        Description = "The password to register with",
        Required = true,
    };

    private readonly UserService _userService;
    private readonly TokenService _tokenService;
    private readonly ChatService _chatService;
    private readonly IMapper _mapper;

    public Register(UserService userService, TokenService tokenService, ChatService chatService, IMapper mapper)
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

        if (await _userService.UserExistsAsync(username))
        {
            return CommandResult.FailureResult("User with this Username already exists", CommandName);
        }

        var user = new User
        {
            Username = username,
            Password = password,
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