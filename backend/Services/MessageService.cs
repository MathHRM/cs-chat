using backend.Commands;
using backend.Models;
using Microsoft.EntityFrameworkCore;

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

        public async Task<IEnumerable<Message>> GetMessagesAsync(string chatId, int? lastMessageId)
        {
            return await _context.Messages
                .Where(m => m.ChatId == chatId)
                .Where(m => lastMessageId == null || m.Id < lastMessageId)
                .OrderBy(m => m.Id)
                .Take(50)
                .ToListAsync();
        }
    }
}