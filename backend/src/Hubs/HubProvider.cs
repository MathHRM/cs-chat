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

    public override async Task OnConnectedAsync()
    {
        var user = await _userService.GetUserByUsernameAsync(Context.User.Identity.Name);

        if (user.CurrentChatId == null)
        {
            user.CurrentChatId = "general";
            await _userService.UpdateUserAsync(user.Id, user);
        }

        await JoinChat(user.CurrentChatId);
    }

    [Authorize]
    public async Task SendMessage(Message message)
    {
        var user = await _userService.GetUserByUsernameAsync(Context.User.Identity.Name);

        await Clients.Group(user.CurrentChatId!).ReceivedMessage(new MessageResource
        {
            User = new UserResponse
            {
                Id = user.Id,
                Username = user.Username
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
        await Groups.AddToGroupAsync(Context.ConnectionId, chatId);
        await Clients.Group(chatId).ReceivedMessage(new MessageResource
        {
            User = new UserResponse
            {
                Id = 0,
                Username = Context.User.Identity.Name
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
}
