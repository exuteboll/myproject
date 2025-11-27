using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApplication3.Services.Interfaces;
using WebApplicatoin3.Domain.Response;

namespace WebApplication3.Services.Realizations
{
    public class OrderService : IOrderService
    {
        private readonly IUserService _userService;

        public OrderService(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<BaseResponse<bool>> UpdateUserProfile(Guid userId, string login, string email)
        {
            try
            {
                var userResponse = await _userService.GetUserById(userId);
                if (userResponse.StatusCode != StatusCode.OK || userResponse.Data == null)
                {
                    return new BaseResponse<bool>
                    {
                        Description = "Пользователь не найден",
                        StatusCode = StatusCode.NotFound
                    };
                }

                var user = userResponse.Data;

                // Проверяем, не занят ли email другим пользователем
                if (user.Email != email)
                {
                    var existingUser = await _userService.GetUserByEmail(email);
                    if (existingUser.StatusCode == StatusCode.OK && existingUser.Data != null)
                    {
                        return new BaseResponse<bool>
                        {
                            Description = "Пользователь с таким email уже существует",
                            StatusCode = StatusCode.BadRequest
                        };
                    }
                    user.Email = email;
                }

                // Проверяем, не занят ли логин другим пользователем
                if (user.Login != login)
                {
                    var existingUser = await _userService.GetUserByLogin(login);
                    if (existingUser.StatusCode == StatusCode.OK && existingUser.Data != null)
                    {
                        return new BaseResponse<bool>
                        {
                            Description = "Пользователь с таким логином уже существует",
                            StatusCode = StatusCode.BadRequest
                        };
                    }
                    user.Login = login;
                }

                var updateResult = await _userService.UpdateUserProfile(user);
                return updateResult;
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
    }
}
