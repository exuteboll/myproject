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
    public class CartStorage : BaseStorage<CartItemDb>, ICartStorage
    {
        public CartStorage(ApplicationDbContext db) : base(db) { }

        public override IQueryable<CartItemDb> GetAll()
        {
            return _db.Set<CartItemDb>()
                .Include(ci => ci.Product)
                .ThenInclude(p => p.Category);
        }

        public async Task<List<CartItemDb>> GetByUserId(Guid userId)
        {
            Console.WriteLine($"=== CartStorage.GetByUserId ===");
            Console.WriteLine($"UserId: {userId}");

            var items = await _db.Set<CartItemDb>()
                .Where(ci => ci.UserId == userId)
                .Include(ci => ci.Product)
                .ThenInclude(p => p.Category)
                .ToListAsync();

            Console.WriteLine($"Found {items.Count} items in database");

            foreach (var item in items)
            {
                Console.WriteLine($"DB Item - Id: {item.Id}, ProductId: {item.ProductId}, Product: {item.Product?.Name}");
            }

            return items;
        }

        public async Task<CartItemDb> GetByUserAndProduct(Guid userId, Guid productId)
        {
            return await _db.Set<CartItemDb>()
                .FirstOrDefaultAsync(ci => ci.UserId == userId && ci.ProductId == productId);
        }

        public async Task<bool> RemoveFromCart(Guid userId, Guid productId)
        {
            var cartItem = await GetByUserAndProduct(userId, productId);
            if (cartItem != null)
            {
                await Delete(cartItem);
                return true;
            }
            return false;
        }

        public async Task ClearCart(Guid userId)
        {
            var cartItems = await GetByUserId(userId);
            foreach (var item in cartItems)
            {
                await Delete(item);
            }
        }

        public async Task<int> GetCartItemsCount(Guid userId)
        {
            return await _db.Set<CartItemDb>()
                .Where(ci => ci.UserId == userId)
                .SumAsync(ci => ci.Quantity);
        }
    }
}
