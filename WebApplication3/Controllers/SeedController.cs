using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication3.DAL.Interfaces;
using WebApplicatoin3.Domain.ModelsDb;

namespace WebApplication3.Controllers
{
    public class SeedController : Controller
    {
        private readonly ICategoryStorage _categoryStorage;
        private readonly IProductStorage _productStorage;

        public SeedController(ICategoryStorage categoryStorage, IProductStorage productStorage)
        {
            _categoryStorage = categoryStorage;
            _productStorage = productStorage;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                await SeedTestData();

                var categoriesCount = await _categoryStorage.GetAll().CountAsync();
                var productsCount = await _productStorage.GetAll().CountAsync();

                return Content($"Seed завершен! Добавлено категорий: {categoriesCount}, товаров: {productsCount}");
            }
            catch (Exception ex)
            {
                // Добавляем внутреннее исключение
                var innerException = ex.InnerException != null ? $"\n\nВнутренняя ошибка: {ex.InnerException.Message}" : "";
                return Content($"Ошибка в Seed: {ex.Message}{innerException}\n\n{ex.StackTrace}");
            }
        }

        private async Task SeedTestData()
        {
            try
            {
                // Добавляем категории
                var categories = new[]
                {
            new CategoryDb
            {
                Id = Guid.NewGuid(),
                Name = "Мягкая мебель",
                ImageUrl = "/img/sofa.jfif",
                ProductsCount = 0,
                CreatedAt = DateTime.UtcNow  // Исправлено на UtcNow
            },
            new CategoryDb
            {
                Id = Guid.NewGuid(),
                Name = "Столы и стулья",
                ImageUrl = "/img/table.jfif",
                ProductsCount = 0,
                CreatedAt = DateTime.UtcNow  // Исправлено на UtcNow
            },
            new CategoryDb
            {
                Id = Guid.NewGuid(),
                Name = "Шкафы и гардеробы",
                ImageUrl = "/img/wardrobe.jfif",
                ProductsCount = 0,
                CreatedAt = DateTime.UtcNow  // Исправлено на UtcNow
            }
        };

                foreach (var category in categories)
                {
                    var existingCategory = await _categoryStorage.GetByName(category.Name);
                    if (existingCategory == null)
                    {
                        await _categoryStorage.Add(category);
                    }
                }

                // Добавляем товары
                var existingCategories = await _categoryStorage.GetAll().ToListAsync();

                var products = new[]
                {
            new ProductDb
            {
                Id = Guid.NewGuid(),
                CategoryId = existingCategories[0].Id,
                Name = "Угловой диван 'Комфорт Плюс'",
                Description = "Просторный угловой диван с ортопедическим основанием и механизмом трансформации.",
                Price = 45900m,
                OldPrice = 52900m,
                Material = "Ткань, дерево, поролон",
                Dimensions = "220x160x90 см",
                Color = "Бежевый",
                ImageUrl = "/img/sofa.jfif",
                CreatedAt = DateTime.UtcNow  // Исправлено на UtcNow
            },
            new ProductDb
            {
                Id = Guid.NewGuid(),
                CategoryId = existingCategories[1].Id,
                Name = "Обеденный стол 'Флоренция'",
                Description = "Элегантный деревянный обеденный стол в классическом стиле.",
                Price = 18900m,
                Material = "Натуральное дерево (орех)",
                Dimensions = "150x90x75 см",
                Color = "Темный орех",
                ImageUrl = "/img/table.jfif",
                CreatedAt = DateTime.UtcNow  // Исправлено на UtcNow
            }
        };

                foreach (var product in products)
                {
                    var existingProduct = await _productStorage.GetAll()
                        .FirstOrDefaultAsync(p => p.Name == product.Name);

                    if (existingProduct == null)
                    {
                        await _productStorage.Add(product);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при добавлении тестовых данных: {ex.Message}");
                throw;
            }
        }
    }
}
