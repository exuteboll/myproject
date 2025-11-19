using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApplication3.DAL.Interfaces;
using WebApplication3.Services.Interfaces;
using WebApplicatoin3.Domain.ModelsDb;
using WebApplicatoin3.Domain.Response;
using Microsoft.EntityFrameworkCore;

namespace WebApplication3.Services.Realizations
{
    public class UserService : IUserService
    {
        private readonly IBaseStorage<UserDb> _userStorage;

        public UserService(IBaseStorage<UserDb> userStorage)
        {
            _userStorage = userStorage;
        }

        public async Task<BaseResponse<UserDb>> GetUserByEmail(string email)
        {
            try
            {
                var user = await _userStorage.GetAll()
                    .FirstOrDefaultAsync(u => u.Email == email);

                if (user == null)
                {
                    return new BaseResponse<UserDb>
                    {
                        Description = "Пользователь не найден",
                        StatusCode = StatusCode.NotFound
                    };
                }

                return new BaseResponse<UserDb>
                {
                    Description = "Пользователь найден",
                    StatusCode = StatusCode.OK,
                    Data = user
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<UserDb>
                {
                    Description = $"Ошибка при получении пользователя: {ex.Message}",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }

        public async Task<BaseResponse<UserDb>> GetUserByLogin(string login)
        {
            try
            {
                var user = await _userStorage.GetAll()
                    .FirstOrDefaultAsync(u => u.Login == login);

                if (user == null)
                {
                    return new BaseResponse<UserDb>
                    {
                        Description = "Пользователь не найден",
                        StatusCode = StatusCode.NotFound
                    };
                }

                return new BaseResponse<UserDb>
                {
                    Description = "Пользователь найден",
                    StatusCode = StatusCode.OK,
                    Data = user
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<UserDb>
                {
                    Description = $"Ошибка при получении пользователя: {ex.Message}",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }

        public async Task<BaseResponse<UserDb>> GetUserById(Guid id)
        {
            try
            {
                var user = await _userStorage.Get(id);

                if (user == null)
                {
                    return new BaseResponse<UserDb>
                    {
                        Description = "Пользователь не найден",
                        StatusCode = StatusCode.NotFound
                    };
                }

                return new BaseResponse<UserDb>
                {
                    Description = "Пользователь найден",
                    StatusCode = StatusCode.OK,
                    Data = user
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<UserDb>
                {
                    Description = $"Ошибка при получении пользователя: {ex.Message}",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }

        public async Task<BaseResponse<bool>> UpdateUserProfile(UserDb user)
        {
            try
            {
                await _userStorage.Update(user);
                return new BaseResponse<bool>
                {
                    Description = "Профиль успешно обновлен",
                    StatusCode = StatusCode.OK,
                    Data = true
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<bool>
                {
                    Description = $"Ошибка при обновлении профиля: {ex.Message}",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }

        public async Task<BaseResponse<bool>> CreateUser(UserDb user)
        {
            try
            {
                await _userStorage.Add(user);
                return new BaseResponse<bool>
                {
                    Description = "Пользователь успешно создан",
                    StatusCode = StatusCode.OK,
                    Data = true
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<bool>
                {
                    Description = $"Ошибка при создании пользователя: {ex.Message}",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }

        public async Task<BaseResponse<bool>> VerifyPassword(UserDb user, string password)
        {
            try
            {
                var isValid = BCrypt.Net.BCrypt.Verify(password, user.Password);
                return new BaseResponse<bool>
                {
                    Description = isValid ? "Пароль верный" : "Пароль неверный",
                    StatusCode = StatusCode.OK,
                    Data = isValid
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<bool>
                {
                    Description = $"Ошибка при проверке пароля: {ex.Message}",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }
    }
}
