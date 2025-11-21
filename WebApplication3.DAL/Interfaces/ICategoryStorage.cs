using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApplicatoin3.Domain.ModelsDb;

namespace WebApplication3.DAL.Interfaces
{
    public interface ICategoryStorage : IBaseStorage<CategoryDb>
    {
        Task<CategoryDb> GetByName(string name);
        Task<List<CategoryDb>> GetActiveCategories();
    }
}
