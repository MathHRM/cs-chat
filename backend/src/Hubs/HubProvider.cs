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
    private readonly MessageService _messageService;

    public HubProvider(UserService userService, CommandHandler commandHandler, IMapper mapper, MessageService messageService)
    {
        _userService = userService;
        _commandHandler = commandHandler;
        _mapper = mapper;
        _messageService = messageService;
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

        var messageModel = await _messageService.CreateMessageAsync(user.CurrentChat!, user, message.Content, message.Type);

        await Clients.Group(user.CurrentChatId!).ReceivedMessage(_mapper.Map<MessageResponse>(messageModel));
    }

    private async Task HandleGuestUser(Message message)
    {
        if (_commandHandler.IsCommand(message.Content))
        {
            await Clients.Caller.ReceivedCommand(await _commandHandler.HandleCommand(message.Content, Context, Groups));
        }
    }
}
