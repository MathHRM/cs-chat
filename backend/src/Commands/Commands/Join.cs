using backend.Services;
using backend.Commands.Results;
using backend.Http.Responses;

namespace backend.Commands.Commands;

public class Join : Command
{
    private readonly UserService _userService;
    private readonly TokenService _tokenService;

    public override string CommandName => "join";

    public override string Description => "Join a chat";

    public Join(UserService userService, TokenService tokenService)
    {
        _userService = userService;
        _tokenService = tokenService;
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
        // var chatId = args["chatId"] as string;
        return CommandResult.SuccessResult($"Chat joined successfully", CommandName, args);
    }
}