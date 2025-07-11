namespace backend.Models;

public class Chat
{
    public int Id { get; set; }
    public ICollection<ChatUser> ChatUsers { get; set; } = new List<ChatUser>();
    public ICollection<Message> Messages { get; set; } = new List<Message>();
}