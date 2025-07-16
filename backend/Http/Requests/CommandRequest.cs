using System.ComponentModel.DataAnnotations;

namespace backend.Http.Requests;

public class CommandRequest
{
    [Required]
    public string Command { get; set; }
}