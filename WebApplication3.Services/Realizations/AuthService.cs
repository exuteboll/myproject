using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using WebApplication3.Services.Interfaces;
using WebApplicatoin3.Domain.Enum;
using WebApplicatoin3.Domain.ModelsDb;
using WebApplicatoin3.Domain.Response;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;


namespace WebApplication3.Services.Realizations
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _configuration;

        public AuthService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<BaseResponse<string>> GenerateToken(UserDb user)
        {
            try
            {
                var jwtKey = _configuration["Jwt:Key"];
                if (string.IsNullOrEmpty(jwtKey))
                {
                    return new BaseResponse<string>
                    {
                        Description = "JWT Key is not configured",
                        StatusCode = StatusCode.InternalServerError
                    };
                }

                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(jwtKey);

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                        new Claim(ClaimTypes.Name, user.Login),
                        new Claim(ClaimTypes.Email, user.Email),
                        new Claim(ClaimTypes.Role, user.Role.ToString())
                    }),
                    Expires = DateTime.UtcNow.AddHours(3),
                    Issuer = _configuration["Jwt:Issuer"],
                    Audience = _configuration["Jwt:Audience"],
                    SigningCredentials = new SigningCredentials(
                        new SymmetricSecurityKey(key),
                        SecurityAlgorithms.HmacSha256Signature)
                };

                var token = tokenHandler.CreateToken(tokenDescriptor);
                var tokenString = tokenHandler.WriteToken(token);

                return new BaseResponse<string>
                {
                    Description = "Токен успешно создан",
                    StatusCode = StatusCode.OK,
                    Data = tokenString
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<string>
                {
                    Description = $"Ошибка при генерации токена: {ex.Message}",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }

        public async Task<BaseResponse<bool>> ValidateToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);

                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = _configuration["Jwt:Issuer"],
                    ValidAudience = _configuration["Jwt:Audience"],
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                return new BaseResponse<bool>
                {
                    Description = "Токен валиден",
                    StatusCode = StatusCode.OK,
                    Data = true
                };
            }
            catch (Exception)
            {
                return new BaseResponse<bool>
                {
                    Description = "Невалидный токен",
                    StatusCode = StatusCode.BadRequest,
                    Data = false
                };
            }
        }

        public async Task<BaseResponse<UserDb>> GetUserFromToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadJwtToken(token);

                var userIdClaim = jwtToken.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);
                var loginClaim = jwtToken.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name);
                var emailClaim = jwtToken.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email);
                var roleClaim = jwtToken.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role);

                if (userIdClaim == null || loginClaim == null || emailClaim == null || roleClaim == null)
                {
                    return new BaseResponse<UserDb>
                    {
                        Description = "Невалидный токен: отсутствуют необходимые claims",
                        StatusCode = StatusCode.BadRequest
                    };
                }

                var user = new UserDb
                {
                    Id = Guid.Parse(userIdClaim.Value),
                    Login = loginClaim.Value,
                    Email = emailClaim.Value,
                    Role = (Role)Enum.Parse(typeof(Role), roleClaim.Value)
                };

                return new BaseResponse<UserDb>
                {
                    Description = "Пользователь получен из токена",
                    StatusCode = StatusCode.OK,
                    Data = user
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<UserDb>
                {
                    Description = $"Ошибка при получении пользователя из токена: {ex.Message}",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }
    }
}
