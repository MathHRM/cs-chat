using backend.Http.Responses;
using backend.Services;
using Microsoft.AspNetCore.SignalR;

namespace backend.Commands;

public class Join : Command
{
    private readonly UserService _userService;
    private readonly TokenService _tokenService;
    private readonly ChatService _chatService;

    public override string CommandName => "join";

    public override string Description => "Join a chat";

    public Join(UserService userService, TokenService tokenService, ChatService chatService)
    {
        _userService = userService;
        _tokenService = tokenService;
        _chatService = chatService;
    }

    public override Dictionary<string, CommandArgument>? Args => new Dictionary<string, CommandArgument>
    {
        {
            "chatId",
            new CommandArgument {
                Name = "chatId",
                IsRequired = true,
                Description = "The chat to join",
                ByPosition = true,
                Position = 0,
            }
        }
    };

    public override async Task<CommandResult> Handle(Dictionary<string, object?> args)
    {
        var chatId = args["chatId"] as string;
        var connection = args["connection"] as HubCallerContext;

        var user = await _userService.GetUserByUsernameAsync(connection.User.Identity?.Name);

        if (user.CurrentChatId == chatId)
        {
            return CommandResult.SuccessResult($"You are already in chat {chatId}", CommandName, "You are already in chat");
        }

        var chatUser = await _chatService.AddUserToChatAsync(chatId, user.Id);
        if (chatUser == null)
        {
            return CommandResult.FailureResult("Failed to join chat.", CommandName);
        }

        user.CurrentChatId = chatId;
        await _userService.UpdateUserAsync(user.Id, user);

        var chat = await _chatService.GetChatByIdAsync(chatId);

        return new JoinResult
        {
            Response = new ChatUserResponse
            {
                Chat = new ChatResponse
                {
                    Id = chatUser.ChatId,
                },
                User = new UserResponse
                {
                    Id = user.Id,
                    Username = user.Username,
                    CurrentChatId = user.CurrentChatId,
                },
                Users = chatUser.Chat.ChatUsers.Select(cu => new UserResponse
                {
                    Id = cu.User.Id,
                    Username = cu.User.Username,
                    CurrentChatId = cu.User.CurrentChatId,
                }).ToList(),
            },
            Result = CommandResultEnum.Success,
            Message = "Chat joined successfully",
            Command = CommandName,
        };
    }
}