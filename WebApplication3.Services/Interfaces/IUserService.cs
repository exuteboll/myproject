using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApplicatoin3.Domain.ModelsDb;
using WebApplicatoin3.Domain.Response;

namespace WebApplication3.Services.Interfaces
{
    public interface IUserService
    {
        Task<BaseResponse<UserDb>> GetUserByEmail(string email);
        Task<BaseResponse<UserDb>> GetUserByLogin(string login);
        Task<BaseResponse<UserDb>> GetUserById(Guid id);
        Task<BaseResponse<bool>> UpdateUserProfile(UserDb user);
        Task<BaseResponse<bool>> CreateUser(UserDb user);
        Task<BaseResponse<bool>> VerifyPassword(UserDb user, string password);
    }
}
