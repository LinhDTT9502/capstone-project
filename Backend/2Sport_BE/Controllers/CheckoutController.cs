using _2Sport_BE.DataContent;
using _2Sport_BE.Infrastructure.DTOs;
using _2Sport_BE.Infrastructure.Enums;
using _2Sport_BE.Infrastructure.Helpers;
using _2Sport_BE.Infrastructure.Services;
using _2Sport_BE.Repository.Models;
using _2Sport_BE.Services;
using MailKit.Search;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace _2Sport_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CheckoutController : ControllerBase
    {
        private readonly IPayOsService _payOsService;
        private readonly IMethodHelper _methodHelper;
        private readonly ISaleOrderService _saleOrderService;
        private readonly PaymentFactory _paymentFactory;
        public CheckoutController(IPayOsService payOsService,
            ISaleOrderService saleOrderService,
            IPaymentService paymentService,
            IMethodHelper methodHelper,
            PaymentFactory paymentFactory)
        {
            _payOsService = payOsService;
            _methodHelper = methodHelper;
            _saleOrderService = saleOrderService;
            _paymentFactory = paymentFactory;
        }

        [HttpPost]
        [Route("checkout-by-payOs")]
        public async Task<IActionResult> CheckoutByPayOs(CheckoutModel checkoutModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (checkoutModel.UserId != GetCurrentUserIdFromToken())
            {
                return Unauthorized();
            }

            if (checkoutModel.PaymentMethodID != (int)OrderMethods.PayOS)
            {
                return BadRequest("Invalid PaymentMethodId. It must match PayOS payment method.");
            }

            var order = await _saleOrderService.GetSaleOrderById(checkoutModel.OrderID)
                         ?? await _saleOrderService.GetSaleOrderByCode(checkoutModel.OrderCode);

            if (order == null)
            {
                return BadRequest("Order not found.");
            }
            if(order.PaymentStatus != (int)PaymentStatus.IsWating || (order.PaymentStatus == (int)PaymentStatus.IsCanceled && order.OrderStatus == (int)OrderStatus.CANCELLED))
            {
                return BadRequest("PaymentStatus is not allowed to checkout.");
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

            var createdLink = await paymentService.ProcessPayment(order.Id);

            if (createdLink.IsSuccess)
            {
                response.Data.PaymentLink = createdLink.Data;
                return Ok(response);
            }
            return BadRequest(createdLink);
        }
        [HttpGet("cancel")]
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
        [HttpGet("return")]
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
        [NonAction]
        protected int GetCurrentUserIdFromToken()
        {
            int UserId = 0;
            try
            {
                if (HttpContext.User.Identity.IsAuthenticated)
                {
                    var identity = HttpContext.User.Identity as ClaimsIdentity;
                    if (identity != null)
                    {
                        IEnumerable<Claim> claims = identity.Claims;
                        string strUserId = identity.FindFirst("UserId").Value;
                        int.TryParse(strUserId, out UserId);

                    }
                }
                return UserId;
            }
            catch
            {
                return UserId;
            }
        }
    }
}
