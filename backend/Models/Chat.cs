using System.ComponentModel.DataAnnotations;

namespace backend.Models;

public class Chat
{
    [Key]
    public string Id { get; set; }
    public bool IsPublic { get; set; }
    public bool IsGroup { get; set; }
    public string? Password { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public ICollection<ChatUser> ChatUsers { get; set; } = new List<ChatUser>();
    public ICollection<Message> Messages { get; set; } = new List<Message>();
}