using _2Sport_BE.Infrastructure.DTOs;
using _2Sport_BE.Infrastructure.Enums;
using _2Sport_BE.Infrastructure.Helpers;
using _2Sport_BE.Repository.Interfaces;
using _2Sport_BE.Service.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Net.payOS;
using Net.payOS.Types;
using Newtonsoft.Json;
using System.Net;
using System.Text;

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
        Task<ResponseDTO<string>> ProcessSaleOrderPayment(int orderId, HttpContext context);
        Task<ResponseDTO<string>> ProcessRentalOrderPayment(int orderId, HttpContext context);

    }
    public interface IPayOsService
    {
        #region Process_PayOS_Response
        Task<ResponseDTO<int>> ProcessCancelledSaleOrder(PaymentResponse paymentResponse);
        Task<ResponseDTO<int>> ProcessCompletedSaleOrder(PaymentResponse paymentResponse);
        Task<ResponseDTO<int>> ProcessCancelledRentalOrder(PaymentResponse paymentResponse);
        Task<ResponseDTO<int>> ProcessCompletedRentalOrder(PaymentResponse paymentResponse);
        #endregion
        Task<ResponseDTO<PaymentLinkInformation>> GetPaymentLinkInformation(string orderCode);

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
        public async Task<ResponseDTO<string>> ProcessSaleOrderPayment(int orderId, HttpContext context)
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
                string content = $"Mã đơn hàng: {order.SaleOrderCode}";
                int expiredAt = (int)(DateTimeOffset.UtcNow.ToUnixTimeSeconds() + (60 * 5));

                var returnStr = _configuration["PayOSSettings:ReturnUrlSaleOrder"];
                var cancelStr = _configuration["PayOSSettings:CancelUrlSaleOrder"];

                PaymentData data = new PaymentData(Convert.ToInt64(order.SaleOrderCode),
                        Int32.Parse(order.TotalAmount.ToString()),
                        content, orders,
                        cancelStr,
                        returnStr,
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
                response.Message = "Mã dao dịch tạo thất bại";
                response.Data = ex.Message;
                return response;
            }
        }
        public async Task<ResponseDTO<string>> ProcessRentalOrderPayment(int orderId, HttpContext context)
        {
            var response = new ResponseDTO<string>();
            try
            {
                var order = await _unitOfWork.RentalOrderRepository
                                .GetObjectAsync(o => o.Id == orderId);
                if (order is null)
                {
                    response.IsSuccess = false;
                    response.Message = "Không tìm thấy đơn hàng với mã ID được cung cấp.";
                    return response;
                }

                List<ItemData> orders = new List<ItemData>();
                var listOrderDetail = (await _unitOfWork.RentalOrderRepository.GetAsync(o => o.ParentOrderCode.Equals(order.RentalOrderCode))).ToList();

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
                    ItemData item = new ItemData($"Sản phẩm thuê: {product.ProductName}",
                                                (int)(listOrderDetail[i].Quantity),
                                                Convert.ToInt32(listOrderDetail[i].RentPrice.ToString()));
                    orders.Add(item);
                }
                if (order.TranSportFee > 0)
                {
                    ItemData item = new ItemData("Chi phi van chuyen", 1, (int)order.TranSportFee);
                    orders.Add(item);
                }
                string content = $"Hoa Don Thue: {order.RentalOrderCode}";
                int expiredAt = (int)(DateTimeOffset.UtcNow.ToUnixTimeSeconds() + (60 * 5));

                var returnStr = _configuration["PayOSSettings:ReturnUrlRentalOrder"];
                var cancelStr = _configuration["PayOSSettings:CancelUrlRentalOrder"];

                PaymentData data = new PaymentData(Convert.ToInt64(order.RentalOrderCode),
                        Int32.Parse(order.TotalAmount.ToString()),
                        content, orders,
                        cancelStr,
                        returnStr,
                        null, order.FullName,
                        order.Email, order.ContactPhone,
                        order.Address,
                        expiredAt);
                var createPayment = await _payOs.createPaymentLink(data);

                response.IsSuccess = true;
                response.Message = "Mã dao dịch tạo thành công";
                response.Data = createPayment.checkoutUrl;


            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "Mã dao dịch tạo thất bại";
                response.Data = ex.Message;
            }
            return response;
        }
        #region Process_PayOs_Response
        public async Task<ResponseDTO<int>> ProcessCancelledSaleOrder(PaymentResponse paymentResponse)
        {
            var response = new ResponseDTO<int>();
            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    var saleOrder = await _unitOfWork.SaleOrderRepository.GetObjectAsync(o => o.SaleOrderCode == paymentResponse.OrderCode);
                    if (saleOrder == null)
                    {
                        response.IsSuccess = false;
                        response.Message = $"SaleOrder with code {paymentResponse.OrderCode} is not found!";
                        response.Data = 0;
                        return response;
                    }
                    //SaleOrder.OrderStatus = (int)OrderStatus.CANCELLED;
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
                var SaleOrder = await _unitOfWork.SaleOrderRepository.GetObjectAsync(o => o.SaleOrderCode == paymentResponse.OrderCode);
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
        public async Task<ResponseDTO<int>> ProcessCancelledRentalOrder(PaymentResponse paymentResponse)
        {
            var response = new ResponseDTO<int>();
            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    var saleOrder = await _unitOfWork.RentalOrderRepository.GetObjectAsync(o => o.RentalOrderCode == paymentResponse.OrderCode);
                    if (saleOrder == null)
                    {
                        response.IsSuccess = false;
                        response.Message = $"Rental Order with code {paymentResponse.OrderCode} is not found!";
                        response.Data = 0;
                        return response;
                    }
                    saleOrder.OrderStatus = (int)OrderStatus.CANCELLED;
                    saleOrder.PaymentStatus = (int)PaymentStatus.IsCanceled;

                    await _unitOfWork.RentalOrderRepository.UpdateAsync(saleOrder);

                    /* //Cập nhật lại số luọng sản phẩm của chi nhánh đó thông qua update lại warehouse
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
                     }*/

                    await transaction.CommitAsync();

                    response.IsSuccess = true;
                    response.Message = $"Cancelled Rental Oder with code {paymentResponse.OrderCode} Succesfully";
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
        public async Task<ResponseDTO<int>> ProcessCompletedRentalOrder(PaymentResponse paymentResponse)
        {
            var response = new ResponseDTO<int>();
            try
            {
                var SaleOrder = await _unitOfWork.RentalOrderRepository.GetObjectAsync(o => o.RentalOrderCode == paymentResponse.OrderCode);
                if (SaleOrder == null)
                {
                    response.IsSuccess = false;
                    response.Message = $"Rental Order with code {paymentResponse.OrderCode} is not found!";
                    response.Data = 0;
                    return response;
                }
                // Cập nhật trạng thái SaleOrder thành "Completed"
                SaleOrder.PaymentStatus = (int)PaymentStatus.IsPaid;
                await _unitOfWork.RentalOrderRepository.UpdateAsync(SaleOrder);

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

        public async Task<ResponseDTO<PaymentLinkInformation>> GetPaymentLinkInformation(string orderCode)
        {
            var response = new ResponseDTO<PaymentLinkInformation>();
            try
            {
                PaymentLinkInformation paymentLinkInformation = await _payOs.getPaymentLinkInformation(long.Parse(orderCode));

                response.IsSuccess = true;
                response.Message = $"Query succesffully";
                response.Data = paymentLinkInformation;
                return response;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
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

        public Task<ResponseDTO<string>> ProcessRentalOrderPayment(int orderId, HttpContext context)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseDTO<string>> ProcessSaleOrderPayment(int orderId, HttpContext context)
        {
            throw new NotImplementedException();
        }
    }
    public interface IVnPayService
    {
        Task<ResponseDTO<PaymentResponseModel>> PaymentRentalOrderExecute(IQueryCollection collections);
        Task<ResponseDTO<PaymentResponseModel>> PaymentSaleOrderExecute(IQueryCollection collections);

        public string QueryTransaction(string orderCode, DateTime payDate, HttpContext context);
    }
    public class VnPayPaymentService : IPaymentService, IVnPayService
    {
        public class QueryRequest
        {
            public string OrderId { get; set; }
            public string PayDate { get; set; }
        }

        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly ISaleOrderService _saleOrderService;
        private readonly IRentalOrderService _rentalOrderService;
        private readonly IMethodHelper _methodHelper;
        public VnPayPaymentService(IUnitOfWork unitOfWork,
            IConfiguration configuration,
            ISaleOrderService saleOrderService,
            IRentalOrderService rentalOrderService,
            IMethodHelper methodHelper)
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _saleOrderService = saleOrderService;
            _rentalOrderService = rentalOrderService;
            _methodHelper = methodHelper;
        }

        public async Task<ResponseDTO<string>> ProcessSaleOrderPayment(int orderId, HttpContext context)
        {
            var model = await _unitOfWork.SaleOrderRepository.GetObjectAsync(o => o.Id == orderId, new string[] {"OrderDetails"});
            var timeZoneById = TimeZoneInfo.FindSystemTimeZoneById(_configuration["TimeZoneId"]);
            var timeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneById);
            var tick = DateTime.Now.Ticks.ToString();
            var pay = new VnPayLibrary();
            var urlCallBack = _configuration["PaymentCallBack:ReturnUrlSaleOrder"];

            pay.AddRequestData("vnp_Version", _configuration["Vnpay:Version"]);
            pay.AddRequestData("vnp_Command", _configuration["Vnpay:Command"]);
            pay.AddRequestData("vnp_TmnCode", _configuration["Vnpay:TmnCode"]);
            pay.AddRequestData("vnp_Amount", ((int)model.TotalAmount * 100).ToString());
            pay.AddRequestData("vnp_CreateDate", timeNow.ToString("yyyyMMddHHmmss"));
            pay.AddRequestData("vnp_CurrCode", _configuration["Vnpay:CurrCode"]);
            pay.AddRequestData("vnp_IpAddr", pay.GetIpAddress(context));
            pay.AddRequestData("vnp_Locale", _configuration["Vnpay:Locale"]);
            pay.AddRequestData("vnp_OrderInfo", $"Thanh toan don hang mua: {model.SaleOrderCode}");
            pay.AddRequestData("vnp_OrderType", "sale order");
            pay.AddRequestData("vnp_ReturnUrl", urlCallBack);
            pay.AddRequestData("vnp_TxnRef", tick);

            var paymentUrl =
                pay.CreateRequestUrl(_configuration["Vnpay:BaseUrl"], _configuration["Vnpay:HashSecret"]);

            return new ResponseDTO<string>()
            {
                Data = paymentUrl,
                IsSuccess = true,
                Message = "Generate payment string"
            };
        }
        public async Task<ResponseDTO<string>> ProcessRentalOrderPayment(int orderId, HttpContext context)
        {
            var model = await _unitOfWork.RentalOrderRepository.GetObjectAsync(o => o.Id == orderId);
            var timeZoneById = TimeZoneInfo.FindSystemTimeZoneById(_configuration["TimeZoneId"]);
            var timeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneById);
            var tick = DateTime.Now.Ticks.ToString();
            var pay = new VnPayLibrary();
            var urlCallBack = _configuration["PaymentCallBack:ReturnUrlRentalOrder"];

            pay.AddRequestData("vnp_Version", _configuration["Vnpay:Version"]);
            pay.AddRequestData("vnp_Command", _configuration["Vnpay:Command"]);
            pay.AddRequestData("vnp_TmnCode", _configuration["Vnpay:TmnCode"]);
            pay.AddRequestData("vnp_Amount", ((int)model.TotalAmount/2 * 100).ToString());
            pay.AddRequestData("vnp_CreateDate", timeNow.ToString("yyyyMMddHHmmss"));
            pay.AddRequestData("vnp_CurrCode", _configuration["Vnpay:CurrCode"]);
            pay.AddRequestData("vnp_IpAddr", pay.GetIpAddress(context));
            pay.AddRequestData("vnp_Locale", _configuration["Vnpay:Locale"]);
            pay.AddRequestData("vnp_OrderInfo", $"Thanh toan don hang thue:" + model.RentalOrderCode);
            pay.AddRequestData("vnp_OrderType", "Rental Order");
            pay.AddRequestData("vnp_ReturnUrl", urlCallBack);
            pay.AddRequestData("vnp_TxnRef", tick);

            var paymentUrl =
                pay.CreateRequestUrl(_configuration["Vnpay:BaseUrl"], _configuration["Vnpay:HashSecret"]);

            return new ResponseDTO<string>()
            {
                Data = paymentUrl,
                IsSuccess = true,
                Message = "Generate payment string"
            };
        }
        public async Task<ResponseDTO<PaymentResponseModel>> PaymentSaleOrderExecute(IQueryCollection collections)
        {
            var pay = new VnPayLibrary();
            var response = pay.GetFullResponseData(collections, _configuration["Vnpay:HashSecret"]);
            var isUpdated = false;
            if (response.IsSuccess)
            {
                if (response.Data.VnPayResponseCode == "00")
                {
                    isUpdated = await _saleOrderService.UpdatePaymentStatusOfSaleOrder(response.Data.OrderCode, (int)PaymentStatus.IsPaid);
                }
                isUpdated = await _saleOrderService.UpdatePaymentStatusOfSaleOrder(response.Data.OrderCode, (int)PaymentStatus.IsCanceled);
            }
            if (isUpdated)
            {
                return response;
            }
            return new ResponseDTO<PaymentResponseModel>()
            {
                IsSuccess = false,
                Message = "Update Payment Status Failed"
            };
        }
        public async Task<ResponseDTO<PaymentResponseModel>> PaymentRentalOrderExecute(IQueryCollection collections)
        {
            var pay = new VnPayLibrary();
            var response = pay.GetFullResponseData(collections, _configuration["Vnpay:HashSecret"]);
            var isUpdated = false;
            if (response.IsSuccess)
            {
                if(response.Data.VnPayResponseCode == "00")
                {
                isUpdated = await _rentalOrderService.UpdateRentalOrderAfterCheckout(response.Data.OrderCode,(int)DepositStatus.Partially_Paid, response.Data.Amount, (int)PaymentStatus.IsDeposited);
                }
                isUpdated = await _rentalOrderService.UpdateRentalOrderAfterCheckout(response.Data.OrderCode, (int)DepositStatus.Not_Paid, decimal.Zero, (int)PaymentStatus.IsCanceled);
            }
            if (isUpdated)
            {
                return response;
            }
            return new ResponseDTO<PaymentResponseModel>()
            {
                IsSuccess = false,
                Message = "Update Payment Status Failed"
            };
        }
        public string QueryTransaction(string orderCode, DateTime payDate, HttpContext context)
        {
            var pay = new VnPayLibrary();
            var vnp_Api = _configuration["Vnpay:vnp_Api"];

            var vnp_HashSecret = _configuration["Vnpay:HashSecret"];

            var vnp_RequestId = DateTime.Now.Ticks.ToString();
            var vnp_Version = _configuration["Vnpay:Version"]; 
            var vnp_Command = "querydr";
            var vnp_TmnCode = _configuration["Vnpay:TmnCode"];
            var vnp_TxnRef = orderCode;
            var vnp_OrderInfo = $"Thanh toan don hang mua: {orderCode}";
            var vnp_TransactionDate = _methodHelper.GetFormattedDateInGMT7(payDate);
            var vnp_CreateDate = DateTime.Now.ToString("yyyyMMddHHmmss");
            var vnp_IpAddr = pay.GetIpAddress(context);

            var signData = $"{vnp_RequestId}|{vnp_Version}|{vnp_Command}|{vnp_TmnCode}|{vnp_TxnRef}|{vnp_TransactionDate}|{vnp_CreateDate}|{vnp_IpAddr}|{vnp_OrderInfo}";
            var vnp_SecureHash = pay.HmacSha512(vnp_HashSecret, signData);

            var qdrData = new
            {
                vnp_RequestId,
                vnp_Version,
                vnp_Command,
                vnp_TmnCode,
                vnp_TxnRef,
                vnp_OrderInfo,
                vnp_TransactionDate,
                vnp_CreateDate,
                vnp_IpAddr,
                vnp_SecureHash
            };

            var jsonData = JsonConvert.SerializeObject(qdrData);

            var httpWebRequest = (HttpWebRequest)WebRequest.Create(vnp_Api);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                streamWriter.Write(jsonData);
            }

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                return streamReader.ReadToEnd();
            }
        }
      
    }
}
