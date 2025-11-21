using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication3.Services.Interfaces;
using WebApplicatoin3.Domain.Models;
using WebApplicatoin3.Domain.Response;

namespace WebApplication3.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly IUserService _userService;

        public AccountController(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<IActionResult> Profile()
        {
            var userId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);
            var userResponse = await _userService.GetUserById(userId);

            // Используем полное имя пространства имен чтобы избежать конфликта
            if (userResponse.StatusCode != WebApplicatoin3.Domain.Response.StatusCode.OK)
            {
                return RedirectToAction("Index", "Home");
            }

            return View(userResponse.Data);
        }
    }
}
