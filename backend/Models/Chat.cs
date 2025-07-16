using System.ComponentModel.DataAnnotations;

namespace backend.Models;

public class Chat
{
    [Key]
    public string Id { get; set; }
    public string Name { get; set; }
    public ICollection<ChatUser> ChatUsers { get; set; } = new List<ChatUser>();
    public ICollection<Message> Messages { get; set; } = new List<Message>();
}