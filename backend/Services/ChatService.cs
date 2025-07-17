using backend.Models;
using backend.Repository;
using Microsoft.EntityFrameworkCore;

namespace backend.Services
{
    public class ChatService
    {
        private readonly ChatRepository _chatRepository;
        private readonly AppDbContext _context;
        private readonly UserService _userService;

        public ChatService(
            ChatRepository chatRepository,
            AppDbContext context,
            UserService userService
        )
        {
            _chatRepository = chatRepository;
            _context = context;
            _userService = userService;
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
            string? name,
            List<User> users,
            bool isPublic,
            bool isGroup,
            string? password = null
        )
        {
            var chat = new Chat
            {
                IsPublic = isPublic,
                IsGroup = isGroup,
                ChatUsers = new List<ChatUser>(),
            };

            foreach (var user in users)
            {
                var chatUser = new ChatUser
                {
                    ChatId = chat.Id,
                    UserId = user.Id,
                };

                chat.ChatUsers.Add(chatUser);
            }

            chat.Id = isGroup ? GeneratePublicChatId() : GeneratePrivateChatId(users[0].Username, users[1].Username);
            chat.Name = name ?? chat.Id;
            chat.Password = password != null && !isPublic ? BCrypt.Net.BCrypt.HashPassword(password) : null;

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

        public async Task<ChatUser?> AddUserToChatAsync(Chat chat, User user)
        {
            // Check if the user is already in the chat
            var exists = await _context.ChatUsers
                .AnyAsync(cu => cu.ChatId == chat.Id && cu.UserId == user.Id);

            if (exists)
            {
                return null;
            }

            var chatUser = new ChatUser
            {
                ChatId = chat.Id,
                UserId = user.Id,
            };

            await _context.ChatUsers.AddAsync(chatUser);
            await _context.SaveChangesAsync();

            return chatUser;
        }

        public async Task<Chat?> JoinChatAsync(string chatName, int userId)
        {
            var user = await _userService.GetUserByIdAsync(userId);

            if (user == null)
            {
                return null;
            }

            var chat = await _chatRepository.GetChatByNameAsync(chatName);
            if (chat == null)
            {
                return null;
            }

            await AddUserToChatAsync(chat, user);

            return chat;
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