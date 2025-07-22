using backend.Models;
using backend.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace backend.Repository
{
    public class ChatRepository : IChatRepository
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

        public async Task<ChatUser?> AddUserToChatAsync(string chatId, int userId)
        {
            // Check if the user is already in the chat
            var exists = await _context.ChatUsers
                .AnyAsync(cu => cu.ChatId == chatId && cu.UserId == userId);

            if (exists)
            {
                return null;
            }

            var chatUser = new ChatUser
            {
                ChatId = chatId,
                UserId = userId,
            };

            await _context.ChatUsers.AddAsync(chatUser);
            await _context.SaveChangesAsync();

            return chatUser;
        }

        public async Task<bool> UserBelongsToChatAsync(int userId, string chatId)
        {
            return await _context.ChatUsers.AnyAsync(cu => cu.UserId == userId && cu.ChatId == chatId);
        }
    }
}