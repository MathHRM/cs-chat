using System.ComponentModel.DataAnnotations;
using backend.Commands;

namespace backend.Models;

public class Message
{
    [Key]
    public int Id { get; set; }
    public string ChatId { get; set; }
    public Chat Chat { get; set; } = null!;
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public string Content { get; set; }
    public DateTime CreatedAT { get; set; }
    public MessageType Type { get; set; } = MessageType.Text;

    public Message()
    {
    }

    public Message(int id, string chatId, int userId, string content)
    {
        Id = id;
        ChatId = chatId;
        UserId = userId;
        Content = content;
        CreatedAT = DateTime.Now;
    }
}
