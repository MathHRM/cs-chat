using System.Text;
using System.CommandLine;
using backend.Http.Responses;
using backend.Services;

namespace backend.Commands;

public class List : CommandBase
{
    public override string CommandName => "list";
    public override string Description => "Lista todos os chats que você tem acesso";
    public override bool ForAuthenticatedUsers => true;
    public override bool ForGuestUsers => false;

    public override Command GetCommandInstance()
    {
        var command = new Command(CommandName, Description);
        command.TreatUnmatchedTokensAsErrors = false;
        return command;
    }

    private readonly UserService _userService;

    public List(UserService userService)
    {
        _userService = userService;
    }

    public override async Task<CommandResult> Handle(ParseResult parseResult)
    {
        var authenticatedUser = await _userService.GetUserByIdAsync(AuthenticatedUserId);
        var chats = authenticatedUser.ChatUsers.OrderBy(c => c.Chat.Name).ToList();
        var chatsMessage = new StringBuilder("Available chats:\n\n");

        foreach (var chatUser in chats)
        {
            var chat = chatUser.Chat;
            var user = chatUser.User;
            var description = string.IsNullOrEmpty(chat.Description) ? "" : $"- {chat.Description}";
            var type = chat.IsGroup ? "Group" : "Private";
            var command = chat.IsGroup ? $"/join {chat.Id}" : $"/chat";

            chatsMessage.AppendLine($"{chat.Name} {description}");
            chatsMessage.AppendLine($"- {type}");
            chatsMessage.AppendLine($"{command}");
            chatsMessage.AppendLine();
        }

        if (chats.Count == 0)
        {
            chatsMessage.AppendLine("Nenhum chat encontrado");
        }

        return new GenericResult
        {
            Result = CommandResultEnum.Success,
            Response = new MessageResponse
            {
                Content = chatsMessage.ToString(),
                CreatedAt = DateTime.UtcNow,
                Type = MessageType.Text,
                User = new UserResponse
                {
                    Id = 0,
                    Username = "Sistema",
                    CurrentChatId = null
                },
            },
            Command = CommandName
        };
    }
}