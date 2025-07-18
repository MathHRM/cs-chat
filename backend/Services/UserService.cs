using backend.Models;
using backend.Repository;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace backend.Services
{
    public class UserService
    {
        private readonly UserRepository _userRepository;
        private readonly AppDbContext _context;

        public UserService(UserRepository userRepository, AppDbContext context)
        {
            _userRepository = userRepository;
            _context = context;
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await _userRepository.GetUserByIdAsync(id);
        }

        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            return await _context.Users
                .Include(u => u.CurrentChat)
                .FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _userRepository.GetAllUsersAsync();
        }

        public async Task<User> CreateUserAsync(User user)
        {
            if (!string.IsNullOrEmpty(user.Password))
            {
                user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
            }

            user.CurrentChatId = "general";

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return user;
        }

        public async Task<User?> UpdateUserAsync(int id, User user)
        {
            var existingUser = await _userRepository.GetUserByIdAsync(id);
            if (existingUser == null)
            {
                return null;
            }

            existingUser.Username = user.Username ?? existingUser.Username;
            existingUser.CurrentChatId = user.CurrentChatId ?? existingUser.CurrentChatId;

            await _userRepository.UpdateUserAsync(existingUser);
            return existingUser;
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            return await _userRepository.DeleteUserAsync(id);
        }

        public async Task<bool> UserExistsAsync(string username)
        {
            var user = await GetUserByUsernameAsync(username);
            return user != null;
        }

        public async Task<User?> ValidateUserCredentialsAsync(string username, string password)
        {
            var user = await GetUserByUsernameAsync(username);
            if (user == null)
            {
                Console.WriteLine("User not found");
                return null;
            }

            if (!BCrypt.Net.BCrypt.Verify(password, user.Password))
            {
                Console.WriteLine("Invalid password");
                return null;
            }

            return user;
        }

        public async Task<User?> ValidateUserCredentialsAsync(int userId)
        {
            var user = await GetUserByIdAsync(userId);

            if (user == null)
            {
                return null;
            }

            return user;
        }

        public async Task<List<ChatUser>> GetChatUsersFromUserAsync(User user)
        {
            if (user == null || user.CurrentChatId == null)
            {
                return new List<ChatUser>();
            }

            return await _context.ChatUsers
                .Where(cu => cu.UserId == user.Id)
                .ToListAsync();
        }

        public async Task<User?> GetUserWithChatsAsync(string username)
        {
            return await _context.Users
                .Include(u => u.ChatUsers)
                .ThenInclude(chatUser => chatUser.Chat)
                .FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task UpdateUserCurrentChatAsync(User user, string chatId, HubCallerContext connection, IGroupManager groups)
        {
            user.CurrentChatId = chatId;
            await UpdateUserAsync(user.Id, user);

            await RemoveUserFromOtherChats(chatId, connection, groups);
            await groups.AddToGroupAsync(connection.ConnectionId, chatId);
        }

        private async Task RemoveUserFromOtherChats(string chatId, HubCallerContext connection, IGroupManager groups)
        {
            var username = connection.User.Identity.Name;
            var user = await GetUserWithChatsAsync(username);

            Logger.Info($"Removing user {username} from other chats: {chatId}");

            foreach (var chatUser in user.ChatUsers.Where(cu => cu.ChatId != chatId))
            {
                await groups.RemoveFromGroupAsync(connection.ConnectionId, chatUser.ChatId);
            }
        }
    }
}