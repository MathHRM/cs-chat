using System.Text;
using System.CommandLine;
using backend.Http.Responses;
using backend.Services;
using AutoMapper;

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
    private readonly IMapper _mapper;

    public List(UserService userService, IMapper mapper)
    {
        _userService = userService;
        _mapper = mapper;
    }

    public override async Task<CommandResult> Handle(ParseResult parseResult)
    {
        var authenticatedUser = await _userService.GetUserByIdAsync(AuthenticatedUserId);
        var chats = authenticatedUser.ChatUsers.OrderBy(c => c.Chat.Name).ToList();

        return new ChatsListResult
        {
            Result = CommandResultEnum.Success,
            Chats = chats.Select(c => _mapper.Map<ChatResponse>(c.Chat)).ToList(),
            Command = CommandName
        };
    }
}