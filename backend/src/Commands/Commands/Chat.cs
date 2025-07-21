using backend.Http.Responses;
using backend.Services;
using Microsoft.AspNetCore.SignalR;
using AutoMapper;

namespace backend.Commands;

public class Chat : Command
{
    private readonly UserService _userService;
    private readonly TokenService _tokenService;
    private readonly ChatService _chatService;
    private readonly IMapper _mapper;

    public override string CommandName => "chat";

    public override string Description => "Chat with a user";

    public override bool ForAuthenticatedUsers => true;
    public override bool ForGuestUsers => false;

    public Chat(UserService userService, TokenService tokenService, ChatService chatService, IMapper mapper)
    {
        _userService = userService;
        _tokenService = tokenService;
        _chatService = chatService;
        _mapper = mapper;
    }

    public override Dictionary<string, CommandArgument>? Args => new Dictionary<string, CommandArgument>
    {
        {
            "username",
            new CommandArgument {
                Name = "username",
                IsRequired = true,
                Description = "The username to chat with",
                ByPosition = true,
                Position = 0,
                Alias = "u",
            }
        }
    };

    public override async Task<CommandResult> Handle(Dictionary<string, string?> args)
    {
        var targetUsername = args["username"] as string;

        if (HubCallerContext == null)
        {
            return CommandResult.UnauthorizedResult(CommandName);
        }

        var currentUser = await _userService.GetUserByIdAsync(AuthenticatedUserId);
        if (currentUser == null)
        {
            return CommandResult.UnauthorizedResult(CommandName);
        }

        if (string.IsNullOrEmpty(targetUsername))
        {
            return CommandResult.FailureResult("Username is required", CommandName);
        }

        if (targetUsername.Equals(currentUser.Username, StringComparison.OrdinalIgnoreCase))
        {
            return CommandResult.FailureResult("You cannot chat with yourself", CommandName);
        }

        var chatId = _chatService.GeneratePrivateChatId(currentUser.Username, targetUsername);

        var existingChat = await _chatService.GetChatByIdAsync(chatId);
        if (existingChat != null)
        {
            await _userService.UpdateUserCurrentChatAsync(currentUser, chatId, HubCallerContext, HubGroups);

            return new JoinResult
            {
                Response = new ChatUserResponse
                {
                    Chat = _mapper.Map<ChatResponse>(existingChat),
                    User = _mapper.Map<UserResponse>(currentUser),
                    Users = existingChat.ChatUsers.Select(cu => _mapper.Map<UserResponse>(cu.User)).ToList(),
                },
                Result = CommandResultEnum.Success,
                Message = "Joined existing chat successfully",
                Command = CommandName,
            };
        }

        var targetUser = await _userService.GetUserByUsernameAsync(targetUsername);
        if (targetUser == null)
        {
            return CommandResult.FailureResult($"User '{targetUsername}' not found", CommandName);
        }

        var users = new List<Models.User> { currentUser, targetUser };
        var orderedUsernames = users.Select(u => u.Username).OrderBy(u => u, StringComparer.OrdinalIgnoreCase).ToList();
        var description = $"Chat between {orderedUsernames[0]} and {orderedUsernames[1]}";
        var name = $"{orderedUsernames[0]}.{orderedUsernames[1]}";
        var password = Guid.NewGuid().ToString();

        var createdChat = await _chatService.CreateChatAsync(name, description, password, false, false, users);

        if (createdChat == null)
        {
            return CommandResult.FailureResult("Failed to create chat", CommandName);
        }

        await _userService.UpdateUserCurrentChatAsync(currentUser, createdChat.Id, HubCallerContext, HubGroups);

        return new JoinResult
        {
            Response = new ChatUserResponse
            {
                Chat = _mapper.Map<ChatResponse>(createdChat),
                User = _mapper.Map<UserResponse>(currentUser),
                Users = users.Select(u => _mapper.Map<UserResponse>(u)).ToList(),
            },
            Result = CommandResultEnum.Success,
            Message = "Chat created successfully",
            Command = CommandName,
        };
    }
}