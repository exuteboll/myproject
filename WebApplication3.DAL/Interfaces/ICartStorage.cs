using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApplicatoin3.Domain.ModelsDb;

namespace WebApplication3.DAL.Interfaces
{
    public interface ICartStorage : IBaseStorage<CartItemDb>
    {
        Task<List<CartItemDb>> GetByUserId(Guid userId);
        Task<CartItemDb> GetByUserAndProduct(Guid userId, Guid productId);
        Task<bool> RemoveFromCart(Guid userId, Guid productId);
        Task ClearCart(Guid userId);
        Task<int> GetCartItemsCount(Guid userId);
    }
}
