using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApplicatoin3.Domain.ModelsDb;

namespace WebApplicatoin3.Domain.Models
{
    public class AdminDashboardViewModel
    {
        public int TotalUsers { get; set; }
        public int TotalProducts { get; set; }
        public int TotalOrders { get; set; }
        public int TotalCategories { get; set; }
        public int TotalRequests { get; set; }
        public decimal TotalRevenue { get; set; }
        public List<UserDb> RecentUsers { get; set; } = new();
        public List<OrderDb> RecentOrders { get; set; } = new();
        public Dictionary<string, int> OrdersByStatus { get; set; } = new();
    }

    public class UserManagementViewModel
    {
        public List<UserDb> Users { get; set; } = new();
        public string SearchQuery { get; set; }
        public string RoleFilter { get; set; }
    }

    public class ProductManagementViewModel
    {
        public List<ProductDb> Products { get; set; } = new();
        public List<CategoryDb> Categories { get; set; } = new();
        public string SearchQuery { get; set; }
        public Guid? CategoryFilter { get; set; }
    }

    public class OrderManagementViewModel
    {
        public List<OrderDb> Orders { get; set; } = new();
        public string StatusFilter { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
    }
    public class CategoryManagementViewModel
    {
        public List<CategoryDb> Categories { get; set; } = new();
    }
    public class RequestManagementViewModel
    {
        public List<RequestDb> Requests { get; set; } = new();
        public string StatusFilter { get; set; }
    }

}
