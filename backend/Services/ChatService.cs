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

        public async Task<Chat?> CreateChatAsync(string? id, List<User> users)
        {
            if (id != null)
            {
                var existingChat = await _chatRepository.GetChatByIdAsync(id);

                if (existingChat != null)
                {
                    return null;
                }
            }

            var chat = new Chat
            {
                Id = id ?? GeneratePublicChatId(),
            };

            foreach (var user in users)
            {
                var chatUser = new ChatUser
                {
                    ChatId = chat.Id,
                    UserId = user.Id,
                };

                await _context.ChatUsers.AddAsync(chatUser);
            }

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

        public async Task<ChatUser?> AddUserToChatAsync(string chatId, int userId)
        {
            var chat = await _chatRepository.GetChatByIdAsync(chatId);
            if (chat == null)
            {
                return null;
            }

            var user = await _userService.GetUserByIdAsync(userId);
            if (user == null)
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

            return $"public-{id}";
        }
    }
}