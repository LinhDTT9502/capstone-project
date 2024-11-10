using _2Sport_BE.DataContent;
using _2Sport_BE.Infrastructure.DTOs;
using _2Sport_BE.Infrastructure.Enums;
using _2Sport_BE.Infrastructure.Helpers;
using _2Sport_BE.Infrastructure.Services;
using _2Sport_BE.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace _2Sport_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VnPayController : ControllerBase
    {

        private readonly IVnPayService _vnPayService;
        private readonly IMethodHelper _methodHelper;
        private readonly ISaleOrderService _saleOrderService;
        private readonly IRentalOrderService _rentalOrderService;
        private readonly PaymentFactory _paymentFactory;
        public VnPayController(IVnPayService vnPayService,
            ISaleOrderService saleOrderService,
            IRentalOrderService rentalOrderService,
            IPaymentService paymentService,
            IMethodHelper methodHelper,
            PaymentFactory paymentFactory)
        {
            _vnPayService = vnPayService;
            _methodHelper = methodHelper;
            _saleOrderService = saleOrderService;
            _paymentFactory = paymentFactory;
            _rentalOrderService = rentalOrderService;
        }
        [HttpPost]
        [Route("checkout-sale-order")]
        public async Task<IActionResult> CheckoutSaleOrderVNPay(CheckoutModel checkoutModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (checkoutModel.PaymentMethodID != (int)OrderMethods.VnPay)
            {
                return BadRequest("Invalid PaymentMethodId. It must match PayOS payment method.");
            }

            var order = await _saleOrderService.FindSaleOrderById(checkoutModel.OrderID)
                         ?? await _saleOrderService.FindSaleOrderByCode(checkoutModel.OrderCode);

            if (order == null)
            {
                return BadRequest("Order not found.");
            }
            if (order.PaymentStatus != (int)PaymentStatus.IsWating || (order.PaymentStatus == (int)PaymentStatus.IsCanceled && order.OrderStatus == (int)OrderStatus.CANCELLED))
            {
                return BadRequest("PaymentStatus is not allowed to checkout.");
            }
            if (order.PaymentMethodId == (int)OrderMethods.COD)
            {
                return BadRequest("PaymentStatus is not allowed to checkout because Your Order is choosing Ship COD");

            }

            if (order.PaymentMethodId != (int)OrderMethods.VnPay)
            {
                order.PaymentMethodId = (int)OrderMethods.VnPay;
                await _saleOrderService.UpdateSaleOrder(order);
            }

            var response = await _saleOrderService.GetSaleOrderDetailByIdAsync(order.Id);
            if (!response.IsSuccess)
            {
                return BadRequest(response);
            }
            var paymentService = _paymentFactory.GetPaymentService(checkoutModel.PaymentMethodID);

            if (paymentService == null)
            {
                return BadRequest("Phương thức thanh toán không hợp lệ.");
            }

            var createdLink = await paymentService.ProcessSaleOrderPayment(order.Id, HttpContext);

            if (createdLink.IsSuccess)
            {
                response.Data.PaymentLink = createdLink.Data;
                return Ok(response);
            }
            return BadRequest(createdLink);
        }
        [HttpGet("sale-order-return")]
        public async Task<IActionResult> HandleSaleOrderReturnVnPay()
        {
            var result = await _vnPayService.PaymentSaleOrderExecute(Request.Query);
            if (result.IsSuccess)
            {
                var redirectUrl = "https://twosport.vercel.app/order_success";
                return Redirect(redirectUrl);
            }
            return Redirect("https://twosport.vercel.app/order-cancel");
        }
        [HttpPost]
        [Route("checkout-rental-order")]
        public async Task<IActionResult> CheckoutRentalOrderVNPay(CheckoutModel checkoutModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (checkoutModel.PaymentMethodID != (int)OrderMethods.VnPay)
            {
                return BadRequest("Invalid PaymentMethodId. It must match PayOS payment method.");
            }

            var order = await _rentalOrderService.FindRentalOrderById(checkoutModel.OrderID)
                         ?? await _rentalOrderService.FindRentalOrderByOrderCode(checkoutModel.OrderCode);

            if (order == null)
            {
                return BadRequest("Order not found.");
            }
            if (order.PaymentStatus != (int)PaymentStatus.IsWating || (order.PaymentStatus == (int)PaymentStatus.IsCanceled && order.OrderStatus == (int)OrderStatus.CANCELLED))
            {
                return BadRequest("PaymentStatus is not allowed to checkout.");
            }
            if (order.PaymentMethodId == (int)OrderMethods.COD)
            {
                return BadRequest("PaymentStatus is not allowed to checkout because Your Order is choosing Ship COD");

            }

            if (order.PaymentMethodId != (int)OrderMethods.VnPay)
            {
                order.PaymentMethodId = (int)OrderMethods.VnPay;
                await _rentalOrderService.UpdaterRentalOrder(order);
            }

            var response = await _rentalOrderService.GetRentalOrderByIdAsync(order.Id);
            if (!response.IsSuccess)
            {
                return BadRequest(response);
            }
            var paymentService = _paymentFactory.GetPaymentService(checkoutModel.PaymentMethodID);

            if (paymentService == null)
            {
                return BadRequest("Phương thức thanh toán không hợp lệ.");
            }

            var createdLink = await paymentService.ProcessRentalOrderPayment(order.Id, HttpContext);

            if (createdLink.IsSuccess)
            {
                response.Data.PaymentLink = createdLink.Data;
                return Ok(response);
            }
            return BadRequest(createdLink);
        }
        [HttpGet("rental-order-return")]
        public async Task<IActionResult> HandleRentalOrderReturnVnPay()
        {
            var result = await _vnPayService.PaymentRentalOrderExecute(Request.Query);
            if (result.IsSuccess)
            {
                var redirectUrl = "https://twosport.vercel.app/order_success";
                return Redirect(redirectUrl);
            }
            return Redirect("https://twosport.vercel.app/order-cancel");
        }
    }
}
