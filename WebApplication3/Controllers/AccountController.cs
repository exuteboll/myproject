using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebApplication3.Services.Interfaces;
using WebApplicatoin3.Domain.ModelsDb;
using WebApplicatoin3.Domain.Response;
using WebApplicatoin3.Domain.ViewModels.Profile;
using DomainStatusCode = WebApplicatoin3.Domain.Response.StatusCode;

namespace WebApplication3.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly IUserService _userService;
        private readonly IOrderService _orderService;
        private readonly IAccountService _accountService;

        public AccountController(IUserService userService,
                               IOrderService orderService,
                               IAccountService accountService)
        {
            _userService = userService;
            _orderService = orderService;
            _accountService = accountService;
        }

        public async Task<IActionResult> Profile()
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            var userResponse = await _userService.GetUserById(userId);
            if (userResponse.StatusCode != DomainStatusCode.OK)
            {
                TempData["Error"] = userResponse.Description;
                return RedirectToAction("Index", "Home");
            }

            var model = new ProfileViewModel
            {
                User = userResponse.Data
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> EditProfile()
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var userResponse = await _userService.GetUserById(userId);

            if (userResponse.StatusCode != DomainStatusCode.OK)
            {
                TempData["Error"] = userResponse.Description;
                return RedirectToAction("Profile");
            }

            var model = new EditProfileViewModel
            {
                Login = userResponse.Data.Login,
                Email = userResponse.Data.Email
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditProfile(EditProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var result = await _orderService.UpdateUserProfile(userId, model.Login, model.Email);

            if (result.StatusCode == DomainStatusCode.OK)
            {
                // Обновляем claims в куках
                await UpdateUserClaims(userId);

                TempData["Success"] = "Профиль успешно обновлен";
                return RedirectToAction("Profile");
            }

            TempData["Error"] = result.Description;
            return View(model);
        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var result = await _accountService.ChangePassword(userEmail, model.CurrentPassword, model.NewPassword);

            if (result.StatusCode == DomainStatusCode.OK)
            {
                TempData["Success"] = "Пароль успешно изменен";
                return RedirectToAction("Profile");
            }

            TempData["Error"] = result.Description;
            return View(model);
        }

        private async Task UpdateUserClaims(Guid userId)
        {
            // Получаем обновленные данные пользователя
            var userResponse = await _userService.GetUserById(userId);
            if (userResponse.StatusCode == DomainStatusCode.OK && userResponse.Data != null)
            {
                var user = userResponse.Data;

                // Выходим из системы
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

                // Создаем новые claims
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Login),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role.ToString())
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddHours(3)
                };

                // Входим снова с обновленными claims
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);
            }
        }
    }
}