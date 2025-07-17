using backend.Http.Responses;
using backend.Models;
using backend.Services;
using Microsoft.AspNetCore.SignalR;
using AutoMapper;

namespace backend.Commands;

public class Create : Command
{
    private readonly UserService _userService;
    private readonly TokenService _tokenService;
    private readonly ChatService _chatService;
    private readonly IMapper _mapper;

    public override string CommandName => "create";

    public override string Description => "Create a chat";

    public Create(UserService userService, TokenService tokenService, ChatService chatService, IMapper mapper)
    {
        _userService = userService;
        _tokenService = tokenService;
        _chatService = chatService;
        _mapper = mapper;
    }

    public override Dictionary<string, CommandArgument>? Args => new Dictionary<string, CommandArgument>
    {
        {
            "name",
            new CommandArgument {
                Name = "name",
                IsRequired = true,
                Description = "The name of the chat to create",
                ByPosition = true,
                Position = 0,
            }
        },
        {
            "private",
            new CommandArgument {
                Name = "private",
                Description = "Whether the chat is private",
                IsFlag = true,
            }
        },
        {
            "password",
            new CommandArgument {
                Name = "password",
                Description = "The password of the chat",
            }
        },
    };

    public override async Task<CommandResult> Handle(Dictionary<string, object?> args)
    {
        var name = args["name"] as string;
        var isPrivate = (args["private"] as string) == "true";
        var password = args["password"] as string;
        var connection = args["connection"] as HubCallerContext;
        var groups = args["groups"] as IGroupManager;
        var user = await _userService.GetUserByUsernameAsync(connection.User.Identity.Name);

        if (connection == null)
        {
            return CommandResult.UnauthorizedResult(CommandName);
        }

        if (isPrivate && password == null)
        {
            return CommandResult.FailureResult("Password is required for private chats", CommandName);
        }

        var chat = await _chatService.CreateChatAsync(name, new List<User> { user }, !isPrivate, true, password);

        return new JoinResult
        {
            Response = new ChatUserResponse
            {
                Chat = _mapper.Map<ChatResponse>(chat),
                User = _mapper.Map<UserResponse>(user),
                Users = chat.ChatUsers.Select(cu => _mapper.Map<UserResponse>(cu.User)).ToList(),
            },
            Result = CommandResultEnum.Success,
            Message = $"Chat {chat.Name} ({chat.Id}) created successfully",
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