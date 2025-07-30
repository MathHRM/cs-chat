using System.ComponentModel.DataAnnotations;
using backend.Commands;

namespace backend.Models;

public class Message
{
    [Key]
    public int Id { get; set; }
    public string ChatId { get; set; }
    public Chat Chat { get; set; } = null!;
    public int? UserId { get; set; }
    public User? User { get; set; }
    public string? ConnectionId { get; set; }
    public string Content { get; set; }
    public DateTime CreatedAT { get; set; } = DateTime.UtcNow;
    public MessageType Type { get; set; } = MessageType.Text;
}
