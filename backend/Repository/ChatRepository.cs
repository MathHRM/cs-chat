using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Repository
{
    public class ChatRepository
    {
        private readonly AppDbContext _context;

        public ChatRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Chat?> GetChatByIdAsync(string id)
        {
            return await _context.Chats.FindAsync(id);
        }

        public async Task<Chat?> GetChatByNameAsync(string name)
        {
            return await _context.Chats.FirstOrDefaultAsync(c => c.Name == name);
        }

        public async Task<IEnumerable<Chat>> GetAllChatsAsync(int userId)
        {
            return await _context.Chats
                .Where(c => c.ChatUsers.Any(cu => cu.UserId == userId))
                .Include(c => c.ChatUsers)
                .ThenInclude(cu => cu.User)
                .ToListAsync();
        }

        public async Task<Chat> AddChatAsync(Chat chat)
        {
            await _context.Chats.AddAsync(chat);
            await _context.SaveChangesAsync();
            return chat;
        }

        public async Task<bool> UpdateChatAsync(Chat chat)
        {
            _context.Chats.Update(chat);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteChatAsync(string id)
        {
            var chat = await _context.Chats.FindAsync(id);
            if (chat != null)
            {
                _context.Chats.Remove(chat);
                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }
    }
}