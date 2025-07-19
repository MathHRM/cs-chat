using backend.Http.Responses;
using backend.Models;
using backend.Services;
using Microsoft.AspNetCore.SignalR;
using AutoMapper;

namespace backend.Commands;

public class Create : Command
{
    private readonly UserService _userService;
    private readonly ChatService _chatService;
    private readonly IMapper _mapper;

    public override string CommandName => "create";

    public override string Description => "Create a chat";

    public Create(UserService userService, ChatService chatService, IMapper mapper)
    {
        _userService = userService;
        _chatService = chatService;
        _mapper = mapper;
    }

    public override Dictionary<string, CommandArgument>? Args => new Dictionary<string, CommandArgument>
    {
        {
            "name",
            new CommandArgument {
                Name = "name",
                Description = "The name of the chat",
                Alias = "n",
                IsRequired = true,
                ByPosition = true,
                Position = 0,
            }
        },
        {
            "description",
            new CommandArgument {
                Name = "description",
                Description = "The description of the chat",
                Alias = "d",
            }
        },
        {
            "password",
            new CommandArgument {
                Name = "password",
                Description = "The password of the chat",
                Alias = "pass",
            }
        },
    };

    public override async Task<CommandResult> Handle(Dictionary<string, string?> args)
    {
        var name = args["name"] as string;
        var description = args["description"] as string;
        var password = args["password"] as string;
        var isPrivate = password != null;
        var user = await _userService.GetUserByUsernameAsync(HubCallerContext.User.Identity.Name);

        if (HubCallerContext == null)
        {
            return CommandResult.UnauthorizedResult(CommandName);
        }

        var chat = await _chatService.CreateChatAsync(name, description, password, !isPrivate, true, new List<User> { user });

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
            Message = $"Chat {chat.Id} created successfully",
            Command = CommandName,
        };
    }
}