using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApplicatoin3.Domain.Response;
using WebApplicatoin3.Domain.ViewModels.LoginAndRegistration;

namespace WebApplication3.Services.Interfaces
{
    public interface IAccountService
    {
        Task<BaseResponse<bool>> Register(RegisterViewModel model);
        Task<BaseResponse<bool>> Login(LoginViewModel model);
        Task<BaseResponse<bool>> Logout();
        Task<BaseResponse<bool>> ChangePassword(string email, string oldPassword, string newPassword);
    }
}
