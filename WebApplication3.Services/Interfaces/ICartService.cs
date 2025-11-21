using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApplicatoin3.Domain.ModelsDb;
using WebApplicatoin3.Domain.Response;

namespace WebApplication3.Services.Interfaces
{
    public interface ICartService
    {
        Task<BaseResponse<bool>> AddToCart(Guid userId, Guid productId, int quantity = 1);
        Task<BaseResponse<bool>> RemoveFromCart(Guid userId, Guid productId);
        Task<BaseResponse<bool>> UpdateQuantity(Guid userId, Guid productId, int quantity);
        Task<BaseResponse<List<CartItemDb>>> GetCartItems(Guid userId);
        Task<BaseResponse<int>> GetCartItemsCount(Guid userId);
        Task<BaseResponse<bool>> ClearCart(Guid userId);
    }
}
