using backend.Models;
using backend.Http.Responses;
using backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace backend.src.Hubs;

public class HubProvider : Hub<IHubProvider>
{
    private readonly UserService _userService;

    public HubProvider(UserService userService)
    {
        _userService = userService;
    }

    [Authorize]
    public async Task SendMessage(Message message)
    {
        var user = await _userService.GetUserByUsernameAsync(Context.User.Identity.Name);
        Logger.Info($"Sending message from user {user.Username} to chat {user.CurrentChatId}");

        await Clients.Group(user.CurrentChatId!).ReceivedMessage(new MessageResource
        {
            User = new UserResponse
            {
                Id = user.Id,
                Username = user.Username,
                CurrentChatId = user.CurrentChatId
            },
            Message = new MessageResponse
            {
                Content = message.Content,
                CreatedAt = DateTime.UtcNow
            },
            Chat = new ChatResponse
            {
                Id = user.CurrentChatId
            }
        });
    }

    public async Task JoinChat(string chatId)
    {
        await RemoveUserFromOtherChats(chatId);
        await Groups.AddToGroupAsync(Context.ConnectionId, chatId);
        await Clients.Group(chatId).ReceivedMessage(new MessageResource
        {
            User = new UserResponse
            {
                Id = 0,
                Username = Context.User.Identity.Name,
                CurrentChatId = chatId
            },
            Message = new MessageResponse
            {
                Content = "Joined the chat",
                CreatedAt = DateTime.UtcNow
            },
            Chat = new ChatResponse
            {
                Id = chatId
            }
        });
    }

    private async Task RemoveUserFromOtherChats(string chatId)
    {
        var username = Context.User.Identity.Name;
        var user = await _userService.GetUserWithChatsAsync(username);

        Logger.Info($"Removing user {username} from other chats: {chatId}");

        foreach (var chatUser in user.ChatUsers.Where(cu => cu.ChatId != chatId))
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, chatUser.ChatId);
        }
    }
}
