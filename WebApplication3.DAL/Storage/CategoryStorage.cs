using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApplicatoin3.Domain.ModelsDb;

namespace WebApplication3.DAL.Storage
{
    public class CategoryStorage : BaseStorage<CategoryDb>
    {
        public CategoryStorage(ApplicationDbContext db) : base(db) { }

        public async Task<CategoryDb> GetByName(string name)
        {
            return await _db.CategoryDb
                .FirstOrDefaultAsync(c => c.Name == name);
        }
    }
}
