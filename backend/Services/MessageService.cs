using backend.Commands;
using backend.Models;

namespace backend.Services
{
    public class MessageService
    {
        private readonly AppDbContext _context;

        public MessageService(
            AppDbContext context
        )
        {
            _context = context;
        }

        public async Task<Message> CreateMessageAsync(Message message)
        {
            _context.Messages.Add(message);
            await _context.SaveChangesAsync();
            return message;
        }

        public async Task<Message> CreateMessageAsync(Models.Chat chat, User user, string content, MessageType type = MessageType.Text)
        {
            var message = new Message
            {
                Chat = chat,
                User = user,
                Content = content,
                Type = type
            };

            _context.Messages.Add(message);
            await _context.SaveChangesAsync();
            return message;
        }
    }
}