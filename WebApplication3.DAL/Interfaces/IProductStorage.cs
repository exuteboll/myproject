using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApplicatoin3.Domain.ModelsDb;

namespace WebApplication3.DAL.Interfaces
{
    public interface IProductStorage : IBaseStorage<ProductDb>
    {
        // Специфичные методы для продуктов
        Task<List<ProductDb>> GetByCategoryId(Guid categoryId);
        Task<List<ProductDb>> SearchProducts(string searchTerm);
        Task<List<ProductDb>> GetFeaturedProducts(int count = 6);
    }
}
