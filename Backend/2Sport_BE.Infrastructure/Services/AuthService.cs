using _2Sport_BE.Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace _2Sport_BE.Service.Services
{
    public interface IAuthService
    {
        //Login, Register, Login Facebook, Google, Forgot Password
        /*Task<ResponseModel<TokenModel>> LoginAsync(UserLogin login);
        Task<ResponseModel<TokenModel>> HandleLoginGoogle(ClaimsPrincipal principal);
        Task<TokenModel> LoginGoogleAsync(User login);
        Task<ResponseModel<TokenModel>> RefreshTokenAsync(TokenModel request);*/
    }
    public class AuthService
    {
    }
}
