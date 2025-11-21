using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApplication3.DAL.Interfaces;
using WebApplication3.Services.Interfaces;
using WebApplicatoin3.Domain.ModelsDb;
using WebApplicatoin3.Domain.Response;

namespace WebApplication3.Services.Realizations
{
    public class CartService : ICartService
    {
        private readonly ICartStorage _cartStorage;
        private readonly IProductStorage _productStorage;

        public CartService(ICartStorage cartStorage, IProductStorage productStorage)
        {
            _cartStorage = cartStorage;
            _productStorage = productStorage;
        }

        public async Task<BaseResponse<bool>> AddToCart(Guid userId, Guid productId, int quantity = 1)
        {
            try
            {
                // Проверяем существование товара
                var product = await _productStorage.Get(productId);
                if (product == null)
                {
                    return new BaseResponse<bool>
                    {
                        Description = "Товар не найден",
                        StatusCode = StatusCode.NotFound
                    };
                }

                // Проверяем есть ли уже товар в корзине
                var existingCartItem = await _cartStorage.GetByUserAndProduct(userId, productId);

                if (existingCartItem != null)
                {
                    // Обновляем количество
                    existingCartItem.Quantity += quantity;
                    await _cartStorage.Update(existingCartItem);
                }
                else
                {
                    // Добавляем новый товар
                    var cartItem = new CartItemDb
                    {
                        Id = Guid.NewGuid(),
                        UserId = userId,
                        ProductId = productId,
                        Quantity = quantity,
                        CreatedAt = DateTime.UtcNow
                    };
                    await _cartStorage.Add(cartItem);
                }

                return new BaseResponse<bool>
                {
                    Description = "Товар добавлен в корзину",
                    StatusCode = StatusCode.OK,
                    Data = true
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<bool>
                {
                    Description = $"Ошибка при добавлении в корзину: {ex.Message}",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }

        public async Task<BaseResponse<bool>> RemoveFromCart(Guid userId, Guid productId)
        {
            try
            {
                var result = await _cartStorage.RemoveFromCart(userId, productId);

                return new BaseResponse<bool>
                {
                    Description = result ? "Товар удален из корзины" : "Товар не найден в корзине",
                    StatusCode = result ? StatusCode.OK : StatusCode.NotFound,
                    Data = result
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<bool>
                {
                    Description = $"Ошибка при удалении из корзины: {ex.Message}",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }

        public async Task<BaseResponse<bool>> UpdateQuantity(Guid userId, Guid productId, int quantity)
        {
            try
            {
                if (quantity <= 0)
                {
                    return await RemoveFromCart(userId, productId);
                }

                var cartItem = await _cartStorage.GetByUserAndProduct(userId, productId);
                if (cartItem == null)
                {
                    return new BaseResponse<bool>
                    {
                        Description = "Товар не найден в корзине",
                        StatusCode = StatusCode.NotFound
                    };
                }

                cartItem.Quantity = quantity;
                await _cartStorage.Update(cartItem);

                return new BaseResponse<bool>
                {
                    Description = "Количество обновлено",
                    StatusCode = StatusCode.OK,
                    Data = true
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<bool>
                {
                    Description = $"Ошибка при обновлении количества: {ex.Message}",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }

        public async Task<BaseResponse<List<CartItemDb>>> GetCartItems(Guid userId)
        {
            try
            {
                Console.WriteLine($"=== CartService.GetCartItems ===");
                Console.WriteLine($"UserId: {userId}");

                var cartItems = await _cartStorage.GetByUserId(userId);
                Console.WriteLine($"CartStorage returned: {cartItems?.Count} items");

                if (cartItems != null)
                {
                    foreach (var item in cartItems)
                    {
                        Console.WriteLine($"Cart Item - Product: {item.Product?.Name}, ProductId: {item.ProductId}");
                    }
                }

                return new BaseResponse<List<CartItemDb>>
                {
                    Description = "Корзина загружена",
                    StatusCode = StatusCode.OK,
                    Data = cartItems
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR in GetCartItems: {ex.Message}");
                return new BaseResponse<List<CartItemDb>>
                {
                    Description = $"Ошибка при загрузке корзины: {ex.Message}",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }

        public async Task<BaseResponse<int>> GetCartItemsCount(Guid userId)
        {
            try
            {
                var count = await _cartStorage.GetCartItemsCount(userId);

                return new BaseResponse<int>
                {
                    Description = "Количество товаров получено",
                    StatusCode = StatusCode.OK,
                    Data = count
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<int>
                {
                    Description = $"Ошибка при получении количества: {ex.Message}",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }

        public async Task<BaseResponse<bool>> ClearCart(Guid userId)
        {
            try
            {
                await _cartStorage.ClearCart(userId);

                return new BaseResponse<bool>
                {
                    Description = "Корзина очищена",
                    StatusCode = StatusCode.OK,
                    Data = true
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<bool>
                {
                    Description = $"Ошибка при очистке корзины: {ex.Message}",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }
    }
}
