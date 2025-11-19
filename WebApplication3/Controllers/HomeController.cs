using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;
using WebApplication3.Models;
using WebApplication3.Services.Interfaces;
using WebApplicatoin3.Domain.ViewModels.LoginAndRegistration;
using WebApplicatoin3.Domain.Response;
using WebApplicatoin3.Domain.Enum;
using WebApplicatoin3.Domain.ModelsDb;



namespace WebApplication3.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IAccountService _accountService;
        private readonly IUserService _userService;
        private readonly IAuthService _authService;

        public HomeController(ILogger<HomeController> logger,
                           IAccountService accountService,
                           IUserService userService,
                           IAuthService authService)
        {
            _logger = logger;
            _accountService = accountService;
            _userService = userService;
            _authService = authService;
        }

        public IActionResult SiteInformation()
        {
            return View();
        }
       
        
            public IActionResult Index()
            {
                return View();
            }

            public IActionResult Contacts()
            {
                return View();
            }
            public IActionResult Privacy()
            {
                return View();
            }
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                            .Select(e => e.ErrorMessage)
                                            .ToList();
                return BadRequest(new { success = false, errors });
            }

            try
            {
                var loginResult = await _accountService.Login(model);
                if (loginResult.StatusCode != WebApplicatoin3.Domain.Response.StatusCode.OK)
                {
                    return BadRequest(new { success = false, error = loginResult.Description });
                }

                // Получаем пользователя для создания claims
                var userResponse = await _userService.GetUserByEmail(model.Email);
                if (userResponse.StatusCode != WebApplicatoin3.Domain.Response.StatusCode.OK)
                {
                    return BadRequest(new { success = false, error = "Ошибка при получении пользователя" });
                }

                // Генерируем JWT токен
                var tokenResult = await _authService.GenerateToken(userResponse.Data);
                if (tokenResult.StatusCode != WebApplicatoin3.Domain.Response.StatusCode.OK)
                {
                    return BadRequest(new { success = false, error = tokenResult.Description });
                }

                // Создаем claims для cookie аутентификации
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, userResponse.Data.Id.ToString()),
                    new Claim(ClaimTypes.Name, userResponse.Data.Login),
                    new Claim(ClaimTypes.Email, userResponse.Data.Email),
                    new Claim(ClaimTypes.Role, userResponse.Data.Role.ToString()),
                    new Claim("Token", tokenResult.Data)
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddHours(3)
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                return Ok(new
                {
                    success = true,
                    message = "Успешный вход",
                    token = tokenResult.Data,
                    user = new
                    {
                        id = userResponse.Data.Id,
                        login = userResponse.Data.Login,
                        email = userResponse.Data.Email,
                        role = userResponse.Data.Role.ToString()
                    }
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, error = $"Ошибка сервера: {ex.Message}" });
            }
        }
        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                            .Select(e => e.ErrorMessage)
                                            .ToList();
                return BadRequest(new { success = false, errors });
            }

            try
            {
                var registerResult = await _accountService.Register(model);
                if (registerResult.StatusCode != WebApplicatoin3.Domain.Response.StatusCode.OK)
                {
                    return BadRequest(new { success = false, error = registerResult.Description });
                }

                // Автоматический вход после регистрации
                var userResponse = await _userService.GetUserByEmail(model.Email);
                if (userResponse.StatusCode != WebApplicatoin3.Domain.Response.StatusCode.OK)
                {
                    return BadRequest(new { success = false, error = "Ошибка при получении пользователя" });
                }

                var tokenResult = await _authService.GenerateToken(userResponse.Data);
                if (tokenResult.StatusCode != WebApplicatoin3.Domain.Response.StatusCode.OK)
                {
                    return BadRequest(new { success = false, error = tokenResult.Description });
                }

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, userResponse.Data.Id.ToString()),
                    new Claim(ClaimTypes.Name, userResponse.Data.Login),
                    new Claim(ClaimTypes.Email, userResponse.Data.Email),
                    new Claim(ClaimTypes.Role, userResponse.Data.Role.ToString()),
                    new Claim("Token", tokenResult.Data)
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddHours(3)
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                return Ok(new
                {
                    success = true,
                    message = "Успешная регистрация",
                    token = tokenResult.Data,
                    user = new
                    {
                        id = userResponse.Data.Id,
                        login = userResponse.Data.Login,
                        email = userResponse.Data.Email,
                        role = userResponse.Data.Role.ToString()
                    }
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, error = $"Ошибка сервера: {ex.Message}" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Ok(new { success = true, message = "Успешный выход" });
        }

        [HttpGet]
        public IActionResult CheckAuth()
        {
            if (User.Identity.IsAuthenticated)
            {
                return Ok(new
                {
                    isAuthenticated = true,
                    user = new
                    {
                        id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value,
                        login = User.FindFirst(ClaimTypes.Name)?.Value,
                        email = User.FindFirst(ClaimTypes.Email)?.Value,
                        role = User.FindFirst(ClaimTypes.Role)?.Value
                    }
                });
            }

            return Ok(new { isAuthenticated = false });
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
