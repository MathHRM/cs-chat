namespace backend.Models
{
    public class Message
    {
        public int Id { get; set; }

        public string Username { get; set; }

        public string Content { get; set; }

        public DateTime CreatedAT { get; set; }

        public Message(int id, string username, string content)
        {
            Id = id;
            Username = username;
            Content = content;
            CreatedAT = DateTime.Now;
        }
    }
}
