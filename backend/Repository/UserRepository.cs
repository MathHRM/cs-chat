using backend.Models;
using backend.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace backend.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetUserByIdAsync(int? id)
        {
            if (id == null)
            {
                return null;
            }

            return await _context.Users
                .Include(u => u.CurrentChat)
                .Include(u => u.ChatUsers)
                .ThenInclude(cu => cu.Chat)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            return await _context.Users
                .Include(u => u.CurrentChat)
                .Include(u => u.ChatUsers)
                .FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<User?> GetUserWithChatsAsync(string username)
        {
            return await _context.Users
                .Include(u => u.ChatUsers)
                .ThenInclude(chatUser => chatUser.Chat)
                .FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<User> AddUserAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<bool> UpdateUserAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }

        public async Task<List<ChatUser>> GetChatUsersFromUserAsync(int userId)
        {
            return await _context.ChatUsers
                .Where(cu => cu.UserId == userId)
                .ToListAsync();
        }

        public async Task<bool> UserExistsAsync(string username)
        {
            return await _context.Users.AnyAsync(u => u.Username == username);
        }
    }
}