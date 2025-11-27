using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication3.Services.Interfaces;
using WebApplicatoin3.Domain.Models;
using WebApplicatoin3.Domain.ModelsDb;
using WebApplicatoin3.Domain.Response;
using StatusCode = WebApplicatoin3.Domain.Response.StatusCode;

namespace WebApplication3.Controllers
{
    [Authorize(Roles = "Admin,Manager")]
    public class AdminController : Controller
    {
        private readonly IAdminService _adminService;
        private readonly ILogger<AdminController> _logger;

        public AdminController(IAdminService adminService, ILogger<AdminController> logger)
        {
            _adminService = adminService;
            _logger = logger;
        }

        public async Task<IActionResult> Dashboard()
        {
            var result = await _adminService.GetDashboardData();
            if (result.StatusCode != WebApplicatoin3.Domain.Response.StatusCode.OK)
            {
                TempData["Error"] = result.Description;
                return View(new AdminDashboardViewModel());
            }
            return View(result.Data);
        }

        public async Task<IActionResult> Users(string search = null, string role = null)
        {
            var result = await _adminService.GetUsers(search, role);
            if (result.StatusCode != WebApplicatoin3.Domain.Response.StatusCode.OK)
            {
                TempData["Error"] = result.Description;
                return View(new UserManagementViewModel());
            }
            return View(result.Data);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateUserRole(Guid userId, string newRole)
        {
            var result = await _adminService.UpdateUserRole(userId, newRole);
            if (result.StatusCode == WebApplicatoin3.Domain.Response.StatusCode.OK)
            {
                TempData["Success"] = result.Description;
            }
            else
            {
                TempData["Error"] = result.Description;
            }
            return RedirectToAction("Users");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(Guid userId)
        {
            var result = await _adminService.DeleteUser(userId);
            if (result.StatusCode == WebApplicatoin3.Domain.Response.StatusCode.OK)
            {
                TempData["Success"] = result.Description;
            }
            else
            {
                TempData["Error"] = result.Description;
            }
            return RedirectToAction("Users");
        }

        public async Task<IActionResult> Products(string search = null, Guid? categoryId = null)
        {
            var result = await _adminService.GetProducts(search, categoryId);
            if (result.StatusCode != WebApplicatoin3.Domain.Response.StatusCode.OK)
            {
                TempData["Error"] = result.Description;
                return View(new ProductManagementViewModel());
            }
            return View(result.Data);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProduct(ProductDb product)
        {
            try
            {
                var result = await _adminService.UpdateProduct(product);
                if (result.StatusCode == WebApplicatoin3.Domain.Response.StatusCode.OK)
                {
                    TempData["Success"] = result.Description;
                }
                else
                {
                    TempData["Error"] = result.Description;
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Ошибка при обновлении товара: {ex.Message}";
            }

            return RedirectToAction("Products");
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct(ProductDb product)
        {
            try
            {
                var result = await _adminService.CreateProduct(product);
                if (result.StatusCode == WebApplicatoin3.Domain.Response.StatusCode.OK)
                {
                    TempData["Success"] = result.Description;
                }
                else
                {
                    TempData["Error"] = result.Description;
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Ошибка при создании товара: {ex.Message}";
            }

            return RedirectToAction("Products");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteProduct(Guid productId)
        {
            var result = await _adminService.DeleteProduct(productId);
            if (result.StatusCode == WebApplicatoin3.Domain.Response.StatusCode.OK)
            {
                TempData["Success"] = result.Description;
            }
            else
            {
                TempData["Error"] = result.Description;
            }
            return RedirectToAction("Products");
        }

        public async Task<IActionResult> Orders(string status = null, DateTime? from = null, DateTime? to = null)
        {
            var result = await _adminService.GetOrders(status, from, to);
            if (result.StatusCode != WebApplicatoin3.Domain.Response.StatusCode.OK)
            {
                TempData["Error"] = result.Description;
                return View(new OrderManagementViewModel());
            }
            return View(result.Data);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateOrderStatus(Guid orderId, string newStatus)
        {
            var result = await _adminService.UpdateOrderStatus(orderId, newStatus);
            if (result.StatusCode == WebApplicatoin3.Domain.Response.StatusCode.OK)
            {
                TempData["Success"] = result.Description;
            }
            else
            {
                TempData["Error"] = result.Description;
            }
            return RedirectToAction("Orders");
        }

        public async Task<IActionResult> Categories()
        {
            var result = await _adminService.GetCategories();
            if (result.StatusCode != WebApplicatoin3.Domain.Response.StatusCode.OK)
            {
                TempData["Error"] = result.Description;
                return View(new CategoryManagementViewModel());
            }
            return View(result.Data);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategory(CategoryDb category)
        {
            var result = await _adminService.CreateCategory(category);
            if (result.StatusCode == WebApplicatoin3.Domain.Response.StatusCode.OK)
            {
                TempData["Success"] = result.Description;
            }
            else
            {
                TempData["Error"] = result.Description;
            }
            return RedirectToAction("Categories");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCategory(Guid categoryId)
        {
            var result = await _adminService.DeleteCategory(categoryId);
            if (result.StatusCode == WebApplicatoin3.Domain.Response.StatusCode.OK)
            {
                TempData["Success"] = result.Description;
            }
            else
            {
                TempData["Error"] = result.Description;
            }
            return RedirectToAction("Categories");
        }

        public async Task<IActionResult> Requests(string status = null)
        {
            var result = await _adminService.GetRequests(status);
            if (result.StatusCode != WebApplicatoin3.Domain.Response.StatusCode.OK)
            {
                TempData["Error"] = result.Description;
                return View(new RequestManagementViewModel());
            }
            return View(result.Data);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateRequestStatus(Guid requestId, string newStatus)
        {
            var result = await _adminService.UpdateRequestStatus(requestId, newStatus);
            if (result.StatusCode == WebApplicatoin3.Domain.Response.StatusCode.OK)
            {
                TempData["Success"] = result.Description;
            }
            else
            {
                TempData["Error"] = result.Description;
            }
            return RedirectToAction("Requests");
        }
    }
}