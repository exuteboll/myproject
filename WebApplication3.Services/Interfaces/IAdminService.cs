using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApplicatoin3.Domain.Models;
using WebApplicatoin3.Domain.ModelsDb;
using WebApplicatoin3.Domain.Response;

namespace WebApplication3.Services.Interfaces
{
    public interface IAdminService
    {
        Task<BaseResponse<AdminDashboardViewModel>> GetDashboardData();
        Task<BaseResponse<UserManagementViewModel>> GetUsers(string search = null, string role = null);
        Task<BaseResponse<bool>> UpdateUserRole(Guid userId, string newRole);
        Task<BaseResponse<bool>> DeleteUser(Guid userId);
        Task<BaseResponse<ProductManagementViewModel>> GetProducts(string search = null, Guid? categoryId = null);
        Task<BaseResponse<bool>> CreateProduct(ProductDb product);
        Task<BaseResponse<bool>> UpdateProduct(ProductDb product);
        Task<BaseResponse<bool>> DeleteProduct(Guid productId);
        Task<BaseResponse<OrderManagementViewModel>> GetOrders(string status = null, DateTime? from = null, DateTime? to = null);
        Task<BaseResponse<bool>> UpdateOrderStatus(Guid orderId, string newStatus);
        Task<BaseResponse<CategoryManagementViewModel>> GetCategories();
        Task<BaseResponse<bool>> CreateCategory(CategoryDb category);
        Task<BaseResponse<bool>> UpdateCategory(CategoryDb category);
        Task<BaseResponse<bool>> DeleteCategory(Guid categoryId);
        Task<BaseResponse<RequestManagementViewModel>> GetRequests(string status = null);
        Task<BaseResponse<bool>> UpdateRequestStatus(Guid requestId, string newStatus);
    }
}
