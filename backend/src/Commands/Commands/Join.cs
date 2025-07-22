using backend.Http.Responses;
using backend.Services;
using Microsoft.AspNetCore.SignalR;
using AutoMapper;
using System.CommandLine;

namespace backend.Commands;

public class Join : CommandBase
{
    public override string CommandName => "join";
    public override string Description => "Join a chat";
    public override bool ForAuthenticatedUsers => true;
    public override bool ForGuestUsers => false;

    public override Command GetCommandInstance()
    {
        var command = new Command(CommandName, Description)
        {
            _chatId,
            _password
        };
        command.TreatUnmatchedTokensAsErrors = false;
        return command;
    }

    // Arguments
    private readonly Argument<string> _chatId = new Argument<string>("chatId")
    {
        Description = "The chat to join",
    };
    private readonly Option<string> _password = new Option<string>("--password", "-pass")
    {
        Description = "The password of the chat",
    };

    private readonly UserService _userService;
    private readonly ChatService _chatService;
    private readonly HubConnectionService _hubConnectionService;
    private readonly IMapper _mapper;

    public Join(UserService userService, ChatService chatService, HubConnectionService hubConnectionService, IMapper mapper)
    {
        _userService = userService;
        _chatService = chatService;
        _hubConnectionService = hubConnectionService;
        _mapper = mapper;
    }

    public override async Task<CommandResult> Handle(ParseResult parseResult)
    {
        var chatId = parseResult.GetValue(_chatId);
        var password = parseResult.GetValue(_password);

        if (HubCallerContext == null)
        {
            return CommandResult.UnauthorizedResult(CommandName);
        }

        var user = await _userService.GetUserByIdAsync(AuthenticatedUserId);

        if (user?.CurrentChat?.Id == chatId)
        {
            return CommandResult.SuccessResult($"You are already in chat {chatId}", CommandName, "You are already in chat");
        }

        var chat = await _chatService.GetChatByIdAsync(chatId);

        if (chat == null)
        {
            return CommandResult.FailureResult("Failed to join chat.", CommandName);
        }

        if (!chat.IsGroup)
        {
            return CommandResult.FailureResult("Chat is not a group to join", CommandName);
        }

        var userBelongsToChat = await _chatService.UserBelongsToChatAsync(user.Id, chat.Id);

        if (!userBelongsToChat && chat.Password != null && password == null)
        {
            return CommandResult.FailureResult("Chat is private and no password provided", CommandName);
        }

        if (!userBelongsToChat && chat.Password != null && !BCrypt.Net.BCrypt.Verify(password, chat.Password))
        {
            return CommandResult.FailureResult("Invalid password", CommandName);
        }

        if (!userBelongsToChat)
        {
            await _chatService.JoinChatAsync(chat.Id, user.Id);
        }

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
            Message = "Chat joined successfully",
            Command = CommandName,
        };
    }
}