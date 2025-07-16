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

    public override async Task OnConnectedAsync()
    {
        if (! Context.User.Identity.IsAuthenticated)
            return;

        var user = await _userService.GetUserByUsernameAsync(Context.User.Identity.Name);

        await Groups.AddToGroupAsync(Context.ConnectionId, user.CurrentChatId!);
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
        Logger.Info(message.Content);

        if (_commandHandler.IsCommand(message.Content))
        {
            await Clients.Caller.ReceivedCommand(await _commandHandler.HandleCommand(message.Content, Context, Groups));

            return;
        }

        await Clients.Group(user.CurrentChatId!).ReceivedMessage(new MessageResource
        {
            User = new UserResponse
            {
                Id = user.Id,
                Username = user.Username,
                CurrentChatId = user.CurrentChatId
            },
            Content = message.Content,
            Type = message.Type,
            CreatedAt = DateTime.UtcNow,
            Chat = new ChatResponse
            {
                Id = user.CurrentChatId
            }
        });
    }

    private async Task HandleGuestUser(Message message)
    {
        if (_commandHandler.IsCommand(message.Content))
        {
            await Clients.Caller.ReceivedCommand(await _commandHandler.HandleCommand(message.Content, Context, Groups));
        }
    }
}
