using ChatWebAPI.Models;

namespace ChatWebAPI.Services
{
    public interface IAllChatsService
    {
        Task<IEnumerable<string>> GetChatsListAsync(int id);
    }
}
