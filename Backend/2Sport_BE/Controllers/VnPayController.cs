/*using _2Sport_BE.DataContent;
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

            var order = await _saleOrderService.GetSaleOrderById(checkoutModel.OrderID)
                         ?? await _saleOrderService.GetSaleOrderByCode(checkoutModel.OrderCode);

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

            if (order.PaymentMethodId != (int)OrderMethods.PayOS)
            {
                order.PaymentMethodId = (int)OrderMethods.PayOS;
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

            var createdLink = await paymentService.ProcessSaleOrderPayment(order.Id);

            if (createdLink.IsSuccess)
            {
                response.Data.PaymentLink = createdLink.Data;
                return Ok(response);
            }
            return BadRequest(createdLink);
        }
        [HttpGet("sale-order-cancel")]
        public async Task<IActionResult> HandleSaleOrderCancelPayOs([FromQuery] PaymentResponse paymentResponse)
        {
            if (!ModelState.IsValid || _methodHelper.AreAnyStringsNullOrEmpty(paymentResponse))
            {
                return BadRequest(new ResponseModel<object>
                {
                    IsSuccess = false,
                    Message = "Invalid request data.",
                    Data = null
                });
            }

            var result = await _payOsService.ProcessCancelledSaleOrder(paymentResponse);
            if (result.IsSuccess)
            {
                var redirectUrl = "https://twosport.vercel.app/order_cancel";
                return Redirect(redirectUrl);
            }
            return BadRequest(result);
        }
        [HttpGet("sale-order-return")]
        public async Task<IActionResult> HandleSaleOrderReturnPayOs([FromQuery] PaymentResponse paymentResponse)
        {
            if (!ModelState.IsValid || _methodHelper.AreAnyStringsNullOrEmpty(paymentResponse))
            {
                return BadRequest(new ResponseModel<object>
                {
                    IsSuccess = false,
                    Message = "Invalid request data.",
                    Data = null
                });
            }

            var result = await _payOsService.ProcessCompletedSaleOrder(paymentResponse);
            if (result.IsSuccess)
            {
                var redirectUrl = "https://twosport.vercel.app/order_success";
                return Redirect(redirectUrl);
            }
            return BadRequest(result);
        }
        [HttpPost]
        [Route("checkout-rental-order")]
        public async Task<IActionResult> CheckoutRentalOrderPayOs(CheckoutModel checkoutModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (checkoutModel.PaymentMethodID != (int)OrderMethods.PayOS)
            {
                return BadRequest("Invalid PaymentMethodId. It must match PayOS payment method.");
            }

            var order = await _rentalOrderService.GetRentalOrderById(checkoutModel.OrderID)
                         ?? await _rentalOrderService.GetRentalOrderByOrderCode(checkoutModel.OrderCode);

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
                return BadRequest("PaymentStatus is not allowed to checkout because Your Order is chosing Ship COD");

            }

            if (order.PaymentMethodId != (int)OrderMethods.PayOS)
            {
                order.PaymentMethodId = (int)OrderMethods.PayOS;
                await _rentalOrderService.UpdaterRentalOrder(order);
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

            var createdLink = await paymentService.ProcessRentalOrderPayment(order.Id);

            if (createdLink.IsSuccess)
            {
                response.Data.PaymentLink = createdLink.Data;
                return Ok(response);
            }
            return BadRequest(createdLink);
        }
        [HttpGet("rental-order-cancel")]
        public async Task<IActionResult> HandleRentalOrderCancelPayOs([FromQuery] PaymentResponse paymentResponse)
        {
            if (!ModelState.IsValid || _methodHelper.AreAnyStringsNullOrEmpty(paymentResponse))
            {
                return BadRequest(new ResponseModel<object>
                {
                    IsSuccess = false,
                    Message = "Invalid request data.",
                    Data = null
                });
            }

            var result = await _payOsService.ProcessCancelledRentalOrder(paymentResponse);
            if (result.IsSuccess)
            {
                var redirectUrl = "https://twosport.vercel.app/order_cancel";
                return Redirect(redirectUrl);
            }
            return BadRequest(result);
        }
        [HttpGet("rental-order-return")]
        public async Task<IActionResult> HandleRentalOrderReturnPayOs([FromQuery] PaymentResponse paymentResponse)
        {
            if (!ModelState.IsValid || _methodHelper.AreAnyStringsNullOrEmpty(paymentResponse))
            {
                return BadRequest(new ResponseModel<object>
                {
                    IsSuccess = false,
                    Message = "Invalid request data.",
                    Data = null
                });
            }

            var result = await _payOsService.ProcessCompletedRentalOrder(paymentResponse);
            if (result.IsSuccess)
            {
                var redirectUrl = "https://twosport.vercel.app/order_success";
                return Redirect(redirectUrl);
            }
            return BadRequest(result);
        }
    }
}
*/