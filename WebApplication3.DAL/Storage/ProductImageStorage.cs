using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApplicatoin3.Domain.ModelsDb;

namespace WebApplication3.DAL.Storage
{
    public class ProductImageStorage : BaseStorage<ProductImageDb>
    {
        public ProductImageStorage(ApplicationDbContext db) : base(db) { }

        public async Task<List<ProductImageDb>> GetByProductId(Guid productId)
        {
            return await _db.ProductImageDb  // ← ProductImageDb вместо ProductImages
                .Where(pi => pi.ProductId == productId)
                .ToListAsync();
        }
    }
}
