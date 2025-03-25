using ChatWebAPI.Models;
using System.Linq.Expressions;

namespace ChatWebAPI.Repository
{
    public interface IRepository<T> where T : class
    {
        Task AddAsync(T entity);
        Task SaveAsync();

        Task<T> GetByIdAsync(int id);

        Task<RegistrationModel> GetByEmailAsync(string email);

        Task<T> FindAsync(Expression<Func<T, bool>> predicate);

        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);

        Task<Chat?> GetChatByName(string name);
    }

}
