using ChatWebAPI.Database;
using ChatWebAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace ChatWebAPI.Services
{
    public class AllChatsService : IAllChatsService
    {
        private readonly ChatDbContext _context;

        public AllChatsService(ChatDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<string>> GetChatsListAsync(int userId)
        {
            return await _context.Chats
                .Where(c => _context.Set<Dictionary<string, object>>("ChatUser")
                    .Any(uc => EF.Property<int>(uc, "UserId") == userId
                            && EF.Property<int>(uc, "ChatId") == c.Id))
                .Select(c => c.ChatName)  // Only select the chat names
                .ToListAsync();
        }

    }
}
