using _2Sport_BE.Infrastructure.Enums;
using _2Sport_BE.Infrastructure.Services;
using _2Sport_BE.Repository.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace _2Sport_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApiTestController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly IPayOsService _payOsService;
        private readonly IVnPayService _vnPayService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISaleOrderService _saleOrderService;
        private readonly IRentalOrderService _rentalOrderService;
        private INotificationService _notificationService;
        public ApiTestController(IPaymentService paymentService,
            IPayOsService payOsService,
            IVnPayService vnPayService,
            IUnitOfWork unitOfWork,
            ISaleOrderService saleOrderService,
            IRentalOrderService rentalOrderService,
            INotificationService notificationService)
        {
            _paymentService = paymentService;
            _payOsService = payOsService;
            _vnPayService = vnPayService;
            _unitOfWork = unitOfWork;
            _saleOrderService = saleOrderService;
            _rentalOrderService = rentalOrderService;
            _notificationService = notificationService;
        }
        [HttpGet("payment-information")]
        public async Task<IActionResult> GetPaymentInformation(string orderCode, int orderType)
        {
            object queryResult;

            // Xử lý bất đồng bộ theo loại order
            if (orderType == 1)
            {
                queryResult = await _saleOrderService.GetSaleOrderBySaleOrderCode(orderCode);
            }
            else if (orderType == 2)
            {
                queryResult = await _rentalOrderService.GetRentalOrderByOrderCodeAsync(orderCode);
            }
            else
            {
                return BadRequest(new { Message = "Invalid order type." });
            }

            if (queryResult == null || ((dynamic)queryResult).Data == null)
            {
                return NotFound(new { Message = "Order not found." });
            }

            var order = ((dynamic)queryResult).Data;

            // Kiểm tra PaymentMethodId
            return order.PaymentMethodId switch
            {
                (int)OrderMethods.PayOS => await GetPayOsPaymentInfoAsync(orderCode),
                (int)OrderMethods.VnPay => GetVnPayPaymentInfo(orderCode, order.TransactionId, order.CreatedAt),
                _ => BadRequest(new { Message = "Unsupported payment method." })
            };
        }

        private async Task<object> GetOrderByTypeAsync(string orderCode, int orderType)
        {
            if (orderType == 1) // Sale Order
            {
                var query = await _saleOrderService.GetSaleOrderBySaleOrderCode(orderCode);
                return query?.Data;
            }
            else if (orderType == 2) // Rental Order
            {
                var query = await _rentalOrderService.GetRentalOrderByOrderCodeAsync(orderCode);
                return query?.Data;
            }
            return null;
        }

        private async Task<IActionResult> GetPayOsPaymentInfoAsync(string orderCode)
        {
            var response = await _payOsService.GetPaymentDetails(orderCode);

            if (!response.IsSuccess)
                return NotFound(new { Message = "Payment details not found." });

            var paymentInfo = new PaymentInfor
            {
                AmountPaid = response.Data.amountPaid.ToString(),
                OrderCode = response.Data.orderCode.ToString(),
                PaymentStatus = response.Data.status,
                PaymentDate = ParseUtcDate(response.Data.createdAt),
                BankName = response.Data.transactions.FirstOrDefault()?.counterAccountBankName ?? "UNKNOWN",
                PaymentMethod = "PayOs"
            };

            return Ok(paymentInfo);
        }

        private IActionResult GetVnPayPaymentInfo(string orderCode, string transactionId, DateTime createdAt)
        {
            var dateUtc = DateTime.SpecifyKind(createdAt, DateTimeKind.Utc);
            var response = _vnPayService.QueryTransaction(orderCode, transactionId,"VnPay", dateUtc, HttpContext);

            if (response != null)
                return Ok(response);

            return BadRequest(new { Message = "Transaction query failed." });
        }

        private string ParseUtcDate(string dateString)
        {
            if (DateTime.TryParse(dateString, out var parsedDate))
            {
                return DateTime.SpecifyKind(parsedDate, DateTimeKind.Utc).ToString("yyyy-MM-dd HH:mm:ss");
            }
            return "Invalid Date";
        }
        [HttpGet]
        [Route("GetPaymentInformationPayOs")]
        public async Task<IActionResult> GetPaymentInformationPayOs(string orderCode)
        {
            var response = await _payOsService.GetPaymentDetails(orderCode);
            if (response.IsSuccess)
            {

                var paymentInfo = new PaymentInfor()
                {
                    AmountPaid = response.Data.amountPaid.ToString(),
                    OrderCode = response.Data.orderCode.ToString(),
                    PaymentStatus = response.Data.status,
                    PaymentDate = DateTime.SpecifyKind(DateTime.Parse(response.Data.createdAt), DateTimeKind.Utc).ToString(),
                    BankName = response.Data.transactions.Count > 0 ? response.Data.transactions.FirstOrDefault().counterAccountBankName : "UNKNOWN"
                };
                return Ok(paymentInfo);
            }
            return NotFound();
        }
        [HttpGet]
        [Route("GetPaymentInformationVnPay")]

        public IActionResult GetPaymentInformation(string orderCode,string transactionId, string date)
        {
            DateTime dateUtc = DateTime.SpecifyKind(DateTime.Parse(date), DateTimeKind.Utc);
            var response = _vnPayService.QueryTransaction(orderCode, transactionId,"VnPay", dateUtc, HttpContext);
           
            return Ok(response);
        }

        [HttpGet]
        [Route("SendNotificationToUser")]
        public async Task<IActionResult> GetPaymentInformationPayOs(int userId ,string message)
        {
            _notificationService.SendNoifyToUser(userId, null, message);
            return Ok();
        }


    }
}
