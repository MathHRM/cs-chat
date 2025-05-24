namespace backend.Models
{
    public class Message
    {
        public int Id { get; set; }

        public string Content { get; set; }

        public DateTime CreatedAT { get; set; }

        public Message(int id, string content)
        {
            Id = id;
            Content = content;
            CreatedAT = DateTime.Now;
        }
    }
}
