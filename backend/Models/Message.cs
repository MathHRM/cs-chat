namespace backend.Models
{
    public class Message
    {
        public int Id { get; set; }
        public int ChatId { get; set; }
        public Chat Chat { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAT { get; set; }

        public Message()
        {
        }

        public Message(int id, int chatId, int userId, string content)
        {
            Id = id;
            ChatId = chatId;
            UserId = userId;
            Content = content;
            CreatedAT = DateTime.Now;
        }
    }
}
