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
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var user = await _userService.GetUserByUsernameAsync(Context.User.Identity.Name);
    }

    [Authorize]
    public async Task SendMessage(Message message)
    {
        var user = await _userService.GetUserByUsernameAsync(Context.User.Identity.Name);

        await Clients.All.ReceivedMessage(new MessageResource
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
            }
        });
    }
}
