using System.ComponentModel.DataAnnotations;

namespace backend.Models;

public class ChatUser
{
    [Key]
    public int Id { get; set; }
    public int ChatId { get; set; }
    public int UserId { get; set; }
    public User User { get; set; }
    public Chat Chat { get; set; }
}