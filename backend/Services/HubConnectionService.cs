using backend.Models;
using backend.Repository.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace backend.Services
{
    public class HubConnectionService
    {
        private readonly IUserRepository _userRepository;

        public HubConnectionService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task UpdateUserCurrentChatAsync(int userId, string chatId, HubCallerContext connection, IGroupManager groups)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null) return;

            user.CurrentChatId = chatId;
            await _userRepository.UpdateUserAsync(user);

            await RemoveUserFromOtherChats(user.Id, chatId, connection, groups);
            await groups.AddToGroupAsync(connection.ConnectionId, chatId);
        }

        public async Task DisconnectUserFromAllChatsAsync(int userId, HubCallerContext connection, IGroupManager groups)
        {
            var chatUsers = await _userRepository.GetChatUsersFromUserAsync(userId);

            foreach (var chatUser in chatUsers)
            {
                await groups.RemoveFromGroupAsync(connection.ConnectionId, chatUser.ChatId);
            }
        }

        private async Task RemoveUserFromOtherChats(int userId, string currentChatId, HubCallerContext connection, IGroupManager groups)
        {
            var chatUsers = await _userRepository.GetChatUsersFromUserAsync(userId);

            Logger.Info($"Removing user {userId} from other chats except: {currentChatId}");

            foreach (var chatUser in chatUsers.Where(cu => cu.ChatId != currentChatId))
            {
                await groups.RemoveFromGroupAsync(connection.ConnectionId, chatUser.ChatId);
            }
        }
    }
}