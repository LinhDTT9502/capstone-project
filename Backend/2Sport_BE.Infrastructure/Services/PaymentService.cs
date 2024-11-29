using _2Sport_BE.Infrastructure.DTOs;
using _2Sport_BE.Infrastructure.Enums;
using _2Sport_BE.Infrastructure.Helpers;
using _2Sport_BE.Repository.Interfaces;
using _2Sport_BE.Repository.Models;
using _2Sport_BE.Service.Enums;
using MailKit.Search;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Net.payOS;
using Net.payOS.Types;
using Newtonsoft.Json;
using StackExchange.Redis;
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
        Task<ResponseDTO<string>> ProcessRentalOrderPayment(int orderId, HttpContext context, bool isPartiallyDeposit);

    }
    public interface IPayOsService
    {
        #region Process_PayOS_Response
        Task<ResponseDTO<SaleOrderVM>> ProcessCancelledSaleOrder(PaymentResponse paymentResponse);
        Task<ResponseDTO<SaleOrderVM>> ProcessCompletedSaleOrder(PaymentResponse paymentResponse);
        Task<ResponseDTO<RentalOrderVM>> ProcessCancelledRentalOrder(PaymentResponse paymentResponse);
        Task<ResponseDTO<RentalOrderVM>> ProcessCompletedRentalOrder(PaymentResponse paymentResponse);
        #endregion
        Task<ResponseDTO<PaymentLinkInformation>> GetPaymentLinkInformation(string orderCode);

    }
    public class PayOsPaymentService : IPaymentService, IPayOsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private PayOS _payOs;
        private PayOSSettings payOSSettings;
        private readonly SaleOrderService _saleOrderService;
        private readonly RentalOrderService _rentalOrderService;

        public PayOsPaymentService(IUnitOfWork unitOfWork,
                                    IConfiguration configuration,
                                    SaleOrderService saleOrderService,
                                    RentalOrderService rentalOrderService)
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
            _saleOrderService = saleOrderService;
            _rentalOrderService = rentalOrderService;
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
        public async Task<ResponseDTO<string>> ProcessRentalOrderPayment(int orderId, HttpContext context, bool isPartiallyDeposit)
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
                var childOrders = (await _unitOfWork.RentalOrderRepository
                                            .GetAsync(o => o.ParentOrderCode.Equals(order.RentalOrderCode))).ToList();
                if (childOrders.Any())
                {
                    foreach (var childOrder in childOrders)
                    {
                        ItemData item = new ItemData($"Sản phẩm thuê: {childOrder.ProductName}",
                                                (int)(childOrder.Quantity),
                                                Convert.ToInt32(childOrder.RentPrice.ToString()));
                        orders.Add(item);
                    }
                }
                else
                {
                    ItemData item = new ItemData($"Sản phẩm thuê: {order.ProductName}",
                                                (int)(order.Quantity),
                                                Convert.ToInt32(order.RentPrice.ToString()));
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

                int totalAmount = isPartiallyDeposit
                    ? (int)Math.Ceiling((decimal)order.TotalAmount / 2)
                    : (int)order.TotalAmount;

                PaymentData data = new PaymentData(Convert.ToInt64(order.RentalOrderCode),
                        totalAmount,
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
        public async Task<ResponseDTO<SaleOrderVM>> ProcessCancelledSaleOrder(PaymentResponse paymentResponse)
        {
            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    var saleOrder = await _unitOfWork.SaleOrderRepository.GetObjectAsync(o => o.SaleOrderCode == paymentResponse.OrderCode);
                    if (saleOrder == null)
                        return _saleOrderService.GenerateErrorResponse($"SaleOrder with code {paymentResponse.OrderCode} is not found!");

                    saleOrder.PaymentStatus = (int)PaymentStatus.IsCanceled;
                    await _unitOfWork.SaleOrderRepository.UpdateAsync(saleOrder);

                    //Cập nhật lại số luọng sản phẩm của chi nhánh đó thông qua update lại warehouse
                    /*                    var SaleOrderDetails = await _unitOfWork.OrderDetailRepository
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
*/

                    await transaction.CommitAsync();

                    return _saleOrderService.GenerateSuccessResponse(saleOrder, $"Cancelled SaleOrder with code {paymentResponse.OrderCode}");
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return _saleOrderService.GenerateErrorResponse(ex.Message);
                }
            }
        }
        public async Task<ResponseDTO<SaleOrderVM>> ProcessCompletedSaleOrder(PaymentResponse paymentResponse)
        {
            try
            {
                var saleOrder = await _unitOfWork.SaleOrderRepository.GetObjectAsync(o => o.SaleOrderCode == paymentResponse.OrderCode);
                if (saleOrder == null)
                {
                    return _saleOrderService.GenerateErrorResponse($"SaleOrder with code {paymentResponse.OrderCode} is not found!");
                }
                // Cập nhật trạng thái SaleOrder thành "Completed"
                saleOrder.PaymentStatus = (int)PaymentStatus.IsPaid;
                await _unitOfWork.SaleOrderRepository.UpdateAsync(saleOrder);
                return _saleOrderService.GenerateSuccessResponse(saleOrder, $"Completed SaleOrder with code {paymentResponse.OrderCode}");
            }
            catch (Exception ex)
            {
                return _saleOrderService.GenerateErrorResponse(ex.Message);
            }
        }
        public async Task<ResponseDTO<RentalOrderVM>> ProcessCancelledRentalOrder(PaymentResponse paymentResponse)
        {
            var response = new ResponseDTO<int>();
            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    var rentalOrder = await _unitOfWork.RentalOrderRepository.GetObjectAsync(o => o.RentalOrderCode == paymentResponse.OrderCode);
                    if (rentalOrder == null)
                        return _rentalOrderService.GenerateErrorResponse($"Rental Order with code {paymentResponse.OrderCode} is not found!");

                    rentalOrder.PaymentStatus = (int)PaymentStatus.IsCanceled;
                    rentalOrder.DepositStatus = (int)PaymentStatus.IsCanceled;
                    await _unitOfWork.RentalOrderRepository.UpdateAsync(rentalOrder);

                    /* 
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
                    */

                    await transaction.CommitAsync();

                    return _rentalOrderService.GenerateSuccessResponse(rentalOrder, null, $"Cancelled Rental Oder with code {paymentResponse.OrderCode} Succesfully");
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return _rentalOrderService.GenerateErrorResponse(ex.Message);
                }
            }
        }
        public async Task<ResponseDTO<RentalOrderVM>> ProcessCompletedRentalOrder(PaymentResponse paymentResponse)
        {
            try
            {
                var rentalOrder = await _unitOfWork.RentalOrderRepository.GetObjectAsync(o => o.RentalOrderCode == paymentResponse.OrderCode);
                if (rentalOrder == null)
                    return _rentalOrderService.GenerateErrorResponse($"Rental Order with code {paymentResponse.OrderCode} is not found!");

                // Cập nhật trạng thái SaleOrder thành "Completed"
                rentalOrder.PaymentStatus = (int)PaymentStatus.IsDeposited;
          
                if(rentalOrder.DepositStatus == (int)DepositStatus.Partially_Pending) rentalOrder.DepositStatus = (int)DepositStatus.Partially_Paid;
                else rentalOrder.DepositStatus = (int)DepositStatus.Paid;

                await _unitOfWork.RentalOrderRepository.UpdateAsync(rentalOrder);
                    
                return _rentalOrderService.GenerateSuccessResponse(rentalOrder, null, $"Completed SaleOrder with code {paymentResponse.OrderCode}");
            }
            catch (Exception ex)
            {
                return _rentalOrderService.GenerateErrorResponse(ex.Message);
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

        public Task<ResponseDTO<string>> ProcessRentalOrderPayment(int orderId, HttpContext context, bool isPartiallyDeposit)
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
        Task<ResponseDTO<RentalOrderVM>> PaymentRentalOrderExecute(IQueryCollection collections);
        Task<ResponseDTO<SaleOrderVM>> PaymentSaleOrderExecute(IQueryCollection collections);

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
        private readonly SaleOrderService saleOrderService;
        private readonly RentalOrderService rentalOrderService;
        public VnPayPaymentService(IUnitOfWork unitOfWork,
            IConfiguration configuration,
            ISaleOrderService saleOrderService,
            IRentalOrderService rentalOrderService,
            IMethodHelper methodHelper,
            RentalOrderService rentalOrderService1,
            SaleOrderService saleOrderService1)
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _saleOrderService = saleOrderService;
            _rentalOrderService = rentalOrderService;
            _methodHelper = methodHelper;
            this.rentalOrderService = rentalOrderService1;
            this.saleOrderService = saleOrderService1;
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
        public async Task<ResponseDTO<string>> ProcessRentalOrderPayment(int orderId, HttpContext context, bool isPartiallyDeposit)
        {
            var order =  _unitOfWork.RentalOrderRepository.FindObject(o => o.Id == orderId);
            if(order == null)
            {
                return new ResponseDTO<string>()
                {
                    IsSuccess = false,
                    Message = "Order is not found"
                };
            }
            var model = await _unitOfWork.RentalOrderRepository.GetObjectAsync(o => o.Id == orderId);
            var timeZoneById = TimeZoneInfo.FindSystemTimeZoneById(_configuration["TimeZoneId"]);
            var timeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneById);
            var tick = DateTime.Now.Ticks.ToString();
            var pay = new VnPayLibrary();
            var urlCallBack = _configuration["PaymentCallBack:ReturnUrlRentalOrder"];

            int totalAmount = isPartiallyDeposit
                    ? (int)Math.Ceiling((decimal)order.TotalAmount / 2)
                    : (int)order.TotalAmount;

            pay.AddRequestData("vnp_Version", _configuration["Vnpay:Version"]);
            pay.AddRequestData("vnp_Command", _configuration["Vnpay:Command"]);
            pay.AddRequestData("vnp_TmnCode", _configuration["Vnpay:TmnCode"]);
            pay.AddRequestData("vnp_Amount", (totalAmount * 100).ToString());
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
        public async Task<ResponseDTO<SaleOrderVM>> PaymentSaleOrderExecute(IQueryCollection collections)
        {
            var pay = new VnPayLibrary();
            var response = pay.GetFullResponseData(collections, _configuration["Vnpay:HashSecret"]);
            var isUpdated = false;

            var order = _unitOfWork.SaleOrderRepository.FindObject(o => o.SaleOrderCode == response.Data.OrderCode);
            if (order == null) return saleOrderService.GenerateErrorResponse("Order not found!");

            if (response.IsSuccess)
            {
                if (response.Data.VnPayResponseCode == "00")
                {
                    isUpdated = await _saleOrderService.UpdatePaymentStatusOfSaleOrder(response.Data.OrderCode, (int)PaymentStatus.IsPaid);
                }
                isUpdated = await _saleOrderService.UpdatePaymentStatusOfSaleOrder(response.Data.OrderCode, (int)PaymentStatus.IsCanceled);
            }
            if (isUpdated) return saleOrderService.GenerateSuccessResponse(order, "Payment status updated successfully.");

            return saleOrderService.GenerateErrorResponse("Failed to update payment status.");
        }
        public async Task<ResponseDTO<RentalOrderVM>> PaymentRentalOrderExecute(IQueryCollection collections)
        {
            var vnPayLib = new VnPayLibrary();
            var response = vnPayLib.GetFullResponseData(collections, _configuration["Vnpay:HashSecret"]);

            if (response == null || response.Data == null)
            {
                return rentalOrderService.GenerateErrorResponse("Invalid response data!");
            }

            var order = _unitOfWork.RentalOrderRepository.FindObject(o => o.RentalOrderCode == response.Data.OrderCode);
            if (order == null) return rentalOrderService.GenerateErrorResponse("Order not found!");


            // Determine deposit status based on the response code
            DepositStatus newDepositStatus = response.Data.VnPayResponseCode == "00"
                ? (order.DepositStatus == (int)DepositStatus.Partially_Pending ? DepositStatus.Partially_Paid : DepositStatus.Paid)
                : DepositStatus.Not_Paid;

            PaymentStatus paymentStatus = response.Data.VnPayResponseCode == "00"
                ? PaymentStatus.IsDeposited
                : PaymentStatus.IsCanceled;

            // Update rental order
            var isUpdated = await _rentalOrderService.UpdateRentalOrderAfterCheckout(
                order.RentalOrderCode,
                (int)newDepositStatus,
                response.Data.Amount,
                (int)paymentStatus
            );

            if (isUpdated) return rentalOrderService.GenerateSuccessResponse(order, null, "Payment status updated successfully.");

            return rentalOrderService.GenerateErrorResponse("Failed to update payment status.");
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
