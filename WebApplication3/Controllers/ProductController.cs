using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication3.DAL.Interfaces;
using WebApplication3.DAL.Storage;
using WebApplication3.Services.Interfaces;
using WebApplicatoin3.Domain.ModelsDb;
using WebApplicatoin3.Domain.Response;

namespace WebApplication3.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductStorage _productStorage;
        private readonly ICategoryStorage _categoryStorage;
        private readonly ILogger<ProductController> _logger;

        public ProductController(IProductStorage productStorage,
                               ICategoryStorage categoryStorage,
                               ILogger<ProductController> logger)
        {
            _productStorage = productStorage;
            _categoryStorage = categoryStorage;
            _logger = logger;
        }

        public async Task<IActionResult> Index(string category = null, string search = null)
        {
            try
            {
                var productsQuery = _productStorage.GetAll()
                    .Include(p => p.Category)
                    .Include(p => p.ProductImages)
                    .Where(p => p.CategoryId != Guid.Empty); // Базовая фильтрация

                // Фильтрация по категории
                if (!string.IsNullOrEmpty(category) && category != "all")
                {
                    productsQuery = productsQuery.Where(p => p.Category.Name == category);
                }

                // Поиск
                if (!string.IsNullOrEmpty(search))
                {
                    search = search.ToLower();
                    productsQuery = productsQuery.Where(p =>
                        p.Name.ToLower().Contains(search) ||
                        p.Description.ToLower().Contains(search));
                }

                var products = await productsQuery.ToListAsync();
                var categories = await _categoryStorage.GetAll().ToListAsync();

                var model = new ProductIndexViewModel
                {
                    Products = products,
                    Categories = categories,
                    SelectedCategory = category,
                    SearchQuery = search
                };

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при загрузке каталога товаров");
                return View("Error");
            }
        }
        public async Task<IActionResult> Catalog(string category = null, string search = null)
        {
            try
            {
                var productsQuery = _productStorage.GetAll(); // Должен включать Category

                // ФИКС: Проверяем фильтрацию по категории
                if (!string.IsNullOrEmpty(category) && category != "all")
                {
                    productsQuery = productsQuery.Where(p => p.Category.Name == category);
                }

                // ФИКС: Проверяем поиск
                if (!string.IsNullOrEmpty(search))
                {
                    search = search.ToLower();
                    productsQuery = productsQuery.Where(p =>
                        p.Name.ToLower().Contains(search) ||
                        p.Description.ToLower().Contains(search));
                }

                var products = await productsQuery.ToListAsync();
                var categories = await _categoryStorage.GetAll().ToListAsync();

                var model = new ProductIndexViewModel
                {
                    Products = products,
                    Categories = categories,
                    SelectedCategory = category,
                    SearchQuery = search
                };

                return View(model);
            }
            catch (Exception ex)
            {
                return Content($"Ошибка в методе Catalog: {ex.Message}");
            }
        }

        public async Task<IActionResult> Details(Guid id)
        {
            try
            {
                // ВРЕМЕННО: простой запрос без Include
                var product = await _productStorage.Get(id);

                if (product == null)
                {
                    return NotFound();
                }

                return View(product);
            }
            catch (Exception ex)
            {
                return Content($"Ошибка при загрузке деталей товара: {ex.Message}");
            }
        }
    }

    public class ProductIndexViewModel
    {
        public List<ProductDb> Products { get; set; }
        public List<CategoryDb> Categories { get; set; }
        public string SelectedCategory { get; set; }
        public string SearchQuery { get; set; }
    }
}