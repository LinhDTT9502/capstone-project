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
using _2Sport_BE.Services;
using _2Sport_BE.Infrastructure.Helpers;
using Twilio.Jwt.AccessToken;


namespace _2Sport_BE.Infrastructure.Services
{
    public class TokenModel
    {
        [JsonProperty("token")]
        public string Token { get; set; }
        [JsonProperty("refreshToken")]
        public string RefreshToken { get; set; }
        [JsonProperty("userId")]
        public int UserId { get; set; }
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
        Task<ResponseDTO<TokenModel>> RefreshAccessTokenAsync(TokenModel request);
        Task<ResponseDTO<string>> SignUpAsync(RegisterModel registerModel);
        Task<ResponseDTO<string>> HandleResetPassword(ResetPasswordRequest resetPasswordRequest);
        Task<ResponseDTO<string>> SignUpMobileAsync(RegisterModel registerModel);
        Task<ResponseDTO<int>> HandleLogoutAsync(TokenModel request);

    }
    public class AuthService : IAuthService
    {

        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;
        private readonly TokenValidationParameters _tokenValidationParameters;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMailService _mailService;
        private readonly IMethodHelper _methodHelper;
        private readonly ICustomerService _customerService;
        public AuthService(
            IUserService userService,
            IConfiguration configuration,
            TokenValidationParameters tokenValidationParameters,
            IUnitOfWork unitOfWork,
            IMethodHelper methodHelper,
            IMailService mailService,
            ICustomerService customerService
            )
        {
            _userService = userService;
            _configuration = configuration;
            _tokenValidationParameters = tokenValidationParameters;
            _unitOfWork = unitOfWork;
            _methodHelper = methodHelper;
            _mailService = mailService;
            _customerService = customerService;
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
                    .GetObjectAsync(_ => _.UserName == requestUser.UserName
                    && _.HashPassword == HashPassword(requestUser.Password), new string[] { "Staffs", "Managers" });

                if (loginUser == null)
                {
                    response.IsSuccess = false;
                    response.Message = "Invalid Username And Password";
                    return response;
                }
                if (!loginUser.EmailConfirmed)
                {
                    response.IsSuccess = false;
                    response.Message = "Your account is not confirmed.";
                    return response;
                }
                if (loginUser.IsActived != true)
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
                var staff = user.Staffs.FirstOrDefault();
                var manager = user.Managers.FirstOrDefault();

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
                    new Claim("EmailConfirmed",user.EmailConfirmed==null?"":user.EmailConfirmed.ToString()),
                    new Claim("StaffId", staff != null ? staff.StaffId.ToString() : ""),
                    new Claim("ManagerId", manager != null ? manager.Id.ToString() : ""),
                    new Claim(ClaimTypes.Role, role.RoleName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    };
                if (staff != null)
                {
                    Subject.Add(new Claim("BranchId", staff.BranchId.ToString()));
                }
                if (manager != null)
                {
                    Subject.Add(new Claim("BranchId", manager.BranchId.ToString()));
                }


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
                    ExpireDate = DateTime.UtcNow.AddDays(7),
                    Used = false
                };

                // Lưu refresh token mới cho thiết bị này
                await _unitOfWork.RefreshTokenRepository.InsertAsync(refreshToken);
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
        public async Task<ResponseDTO<TokenModel>> RefreshAccessTokenAsync(TokenModel request)
        {
            ResponseDTO<TokenModel> response = new ResponseDTO<TokenModel>();
            if (string.IsNullOrWhiteSpace(request.Token) || string.IsNullOrWhiteSpace(request.RefreshToken))
            {
                response.IsSuccess = false;
                response.Message = "Token hoặc Refresh Token không hợp lệ.";
                return response;
            }
            try
            {
                var authResponse = await GetRefreshTokenAsync(request.Token, request.RefreshToken, request.UserId);
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
        private async Task<AuthenticationResult> GetRefreshTokenAsync(string token, string refreshToken, int userID)
        {
            var validatedToken = GetPrincipalFromToken(token);

            if (validatedToken == null)
            {
                return new AuthenticationResult { Errors = new[] { "Access token không hợp lệ" } };
            }

            var expiryDateUnix =
                long.Parse(validatedToken.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Exp).Value);

            var expiryDateTimeUtc = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                .AddSeconds(expiryDateUnix);

            if (expiryDateTimeUtc > DateTime.UtcNow)
            {
                return new AuthenticationResult { Errors = new[] { "Access token này chưa hết hạn" } };
            }

            var jti = validatedToken.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Jti).Value;

            var storedRefreshToken = await _unitOfWork.RefreshTokenRepository
                .GetObjectAsync(x => x.Token == refreshToken, new string[] { "User" });

            if (storedRefreshToken == null)
            {
                return new AuthenticationResult { Errors = new[] { "Refresh Token không tồn tại." } };
            }
            if (storedRefreshToken.UserId.Value != userID)
            {
                return new AuthenticationResult { Errors = new[] { "UserId không tồn tại hoặc không đúng." } };
            }
            if (DateTime.UtcNow > storedRefreshToken.ExpireDate)
            {
                return new AuthenticationResult { Errors = new[] { "Refresh Token đã hết hạn." } }; 
            }

            if (storedRefreshToken.Used.HasValue && storedRefreshToken.Used == true)
            {
                return new AuthenticationResult { Errors = new[] { "Refresh Token đã được sử dụng." } };
            }

            if (storedRefreshToken.JwtId != jti)
            {
                return new AuthenticationResult { Errors = new[] { "Refresh Token không khớp với JWT." } };
            }

            storedRefreshToken.Used = true;
            await _unitOfWork.RefreshTokenRepository.UpdateAsync(storedRefreshToken);
            await _unitOfWork.SaveChanges();

            var userIdClaim = validatedToken.Claims.SingleOrDefault(x => x.Type == "UserId");

            if (userIdClaim == null || !long.TryParse(userIdClaim.Value, out var userId))
            {
                return new AuthenticationResult { Errors = new[] { "User không hợp lệ trong JWT." } };
            }

            var user = await _unitOfWork.UserRepository.GetObjectAsync(u => u.Id == userId);
            if (user == null)
            {
                return new AuthenticationResult { Errors = new[] { "Không tìm thấy User." } };
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

            // Lấy thông tin từ ClaimsPrincipal
            var googleId = principal.FindFirst(ClaimTypes.NameIdentifier);
            var email = principal.FindFirst(ClaimTypes.Email);
            var phone = principal.FindFirst(ClaimTypes.MobilePhone);

            if (googleId == null || email == null)
            {
                return new ResponseDTO<TokenModel> { IsSuccess = false, Message = "Error retrieving Google user information" };
            }

            using var transaction = await _unitOfWork.BeginTransactionAsync(); // Bắt đầu transaction
            try
            {
                var isExisted = await _unitOfWork.UserRepository.GetObjectAsync(
                    _ => _.Email == email.Value, new string[] { "Staffs", "Managers" });

                if (isExisted != null)
                {
                    if (isExisted.GoogleId == null)
                    {
                        isExisted.GoogleId = googleId.Value;
                        await _unitOfWork.UserRepository.UpdateAsync(isExisted);
                    }

                    if (isExisted.EmailConfirmed)
                    {
                        result.IsSuccess = true;
                        result.Message = "Login successfully!";
                        result.Data = await LoginGoogleAsync(isExisted);

                        await transaction.CommitAsync(); // Cam kết transaction
                        return result;
                    }
                }
                else
                {
                    // Tạo mới User
                    User user = new User()
                    {
                        FullName = email.Value,
                        Email = email.Value,
                        PhoneNumber = phone != null ? phone.Value : "UNKNOWN",
                        RoleId = (int)UserRole.Customer,
                        GoogleId = googleId.Value,
                        EmailConfirmed = true,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now,
                        IsActived = true,
                    };

                    await _unitOfWork.UserRepository.InsertAsync(user);
                    await _unitOfWork.SaveChanges(); // Lưu User trước khi tạo Customer

                    // Tạo Customer
                    var createCusDetail = await _customerService.CreateCusomerAsync(new CustomerCM()
                    {
                        UserId = user.Id,
                    });

                    if (!createCusDetail.IsSuccess)
                    {
                        throw new Exception("Failed to create customer details.");
                    }

                    result.IsSuccess = true;
                    result.Message = "Login successfully!";
                    result.Data = await LoginGoogleAsync(user);

                    await transaction.CommitAsync(); // Cam kết transaction
                    return result;
                }
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(); // Hoàn tác transaction nếu có lỗi
                result.IsSuccess = false;
                result.Message = $"An error occurred: {ex.Message}";
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

                    // Tạo Customer
                    var createCusDetail = await _customerService.CreateCusomerAsync(new CustomerCM()
                    {
                        UserId = checkExist.Id,
                    });

                    if (!createCusDetail.IsSuccess)
                    {
                        throw new Exception("Failed to create customer details.");
                    }
                    response.IsSuccess = true;
                    response.Message = "Sign Up Successfully, Please check email to verify account";
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
        public async Task<ResponseDTO<string>> SignUpMobileAsync(RegisterModel registerModel)
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
                using (var transaction = await _unitOfWork.BeginTransactionAsync())
                {
                    try
                    {
                        var user = new User()
                        {
                            UserName = registerModel.Username,
                            HashPassword = HashPassword(registerModel.Password),
                            Email = registerModel.Email,
                            FullName = registerModel.FullName,
                            EmailConfirmed = false,
                            RoleId = (int)UserRole.Customer,
                            IsActived = true,
                            CreatedAt = DateTime.Now,
                            Token = _methodHelper.GenerateOTPCode()
                        };

                        await _unitOfWork.UserRepository.InsertAsync(user);
                        await _unitOfWork.SaveChanges();

                        var isSent = await _mailService.SenOTPMaillAsync(user.Email, user.Token);
                        if (!isSent)
                        {
                            await transaction.RollbackAsync();
                            response.IsSuccess = false;
                            response.Message = "Cannot sent email";
                            response.Data = "";
                        }

                        await transaction.CommitAsync();
                        response.IsSuccess = true;
                        response.Message = "Sign Up Successfully, Please check email to verify account";
                        response.Data = $"UserId = {user.Id}";
                    }
                    catch (DbUpdateException)
                    {
                        await transaction.RollbackAsync();
                        response.IsSuccess = false;
                        response.Message = "Db exception";
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        response.IsSuccess = false;
                        response.Message = ex.Message;
                    }
                }
            }
            return response;
        }

        public async Task<ResponseDTO<int>> HandleLogoutAsync(TokenModel request)
        {
            ResponseDTO<int> response = new ResponseDTO<int>();
            try
            {
                var refreshToken = await _unitOfWork.RefreshTokenRepository
                                    .GetObjectAsync(rt => rt.Token == request.RefreshToken && rt.Used != true);
                if (refreshToken == null)
                {
                    response.IsSuccess = false;
                    response.Message = "Refresh Token không tồn tại.";
                    response.Data = 0;
                    return response;
                }
                if (refreshToken.UserId  != request.UserId)
                {
                    response.IsSuccess = false;
                    response.Message = "UserId không tồn tại hoặc không đúng.";
                    response.Data = 0;
                    return response;
                }
                if (refreshToken.Used.HasValue && refreshToken.Used == true)
                {
                    response.IsSuccess = false;
                    response.Message = "Refresh Token đã được sử dụng.";
                    response.Data = 0;
                    return response;
                }

                refreshToken.Used = true;
                await _unitOfWork.RefreshTokenRepository.UpdateAsync(refreshToken);
                await _unitOfWork.SaveChanges();
                
                response.IsSuccess = false;
                response.Message = "Lougout thành công";
                response.Data = 1;
                return response;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = $"Something went wrong! Error: {ex.Message}";
                response.Data = 0;

                return response;
            }
        }
    }
}
