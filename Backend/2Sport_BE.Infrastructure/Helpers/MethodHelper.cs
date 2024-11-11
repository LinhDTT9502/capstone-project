using _2Sport_BE.Infrastructure.DTOs;
using System.Security.Cryptography;
using System.Text;

namespace _2Sport_BE.Infrastructure.Helpers
{
    public interface IMethodHelper
    {
        string GenerateOrderCode();
        string GenerateOTPCode();

        bool AreAnyStringsNullOrEmpty(PaymentResponse response);
        bool CheckValidOfRentalDate(DateTime? from, DateTime? to);
        string HashPassword(string password);
    }
    public class MethodHelper : IMethodHelper
    {
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
        public bool CheckValidOfRentalDate(DateTime? from, DateTime? to)
        {
            if (from == null || to == null) return false;

            if (from.Value.Date < DateTime.Now.Date || to.Value.Date < DateTime.Now.Date) return false;
            if (from.Value.Date >= to.Value.Date) return false;

            return true;
        }
    }
}
