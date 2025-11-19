using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApplicatoin3.Domain.ModelsDb;

namespace WebApplication3.DAL.Storage
{
    public class ProductStorage : BaseStorage<ProductDb>
    {
        public ProductStorage(ApplicationDbContext db) : base(db) { }

        public override IQueryable<ProductDb> GetAll()
        {
            return _db.ProductDb
                .Include(p => p.Category); 
        }

        public override async Task<ProductDb> Get(Guid id)
        {
            return await _db.ProductDb
                .Include(p => p.Category) 
                 .FirstOrDefaultAsync(p => p.Id == id);
        }
    }
}
