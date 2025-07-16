using backend.Http.Responses;
using backend.Services;
using Microsoft.AspNetCore.SignalR;

namespace backend.Commands;

public class Chat : Command
{
    private readonly UserService _userService;
    private readonly TokenService _tokenService;
    private readonly ChatService _chatService;

    public override string CommandName => "chat";

    public override string Description => "Chat with a user";

    public Chat(UserService userService, TokenService tokenService, ChatService chatService)
    {
        _userService = userService;
        _tokenService = tokenService;
        _chatService = chatService;
    }

    public override Dictionary<string, CommandArgument>? Args => new Dictionary<string, CommandArgument>
    {
        {
            "username",
            new CommandArgument {
                Name = "username",
                IsRequired = true,
                Description = "The username to chat with",
                ByPosition = true,
                Position = 0,
            }
        }
    };

    public override async Task<CommandResult> Handle(Dictionary<string, object?> args)
    {
        var targetUsername = args["username"] as string;
        var connection = args["connection"] as HubCallerContext;
        var groups = args["groups"] as IGroupManager;

        if (connection == null)
        {
            return CommandResult.UnauthorizedResult(CommandName);
        }

        var currentUser = await _userService.GetUserByUsernameAsync(connection.User.Identity?.Name);
        if (currentUser == null)
        {
            return CommandResult.UnauthorizedResult(CommandName);
        }

        if (string.IsNullOrEmpty(targetUsername))
        {
            return CommandResult.FailureResult("Username is required", CommandName);
        }

        // Can't chat with yourself
        if (targetUsername.Equals(currentUser.Username, StringComparison.OrdinalIgnoreCase))
        {
            return CommandResult.FailureResult("You cannot chat with yourself", CommandName);
        }

        var chatId = _chatService.GeneratePrivateChatId(currentUser.Username, targetUsername);

        // Check if chat already exists
        var existingChat = await _chatService.GetChatByIdAsync(chatId);
        if (existingChat != null)
        {
            // Update current user's chat and join the existing chat
            currentUser.CurrentChatId = chatId;
            await _userService.UpdateUserAsync(currentUser.Id, currentUser);

            await RemoveUserFromOtherChats(chatId, connection, groups);
            await groups.AddToGroupAsync(connection.ConnectionId, chatId);

            return new JoinResult
            {
                Response = new ChatUserResponse
                {
                    Chat = new ChatResponse
                    {
                        Id = chatId,
                        Name = existingChat.Name,
                    },
                    User = new UserResponse
                    {
                        Id = currentUser.Id,
                        Username = currentUser.Username,
                        CurrentChatId = currentUser.CurrentChatId,
                    },
                    Users = existingChat.ChatUsers.Select(cu => new UserResponse
                    {
                        Id = cu.User.Id,
                        Username = cu.User.Username,
                        CurrentChatId = cu.User.CurrentChatId,
                    }).ToList(),
                },
                Result = CommandResultEnum.Success,
                Message = "Joined existing chat successfully",
                Command = CommandName,
            };
        }

        // Verify target user exists
        var targetUser = await _userService.GetUserByUsernameAsync(targetUsername);
        if (targetUser == null)
        {
            return CommandResult.FailureResult($"User '{targetUsername}' not found", CommandName);
        }

        // Create new chat with both users
        var users = new List<Models.User> { currentUser, targetUser };
        var createdChat = await _chatService.CreateChatAsync(null, users, false);

        if (createdChat == null)
        {
            return CommandResult.FailureResult("Failed to create chat", CommandName);
        }

        // Update current user's chat
        currentUser.CurrentChatId = createdChat.Id;
        await _userService.UpdateUserAsync(currentUser.Id, currentUser);

        // Join the chat group
        await RemoveUserFromOtherChats(createdChat.Id, connection, groups);
        await groups.AddToGroupAsync(connection.ConnectionId, createdChat.Id);

        return new JoinResult
        {
            Response = new ChatUserResponse
            {
                Chat = new ChatResponse
                {
                    Id = createdChat.Id,
                    Name = createdChat.Name,
                },
                User = new UserResponse
                {
                    Id = currentUser.Id,
                    Username = currentUser.Username,
                    CurrentChatId = currentUser.CurrentChatId,
                },
                Users = new List<UserResponse>
                {
                    new UserResponse
                    {
                        Id = currentUser.Id,
                        Username = currentUser.Username,
                        CurrentChatId = currentUser.CurrentChatId,
                    },
                    new UserResponse
                    {
                        Id = targetUser.Id,
                        Username = targetUser.Username,
                        CurrentChatId = targetUser.CurrentChatId,
                    }
                },
            },
            Result = CommandResultEnum.Success,
            Message = "Chat created successfully",
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