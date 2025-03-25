using Microsoft.EntityFrameworkCore;
using ChatWebAPI.Repository;
using System;
using ChatWebAPI.Database;
using System.Linq.Expressions;
using ChatWebAPI.Models;


namespace ChatWebAPI.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ChatDbContext _context;
        private readonly DbSet<T> _dbSet;

        public Repository(ChatDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<T> GetByIdAsync(int id)
        {
            var entity = await _dbSet.FindAsync(id);
            if (entity == null)
            {
                Console.WriteLine($"Entity of type {typeof(T).Name} with ID {id} not found.");
            }
            return entity;
        }

        public async Task<T> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await _context.Set<T>().FirstOrDefaultAsync(predicate);
        }

        public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
        {
            return await _context.Set<T>().AnyAsync(predicate);
        }

        public async Task<RegistrationModel> GetByEmailAsync(string email)
        {
            return await _context.RegisterModels.FirstOrDefaultAsync(user => user.Email == email);
        }

        public async Task<Chat?> GetChatByName(string name)
        {
            return await _context.Chats.Include(c => c.Users).FirstOrDefaultAsync(chat => chat.ChatName == name);
        }

    }

}
