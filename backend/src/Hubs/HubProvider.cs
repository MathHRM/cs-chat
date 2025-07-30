using backend.Commands;
using backend.Models;
using backend.Http.Responses;
using backend.Services;
using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace backend.src.Hubs;

public class HubProvider : Hub<IHubProvider>
{
    private readonly UserService _userService;
    private readonly CommandHandler _commandHandler;
    private readonly IMapper _mapper;
    private readonly MessageService _messageService;
    private readonly ChatService _chatService;

    public HubProvider(UserService userService, CommandHandler commandHandler, IMapper mapper, MessageService messageService, ChatService chatService)
    {
        _userService = userService;
        _commandHandler = commandHandler;
        _mapper = mapper;
        _messageService = messageService;
        _chatService = chatService;
    }

    public override async Task OnConnectedAsync()
    {
        if (!Context.User.Identity.IsAuthenticated)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "guest");

            return;
        }

        var authenticatedUserId = int.Parse(Context.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var user = await _userService.GetUserByIdAsync(authenticatedUserId);

        await Groups.AddToGroupAsync(Context.ConnectionId, user.CurrentChatId!);
    }

    public async Task SendMessage(Message message)
    {
        if (!Context.User.Identity.IsAuthenticated)
        {
            await HandleGuestUser(message);

            return;
        }

        var authenticatedUserId = int.Parse(Context.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var user = await _userService.GetUserByIdAsync(authenticatedUserId);

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

            return;
        }

        var guestChat = await _chatService.GetChatByIdAsync("guest");

        var guestMessage = await _messageService.CreateMessageAsync(guestChat, null, message.Content, message.Type, Context.ConnectionId!);

        await Clients.Group("guest").ReceivedMessage(_mapper.Map<MessageResponse>(guestMessage));
    }
}
