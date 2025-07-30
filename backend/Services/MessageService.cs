using backend.Commands;
using backend.Models;
using backend.Repository.Interfaces;

namespace backend.Services
{
    public class MessageService
    {
        private readonly IMessageRepository _messageRepository;

        public MessageService(IMessageRepository messageRepository)
        {
            _messageRepository = messageRepository;
        }

        public async Task<Message> CreateMessageAsync(Message message)
        {
            return await _messageRepository.CreateMessageAsync(message);
        }

        public async Task<Message> CreateMessageAsync(Models.Chat chat, User? user, string content, MessageType type = MessageType.Text, string? connectionId = null)
        {
            return await _messageRepository.CreateMessageAsync(chat.Id, user?.Id, content, type, connectionId);
        }

        public async Task<IEnumerable<Message>> GetMessagesAsync(string chatId, int? lastMessageId)
        {
            return await _messageRepository.GetMessagesAsync(chatId, lastMessageId);
        }

        public async Task<Message?> GetMessageByIdAsync(int id)
        {
            return await _messageRepository.GetMessageByIdAsync(id);
        }

        public async Task<bool> DeleteMessageAsync(int id)
        {
            return await _messageRepository.DeleteMessageAsync(id);
        }
    }
}