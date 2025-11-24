using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApplicatoin3.Domain.ModelsDb;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace WebApplicatoin3.Domain.ViewModels.Profile
{
    public class ProfileViewModel
    {
        public UserDb User { get; set; }
    }

    public class EditProfileViewModel
    {
        [Required(ErrorMessage = "Логин обязателен")]
        [MaxLength(20, ErrorMessage = "Логин должен быть не длиннее 20 символов")]
        [MinLength(3, ErrorMessage = "Логин должен быть не короче 3 символов")]
        public string Login { get; set; }

        [Required(ErrorMessage = "Email обязателен")]
        [EmailAddress(ErrorMessage = "Некорректный формат email")]
        public string Email { get; set; }

        // Новое поле для аватарки
        [Display(Name = "Аватарка")]
        public IFormFile Avatar { get; set; }
    }

    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage = "Текущий пароль обязателен")]
        [DataType(DataType.Password)]
        public string CurrentPassword { get; set; }

        [Required(ErrorMessage = "Новый пароль обязателен")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Пароль должен быть не короче 6 символов")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Подтверждение пароля обязательно")]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Пароли не совпадают")]
        public string ConfirmPassword { get; set; }
    }
}
