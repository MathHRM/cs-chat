using backend.Services;
using backend.Commands.Results;
using backend.Http.Responses;
using backend.Services;
using Microsoft.AspNetCore.SignalR;
using System.Linq;

namespace backend.Commands.Commands;

public class Join : Command
{
    private readonly UserService _userService;
    private readonly TokenService _tokenService;
    private readonly ChatService _chatService;
    public override string CommandName => "join";

    public override string Description => "Join a chat";

    public Join(UserService userService, TokenService tokenService, ChatService chatService)
    {
        _userService = userService;
        _tokenService = tokenService;
        _chatService = chatService;
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
            }
        }
    };

    public override async Task<CommandResult> Handle(Dictionary<string, object> args)
    {
        return CommandResult.SuccessResult($"Chat joined successfully", CommandName, args);
    }
}