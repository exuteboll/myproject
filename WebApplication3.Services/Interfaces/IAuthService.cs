using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApplicatoin3.Domain.ModelsDb;
using WebApplicatoin3.Domain.Response;

namespace WebApplication3.Services.Interfaces
{
    public interface IAuthService
    {
        Task<BaseResponse<string>> GenerateToken(UserDb user);
        Task<BaseResponse<bool>> ValidateToken(string token);
        Task<BaseResponse<UserDb>> GetUserFromToken(string token);
    }
}
