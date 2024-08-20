using _2Sport_BE.DataContent;
using _2Sport_BE.Infrastructure.Services;
using _2Sport_BE.Repository.Interfaces;
using _2Sport_BE.Repository.Models;
using _2Sport_BE.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace _2Sport_BE.API.Services
{
    public interface IIdentityService
    {
        Task<ResponseModel<TokenModel>> LoginAsync(UserLogin login);
        Task<ResponseModel<TokenModel>> LoginGoogleAsync(User login);
        Task<ResponseModel<TokenModel>> RefreshTokenAsync(TokenModel request);
    }

    public class IdentityService : IIdentityService
    {
        private readonly TwoSportDBContext _context;
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;
        private readonly TokenValidationParameters _tokenValidationParameters;
        private readonly IUnitOfWork _unitOfWork;
        
        public IdentityService(TwoSportDBContext context,
            IOptions<ServiceConfiguration> settings,
            IUserService userService,
            IConfiguration configuration,
            TokenValidationParameters tokenValidationParameters,
            IUnitOfWork unitOfWork)
        {
            _context = context;
            _userService = userService;
            _configuration = configuration;
            _tokenValidationParameters = tokenValidationParameters;
            _unitOfWork = unitOfWork;
        }


        public async Task<ResponseModel<TokenModel>> LoginAsync(UserLogin login)
        {
            ResponseModel<TokenModel> response = new ResponseModel<TokenModel>();
            try
            {
                var loginUser = await _context.Users.FirstOrDefaultAsync(_ => _.UserName == login.UserName && _.Password == login.Password);
                    if(loginUser == null)
                    {
                        response.IsSuccess = false;
                        response.Message = "Invalid Username And Password";
                        return response;
                    }
                    if(loginUser != null && loginUser.IsActive != true)
                    {
                    response.IsSuccess = false;
                    response.Message = "Not Permission";
                    return response;
                    }
                AuthenticationResult authenticationResult = await AuthenticateAsync(loginUser);
                if (authenticationResult != null && authenticationResult.Success)
                {
                    response.Message = "Query successfully";
                    response.IsSuccess = true;
                    response.Data = new TokenModel() {UserId = loginUser.Id, Token = authenticationResult.Token, RefreshToken = authenticationResult.RefreshToken };
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
            string serect = _configuration.GetSection("ServiceConfiguration:JwtSettings:Secret").Value;
            // authentication successful so generate jwt token  
            AuthenticationResult authenticationResult = new AuthenticationResult();
            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                var symmetricKey = Encoding.UTF8.GetBytes(serect);

                var roleName = _context.Roles.FirstOrDefault(_ => _.Id == user.RoleId).RoleName;
                ClaimsIdentity Subject = new ClaimsIdentity(new Claim[]
                    {
                    new Claim("UserId", user.Id.ToString()),
                    new Claim("FullName", user.FullName),
                    new Claim("Email",user.Email==null?"":user.Email),
                    new Claim("UserName",user.UserName==null?"":user.UserName),
                    new Claim("Phone",user.Phone==null?"":user.Phone),
                    new Claim("Gender",user.Gender==null?"":user.Gender),
                    new Claim("Address",user.Address==null?"":user.Address),
                    new Claim(ClaimTypes.Role, roleName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    });

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = Subject,
                    Expires = DateTime.UtcNow.Add(TimeSpan.Parse(_configuration.GetSection("ServiceConfiguration:JwtSettings:TokenLifetime").Value)),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(symmetricKey), SecurityAlgorithms.HmacSha256Signature)
                };
                var token = tokenHandler.CreateToken(tokenDescriptor);
                authenticationResult.Token = tokenHandler.WriteToken(token);

                var refreshToken = new RefreshToken
                {
                    Token = Guid.NewGuid().ToString(),
                    JwtId = token.Id,
                    UserId = user.Id,
                    CreateDate = DateTime.UtcNow,
                    ExpiryDate = DateTime.UtcNow.AddMonths(6),
                    Used = false
                };
                var exist = await _context.RefreshTokens.FirstOrDefaultAsync(_ => _.UserId == refreshToken.UserId && _.Used == false);
                if (exist != null)
                {
                    exist.Token = refreshToken.Token;
                    exist.JwtId = refreshToken.JwtId;
                    exist.CreateDate = refreshToken.CreateDate;
                    exist.ExpiryDate = refreshToken.ExpiryDate;
                    _context.RefreshTokens.Update(exist);
                }
                else
                {
                await _context.RefreshTokens.AddAsync(refreshToken);
                }
                await _context.SaveChangesAsync();
                authenticationResult.RefreshToken = refreshToken.Token;
                authenticationResult.Success = true;
                return authenticationResult;
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public async Task<ResponseModel<TokenModel>> RefreshTokenAsync(TokenModel request)
        {
            ResponseModel<TokenModel> response = new ResponseModel<TokenModel>();
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

            if (DateTime.UtcNow > storedRefreshToken.ExpiryDate)
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

        public async Task<ResponseModel<TokenModel>> LoginGoogleAsync(User login)
        {
            ResponseModel<TokenModel> response = new ResponseModel<TokenModel>();
            AuthenticationResult authenticationResult = await AuthenticateAsync(login);
            if (authenticationResult != null && authenticationResult.Success)
            {
                response.Message = "Query successfully";
                response.IsSuccess = true;
                response.Data = new TokenModel() { Token = authenticationResult.Token, RefreshToken = authenticationResult.RefreshToken };
            }
            else
            {
                response.Message = "Something went wrong!";
                response.IsSuccess = false;
            }

            return response;
        }
    }
}
