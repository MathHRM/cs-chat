using backend.Models;
using backend.Repository.Interfaces;

namespace backend.Services
{
    public class ChatService
    {
        private readonly IChatRepository _chatRepository;
        private readonly IUserRepository _userRepository;

        public ChatService(IChatRepository chatRepository, IUserRepository userRepository)
        {
            _chatRepository = chatRepository;
            _userRepository = userRepository;
        }

        public async Task<Chat?> GetChatByIdAsync(string id)
        {
            return await _chatRepository.GetChatByIdAsync(id);
        }

        public async Task<IEnumerable<Chat>> GetAllChatsAsync(int userId)
        {
            return await _chatRepository.GetAllChatsAsync(userId);
        }

        public async Task<Chat> CreateChatAsync(
            string name,
            string? description,
            string? password,
            bool isPublic,
            bool isGroup,
            List<User> users)
        {
            var chat = new Chat
            {
                IsPublic = isPublic,
                IsGroup = isGroup,
                Name = name,
                Description = description,
                ChatUsers = new List<ChatUser>(),
            };

            chat.Id = isGroup ? GeneratePublicChatId() : GeneratePrivateChatId(users[0].Username, users[1].Username);
            chat.Password = password != null ? BCrypt.Net.BCrypt.HashPassword(password) : null;

            // Create the chat first
            var createdChat = await _chatRepository.AddChatAsync(chat);

            // Add users to the chat
            foreach (var user in users)
            {
                await _chatRepository.AddUserToChatAsync(createdChat.Id, user.Id);
            }

            return createdChat;
        }

        public async Task<Chat?> UpdateChatAsync(string id, Chat updatedChat)
        {
            var existingChat = await _chatRepository.GetChatByIdAsync(id);
            if (existingChat == null)
            {
                return null;
            }

            // Update properties
            if (!string.IsNullOrEmpty(updatedChat.Name))
            {
                existingChat.Name = updatedChat.Name;
            }

            if (updatedChat.Description != null)
            {
                existingChat.Description = updatedChat.Description;
            }

            await _chatRepository.UpdateChatAsync(existingChat);
            return existingChat;
        }

        public async Task<bool> DeleteChatAsync(string id)
        {
            return await _chatRepository.DeleteChatAsync(id);
        }

        public async Task<Chat?> JoinChatAsync(string chatId, int userId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
            {
                return null;
            }

            var chat = await _chatRepository.GetChatByIdAsync(chatId);
            if (chat == null)
            {
                return null;
            }

            await _chatRepository.AddUserToChatAsync(chatId, userId);
            return chat;
        }

        public async Task<bool> UserBelongsToChatAsync(int userId, string chatId)
        {
            return await _chatRepository.UserBelongsToChatAsync(userId, chatId);
        }

        public string GeneratePrivateChatId(string username1, string username2)
        {
            if (username1.CompareTo(username2) > 0)
            {
                (username1, username2) = (username2, username1);
            }

            return $"private-{username1}-{username2}";
        }

        public string GeneratePublicChatId()
        {
            var id = Guid.NewGuid().ToString().ToUpper().Substring(0, 5);
            return id;
        }
    }
}