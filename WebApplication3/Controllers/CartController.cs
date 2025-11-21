using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebApplication3.Services.Interfaces;
using WebApplicatoin3.Domain.ModelsDb;
using WebApplicatoin3.Domain.Response;
using DomainStatusCode = WebApplicatoin3.Domain.Response.StatusCode;

namespace WebApplication3.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly ICartService _cartService;
        private readonly ILogger<CartController> _logger;

        public CartController(ICartService cartService, ILogger<CartController> logger)
        {
            _cartService = cartService;
            _logger = logger;
        }

        private Guid GetCurrentUserId()
        {
            return Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var userId = GetCurrentUserId();
                var cartResponse = await _cartService.GetCartItems(userId);

                if (cartResponse.StatusCode != DomainStatusCode.OK)
                {
                    TempData["Error"] = cartResponse.Description;
                    return View(new List<CartItemDb>());
                }

                return View(cartResponse.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при загрузке корзины");
                TempData["Error"] = "Ошибка при загрузке корзины";
                return View(new List<CartItemDb>());
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartRequest request)
        {
            try
            {
                var userId = GetCurrentUserId();
                var result = await _cartService.AddToCart(userId, request.ProductId, request.Quantity);

                if (result.StatusCode == DomainStatusCode.OK)
                {
                    var countResponse = await _cartService.GetCartItemsCount(userId);
                    var cartCount = countResponse.StatusCode == DomainStatusCode.OK ? countResponse.Data : 0;

                    return Json(new { success = true, message = result.Description, cartCount });
                }

                return Json(new { success = false, error = result.Description });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при добавлении в корзину");
                return Json(new { success = false, error = "Ошибка при добавлении в корзину" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFromCart([FromBody] RemoveFromCartRequest request)
        {
            try
            {
                var userId = GetCurrentUserId();
                var result = await _cartService.RemoveFromCart(userId, request.ProductId);

                if (result.StatusCode == DomainStatusCode.OK)
                {
                    var countResponse = await _cartService.GetCartItemsCount(userId);
                    var cartCount = countResponse.StatusCode == DomainStatusCode.OK ? countResponse.Data : 0;

                    return Json(new { success = true, message = result.Description, cartCount });
                }

                return Json(new { success = false, error = result.Description });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при удалении из корзины");
                return Json(new { success = false, error = "Ошибка при удалении из корзины" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateQuantity([FromBody] UpdateQuantityRequest request)
        {
            try
            {
                var userId = GetCurrentUserId();
                var result = await _cartService.UpdateQuantity(userId, request.ProductId, request.Quantity);

                if (result.StatusCode == DomainStatusCode.OK)
                {
                    return Json(new { success = true, message = result.Description });
                }

                return Json(new { success = false, error = result.Description });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при обновлении количества");
                return Json(new { success = false, error = "Ошибка при обновлении количества" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> ClearCart()
        {
            try
            {
                var userId = GetCurrentUserId();
                var result = await _cartService.ClearCart(userId);

                if (result.StatusCode == DomainStatusCode.OK)
                {
                    return Json(new { success = true, message = result.Description });
                }

                return Json(new { success = false, error = result.Description });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при очистке корзины");
                return Json(new { success = false, error = "Ошибка при очистке корзины" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetCartItemsCount()
        {
            try
            {
                if (!User.Identity.IsAuthenticated)
                {
                    return Json(new { success = false, data = 0 });
                }

                var userId = GetCurrentUserId();
                var countResponse = await _cartService.GetCartItemsCount(userId);

                return Json(new
                {
                    success = countResponse.StatusCode == DomainStatusCode.OK,
                    data = countResponse.Data
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении количества товаров в корзине");
                return Json(new { success = false, data = 0 });
            }
        }
    }

    public class AddToCartRequest
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; } = 1;
    }

    public class RemoveFromCartRequest
    {
        public Guid ProductId { get; set; }
    }

    public class UpdateQuantityRequest
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
    }
}