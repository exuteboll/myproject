using WebApplication3.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace WebApplication3.DAL.Storage
{
    public class BaseStorage<T> : IBaseStorage<T> where T : class
    {
        protected readonly ApplicationDbContext _db;

        public BaseStorage(ApplicationDbContext db)
        {
            _db = db;
        }

        protected DbSet<T> GetDbSet() => _db.Set<T>();

        public virtual async Task<T> Get(Guid id)
        {
            return await GetDbSet().FindAsync(id);
        }

        public virtual IQueryable<T> GetAll()
        {
            return GetDbSet().AsQueryable();
        }

        public virtual async Task Add(T item)
        {
            await GetDbSet().AddAsync(item);
            await _db.SaveChangesAsync();
        }

        public virtual async Task Delete(T item)
        {
            GetDbSet().Remove(item);
            await _db.SaveChangesAsync();
        }

        public virtual async Task<T> Update(T item)
        {
            GetDbSet().Update(item);
            await _db.SaveChangesAsync();
            return item;
        }
    }
}