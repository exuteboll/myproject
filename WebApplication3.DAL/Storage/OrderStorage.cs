using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApplicatoin3.Domain.ModelsDb;

namespace WebApplication3.DAL.Storage
{
    public class OrderStorage : BaseStorage<OrderDb>
    {
        public OrderStorage(ApplicationDbContext db) : base(db) { }

        public override IQueryable<OrderDb> GetAll()
        {
            return _db.orderDb
                .Include(o => o.User); // Работает без OrderItems
        }

        public override async Task<OrderDb> Get(Guid id)
        {
            return await _db.orderDb
                .Include(o => o.User) // Работает без OrderItems
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        // Дополнительные методы
        public async Task<List<OrderDb>> GetByUserId(Guid userId)
        {
            return await _db.orderDb
                .Where(o => o.UserId == userId)
                .Include(o => o.User)
                .ToListAsync();
        }
    }
}
