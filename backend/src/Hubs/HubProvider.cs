using backend.Commands;
using backend.Models;
using backend.Http.Responses;
using backend.Services;
using AutoMapper;
using Microsoft.AspNetCore.SignalR;

namespace backend.src.Hubs;

public class HubProvider : Hub<IHubProvider>
{
    private readonly UserService _userService;
    private readonly CommandHandler _commandHandler;
    private readonly IMapper _mapper;

    public HubProvider(UserService userService, CommandHandler commandHandler, IMapper mapper)
    {
        _userService = userService;
        _commandHandler = commandHandler;
        _mapper = mapper;
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

        await Clients.Group(user.CurrentChatId!).ReceivedMessage(new MessageResponse
        {
            User = _mapper.Map<UserResponse>(user),
            Content = message.Content,
            Type = message.Type,
            CreatedAt = DateTime.UtcNow,
            Chat = _mapper.Map<ChatResponse>(user.CurrentChat)
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
