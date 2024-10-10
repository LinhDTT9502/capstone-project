using _2Sport_BE.Infrastructure.Services;
using _2Sport_BE.Repository.Implements;
using _2Sport_BE.Repository.Interfaces;
using _2Sport_BE.Repository.Models;
using _2Sport_BE.Service.Services;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;
using Net.payOS;
using Net.payOS.Types;

namespace _2Sport_BE.Service.Services
{
    public class PayOSSettings
    {
        public string ClientId { get; set; }
        public string ApiKey { get; set; }
        public string ChecksumKey { get; set; }
    }
    public interface IPaymentService
    {
        //PAYOS
        Task<string> PaymentWithPayOs(int orderId);
        //VNPay
        Task PaymentWithVnPay(int orderId);
        Task<PaymentLinkInformation> CancelPaymentLink(int orderId, string reason);
        Task<PaymentLinkInformation> GetPaymentLinkInformationAsync(int orderId);
    }
    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private PayOS _payOs;
        private PayOSSettings payOSSettings;
        public PaymentService(IUnitOfWork unitOfWork, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            payOSSettings = new PayOSSettings()
            {
                ClientId = _configuration["PayOSSettings:ClientId"],
                ApiKey = _configuration["PayOSSettings:ApiKey"],
                ChecksumKey = _configuration["PayOSSettings:ChecksumKey"]
            };
            _payOs = new PayOS(payOSSettings.ClientId, payOSSettings.ApiKey, payOSSettings.ChecksumKey);
        }

        public async Task<string> PaymentWithPayOs(int orderId)
        {
            var order = (await _unitOfWork.OrderRepository
                                .GetAsync(o => o.Id == orderId, "OrderDetails")).FirstOrDefault();
            if(order != null)
            {
                List<ItemData> orders = new List<ItemData>();
                var listOrderDetail = order.OrderDetails.ToList();

                var user = await _unitOfWork.UserRepository.GetObjectAsync(u => u.Id == order.UserId);

                for(int i = 0; i < listOrderDetail.Count; i++)
                {   var product = await _unitOfWork.ProductRepository
                                        .GetObjectAsync(p => p.Id == listOrderDetail[i].ProductId);

                    ItemData item = new ItemData(product.ProductName,
                                                (int)(listOrderDetail[i].Quantity),
                                                Convert.ToInt32(listOrderDetail[i].Price.ToString()));
                    orders.Add(item);
                }
                if (order.TranSportFee > 0)
                {
                    ItemData item = new ItemData("Chi phi van chuyen", 1,(int) order.TranSportFee);
                    orders.Add(item);
                }
                string content = $"Thanh toan hoa don {order.OrderCode}";
                int expiredAt = (int)(DateTimeOffset.UtcNow.ToUnixTimeSeconds() + (60 * 5));
                PaymentData data = new PaymentData(Int32.Parse(order.OrderCode), Int32.Parse(order.IntoMoney.ToString()), content, orders,
                    "https://twosportapiv2.azurewebsites.net/api/Payment/cancel", "https://twosportapiv2.azurewebsites.net/api/Payment/return",
                    null, user.FullName, user.Email, user.Phone, "", expiredAt);
                var createPayment = await _payOs.createPaymentLink(data);
                return createPayment.checkoutUrl;
            }
            return String.Empty;
        }

        public Task PaymentWithVnPay(int orderId)
        {
            throw new NotImplementedException();
        }

        public async Task<PaymentLinkInformation> CancelPaymentLink(int orderId, string reason)
        {
            PaymentLinkInformation cancelledPaymentLinkInfo = await _payOs.cancelPaymentLink(orderId, reason);
            return cancelledPaymentLinkInfo;
        }
        public async Task<PaymentLinkInformation> GetPaymentLinkInformationAsync(int orderId)
        {

            PaymentLinkInformation paymentLinkInformation = await _payOs.getPaymentLinkInformation(orderId);

            return paymentLinkInformation;
        }
    }
}
