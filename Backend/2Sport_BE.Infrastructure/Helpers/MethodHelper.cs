using _2Sport_BE.Infrastructure.DTOs;
using _2Sport_BE.Repository.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace _2Sport_BE.Infrastructure.Helpers
{
    public class ValidationResult
    {
        public bool IsValid { get; private set; }
        public string ErrorMessage { get; private set; }

        private ValidationResult(bool isValid, string errorMessage)
        {
            IsValid = isValid;
            ErrorMessage = errorMessage;
        }

        public static ValidationResult Valid()
        {
            return new ValidationResult(true, string.Empty);
        }

        public static ValidationResult Invalid(string errorMessage)
        {
            return new ValidationResult(false, errorMessage);
        }
    }
    public interface IMethodHelper
    {
        string GenerateOrderCode();
        string GenerateOTPCode();
        string GenerateJwtForStrings(Dictionary<string, string> claims);

        bool AreAnyStringsNullOrEmpty(PaymentResponse response);
        bool CheckValidOfRentalDate(DateTime startDate, DateTime endDate, DateTime receivedDate);
        string HashPassword(string password);
        string GetFormattedDateInGMT7(DateTime date);
        ClaimsPrincipal GetPrincipalFromToken(string token);
        DateTime GetTimeInUtcPlus7();

        int GetTimeSpan(DateTime d1, DateTime d2);
    }
    public class MethodHelper : IMethodHelper
    {   
        private readonly IConfiguration _configuration;
        private readonly TokenValidationParameters _tokenValidationParameters;

        public MethodHelper(IConfiguration configuration, TokenValidationParameters tokenValidationParameters)
        {
            _configuration = configuration;
            _tokenValidationParameters = tokenValidationParameters;
        }
        public string GetFormattedDateInGMT7(DateTime date)
        {
            // Chuyển đổi thời gian sang GMT+7
            DateTime dateInGmt7 = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(date, "SE Asia Standard Time");

            // Format ngày theo định dạng `yyyyMMddHHmmss`
            return dateInGmt7.ToString("yyyyMMddHHmmss", CultureInfo.InvariantCulture);
        }
        public string HashPassword(string password)
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
        public string GenerateOTPCode()
        {
            Random random = new Random();
            string randomPart = random.Next(100000, 999999).ToString();
            return randomPart;
        }
        public string GenerateOrderCode()
        {
            string datePart = DateTime.UtcNow.ToString("yyMMdd");

            Random random = new Random();
            string randomPart = random.Next(1000, 9999).ToString();

            string SaleOrderCode = $"{datePart}{randomPart}";

            return SaleOrderCode;
        }
        public bool AreAnyStringsNullOrEmpty(PaymentResponse response)
        {
            return string.IsNullOrEmpty(response.Status) ||
                   string.IsNullOrEmpty(response.Code) ||
                   string.IsNullOrEmpty(response.Id) ||
                   string.IsNullOrEmpty(response.OrderCode);
        }
        private double PercentageChange(int current, int previous, out bool isIncrease)
        {
            if (previous == 0)
            {
                isIncrease = current > 0;
                return current == 0 ? 0 : 100;
            }

            double change = ((double)(current - previous) / previous) * 100;
            isIncrease = change >= 0;
            return change;
        }
        public bool CheckValidOfRentalDate(DateTime startDate, DateTime endDate, DateTime receivedDate)
        {
            
            if (startDate > endDate)
            {
                return false; // Ngày bắt đầu thuê không thể lớn hơn ngày kết thúc
            }
            if (receivedDate > startDate || receivedDate > endDate)
            {
                return false; // Ngày nhận sản phẩm phải trước hoặc bằng ngày bắt đầu thuê, và không được sau ngày kết thúc
            }

            return true;
        }
        public string GenerateJwtForStrings(Dictionary<string, string> claims)
        {         
            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                var symmetricKey = Encoding.UTF8.GetBytes(_configuration.GetSection("ServiceConfiguration:JwtSettings:Secret").Value);

                var Subject = new List<Claim>();
                if (claims != null && claims.Count > 0)
                {
                    foreach (var claim in claims)
                    {
                        Subject.Add(new Claim(claim.Key, claim.Value));
                    }
                }

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(Subject),
                    Expires = DateTime.UtcNow.AddMinutes(5),
                    SigningCredentials = new SigningCredentials
                    (new SymmetricSecurityKey(symmetricKey), SecurityAlgorithms.HmacSha256Signature)
                };

                var token = tokenHandler.CreateToken(tokenDescriptor);
                string result = tokenHandler.WriteToken(token);

                return result;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public ClaimsPrincipal GetPrincipalFromToken(string token)
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

        public DateTime GetTimeInUtcPlus7()
        {
            TimeSpan utcOffset = TimeSpan.FromHours(7);
            TimeZoneInfo utcPlus7TimeZone = TimeZoneInfo.CreateCustomTimeZone("UTC+7", utcOffset, "UTC+7", "UTC+7");

            // Lấy thời gian UTC hiện tại và chuyển đổi
            DateTime utcNow = DateTime.UtcNow;
            return TimeZoneInfo.ConvertTimeFromUtc(utcNow, utcPlus7TimeZone);
        }

        public int GetTimeSpan(DateTime d1, DateTime d2)
        {
            TimeSpan t = d1 - d2;
            return t.Days;
        }
    }
}
