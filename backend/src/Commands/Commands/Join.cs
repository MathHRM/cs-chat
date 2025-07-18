using backend.Http.Responses;
using backend.Services;
using Microsoft.AspNetCore.SignalR;
using AutoMapper;

namespace backend.Commands;

public class Join : Command
{
    private readonly UserService _userService;
    private readonly ChatService _chatService;
    private readonly IMapper _mapper;

    public override string CommandName => "join";

    public override string Description => "Join a chat";

    public Join(UserService userService, ChatService chatService, IMapper mapper)
    {
        _userService = userService;
        _chatService = chatService;
        _mapper = mapper;
    }

    public override Dictionary<string, CommandArgument>? Args => new Dictionary<string, CommandArgument>
    {
        {
            "chatId",
            new CommandArgument {
                Name = "chatId",
                IsRequired = true,
                Description = "The chat to join",
                ByPosition = true,
                Position = 0,
                Alias = "c",
            }
        },
        {
            "password",
            new CommandArgument {
                Name = "password",
                Description = "The password of the chat",
                Alias = "pass",
            }
        }
    };

    public override async Task<CommandResult> Handle(Dictionary<string, string?> args)
    {
        var chatId = args["chatId"] as string;
        var password = args["password"] as string;

        if (HubCallerContext == null)
        {
            return CommandResult.UnauthorizedResult(CommandName);
        }

        var user = await _userService.GetUserByUsernameAsync(HubCallerContext.User.Identity?.Name);

        if (user.CurrentChat.Id == chatId)
        {
            return CommandResult.SuccessResult($"You are already in chat {chatId}", CommandName, "You are already in chat");
        }

        var chat = await _chatService.GetChatByIdAsync(chatId);

        if (chat == null)
        {
            return CommandResult.FailureResult("Failed to join chat.", CommandName);
        }

        if (!chat.IsGroup)
        {
            return CommandResult.FailureResult("Chat is not a group to join", CommandName);
        }

        if (!chat.IsPublic && password == null)
        {
            return CommandResult.FailureResult("Chat is private and no password provided", CommandName);
        }

        if (chat.Password != null && !BCrypt.Net.BCrypt.Verify(password, chat.Password))
        {
            return CommandResult.FailureResult("Invalid password", CommandName);
        }

        await _chatService.JoinChatAsync(chat.Id, user.Id);

        await _userService.UpdateUserCurrentChatAsync(user, chat.Id, HubCallerContext, HubGroups);

        return new JoinResult
        {
            Response = new ChatUserResponse
            {
                Chat = _mapper.Map<ChatResponse>(chat),
                User = _mapper.Map<UserResponse>(user),
                Users = chat.ChatUsers.Select(cu => _mapper.Map<UserResponse>(cu.User)).ToList(),
            },
            Result = CommandResultEnum.Success,
            Message = "Chat joined successfully",
            Command = CommandName,
        };
    }
}