using backend.Commands;
using backend.Models;
using backend.Http.Responses;
using backend.Services;
using Microsoft.AspNetCore.SignalR;

namespace backend.src.Hubs;

public class HubProvider : Hub<IHubProvider>
{
    private readonly UserService _userService;
    private readonly CommandHandler _commandHandler;

    public HubProvider(UserService userService, CommandHandler commandHandler)
    {
        _userService = userService;
        _commandHandler = commandHandler;
    }

    public async Task SendMessage(Message message)
    {
        if (! Context.User.Identity.IsAuthenticated)
        {
            await HandleGuestUser(message);

            return;
        }

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

        if (_commandHandler.IsCommand(message.Content))
        {
            await Clients.Caller.ReceivedCommand(await _commandHandler.HandleCommand(message.Content, Context));
        }
    }

    private async Task HandleGuestUser(Message message)
    {
        await Clients.Caller.ReceivedMessage(new MessageResource
        {
            Message = new MessageResponse
            {
                Content = message.Content,
                CreatedAt = DateTime.UtcNow
            }
        });

        if (_commandHandler.IsCommand(message.Content))
        {
            await Clients.Caller.ReceivedCommand(await _commandHandler.HandleCommand(message.Content, Context));
        }
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
