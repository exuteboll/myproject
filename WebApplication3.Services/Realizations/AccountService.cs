using Org.BouncyCastle.Crypto.Generators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApplication3.DAL.Interfaces;
using WebApplication3.Services.Interfaces;
using WebApplicatoin3.Domain.Enum;
using WebApplicatoin3.Domain.ModelsDb;
using WebApplicatoin3.Domain.Response;
using WebApplicatoin3.Domain.ViewModels.LoginAndRegistration;


namespace WebApplication3.Services.Realizations
{
    public class AccountService : IAccountService
    {
        private readonly IUserService _userService;
        private readonly IAuthService _authService;
        private readonly IBaseStorage<UserDb> _userStorage;

        public AccountService(IUserService userService, IAuthService authService, IBaseStorage<UserDb> userStorage)
        {
            _userService = userService;
            _authService = authService;
            _userStorage = userStorage;
        }

        public async Task<BaseResponse<bool>> Register(RegisterViewModel model)
        {
            try
            {
                // Проверяем, существует ли пользователь с таким email или логином
                var existingUserByEmail = await _userService.GetUserByEmail(model.Email);
                if (existingUserByEmail.StatusCode == StatusCode.OK && existingUserByEmail.Data != null)
                {
                    return new BaseResponse<bool>
                    {
                        Description = "Пользователь с таким email уже существует",
                        StatusCode = StatusCode.BadRequest
                    };
                }

                var existingUserByLogin = await _userService.GetUserByLogin(model.Login);
                if (existingUserByLogin.StatusCode == StatusCode.OK && existingUserByLogin.Data != null)
                {
                    return new BaseResponse<bool>
                    {
                        Description = "Пользователь с таким логином уже существует",
                        StatusCode = StatusCode.BadRequest
                    };
                }

                // Создаем нового пользователя
                var user = new UserDb
                {
                    Id = Guid.NewGuid(),
                    Login = model.Login,
                    Email = model.Email,
                    Password = BCrypt.Net.BCrypt.HashPassword(model.Password),
                    Role = Role.User,
                    pathImage = string.Empty,
                    CreatedAt = DateTime.Now // ИЗМЕНЕНО: DateTime.Now вместо UtcNow
                };

                var createResult = await _userService.CreateUser(user);
                if (createResult.StatusCode != StatusCode.OK)
                {
                    return new BaseResponse<bool>
                    {
                        Description = createResult.Description,
                        StatusCode = createResult.StatusCode
                    };
                }

                return new BaseResponse<bool>
                {
                    Description = "Пользователь успешно зарегистрирован",
                    StatusCode = StatusCode.OK,
                    Data = true
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<bool>
                {
                    Description = $"Ошибка при регистрации: {ex.Message}",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }

        public async Task<BaseResponse<bool>> Login(LoginViewModel model)
        {
            try
            {
                // Ищем пользователя по email (в вашей форме используется email для входа)
                var userResponse = await _userService.GetUserByEmail(model.Email);
                if (userResponse.StatusCode != StatusCode.OK || userResponse.Data == null)
                {
                    return new BaseResponse<bool>
                    {
                        Description = "Пользователь не найден",
                        StatusCode = StatusCode.NotFound
                    };
                }

                // Проверяем пароль
                var passwordValid = await _userService.VerifyPassword(userResponse.Data, model.Password);
                if (passwordValid.StatusCode != StatusCode.OK || !passwordValid.Data)
                {
                    return new BaseResponse<bool>
                    {
                        Description = "Неверный пароль",
                        StatusCode = StatusCode.BadRequest
                    };
                }

                return new BaseResponse<bool>
                {
                    Description = "Успешная авторизация",
                    StatusCode = StatusCode.OK,
                    Data = true
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<bool>
                {
                    Description = $"Ошибка при авторизации: {ex.Message}",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }

        public async Task<BaseResponse<bool>> Logout()
        {
            return new BaseResponse<bool>
            {
                Description = "Успешный выход",
                StatusCode = StatusCode.OK,
                Data = true
            };
        }

        public async Task<BaseResponse<bool>> ChangePassword(string email, string oldPassword, string newPassword)
        {
            try
            {
                var userResponse = await _userService.GetUserByEmail(email);
                if (userResponse.StatusCode != StatusCode.OK || userResponse.Data == null)
                {
                    return new BaseResponse<bool>
                    {
                        Description = "Пользователь не найден",
                        StatusCode = StatusCode.NotFound
                    };
                }

                // Проверяем старый пароль
                var passwordValid = await _userService.VerifyPassword(userResponse.Data, oldPassword);
                if (passwordValid.StatusCode != StatusCode.OK || !passwordValid.Data)
                {
                    return new BaseResponse<bool>
                    {
                        Description = "Неверный текущий пароль",
                        StatusCode = StatusCode.BadRequest
                    };
                }

                // Обновляем пароль
                userResponse.Data.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
                var updateResult = await _userService.UpdateUserProfile(userResponse.Data);

                return new BaseResponse<bool>
                {
                    Description = updateResult.Description,
                    StatusCode = updateResult.StatusCode,
                    Data = updateResult.Data
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<bool>
                {
                    Description = $"Ошибка при изменении пароля: {ex.Message}",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }
    }
}
