using backend.Http.Responses;
using backend.Models;
using backend.Services;
using Microsoft.AspNetCore.SignalR;
using AutoMapper;
using System.CommandLine;

namespace backend.Commands;

public class Create : CommandBase
{
    public override string CommandName => "create";
    public override string Description => "Crie um chat";
    public override bool ForAuthenticatedUsers => true;
    public override bool ForGuestUsers => false;

    public override Command GetCommandInstance()
    {
        var command = new Command(CommandName, Description)
        {
            _name,
            _description,
            _password
        };
        command.TreatUnmatchedTokensAsErrors = false;
        return command;
    }

    // Arguments
    private readonly Argument<string> _name = new Argument<string>("name")
    {
        Description = "O nome do chat",
    };
    private readonly Option<string> _description = new Option<string>("--description", "-d")
    {
        Description = "A descrição do chat",
    };
    private readonly Option<string> _password = new Option<string>("--password", "-pass")
    {
        Description = "A senha do chat",
    };

    private readonly UserService _userService;
    private readonly ChatService _chatService;
    private readonly HubConnectionService _hubConnectionService;
    private readonly IMapper _mapper;

    public Create(UserService userService, ChatService chatService, HubConnectionService hubConnectionService, IMapper mapper)
    {
        _userService = userService;
        _chatService = chatService;
        _hubConnectionService = hubConnectionService;
        _mapper = mapper;
    }

    public override async Task<CommandResult> Handle(ParseResult parseResult)
    {
        var name = parseResult.GetValue(_name);
        var description = parseResult.GetValue(_description);
        var password = parseResult.GetValue(_password);
        var isPrivate = password != null;
        var user = await _userService.GetUserByIdAsync(AuthenticatedUserId);

        if (user == null)
        {
            return CommandResult.UnauthorizedResult(CommandName);
        }

        if (HubCallerContext == null)
        {
            return CommandResult.UnauthorizedResult(CommandName);
        }

        var chat = await _chatService.CreateChatAsync(name, description, password, !isPrivate, true, new List<User> { user });

        await _hubConnectionService.UpdateUserCurrentChatAsync(user.Id, chat.Id, HubCallerContext, HubGroups);

        return new JoinResult
        {
            Response = new ChatUserResponse
            {
                Chat = _mapper.Map<ChatResponse>(chat),
                User = _mapper.Map<UserResponse>(user),
                Users = chat.ChatUsers.Select(cu => _mapper.Map<UserResponse>(cu.User)).ToList(),
            },
            Result = CommandResultEnum.Success,
            Message = $"Chat {chat.Id} criado com sucesso",
            Command = CommandName,
        };
    }
}