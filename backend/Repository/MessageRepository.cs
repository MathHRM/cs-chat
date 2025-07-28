using backend.Models;
using backend.Repository.Interfaces;
using backend.Commands;
using Microsoft.EntityFrameworkCore;

namespace backend.Repository
{
    public class MessageRepository : IMessageRepository
    {
        private readonly AppDbContext _context;

        public MessageRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Message> CreateMessageAsync(Message message)
        {
            _context.Messages.Add(message);
            await _context.SaveChangesAsync();
            return message;
        }

        public async Task<Message> CreateMessageAsync(string chatId, int? userId, string content, MessageType type = MessageType.Text, string? connectionId = null)
        {
            var message = new Message
            {
                ChatId = chatId,
                UserId = userId,
                Content = content,
                Type = type,
                ConnectionId = connectionId
            };

            _context.Messages.Add(message);
            await _context.SaveChangesAsync();
            return message;
        }

        public async Task<IEnumerable<Message>> GetMessagesAsync(string chatId, int? lastMessageId = null)
        {
            return await _context.Messages
                .Where(m => m.ChatId == chatId)
                .Where(m => lastMessageId == null || m.Id < lastMessageId)
                .Include(m => m.User)
                .Include(m => m.Chat)
                .OrderByDescending(m => m.Id)
                .Take(50)
                .ToListAsync();
        }

        public async Task<Message?> GetMessageByIdAsync(int id)
        {
            return await _context.Messages
                .Include(m => m.User)
                .Include(m => m.Chat)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<bool> DeleteMessageAsync(int id)
        {
            var message = await _context.Messages.FindAsync(id);
            if (message != null)
            {
                _context.Messages.Remove(message);
                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }
    }
}