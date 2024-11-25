using _2Sport_BE.Infrastructure.DTOs;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace _2Sport_BE.Infrastructure.Helpers
{
    public interface IMethodHelper
    {
        string GenerateOrderCode();
        string GenerateOTPCode();

        bool AreAnyStringsNullOrEmpty(PaymentResponse response);
        bool CheckValidOfRentalDate(DateTime startDate, DateTime endDate, DateTime receivedDate);
        string HashPassword(string password);
        public string GetFormattedDateInGMT7(DateTime date);
    }
    public class MethodHelper : IMethodHelper
    {
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
    }
}
