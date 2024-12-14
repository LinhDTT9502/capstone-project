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
        public ApiTestController(IPaymentService paymentService,
            IPayOsService payOsService,
            IVnPayService vnPayService,
            IUnitOfWork unitOfWork,
            ISaleOrderService saleOrderService,
            IRentalOrderService rentalOrderService)
        {
            _paymentService = paymentService;
            _payOsService = payOsService;
            _vnPayService = vnPayService;
            _unitOfWork = unitOfWork;
            _saleOrderService = saleOrderService;
            _rentalOrderService = rentalOrderService;
        }
        [HttpGet]
        [Route("Get-Payment-Information")]
        public async Task<IActionResult> GetPaymentInformation(string orderCode, int orderType)
        {
            if (orderType == 1)//sale
            {
                var query = await  _saleOrderService.GetSaleOrderBySaleOrderCode(orderCode);
                var order = query.Data;
                if(order == null) { return NotFound(); }
                if (order.PaymentMethodId == (int)OrderMethods.PayOS)
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
                            BankName = response.Data.transactions.FirstOrDefault() !=  null ? response.Data.transactions.FirstOrDefault().counterAccountBankName : "UNKNOWN"
                        };
                        return Ok(paymentInfo);
                    }
                    return NotFound();
                }
                else if (order.PaymentMethodId == (int)OrderMethods.VnPay)
                {
                    DateTime dateUtc = DateTime.SpecifyKind(DateTime.Parse(order.CreatedAt.ToString()), DateTimeKind.Utc);
                    var response = _vnPayService.QueryTransaction(order.SaleOrderCode, order.TransactionId, dateUtc, HttpContext);

                    if (response != null)
                    {
                        return Ok(response);
                    }
                    return BadRequest();
                }
            }
            else if (orderType == 2)
            {
                var query = await _rentalOrderService.GetRentalOrderByOrderCodeAsync(orderCode);
                var order = query.Data;
                if(order == null) return NotFound();

                if (order.PaymentMethodId == (int)OrderMethods.PayOS)
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
                            BankName = response.Data.transactions.FirstOrDefault() != null ? response.Data.transactions.FirstOrDefault().counterAccountBankName : "UNKNOWN"
                        };
                        return Ok(paymentInfo);
                    }
                    return NotFound();
                }
                else if (order.PaymentMethodId == (int)OrderMethods.VnPay)
                {
                    DateTime dateUtc = DateTime.SpecifyKind(DateTime.Parse(order.CreatedAt.ToString()), DateTimeKind.Utc);
                    var response = _vnPayService.QueryTransaction(order.RentalOrderCode, order.TransactionId, dateUtc, HttpContext);

                    if (response != null)
                    {
                        return Ok(response);
                    }
                    return BadRequest();
                }
            }
            return BadRequest();
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
                    BankName = response.Data.transactions.FirstOrDefault().counterAccountBankName ?? "UNKNOWN"
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
            var response = _vnPayService.QueryTransaction(orderCode, transactionId, dateUtc, HttpContext);
           
            return Ok(response);
        }

    }
}
