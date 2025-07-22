using backend.Models;
using backend.Repository.Interfaces;

namespace backend.Services
{
    public class UserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<User?> GetUserByIdAsync(int? id)
        {
            return await _userRepository.GetUserByIdAsync(id);
        }

        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            return await _userRepository.GetUserByUsernameAsync(username);
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

            // Set default chat for new users
            user.CurrentChatId = "general";

            return await _userRepository.AddUserAsync(user);
        }

        public async Task<User?> UpdateUserAsync(int id, User updatedUser, string? newPassword = null)
        {
            var existingUser = await _userRepository.GetUserByIdAsync(id);
            if (existingUser == null)
            {
                return null;
            }

            // Check if username is being changed and if it's already taken
            if (!string.IsNullOrEmpty(updatedUser.Username) &&
                updatedUser.Username != existingUser.Username)
            {
                if (await _userRepository.UserExistsAsync(updatedUser.Username))
                {
                    return null; // Username already taken
                }
                existingUser.Username = updatedUser.Username;
            }

            // Update password if provided
            if (!string.IsNullOrEmpty(newPassword))
            {
                existingUser.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
            }

            // Update other properties
            if (!string.IsNullOrEmpty(updatedUser.CurrentChatId))
            {
                existingUser.CurrentChatId = updatedUser.CurrentChatId;
            }

            await _userRepository.UpdateUserAsync(existingUser);
            return existingUser;
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            return await _userRepository.DeleteUserAsync(id);
        }

        public async Task<bool> UserExistsAsync(string username)
        {
            return await _userRepository.UserExistsAsync(username);
        }

        public async Task<User?> ValidateUserCredentialsAsync(string username, string password)
        {
            var user = await _userRepository.GetUserByUsernameAsync(username);
            if (user == null)
            {
                return null;
            }

            if (!BCrypt.Net.BCrypt.Verify(password, user.Password))
            {
                return null;
            }

            return user;
        }

        public async Task<User?> GetUserWithChatsAsync(string username)
        {
            return await _userRepository.GetUserWithChatsAsync(username);
        }

        public async Task<List<ChatUser>> GetChatUsersFromUserAsync(int userId)
        {
            return await _userRepository.GetChatUsersFromUserAsync(userId);
        }
    }
}