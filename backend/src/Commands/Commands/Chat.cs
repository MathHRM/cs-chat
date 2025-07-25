using backend.Http.Responses;
using backend.Services;
using Microsoft.AspNetCore.SignalR;
using AutoMapper;
using System.CommandLine;

namespace backend.Commands;

public class Chat : CommandBase
{
    public override string CommandName => "chat";
    public override string Description => "Converse com um usuário";
    public override bool ForAuthenticatedUsers => true;
    public override bool ForGuestUsers => false;

    public override Command GetCommandInstance()
    {
        var command = new Command(CommandName, Description)
        {
            _username,
        };
        command.TreatUnmatchedTokensAsErrors = false;
        return command;
    }

    // Arguments
    private readonly Argument<string> _username = new Argument<string>("username") {
        Description = "O nome de usuário para conversar",
    };

    private readonly UserService _userService;
    private readonly ChatService _chatService;
    private readonly HubConnectionService _hubConnectionService;
    private readonly IMapper _mapper;

    public Chat(UserService userService, ChatService chatService, HubConnectionService hubConnectionService, IMapper mapper)
    {
        _userService = userService;
        _chatService = chatService;
        _hubConnectionService = hubConnectionService;
        _mapper = mapper;
    }

    public override async Task<CommandResult> Handle(ParseResult parseResult)
    {
        var targetUsername = parseResult.GetValue(_username);

        if (HubCallerContext == null)
        {
            return CommandResult.UnauthorizedResult(CommandName);
        }

        var currentUser = await _userService.GetUserByIdAsync(AuthenticatedUserId);
        if (currentUser == null)
        {
            return CommandResult.UnauthorizedResult(CommandName);
        }

        if (string.IsNullOrEmpty(targetUsername))
        {
            return CommandResult.FailureResult("O nome de usuário é obrigatório", CommandName);
        }

        if (targetUsername.Equals(currentUser.Username, StringComparison.OrdinalIgnoreCase))
        {
            return CommandResult.FailureResult("Você não pode conversar com você mesmo", CommandName);
        }

        var chatId = _chatService.GeneratePrivateChatId(currentUser.Username, targetUsername);

        var existingChat = await _chatService.GetChatByIdAsync(chatId);
        if (existingChat != null)
        {
            await _hubConnectionService.UpdateUserCurrentChatAsync(currentUser.Id, chatId, HubCallerContext, HubGroups);

            return new JoinResult
            {
                Response = new ChatUserResponse
                {
                    Chat = _mapper.Map<ChatResponse>(existingChat),
                    User = _mapper.Map<UserResponse>(currentUser),
                    Users = existingChat.ChatUsers.Select(cu => _mapper.Map<UserResponse>(cu.User)).ToList(),
                },
                Result = CommandResultEnum.Success,
                Message = "Entrou no chat com sucesso",
                Command = CommandName,
            };
        }

        var targetUser = await _userService.GetUserByUsernameAsync(targetUsername);
        if (targetUser == null)
        {
            return CommandResult.FailureResult($"Usuário '{targetUsername}' não encontrado", CommandName);
        }

        var users = new List<Models.User> { currentUser, targetUser };
        var orderedUsernames = users.Select(u => u.Username).OrderBy(u => u, StringComparer.OrdinalIgnoreCase).ToList();
        var description = $"Chat entre {orderedUsernames[0]} e {orderedUsernames[1]}";
        var name = $"{orderedUsernames[0]}.{orderedUsernames[1]}";
        var password = Guid.NewGuid().ToString();

        var createdChat = await _chatService.CreateChatAsync(name, description, password, false, false, users);

        if (createdChat == null)
        {
            return CommandResult.FailureResult("Falha ao criar chat", CommandName);
        }

        await _hubConnectionService.UpdateUserCurrentChatAsync(currentUser.Id, createdChat.Id, HubCallerContext, HubGroups);

        return new JoinResult
        {
            Response = new ChatUserResponse
            {
                Chat = _mapper.Map<ChatResponse>(createdChat),
                User = _mapper.Map<UserResponse>(currentUser),
                Users = users.Select(u => _mapper.Map<UserResponse>(u)).ToList(),
            },
            Result = CommandResultEnum.Success,
            Message = "Chat criado com sucesso",
            Command = CommandName,
        };
    }
}