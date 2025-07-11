using backend.Models;
using backend.Repository;
using Microsoft.EntityFrameworkCore;

namespace backend.Services
{
    public class ChatService
    {
        private readonly ChatRepository _chatRepository;
        private readonly AppDbContext _context;

        public ChatService(ChatRepository chatRepository, AppDbContext context)
        {
            _chatRepository = chatRepository;
            _context = context;
        }

        public async Task<Chat?> GetChatByIdAsync(string id)
        {
            return await _chatRepository.GetChatByIdAsync(id);
        }

        public async Task<IEnumerable<Chat>> GetAllChatsAsync(int userId)
        {
            return await _chatRepository.GetAllChatsAsync(userId);
        }

        public async Task<Chat?> CreateChatAsync(string? id)
        {
            if (id != null)
            {
                var existingChat = await _chatRepository.GetChatByIdAsync(id);

                if (existingChat != null)
                {
                    return null;
                }
            }

            id = string.IsNullOrEmpty(id)
                ? Guid.NewGuid().ToString().ToUpper().Substring(0, 5)
                : id;

            var chat = new Chat
            {
                Id = id,
            };
            await _context.Chats.AddAsync(chat);
            await _context.SaveChangesAsync();

            return chat;
        }

        public async Task<Chat?> UpdateChatAsync(string id, Chat chat)
        {
            var existingChat = await _chatRepository.GetChatByIdAsync(id);
            if (existingChat == null)
            {
                return null;
            }

            existingChat.Id = chat.Id ?? existingChat.Id;

            await _chatRepository.UpdateChatAsync(existingChat);
            return existingChat;
        }

        public async Task<bool> DeleteChatAsync(string id)
        {
            return await _chatRepository.DeleteChatAsync(id);
        }
    }
}