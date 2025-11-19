using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApplicatoin3.Domain.Enum;

namespace WebApplicatoin3.Domain.Response
{
    public class BaseResponse<T> : IBaseResponse<T>
    {
        public string Description { get; set; }
        public StatusCode  StatusCode { get; set; }
        public T Data { get; set; }
    }
    public interface IBaseResponse<T>
    {
        public T Data { get; set; }
    }
    public enum StatusCode
    {
        OK = 200,
        InternalServerError = 500,
        NotFound = 404,
        BadRequest = 400,
      
    }
}
