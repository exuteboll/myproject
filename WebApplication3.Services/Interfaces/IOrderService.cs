using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApplicatoin3.Domain.Response;

namespace WebApplication3.Services.Interfaces
{
    public interface IOrderService
    {
        Task<BaseResponse<bool>> UpdateUserProfile(Guid userId, string login, string email);
    }   
}
