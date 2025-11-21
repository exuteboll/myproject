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
    public class CategoryStorage : BaseStorage<CategoryDb>, ICategoryStorage
    {
        public CategoryStorage(ApplicationDbContext db) : base(db) { }

        public async Task<CategoryDb> GetByName(string name)
        {
            return await _db.CategoryDb
                .FirstOrDefaultAsync(c => c.Name == name);
        }

        public async Task<List<CategoryDb>> GetActiveCategories()
        {
            return await _db.CategoryDb
                .OrderBy(c => c.Name)
                .ToListAsync();
        }
    }
}
