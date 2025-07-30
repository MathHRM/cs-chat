using backend.Http.Responses;

namespace backend.Commands.Responses;

public class HelpResult : CommandResult
{
    public List<CommandResponse> Response { get; set; }
}