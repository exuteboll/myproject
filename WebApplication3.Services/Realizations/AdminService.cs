using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WebApplication3.DAL.Interfaces;
using WebApplication3.Services.Interfaces;
using WebApplicatoin3.Domain.Enum;
using WebApplicatoin3.Domain.Models;
using WebApplicatoin3.Domain.ModelsDb;
using WebApplicatoin3.Domain.Response;


namespace WebApplication3.Services.Realizations
{
        public class AdminService : IAdminService
        {
            private readonly IBaseStorage<UserDb> _userStorage;
            private readonly IBaseStorage<ProductDb> _productStorage;
            private readonly IBaseStorage<OrderDb> _orderStorage;
            private readonly IBaseStorage<CategoryDb> _categoryStorage;
            private readonly IBaseStorage<CartItemDb> _cartStorage;
            private readonly IBaseStorage<RequestDb> _requestStorage;

            public AdminService(
                IBaseStorage<UserDb> userStorage,
                IBaseStorage<ProductDb> productStorage,
                IBaseStorage<OrderDb> orderStorage,
                IBaseStorage<CategoryDb> categoryStorage,
                IBaseStorage<CartItemDb> cartStorage,
                IBaseStorage<RequestDb> requestStorage)
            {
                _userStorage = userStorage;
                _productStorage = productStorage;
                _orderStorage = orderStorage;
                _categoryStorage = categoryStorage;
                _cartStorage = cartStorage;
                _requestStorage = requestStorage;
            }

            public async Task<BaseResponse<AdminDashboardViewModel>> GetDashboardData()
            {
                try
                {
                    var totalUsers = await _userStorage.GetAll().CountAsync();
                    var totalProducts = await _productStorage.GetAll().CountAsync();
                    var totalOrders = await _orderStorage.GetAll().CountAsync();
                    var totalCategories = await _categoryStorage.GetAll().CountAsync();
                    var totalRequests = await _requestStorage.GetAll().CountAsync();

                    var totalRevenue = await _orderStorage.GetAll()
                        .Where(o => o.Status == (int)OrderStatus.Paid || o.Status == (int)OrderStatus.Delivered)
                        .SumAsync(o => o.Price * o.Quantity);

                    var recentUsers = await _userStorage.GetAll()
                        .OrderByDescending(u => u.CreatedAt)
                        .Take(5)
                        .ToListAsync();

                    var recentOrders = await _orderStorage.GetAll()
                        .Include(o => o.User)
                        .OrderByDescending(o => o.CreatedAt)
                        .Take(5)
                        .ToListAsync();

                    // Статистика по статусам заказов
                    var ordersByStatus = await _orderStorage.GetAll()
                        .GroupBy(o => o.Status)
                        .Select(g => new { Status = g.Key, Count = g.Count() })
                        .ToListAsync();

                    var dashboard = new AdminDashboardViewModel
                    {
                        TotalUsers = totalUsers,
                        TotalProducts = totalProducts,
                        TotalOrders = totalOrders,
                        TotalCategories = totalCategories,
                        TotalRevenue = totalRevenue,
                        TotalRequests = totalRequests,
                        RecentUsers = recentUsers,
                        RecentOrders = recentOrders,
                        OrdersByStatus = ordersByStatus.ToDictionary(x => x.Status.ToString(), x => x.Count)
                    };

                    return new BaseResponse<AdminDashboardViewModel>
                    {
                        Data = dashboard,
                        StatusCode = StatusCode.OK,
                        Description = "Данные для дашборда получены"
                    };
                }
                catch (Exception ex)
                {
                    return new BaseResponse<AdminDashboardViewModel>
                    {
                        StatusCode = StatusCode.InternalServerError,
                        Description = $"Ошибка при получении данных дашборда: {ex.Message}"
                    };
                }
            }

            public async Task<BaseResponse<UserManagementViewModel>> GetUsers(string search = null, string role = null)
            {
                try
                {
                    var usersQuery = _userStorage.GetAll().AsQueryable();

                    if (!string.IsNullOrEmpty(search))
                    {
                        search = search.ToLower();
                        usersQuery = usersQuery.Where(u =>
                            u.Login.ToLower().Contains(search) ||
                            u.Email.ToLower().Contains(search));
                    }

                    if (!string.IsNullOrEmpty(role) && Enum.TryParse<Role>(role, out var roleEnum))
                    {
                        usersQuery = usersQuery.Where(u => u.Role == roleEnum);
                    }

                    var users = await usersQuery
                        .OrderByDescending(u => u.CreatedAt)
                        .ToListAsync();

                    var viewModel = new UserManagementViewModel
                    {
                        Users = users,
                        SearchQuery = search,
                        RoleFilter = role
                    };

                    return new BaseResponse<UserManagementViewModel>
                    {
                        Data = viewModel,
                        StatusCode = StatusCode.OK
                    };
                }
                catch (Exception ex)
                {
                    return new BaseResponse<UserManagementViewModel>
                    {
                        StatusCode = StatusCode.InternalServerError,
                        Description = $"Ошибка при получении пользователей: {ex.Message}"
                    };
                }
            }

            public async Task<BaseResponse<bool>> UpdateUserRole(Guid userId, string newRole)
            {
                try
                {
                    var user = await _userStorage.Get(userId);
                    if (user == null)
                    {
                        return new BaseResponse<bool>
                        {
                            StatusCode = StatusCode.NotFound,
                            Description = "Пользователь не найден"
                        };
                    }

                    if (Enum.TryParse<Role>(newRole, out var roleEnum))
                    {
                        user.Role = roleEnum;
                        await _userStorage.Update(user);

                        return new BaseResponse<bool>
                        {
                            Data = true,
                            StatusCode = StatusCode.OK,
                            Description = "Роль пользователя обновлена"
                        };
                    }

                    return new BaseResponse<bool>
                    {
                        StatusCode = StatusCode.BadRequest,
                        Description = "Некорректная роль"
                    };
                }
                catch (Exception ex)
                {
                    return new BaseResponse<bool>
                    {
                        StatusCode = StatusCode.InternalServerError,
                        Description = $"Ошибка при обновлении роли: {ex.Message}"
                    };
                }
            }

            public async Task<BaseResponse<bool>> DeleteUser(Guid userId)
            {
                try
                {
                    var user = await _userStorage.Get(userId);
                    if (user == null)
                    {
                        return new BaseResponse<bool>
                        {
                            StatusCode = StatusCode.NotFound,
                            Description = "Пользователь не найден"
                        };
                    }

                    // Проверяем, есть ли у пользователя заказы
                    var userOrders = await _orderStorage.GetAll()
                        .Where(o => o.UserId == userId)
                        .AnyAsync();

                    if (userOrders)
                    {
                        return new BaseResponse<bool>
                        {
                            StatusCode = StatusCode.BadRequest,
                            Description = "Нельзя удалить пользователя с существующими заказами"
                        };
                    }

                    // Удаляем корзину пользователя
                    var userCartItems = await _cartStorage.GetAll()
                        .Where(c => c.UserId == userId)
                        .ToListAsync();

                    foreach (var cartItem in userCartItems)
                    {
                        await _cartStorage.Delete(cartItem);
                    }

                    // Удаляем пользователя
                    await _userStorage.Delete(user);

                    return new BaseResponse<bool>
                    {
                        Data = true,
                        StatusCode = StatusCode.OK,
                        Description = "Пользователь удален"
                    };
                }
                catch (Exception ex)
                {
                    return new BaseResponse<bool>
                    {
                        StatusCode = StatusCode.InternalServerError,
                        Description = $"Ошибка при удалении пользователя: {ex.Message}"
                    };
                }
            }

            public async Task<BaseResponse<ProductManagementViewModel>> GetProducts(string search = null, Guid? categoryId = null)
            {
                try
                {
                    var productsQuery = _productStorage.GetAll().AsQueryable();

                    if (!string.IsNullOrEmpty(search))
                    {
                        search = search.ToLower();
                        productsQuery = productsQuery.Where(p =>
                            p.Name.ToLower().Contains(search) ||
                            p.Description.ToLower().Contains(search));
                    }

                    if (categoryId.HasValue && categoryId.Value != Guid.Empty)
                    {
                        productsQuery = productsQuery.Where(p => p.CategoryId == categoryId.Value);
                    }

                    var products = await productsQuery
                        .Include(p => p.Category)
                        .OrderByDescending(p => p.CreatedAt)
                        .ToListAsync();

                    var categories = await _categoryStorage.GetAll().ToListAsync();

                    var viewModel = new ProductManagementViewModel
                    {
                        Products = products,
                        Categories = categories,
                        SearchQuery = search,
                        CategoryFilter = categoryId
                    };

                    return new BaseResponse<ProductManagementViewModel>
                    {
                        Data = viewModel,
                        StatusCode = StatusCode.OK
                    };
                }
                catch (Exception ex)
                {
                    return new BaseResponse<ProductManagementViewModel>
                    {
                        StatusCode = StatusCode.InternalServerError,
                        Description = $"Ошибка при получении товаров: {ex.Message}"
                    };
                }
            }

        public async Task<BaseResponse<bool>> CreateProduct(ProductDb product)
        {
            try
            {
                // Валидация обязательных полей
                if (string.IsNullOrEmpty(product.Name))
                {
                    return new BaseResponse<bool>
                    {
                        StatusCode = StatusCode.BadRequest,
                        Description = "Название товара обязательно"
                    };
                }

                if (product.Price <= 0)
                {
                    return new BaseResponse<bool>
                    {
                        StatusCode = StatusCode.BadRequest,
                        Description = "Цена должна быть больше 0"
                    };
                }

                // Проверяем существование категории
                var category = await _categoryStorage.Get(product.CategoryId);
                if (category == null)
                {
                    return new BaseResponse<bool>
                    {
                        StatusCode = StatusCode.BadRequest,
                        Description = "Указанная категория не существует"
                    };
                }

                // Обрабатываем NULL значения для необязательных полей
                product.Description ??= string.Empty;
                product.Material ??= string.Empty;
                product.Dimensions ??= string.Empty;
                product.Color ??= string.Empty;
                product.ImageUrl ??= string.Empty;

                // Если OldPrice не указан, устанавливаем null
                if (product.OldPrice <= 0)
                {
                    product.OldPrice = null;
                }

                // Устанавливаем ID и дату создания
                product.Id = Guid.NewGuid();
                product.CreatedAt = DateTime.UtcNow;

                // Сохраняем товар
                await _productStorage.Add(product);

                return new BaseResponse<bool>
                {
                    Data = true,
                    StatusCode = StatusCode.OK,
                    Description = "Товар успешно создан"
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<bool>
                {
                    StatusCode = StatusCode.InternalServerError,
                    Description = $"Ошибка при создании товара: {ex.Message}"
                };
            }
        }

        public async Task<BaseResponse<bool>> UpdateProduct(ProductDb product)
        {
            try
            {
                var existingProduct = await _productStorage.Get(product.Id);
                if (existingProduct == null)
                {
                    return new BaseResponse<bool>
                    {
                        StatusCode = StatusCode.NotFound,
                        Description = "Товар не найден"
                    };
                }

                // Валидация обязательных полей
                if (string.IsNullOrEmpty(product.Name))
                {
                    return new BaseResponse<bool>
                    {
                        StatusCode = StatusCode.BadRequest,
                        Description = "Название товара обязательно"
                    };
                }

                if (product.Price <= 0)
                {
                    return new BaseResponse<bool>
                    {
                        StatusCode = StatusCode.BadRequest,
                        Description = "Цена должна быть больше 0"
                    };
                }

                // Проверяем существование категории
                var category = await _categoryStorage.Get(product.CategoryId);
                if (category == null)
                {
                    return new BaseResponse<bool>
                    {
                        StatusCode = StatusCode.BadRequest,
                        Description = "Указанная категория не существует"
                    };
                }

                // Обновляем поля с обработкой NULL значений
                existingProduct.Name = product.Name;
                existingProduct.Description = product.Description ?? string.Empty;
                existingProduct.Price = product.Price;
                existingProduct.OldPrice = product.OldPrice <= 0 ? null : product.OldPrice;
                existingProduct.CategoryId = product.CategoryId;
                existingProduct.Material = product.Material ?? string.Empty;
                existingProduct.Dimensions = product.Dimensions ?? string.Empty;
                existingProduct.Color = product.Color ?? string.Empty;
                existingProduct.ImageUrl = product.ImageUrl ?? string.Empty;

                // Сохраняем изменения
                await _productStorage.Update(existingProduct);

                return new BaseResponse<bool>
                {
                    Data = true,
                    StatusCode = StatusCode.OK,
                    Description = "Товар успешно обновлен"
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<bool>
                {
                    StatusCode = StatusCode.InternalServerError,
                    Description = $"Ошибка при обновлении товара: {ex.Message}"
                };
            }
        }

            public async Task<BaseResponse<bool>> DeleteProduct(Guid productId)
            {
                try
                {
                    var product = await _productStorage.Get(productId);
                    if (product == null)
                    {
                        return new BaseResponse<bool>
                        {
                            StatusCode = StatusCode.NotFound,
                            Description = "Товар не найден"
                        };
                    }

                    // Проверяем, есть ли заказы с этим товаром
                    var productInOrders = await _orderStorage.GetAll()
                        .Where(o => o.ProductId == productId)
                        .AnyAsync();

                    if (productInOrders)
                    {
                        return new BaseResponse<bool>
                        {
                            StatusCode = StatusCode.BadRequest,
                            Description = "Нельзя удалить товар, который есть в заказах"
                        };
                    }

                    // Проверяем, есть ли товар в корзинах
                    var productInCarts = await _cartStorage.GetAll()
                        .Where(c => c.ProductId == productId)
                        .AnyAsync();

                    if (productInCarts)
                    {
                        // Удаляем товар из всех корзин
                        var cartItems = await _cartStorage.GetAll()
                            .Where(c => c.ProductId == productId)
                            .ToListAsync();

                        foreach (var cartItem in cartItems)
                        {
                            await _cartStorage.Delete(cartItem);
                        }
                    }

                    await _productStorage.Delete(product);

                    return new BaseResponse<bool>
                    {
                        Data = true,
                        StatusCode = StatusCode.OK,
                        Description = "Товар успешно удален"
                    };
                }
                catch (Exception ex)
                {
                    return new BaseResponse<bool>
                    {
                        StatusCode = StatusCode.InternalServerError,
                        Description = $"Ошибка при удалении товара: {ex.Message}"
                    };
                }
            }

            public async Task<BaseResponse<OrderManagementViewModel>> GetOrders(string status = null, DateTime? from = null, DateTime? to = null)
            {
                try
                {
                    var ordersQuery = _orderStorage.GetAll()
                        .Include(o => o.User)
                        .Include(o => o.Product)
                        .AsQueryable();

                    if (!string.IsNullOrEmpty(status) && int.TryParse(status, out var statusInt))
                    {
                        ordersQuery = ordersQuery.Where(o => o.Status == statusInt);
                    }

                    if (from.HasValue)
                    {
                        ordersQuery = ordersQuery.Where(o => o.CreatedAt >= from.Value);
                    }

                    if (to.HasValue)
                    {
                        ordersQuery = ordersQuery.Where(o => o.CreatedAt <= to.Value.AddDays(1));
                    }

                    var orders = await ordersQuery
                        .OrderByDescending(o => o.CreatedAt)
                        .ToListAsync();

                    var viewModel = new OrderManagementViewModel
                    {
                        Orders = orders,
                        StatusFilter = status,
                        DateFrom = from,
                        DateTo = to
                    };

                    return new BaseResponse<OrderManagementViewModel>
                    {
                        Data = viewModel,
                        StatusCode = StatusCode.OK
                    };
                }
                catch (Exception ex)
                {
                    return new BaseResponse<OrderManagementViewModel>
                    {
                        StatusCode = StatusCode.InternalServerError,
                        Description = $"Ошибка при получении заказов: {ex.Message}"
                    };
                }
            }

            public async Task<BaseResponse<bool>> UpdateOrderStatus(Guid orderId, string newStatus)
            {
                try
                {
                    var order = await _orderStorage.Get(orderId);
                    if (order == null)
                    {
                        return new BaseResponse<bool>
                        {
                            StatusCode = StatusCode.NotFound,
                            Description = "Заказ не найден"
                        };
                    }

                    if (int.TryParse(newStatus, out var statusInt))
                    {
                        order.Status = statusInt;
                        await _orderStorage.Update(order);

                        return new BaseResponse<bool>
                        {
                            Data = true,
                            StatusCode = StatusCode.OK,
                            Description = "Статус заказа обновлен"
                        };
                    }

                    return new BaseResponse<bool>
                    {
                        StatusCode = StatusCode.BadRequest,
                        Description = "Некорректный статус заказа"
                    };
                }
                catch (Exception ex)
                {
                    return new BaseResponse<bool>
                    {
                        StatusCode = StatusCode.InternalServerError,
                        Description = $"Ошибка при обновлении статуса заказа: {ex.Message}"
                    };
                }
            }

            public async Task<BaseResponse<CategoryManagementViewModel>> GetCategories()
            {
                try
                {
                    var categories = await _categoryStorage.GetAll()
                        .OrderBy(c => c.Name)
                        .ToListAsync();

                    // Получаем количество товаров в каждой категории
                    var productsCount = await _productStorage.GetAll()
                        .GroupBy(p => p.CategoryId)
                        .Select(g => new { CategoryId = g.Key, Count = g.Count() })
                        .ToDictionaryAsync(x => x.CategoryId, x => x.Count);

                    foreach (var category in categories)
                    {
                        category.ProductsCount = productsCount.GetValueOrDefault(category.Id, 0);
                    }

                    var viewModel = new CategoryManagementViewModel
                    {
                        Categories = categories
                    };

                    return new BaseResponse<CategoryManagementViewModel>
                    {
                        Data = viewModel,
                        StatusCode = StatusCode.OK
                    };
                }
                catch (Exception ex)
                {
                    return new BaseResponse<CategoryManagementViewModel>
                    {
                        StatusCode = StatusCode.InternalServerError,
                        Description = $"Ошибка при получении категорий: {ex.Message}"
                    };
                }
            }

            public async Task<BaseResponse<bool>> CreateCategory(CategoryDb category)
            {
                try
                {
                    if (string.IsNullOrEmpty(category.Name))
                    {
                        return new BaseResponse<bool>
                        {
                            StatusCode = StatusCode.BadRequest,
                            Description = "Название категории обязательно"
                        };
                    }

                    // Проверяем, существует ли категория с таким именем
                    var existingCategory = await _categoryStorage.GetAll()
                        .FirstOrDefaultAsync(c => c.Name.ToLower() == category.Name.ToLower());

                    if (existingCategory != null)
                    {
                        return new BaseResponse<bool>
                        {
                            StatusCode = StatusCode.BadRequest,
                            Description = "Категория с таким названием уже существует"
                        };
                    }

                    category.Id = Guid.NewGuid();
                    category.CreatedAt = DateTime.UtcNow;
                    category.ProductsCount = 0;

                    await _categoryStorage.Add(category);

                    return new BaseResponse<bool>
                    {
                        Data = true,
                        StatusCode = StatusCode.OK,
                        Description = "Категория успешно создана"
                    };
                }
                catch (Exception ex)
                {
                    return new BaseResponse<bool>
                    {
                        StatusCode = StatusCode.InternalServerError,
                        Description = $"Ошибка при создании категории: {ex.Message}"
                    };
                }
            }

            public async Task<BaseResponse<bool>> UpdateCategory(CategoryDb category)
            {
                try
                {
                    var existingCategory = await _categoryStorage.Get(category.Id);
                    if (existingCategory == null)
                    {
                        return new BaseResponse<bool>
                        {
                            StatusCode = StatusCode.NotFound,
                            Description = "Категория не найдена"
                        };
                    }

                    if (string.IsNullOrEmpty(category.Name))
                    {
                        return new BaseResponse<bool>
                        {
                            StatusCode = StatusCode.BadRequest,
                            Description = "Название категории обязательно"
                        };
                    }

                    // Проверяем, существует ли другая категория с таким именем
                    var duplicateCategory = await _categoryStorage.GetAll()
                        .FirstOrDefaultAsync(c => c.Name.ToLower() == category.Name.ToLower() && c.Id != category.Id);

                    if (duplicateCategory != null)
                    {
                        return new BaseResponse<bool>
                        {
                            StatusCode = StatusCode.BadRequest,
                            Description = "Категория с таким названием уже существует"
                        };
                    }

                    existingCategory.Name = category.Name;
                    existingCategory.ImageUrl = category.ImageUrl;

                    await _categoryStorage.Update(existingCategory);

                    return new BaseResponse<bool>
                    {
                        Data = true,
                        StatusCode = StatusCode.OK,
                        Description = "Категория успешно обновлена"
                    };
                }
                catch (Exception ex)
                {
                    return new BaseResponse<bool>
                    {
                        StatusCode = StatusCode.InternalServerError,
                        Description = $"Ошибка при обновлении категории: {ex.Message}"
                    };
                }
            }

            public async Task<BaseResponse<bool>> DeleteCategory(Guid categoryId)
            {
                try
                {
                    var category = await _categoryStorage.Get(categoryId);
                    if (category == null)
                    {
                        return new BaseResponse<bool>
                        {
                            StatusCode = StatusCode.NotFound,
                            Description = "Категория не найдена"
                        };
                    }

                    // Проверяем, есть ли товары в этой категории
                    var productsInCategory = await _productStorage.GetAll()
                        .Where(p => p.CategoryId == categoryId)
                        .AnyAsync();

                    if (productsInCategory)
                    {
                        return new BaseResponse<bool>
                        {
                            StatusCode = StatusCode.BadRequest,
                            Description = "Нельзя удалить категорию, в которой есть товары"
                        };
                    }

                    await _categoryStorage.Delete(category);

                    return new BaseResponse<bool>
                    {
                        Data = true,
                        StatusCode = StatusCode.OK,
                        Description = "Категория успешно удалена"
                    };
                }
                catch (Exception ex)
                {
                    return new BaseResponse<bool>
                    {
                        StatusCode = StatusCode.InternalServerError,
                        Description = $"Ошибка при удалении категории: {ex.Message}"
                    };
                }
            }

            public async Task<BaseResponse<RequestManagementViewModel>> GetRequests(string status = null)
            {
                try
                {
                    var requestsQuery = _requestStorage.GetAll()
                        .Include(r => r.User)
                        .AsQueryable();

                    if (!string.IsNullOrEmpty(status) && int.TryParse(status, out var statusInt))
                    {
                        requestsQuery = requestsQuery.Where(r => r.Status == statusInt);
                    }

                    var requests = await requestsQuery
                        .OrderByDescending(r => r.CreatedAt)
                        .ToListAsync();

                    var viewModel = new RequestManagementViewModel
                    {
                        Requests = requests,
                        StatusFilter = status
                    };

                    return new BaseResponse<RequestManagementViewModel>
                    {
                        Data = viewModel,
                        StatusCode = StatusCode.OK
                    };
                }
                catch (Exception ex)
                {
                    return new BaseResponse<RequestManagementViewModel>
                    {
                        StatusCode = StatusCode.InternalServerError,
                        Description = $"Ошибка при получении заявок: {ex.Message}"
                    };
                }
            }

            public async Task<BaseResponse<bool>> UpdateRequestStatus(Guid requestId, string newStatus)
            {
                try
                {
                    var request = await _requestStorage.Get(requestId);
                    if (request == null)
                    {
                        return new BaseResponse<bool>
                        {
                            StatusCode = StatusCode.NotFound,
                            Description = "Заявка не найдена"
                        };
                    }

                    if (int.TryParse(newStatus, out var statusInt))
                    {
                        request.Status = statusInt;
                        await _requestStorage.Update(request);

                        return new BaseResponse<bool>
                        {
                            Data = true,
                            StatusCode = StatusCode.OK,
                            Description = "Статус заявки обновлен"
                        };
                    }

                    return new BaseResponse<bool>
                    {
                        StatusCode = StatusCode.BadRequest,
                        Description = "Некорректный статус заявки"
                    };
                }
                catch (Exception ex)
                {
                    return new BaseResponse<bool>
                    {
                        StatusCode = StatusCode.InternalServerError,
                        Description = $"Ошибка при обновлении статуса заявки: {ex.Message}"
                    };
                }
            }
        }
    }
