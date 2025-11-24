using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApplication3.Services.Interfaces;
using WebApplicatoin3.Domain.Response;
using DomainStatusCode = WebApplicatoin3.Domain.Response.StatusCode;

namespace WebApplication3.Services.Realizations
{
    public class OrderService : IOrderService
    {
        private readonly IUserService _userService;

        public OrderService(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<BaseResponse<bool>> UpdateUserProfile(Guid userId, string login, string email, string avatarPath = null)
        {
            try
            {
                var userResponse = await _userService.GetUserById(userId);
                if (userResponse.StatusCode != DomainStatusCode.OK || userResponse.Data == null)
                {
                    return new BaseResponse<bool>
                    {
                        Description = "Пользователь не найден",
                        StatusCode = DomainStatusCode.NotFound
                    };
                }

                var user = userResponse.Data;

                // Сохраняем старый путь к аватарке для возможного удаления
                var oldAvatarPath = user.pathImage;

                // Проверяем, не занят ли email другим пользователем
                if (user.Email != email)
                {
                    var existingUser = await _userService.GetUserByEmail(email);
                    if (existingUser.StatusCode == DomainStatusCode.OK && existingUser.Data != null)
                    {
                        return new BaseResponse<bool>
                        {
                            Description = "Пользователь с таким email уже существует",
                            StatusCode = DomainStatusCode.BadRequest
                        };
                    }
                    user.Email = email;
                }

                // Проверяем, не занят ли логин другим пользователем
                if (user.Login != login)
                {
                    var existingUser = await _userService.GetUserByLogin(login);
                    if (existingUser.StatusCode == DomainStatusCode.OK && existingUser.Data != null)
                    {
                        return new BaseResponse<bool>
                        {
                            Description = "Пользователь с таким логином уже существует",
                            StatusCode = DomainStatusCode.BadRequest
                        };
                    }
                    user.Login = login;
                }

                // Обновляем аватарку, если передана новая
                if (!string.IsNullOrEmpty(avatarPath))
                {
                    user.pathImage = avatarPath;
                }

                var updateResult = await _userService.UpdateUserProfile(user);

                // Если обновление прошло успешно и есть новая аватарка, удаляем старую
                if (updateResult.StatusCode == DomainStatusCode.OK &&
                    !string.IsNullOrEmpty(avatarPath) &&
                    !string.IsNullOrEmpty(oldAvatarPath) &&
                    oldAvatarPath != avatarPath)
                {
                    // Здесь можно вызвать метод для удаления старой аватарки
                    // DeleteOldAvatar(oldAvatarPath);
                }

                return updateResult;
            }
            catch (Exception ex)
            {
                return new BaseResponse<bool>
                {
                    Description = $"Ошибка при обновлении профиля: {ex.Message}",
                    StatusCode = DomainStatusCode.InternalServerError
                };
            }
        }
    }
}
