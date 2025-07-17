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
            "chatName",
            new CommandArgument {
                Name = "chatName",
                IsRequired = true,
                Description = "The chat to join",
                ByPosition = true,
                Position = 0,
            }
        }
    };

    public override async Task<CommandResult> Handle(Dictionary<string, object?> args)
    {
        var chatName = args["chatName"] as string;
        var connection = args["connection"] as HubCallerContext;
        var groups = args["groups"] as IGroupManager;

        if (connection == null)
        {
            return CommandResult.UnauthorizedResult(CommandName);
        }

        var user = await _userService.GetUserByUsernameAsync(connection.User.Identity?.Name);

        if (user.CurrentChat.Name == chatName)
        {
            return CommandResult.SuccessResult($"You are already in chat {chatName}", CommandName, "You are already in chat");
        }

        var chat = await _chatService.JoinChatAsync(chatName, user.Id);

        if (chat == null)
        {
            return CommandResult.FailureResult("Failed to join chat.", CommandName);
        }

        await RemoveUserFromOtherChats(chat.Id, connection, groups);
        await groups.AddToGroupAsync(connection.ConnectionId, chat.Id);

        return new JoinResult
        {
            Response = new ChatUserResponse
            {
                Chat = new ChatResponse
                {
                    Id = chat.Id,
                    Name = chat.Name,
                    IsPublic = chat.IsPublic,
                    IsGroup = chat.IsGroup,
                },
                User = new UserResponse
                {
                    Id = user.Id,
                    Username = user.Username,
                    CurrentChatId = user.CurrentChatId,
                },
                Users = chat.ChatUsers.Select(cu => new UserResponse
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

    private async Task RemoveUserFromOtherChats(string chatId, HubCallerContext connection, IGroupManager groups)
    {
        var username = connection.User.Identity.Name;
        var user = await _userService.GetUserWithChatsAsync(username);

        Logger.Info($"Removing user {username} from other chats: {chatId}");

        foreach (var chatUser in user.ChatUsers.Where(cu => cu.ChatId != chatId))
        {
            await groups.RemoveFromGroupAsync(connection.ConnectionId, chatUser.ChatId);
        }
    }
}