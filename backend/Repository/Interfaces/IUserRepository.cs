using backend.Models;

namespace backend.Repository.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetUserByIdAsync(int? id);
        Task<User?> GetUserByUsernameAsync(string username);
        Task<User?> GetUserWithChatsAsync(string username);
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<User> AddUserAsync(User user);
        Task<bool> UpdateUserAsync(User user);
        Task<bool> DeleteUserAsync(int id);
        Task<List<ChatUser>> GetChatUsersFromUserAsync(int userId);
        Task<bool> UserExistsAsync(string username);
    }
}