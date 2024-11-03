using _2Sport_BE.Repository.Data;
using _2Sport_BE.Repository.Interfaces;
using _2Sport_BE.Repository.Models;
using _2Sport_BE.Infrastructure.DTOs;
using _2Sport_BE.Infrastructure.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text;
using System.Security.Claims;


namespace _2Sport_BE.Infrastructure.Services
{
    public class TokenModel
    {
        [JsonProperty("token")]
        public string Token { get; set; }
        [JsonProperty("refreshToken")]
        public string RefreshToken { get; set; }
        [JsonProperty("userId")]
        public int? UserId { get; set; }
    }
    public class AuthenticationResult : TokenModel
    {
        public bool Success { get; set; }
        public IEnumerable<string> Errors { get; set; }
    }
    public interface IAuthService
    {
        Task<ResponseDTO<TokenModel>> LoginAsync(UserLogin login);
        Task<ResponseDTO<TokenModel>> HandleLoginGoogle(ClaimsPrincipal principal);
        Task<TokenModel> LoginGoogleAsync(User login);
        Task<ResponseDTO<TokenModel>> RefreshTokenAsync(TokenModel request);
        Task<ResponseDTO<string>> SignUpAsync(RegisterModel registerModel);
        Task<ResponseDTO<string>> HandleResetPassword(ResetPasswordRequest resetPasswordRequest);
    }
    public class AuthService : IAuthService
    {
        private readonly TwoSportCapstoneDbContext _context;
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;
        private readonly TokenValidationParameters _tokenValidationParameters;
        private readonly IUnitOfWork _unitOfWork;

        public AuthService(TwoSportCapstoneDbContext context,
            IUserService userService,
            IConfiguration configuration,
            TokenValidationParameters tokenValidationParameters,
            IUnitOfWork unitOfWork
            )
        {
            _context = context;
            _userService = userService;
            _configuration = configuration;
            _tokenValidationParameters = tokenValidationParameters;
            _unitOfWork = unitOfWork;
        }
        private string HashPassword(string password)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));

                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
        public async Task<ResponseDTO<TokenModel>> LoginAsync(UserLogin requestUser)
        {
            ResponseDTO<TokenModel> response = new ResponseDTO<TokenModel>();
            try
            {
                var loginUser = await _unitOfWork.UserRepository
                    .GetObjectAsync(_ => _.UserName == requestUser.UserName && _.HashPassword == HashPassword(requestUser.Password));

                if (loginUser == null)
                {
                    response.IsSuccess = false;
                    response.Message = "Invalid Username And Password";
                    return response;
                }

                if (loginUser != null && loginUser.IsActived != true)
                {
                    response.IsSuccess = false;
                    response.Message = "Not Permission";
                    return response;
                }
                //Khi ma login thanh cong, se tao token qua ham AuthenticateAsync(User)
                var authenticationResult = await AuthenticateAsync(loginUser);

                if (authenticationResult != null && authenticationResult.Success)
                {
                    response.Message = "Query successfully";
                    response.IsSuccess = true;
                    response.Data = new TokenModel() { UserId = loginUser.Id, Token = authenticationResult.Token, RefreshToken = authenticationResult.RefreshToken };
                }
                else
                {
                    response.Message = "Something went wrong!";
                    response.IsSuccess = false;
                }

                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<AuthenticationResult> AuthenticateAsync(User user)
        {
            var authenticationResult = new AuthenticationResult();
            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                var symmetricKey = Encoding.UTF8.GetBytes(_configuration.GetSection("ServiceConfiguration:JwtSettings:Secret").Value);

                var role = await _unitOfWork.RoleRepository.GetObjectAsync(_ => _.Id == user.RoleId);

                var Subject = new List<Claim>
                    {
                    new Claim("UserId", user.Id.ToString()),
                    new Claim("UserName",user.UserName==null?"":user.UserName),
                    new Claim("FullName", user.FullName),
                    new Claim("Email",user.Email==null?"":user.Email),
                    new Claim("Phone",user.PhoneNumber==null?"":user.PhoneNumber),
                    new Claim("Gender",user.Gender==null?"":user.Gender),
                    new Claim("Address",user.Address==null?"":user.Address),
                    new Claim("DOB",user.DOB.ToString()==null?"":user.DOB.ToString()),
                    new Claim(ClaimTypes.Role, role.RoleName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    };

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(Subject),
                    Expires = DateTime.UtcNow.
                    Add(TimeSpan.Parse(_configuration.GetSection("ServiceConfiguration:JwtSettings:TokenLifetime").Value)),
                    SigningCredentials = new SigningCredentials
                    (new SymmetricSecurityKey(symmetricKey), SecurityAlgorithms.HmacSha256Signature)
                };

                var token = tokenHandler.CreateToken(tokenDescriptor);
                authenticationResult.Token = tokenHandler.WriteToken(token);

                var refreshToken = new RefreshToken
                {
                    Token = Guid.NewGuid().ToString(),
                    JwtId = token.Id,
                    UserId = user.Id,
                    CreateDate = DateTime.UtcNow,
                    ExpireDate = DateTime.UtcNow.AddMonths(6),
                    Used = false
                };
                var exist = await _unitOfWork.RefreshTokenRepository.
                    GetObjectAsync(_ => _.UserId == refreshToken.UserId && _.Used == false);
                if (exist != null)
                {
                    exist.Token = refreshToken.Token;
                    exist.JwtId = refreshToken.JwtId;
                    exist.CreateDate = refreshToken.CreateDate;
                    exist.ExpireDate = refreshToken.ExpireDate;
                    await _unitOfWork.RefreshTokenRepository.UpdateAsync(exist);
                }
                else
                {
                    await _unitOfWork.RefreshTokenRepository.InsertAsync(refreshToken);
                }
                await _unitOfWork.SaveChanges();
                //return
                authenticationResult.RefreshToken = refreshToken.Token;
                authenticationResult.Success = true;
                return authenticationResult;
            }
            catch (Exception ex)
            {
                authenticationResult.Success = false;
                authenticationResult.Errors = new List<string> { ex.Message };
            }
            return authenticationResult;
        }
        public async Task<ResponseDTO<TokenModel>> RefreshTokenAsync(TokenModel request)
        {
            ResponseDTO<TokenModel> response = new ResponseDTO<TokenModel>();
            try
            {
                var authResponse = await GetRefreshTokenAsync(request.Token, request.RefreshToken);
                if (!authResponse.Success)
                {

                    response.IsSuccess = false;
                    response.Message = string.Join(",", authResponse.Errors);
                    return response;
                }
                TokenModel refreshTokenModel = new TokenModel();
                refreshTokenModel.Token = authResponse.Token;
                refreshTokenModel.RefreshToken = authResponse.RefreshToken;
                response.Data = refreshTokenModel;
                return response;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "Something went wrong!";
                return response;
            }
        }
        private async Task<AuthenticationResult> GetRefreshTokenAsync(string token, string refreshToken)
        {
            var validatedToken = GetPrincipalFromToken(token);

            if (validatedToken == null)
            {
                return new AuthenticationResult { Errors = new[] { "Invalid Token" } };
            }

            var expiryDateUnix =
                long.Parse(validatedToken.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Exp).Value);

            var expiryDateTimeUtc = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                .AddSeconds(expiryDateUnix);

            if (expiryDateTimeUtc > DateTime.Now)
            {
                return new AuthenticationResult { Errors = new[] { "This token hasn't expired yet" } };
            }

            var jti = validatedToken.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Jti).Value;

            var storedRefreshToken = _context.RefreshTokens.FirstOrDefault(x => x.Token == refreshToken);

            if (storedRefreshToken == null)
            {
                return new AuthenticationResult { Errors = new[] { "This refresh token does not exist" } };
            }

            if (DateTime.UtcNow > storedRefreshToken.ExpireDate)
            {
                return new AuthenticationResult { Errors = new[] { "This refresh token has expired" } };
            }

            if (storedRefreshToken.Used.HasValue && storedRefreshToken.Used == true)
            {
                return new AuthenticationResult { Errors = new[] { "This refresh token has been used" } };
            }

            if (storedRefreshToken.JwtId != jti)
            {
                return new AuthenticationResult { Errors = new[] { "This refresh token does not match this JWT" } };
            }

            storedRefreshToken.Used = true;
            _context.RefreshTokens.Update(storedRefreshToken);

            await _context.SaveChangesAsync();
            string strUserId = validatedToken.Claims.Single(x => x.Type == "UserId").Value;
            long userId = 0;
            long.TryParse(strUserId, out userId);
            var user = _context.Users.FirstOrDefault(c => c.Id == userId);
            if (user == null)
            {
                return new AuthenticationResult { Errors = new[] { "User Not Found" } };
            }

            return await AuthenticateAsync(user);
        }
        private ClaimsPrincipal GetPrincipalFromToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                var tokenValidationParameters = _tokenValidationParameters.Clone();
                tokenValidationParameters.ValidateLifetime = false;
                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var validatedToken);//
                if (!IsJwtWithValidSecurityAlgorithm(validatedToken))
                {
                    return null;
                }

                return principal;
            }
            catch
            {
                return null;
            }
        }
        private bool IsJwtWithValidSecurityAlgorithm(SecurityToken validatedToken)
        {
            return (validatedToken is JwtSecurityToken jwtSecurityToken) &&
                   jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                       StringComparison.InvariantCultureIgnoreCase);
        }
        public async Task<ResponseDTO<TokenModel>> HandleLoginGoogle(ClaimsPrincipal principal)
        {
            var result = new ResponseDTO<TokenModel>();

            var googleId = principal.FindFirst(ClaimTypes.NameIdentifier);
            //Get principals from Google response
            var email = principal.FindFirst(ClaimTypes.Email);
            var phone = principal.FindFirst(ClaimTypes.MobilePhone);

            if (googleId == null || email == null)
            {
                return new ResponseDTO<TokenModel> { IsSuccess = false, Message = "Error retrieving Google user information" };
            }
            try
            {
                var user = await _unitOfWork.UserRepository.GetObjectAsync(_ => _.GoogleId == googleId.Value || _.Email == email.Value);
                //Check user if it is not exist
                if (user == null)
                {
                    user = new User()
                    {
                        FullName = email.Value,
                        Email = email.Value,
                        PhoneNumber = phone.Value ?? "",
                        RoleId = (int)UserRole.Customer,
                        GoogleId = googleId.Value,
                        EmailConfirmed = true,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now,
                        IsActived = true,

                    };
                    await _unitOfWork.UserRepository.InsertAsync(user);
                }
                else
                {
                    user.EmailConfirmed = true;
                    user.UpdatedAt = DateTime.Now;
                    await _unitOfWork.UserRepository.UpdateAsync(user);
                }

                result.IsSuccess = true;
                result.Message = "Login successfully!";
                result.Data = await LoginGoogleAsync(user);
            }
            catch (Exception)
            {

                result.IsSuccess = true;
                result.Message = "Login successfully!";
                
            }
            

            return result;
        }
        public async Task<TokenModel> LoginGoogleAsync(User login)
        {
            var tokenModelResult = new TokenModel();
            AuthenticationResult authenticationResult = await AuthenticateAsync(login);

            if (authenticationResult != null && authenticationResult.Success)
            {
                tokenModelResult.RefreshToken = authenticationResult.RefreshToken;
                tokenModelResult.Token = authenticationResult.Token;
                tokenModelResult.UserId = login.Id;
            }

            return tokenModelResult;
        }
        public async Task<ResponseDTO<string>> SignUpAsync(RegisterModel registerModel)
        {
            var response = new ResponseDTO<string>();

            var checkExist = await _unitOfWork.UserRepository
                .GetObjectAsync(_ =>
                _.Email.ToLower().Equals(registerModel.Email.ToLower()) ||
                _.UserName.Equals(registerModel.Username));
            if (checkExist != null)
            {
                response.IsSuccess = false;
                response.Message = "Already have an account!";
                response.Data = "";
            }
            else
            {
                try
                {
                    checkExist = new User()
                    {
                        UserName = registerModel.Username,
                        HashPassword = HashPassword(registerModel.Password),
                        Email = registerModel.Email,
                        FullName = registerModel.FullName,
                        EmailConfirmed = false,
                        RoleId = (int)UserRole.Customer,
                        IsActived = true,
                        CreatedAt = DateTime.Now
                    };

                    await _unitOfWork.UserRepository.InsertAsync(checkExist);
                    _unitOfWork.Save();

                    response.IsSuccess = true;
                    response.Message = "Sign Up Successfully";
                    response.Data = checkExist.Id.ToString();
                }
                catch (DbUpdateException)
                {
                    response.IsSuccess = false;
                    response.Message = "Db exception";
                }
                catch (Exception ex)
                {
                    response.IsSuccess = false;
                    response.Message = ex.Message;
                    throw ex;
                }
            }
            return response;
        }
        public async Task<ResponseDTO<string>> HandleResetPassword(ResetPasswordRequest resetPasswordRequest)
        {
            var response = new ResponseDTO<string>();
            var user = await _unitOfWork.UserRepository
                .GetObjectAsync(u => u.Email.Equals(resetPasswordRequest.Email) &&
                                u.PasswordResetToken.Equals(resetPasswordRequest.Token));
            if (user == null)
            {
                response.IsSuccess = false;
                response.Message = "Invalid or expired token or User not found.";
            }
            else
            {
                user.HashPassword = HashPassword(resetPasswordRequest.NewPassword);
                user.PasswordResetToken = null;
                await _userService.UpdateUserAsync(user.Id, user);

                response.IsSuccess = true;
                response.Message = "Password reset successful.";
            }          
            return response;
        }
    }
}
