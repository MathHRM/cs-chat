using backend.Http.Responses;

namespace backend.Commands;

public class GenericResult : CommandResult
{
    public MessageResponse? Response { get; set; }
}