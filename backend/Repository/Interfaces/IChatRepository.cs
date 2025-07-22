using backend.Models;

namespace backend.Repository.Interfaces
{
    public interface IChatRepository
    {
        Task<Chat?> GetChatByIdAsync(string id);
        Task<IEnumerable<Chat>> GetAllChatsAsync(int userId);
        Task<Chat> AddChatAsync(Chat chat);
        Task<bool> UpdateChatAsync(Chat chat);
        Task<bool> DeleteChatAsync(string id);
        Task<ChatUser?> AddUserToChatAsync(string chatId, int userId);
        Task<bool> UserBelongsToChatAsync(int userId, string chatId);
    }
}