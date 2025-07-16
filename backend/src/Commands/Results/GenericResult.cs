using backend.Http.Responses;

namespace backend.Commands;

public class GenericResult : CommandResult
{
    public MessageResource? Response { get; set; }
}