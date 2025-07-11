using System.ComponentModel.DataAnnotations;

namespace backend.Models;

public class ChatUser
{
    [Key]
    public int Id { get; set; }
    public string ChatId { get; set; }
    public Chat Chat { get; set; } = null!;
    public int UserId { get; set; }
    public User User { get; set; } = null!;
}