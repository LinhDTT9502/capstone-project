using _2Sport_BE.Infrastructure.DTOs;
using _2Sport_BE.Infrastructure.Enums;
using _2Sport_BE.Repository.Interfaces;
using Microsoft.Extensions.Configuration;
using Net.payOS;
using Net.payOS.Types;

namespace _2Sport_BE.Infrastructure.Services
{
    public class PayOSSettings
    {
        public string ClientId { get; set; }
        public string ApiKey { get; set; }
        public string ChecksumKey { get; set; }
    }
    public interface IPaymentService
    {
        Task<ResponseDTO<string>> ProcessPayment(int orderId);
    }
    public interface IPayOsService
    {
        //Task<string> PaymenRentalOrdertWithPayOs(int orderId);
        Task<PaymentLinkInformation> CancelPaymentLink(int orderId, string reason);
        Task<PaymentLinkInformation> GetPaymentLinkInformationAsync(int orderId);
        #region Process_PayOS_Response
        Task<ResponseDTO<int>> ProcessCancelledSaleOrder(PaymentResponse paymentResponse);
        Task<ResponseDTO<int>> ProcessCompletedSaleOrder(PaymentResponse paymentResponse);
        #endregion
    }
    public class PayOsPaymentService : IPaymentService, IPayOsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private PayOS _payOs;
        private PayOSSettings payOSSettings;
        public PayOsPaymentService(IUnitOfWork unitOfWork, IConfiguration configuration)
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
        public async Task<ResponseDTO<string>> ProcessPayment(int orderId)
        {
            var response = new ResponseDTO<string>();
            try
            {
                var order = await _unitOfWork.SaleOrderRepository
                                .GetObjectAsync(o => o.Id == orderId, new string[] { "OrderDetails" });
                if (order is null)
                {
                    response.IsSuccess = false;
                    response.Message = "Không tìm thấy đơn hàng với mã ID được cung cấp.";
                    return response;
                }

                List<ItemData> orders = new List<ItemData>();
                var listOrderDetail = order.OrderDetails.ToList();

                for (int i = 0; i < listOrderDetail.Count; i++)
                {
                    var product = await _unitOfWork.ProductRepository
                                            .GetObjectAsync(p => p.Id == listOrderDetail[i].ProductId);
                    if (product is null)
                    {
                        response.IsSuccess = false;
                        response.Message = $"Không tìm thấy sản phẩm với mã ID: {listOrderDetail[i].ProductId}";
                        return response;
                    }
                    ItemData item = new ItemData(product.ProductName,
                                                (int)(listOrderDetail[i].Quantity),
                                                Convert.ToInt32(listOrderDetail[i].UnitPrice.ToString()));
                    orders.Add(item);
                }
                if (order.TranSportFee > 0)
                {
                    ItemData item = new ItemData("Chi phi van chuyen", 1, (int)order.TranSportFee);
                    orders.Add(item);
                }
                string content = $"Mã đơn hàng: {order.OrderCode}";
                int expiredAt = (int)(DateTimeOffset.UtcNow.ToUnixTimeSeconds() + (60 * 5));
                PaymentData data = new PaymentData(Convert.ToInt64(order.OrderCode),
                        Int32.Parse(order.TotalAmount.ToString()),
                        content, orders,
                        "https://localhost:7276/api/Checkout/cancel",
                        "https://localhost:7276/api/Checkout/return",
                        null, order.FullName,
                        order.Email, order.ContactPhone,
                        order.Address,
                        expiredAt);
                var createPayment = await _payOs.createPaymentLink(data);

                response.IsSuccess = true;
                response.Message = "Mã dao dịch tạo thành công";
                response.Data = createPayment.checkoutUrl;

                return response;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
                return response;
            }
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
        public async Task<string> PaymentWithPayOsForRental(int orderId)
        {
            try
            {
                /*var order = await _unitOfWork.RentalOrderRepository
                                .GetObjectAsync(o => o.Id == orderId, new string[] { "OrderDetails" });
                if (order != null)
                {
                    List<ItemData> orders = new List<ItemData>();
                    var listOrderDetail = order.OrderDetails.ToList();

                    for (int i = 0; i < listOrderDetail.Count; i++)
                    {
                        var product = await _unitOfWork.ProductRepository
                                            .GetObjectAsync(p => p.Id == listOrderDetail[i].ProductId);

                        ItemData item = new ItemData(product.ProductName,
                                                    (int)(listOrderDetail[i].Quantity),
                                                    Convert.ToInt32(listOrderDetail[i].Price.ToString()));
                        orders.Add(item);
                    }
                    if (order.TranSportFee > 0)
                    {
                        ItemData item = new ItemData("Chi phi van chuyen", 1, (int)order.TranSportFee);
                        orders.Add(item);
                    }
                    string content = $"Hoa don {order.OrderCode}";
                    int expiredAt = (int)(DateTimeOffset.UtcNow.ToUnixTimeSeconds() + (60 * 5));
                    PaymentData data = new PaymentData(Convert.ToInt64(order.OrderCode), Int32.Parse(order.IntoMoney.ToString()), content, orders,
                        "https://twosportapiv2.azurewebsites.net/api/Payment/cancel", "https://twosportapiv2.azurewebsites.net/api/Payment/return",
                        null, +.FullName, guest.Email, guest.PhoneNumber, guest.Address, expiredAt);
                    var createPayment = await _payOs.createPaymentLink(data);
                    return createPayment.checkoutUrl;
                }*/
                return "Somethings is wrong when creating payOs link";
            }
            catch (Exception ex)
            {

                return ex.Message;
            }
        }
        #region Process_PayOs_Response
        public async Task<ResponseDTO<int>> ProcessCancelledSaleOrder(PaymentResponse paymentResponse)
        {
            var response = new ResponseDTO<int>();
            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    var saleOrder = await _unitOfWork.SaleOrderRepository.GetObjectAsync(o => o.OrderCode == paymentResponse.OrderCode);
                    if (saleOrder == null)
                    {
                        response.IsSuccess = false;
                        response.Message = $"SaleOrder with code {paymentResponse.OrderCode} is not found!";
                        response.Data = 0;
                        return response;
                    }
                    saleOrder.OrderStatus = (int)OrderStatus.CANCELLED;
                    saleOrder.PaymentStatus = (int)PaymentStatus.IsCanceled;

                    await _unitOfWork.SaleOrderRepository.UpdateAsync(saleOrder);

                    //Cập nhật lại số luọng sản phẩm của chi nhánh đó thông qua update lại warehouse
                    var SaleOrderDetails = await _unitOfWork.OrderDetailRepository
                                                        .GetAsync(od => od.SaleOrderId == saleOrder.Id);

                    foreach (var item in SaleOrderDetails)
                    {
                        var productInWarehouse = await _unitOfWork.WarehouseRepository
                            .GetObjectAsync(wh => wh.ProductId == item.ProductId && wh.BranchId == item.BranchId);
                        if (productInWarehouse != null)
                        {
                            productInWarehouse.AvailableQuantity += item.Quantity;
                            await _unitOfWork.WarehouseRepository.UpdateAsync(productInWarehouse);
                        }
                    }

                    await transaction.CommitAsync();

                    response.IsSuccess = true;
                    response.Message = $"Cancelled SaleOrder with code {paymentResponse.OrderCode}";
                    response.Data = 1;
                    return response;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    response.IsSuccess = false;
                    response.Message = ex.Message;
                    response.Data = 0;
                    return response;
                }
            }
        }
        public async Task<ResponseDTO<int>> ProcessCompletedSaleOrder(PaymentResponse paymentResponse)
        {
            var response = new ResponseDTO<int>();
            try
            {
                var SaleOrder = await _unitOfWork.SaleOrderRepository.GetObjectAsync(o => o.OrderCode == paymentResponse.OrderCode);
                if (SaleOrder == null)
                {
                    response.IsSuccess = false;
                    response.Message = $"SaleOrder with code {paymentResponse.OrderCode} is not found!";
                    response.Data = 0;
                    return response;
                }
                // Cập nhật trạng thái SaleOrder thành "Completed"
                SaleOrder.PaymentStatus = (int)PaymentStatus.IsPaid;
                await _unitOfWork.SaleOrderRepository.UpdateAsync(SaleOrder);

                response.IsSuccess = true;
                response.Message = $"Completed SaleOrder with code {paymentResponse.OrderCode}";
                response.Data = 1;
                return response;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
                response.Data = 0;
                return response;
            }
        }
        #endregion
    }
    public class PaypalPaymentService : IPaymentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        public PaypalPaymentService(IUnitOfWork unitOfWork, IConfiguration configuration)
        {
        }

        public Task<ResponseDTO<string>> ProcessPayment(int orderId)
        {
            throw new NotImplementedException();
        }
    }
}
