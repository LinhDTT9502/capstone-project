using _2Sport_BE.Infrastructure.DTOs;

namespace _2Sport_BE.Infrastructure.Helpers
{
    public interface IMethodHelper
    {
        string GenerateOrderCode();
        bool AreAnyStringsNullOrEmpty(PaymentResponse response);
    }
    public class MethodHelper : IMethodHelper
    {
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
    }
}
