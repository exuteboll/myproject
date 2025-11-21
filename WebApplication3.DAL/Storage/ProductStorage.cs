using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApplication3.DAL.Interfaces;
using WebApplicatoin3.Domain.ModelsDb;

namespace WebApplication3.DAL.Storage
{
    public class ProductStorage : BaseStorage<ProductDb>, IProductStorage
    {
        public ProductStorage(ApplicationDbContext db) : base(db) { }

        public override IQueryable<ProductDb> GetAll()
        {
            return _db.ProductDb
     .Include(p => p.Category)  // Восстанавливаем
     .AsQueryable();

        }

        public override async Task<ProductDb> Get(Guid id)
        {
            return await _db.ProductDb
       .Include(p => p.Category)  // Восстанавливаем
       .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<List<ProductDb>> GetByCategoryId(Guid categoryId)
        {
            return new List<ProductDb>();
        }

        public async Task<List<ProductDb>> SearchProducts(string searchTerm)
        {
            return new List<ProductDb>();
        }

        public async Task<List<ProductDb>> GetFeaturedProducts(int count = 6)
        {
            return new List<ProductDb>();
        }
    }
}
