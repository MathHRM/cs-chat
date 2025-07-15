using backend.Http.Responses;

namespace backend.Commands.Results;

public class GenericResult : CommandResult
{
    public MessageResource? Response { get; set; }
}