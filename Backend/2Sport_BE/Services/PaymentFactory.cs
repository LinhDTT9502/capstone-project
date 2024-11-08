using _2Sport_BE.Infrastructure.Services;

namespace _2Sport_BE.Services
{
    public class PaymentFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public PaymentFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IPaymentService GetPaymentService(int paymentMethod)
        {
            return paymentMethod switch
            {
                2 => _serviceProvider.GetService<PayOsPaymentService>(),
                3 => _serviceProvider.GetService<PaypalPaymentService>(),
                4 => _serviceProvider.GetService<VnPayPaymentService>(),
                _ => throw new ArgumentException("Phương thức thanh toán không hợp lệ.")
            };
        }
    }

}
