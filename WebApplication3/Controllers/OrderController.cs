using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication3.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        // УДАЛИТЕ весь метод Cart() - он нам больше не нужен
        // Оставьте только методы для оформления заказов в будущем

        public IActionResult Index()
        {
            // Здесь в будущем будет список заказов
            return View();
        }
    }
}