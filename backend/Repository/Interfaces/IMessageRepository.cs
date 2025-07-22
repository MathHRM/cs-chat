using backend.Models;
using backend.Commands;

namespace backend.Repository.Interfaces
{
    public interface IMessageRepository
    {
        Task<Message> CreateMessageAsync(Message message);
        Task<Message> CreateMessageAsync(string chatId, int userId, string content, MessageType type = MessageType.Text);
        Task<IEnumerable<Message>> GetMessagesAsync(string chatId, int? lastMessageId = null);
        Task<Message?> GetMessageByIdAsync(int id);
        Task<bool> DeleteMessageAsync(int id);
    }
}