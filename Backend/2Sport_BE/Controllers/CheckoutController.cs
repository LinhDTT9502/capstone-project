using _2Sport_BE.DataContent;
using _2Sport_BE.Infrastructure.DTOs;
using _2Sport_BE.Infrastructure.Enums;
using _2Sport_BE.Infrastructure.Helpers;
using _2Sport_BE.Infrastructure.Services;
using _2Sport_BE.Repository.Models;
using _2Sport_BE.Service.Enums;
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
        private readonly IVnPayService _vnPayService;
        private readonly IMethodHelper _methodHelper;
        private readonly ISaleOrderService _saleOrderService;
        private readonly IRentalOrderService _rentalOrderService;
        private readonly PaymentFactory _paymentFactory;
        public CheckoutController(IPayOsService payOsService,
            ISaleOrderService saleOrderService,
            IRentalOrderService rentalOrderService,
            IPaymentService paymentService,
            IMethodHelper methodHelper,
            PaymentFactory paymentFactory,
            IVnPayService vnPayService)
        {
            _payOsService = payOsService;
            _methodHelper = methodHelper;
            _saleOrderService = saleOrderService;
            _paymentFactory = paymentFactory;
            _rentalOrderService = rentalOrderService;
            _vnPayService = vnPayService;
        }

        [HttpPost]
        [Route("checkout-sale-order")]
        public async Task<IActionResult> CheckoutSaleOrder(CheckoutModel checkoutModel)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);


            var order = await _saleOrderService.FindSaleOrderById(checkoutModel.OrderID)
                         ?? await _saleOrderService.FindSaleOrderByCode(checkoutModel.OrderCode);
            if (order == null) return BadRequest("Order not found.");

            /*if(order.PaymentStatus != (int)PaymentStatus.IsWating || (order.PaymentStatus == (int)PaymentStatus.IsCanceled && order.OrderStatus == (int)OrderStatus.CANCELLED))
            {
                return BadRequest("PaymentStatus is not allowed to checkout.");
            }*/

            if (checkoutModel.PaymentMethodID == (int)OrderMethods.PayOS || checkoutModel.PaymentMethodID == (int)OrderMethods.VnPay)
            {
                var paymentService = _paymentFactory.GetPaymentService(checkoutModel.PaymentMethodID);
                if (paymentService == null) return BadRequest("Phương thức thanh toán không hợp lệ.");

                var createdLink = await paymentService.ProcessSaleOrderPayment(order.Id, HttpContext);
                if (!createdLink.IsSuccess) return BadRequest(createdLink);


                if (order.PaymentMethodId != checkoutModel.PaymentMethodID)
                {
                    order.PaymentMethodId = checkoutModel.PaymentMethodID;
                    order.PaymentStatus = (int)PaymentStatus.PENDING;
                    await _saleOrderService.UpdateSaleOrder(order);
                }

                var response = await _saleOrderService.GetSaleOrderDetailsByIdAsync(order.Id);
                if (response.IsSuccess)
                {
                    response.Data.PaymentLink = createdLink.Data;
                    return Ok(response);
                }
                return BadRequest(response);
            }
            else if (checkoutModel.PaymentMethodID == (int)OrderMethods.COD || checkoutModel.PaymentMethodID == (int)OrderMethods.BankTransfer)
            {

                if (order.PaymentMethodId != checkoutModel.PaymentMethodID)
                {
                    order.PaymentMethodId = checkoutModel.PaymentMethodID;
                    order.PaymentStatus = (int)PaymentStatus.PENDING;
                    await _saleOrderService.UpdateSaleOrder(order);
                }
                var response = await _saleOrderService.GetSaleOrderDetailsByIdAsync(order.Id);

                if (!response.IsSuccess) return Ok(response);
                return BadRequest(response);

            }
            return BadRequest("Unsupported payment method.");
        }
        [HttpGet("sale-order-cancel-payos")]
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
            if (result.IsSuccess) return Redirect("https://twosportshop.vercel.app/payment-cancel");
            return Redirect("https://twosportshop.vercel.app/payment-error");
        }
        [HttpGet("sale-order-return-payos")]
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

            if (result.IsSuccess) return Redirect("https://twosportshop.vercel.app/payment-success");
            return Redirect("https://twosportshop.vercel.app/payment-error");
        }
        [HttpPost]
        [Route("checkout-rental-order")]
        public async Task<IActionResult> CheckoutRentalOrder(CheckoutModel checkoutModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var order = await _rentalOrderService.FindRentalOrderById(checkoutModel.OrderID)
                         ?? await _rentalOrderService.FindRentalOrderByOrderCode(checkoutModel.OrderCode);

            if (order == null)
            {
                return BadRequest("Rental Order not found.");
            }
            /*if (order.PaymentStatus != (int)PaymentStatus.IsWating || (order.PaymentStatus == (int)PaymentStatus.IsCanceled && order.OrderStatus == (int)OrderStatus.CANCELLED))
            {
                return BadRequest("PaymentStatus is not allowed to checkout.");
            }*/

            if (checkoutModel.PaymentMethodID == (int)OrderMethods.PayOS || checkoutModel.PaymentMethodID == (int)OrderMethods.VnPay)
            {

                var paymentService = _paymentFactory.GetPaymentService(checkoutModel.PaymentMethodID);
                if (paymentService == null)
                {
                    return BadRequest("Phương thức thanh toán không hợp lệ.");
                }
                var createdLink = new ResponseDTO<string>();
                if (!string.IsNullOrEmpty(checkoutModel.TransactionType) && checkoutModel.TransactionType == "DEPOSIT_50")
                {
                    order.DepositStatus = (int)DepositStatus.PARTIALLY_PENDING;
                    createdLink = await paymentService.ProcessRentalOrderPayment(order.Id, HttpContext, true);
                }
                else if(checkoutModel.TransactionType == "DEPOSIT_100")
                {
                    createdLink = await paymentService.ProcessRentalOrderPayment(order.Id, HttpContext, false);
                    order.DepositStatus = (int)DepositStatus.PENDING;
                }
                else
                {
                    return BadRequest("Loại giao dịch không hợp lệ.");
                }

                if (!createdLink.IsSuccess) return BadRequest(createdLink);


                if (order.PaymentMethodId != checkoutModel.PaymentMethodID)
                {
                    order.PaymentMethodId = checkoutModel.PaymentMethodID;
                    order.PaymentStatus = (int)PaymentStatus.PENDING;
                    await _rentalOrderService.UpdaterRentalOrder(order);
                }

                var response = await _rentalOrderService.GetRentalOrderDetailsByIdAsync(order.Id);

                if (response.IsSuccess)
                {
                    response.Data.PaymentLink = createdLink.Data;
                    return Ok(response);
                }
                return BadRequest(response);
            }
            else if (checkoutModel.PaymentMethodID == (int)OrderMethods.COD || checkoutModel.PaymentMethodID == (int)OrderMethods.BankTransfer)
            {

                if (order.PaymentMethodId != checkoutModel.PaymentMethodID)
                {
                    order.DepositStatus = (int)DepositStatus.NOT_PAID;
                    order.PaymentStatus = (int)PaymentStatus.PENDING;
                    order.PaymentMethodId = checkoutModel.PaymentMethodID;
                    await _rentalOrderService.UpdaterRentalOrder(order);
                }

                var response = await _rentalOrderService.GetRentalOrderDetailsByIdAsync(order.Id);
                 
                if (response.IsSuccess) return Ok(response);
                return BadRequest(response);
            }
            return BadRequest("Unsupported payment method.");
        }
        [HttpGet("rental-order-cancel-payos")]
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

            if (result.IsSuccess) return Redirect("https://twosportshop.vercel.app/payment-cancel");
            return Redirect("https://twosportshop.vercel.app/payment-error");
        }
        [HttpGet("rental-order-return-payos")]
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

            if (result.IsSuccess) return Redirect("https://twosportshop.vercel.app/payment-success");
            return  Redirect("https://twosportshop.vercel.app/payment-error");
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
        [HttpGet("sale-order-return-vnpay")]
        public async Task<IActionResult> HandleSaleOrderReturnVnPay()
        {
            var result = await _vnPayService.PaymentSaleOrderExecute(Request.Query);

            if (result.IsSuccess)
            {
                if(result.Message == "Completed") return Redirect("https://twosportshop.vercel.app/payment-success");
                return Redirect("https://twosportshop.vercel.app/payment-cancel");
            }   
            return Redirect("https://twosportshop.vercel.app/payment-error");
        }
        [HttpGet("rental-order-return-vnpay")]
        public async Task<IActionResult> HandleRentalOrderReturnVnPay()
        {
            var result = await _vnPayService.PaymentRentalOrderExecute(Request.Query);

            if (result.IsSuccess)
            {
                if (result.Message == "Completed") return Redirect("https://twosportshop.vercel.app/payment-success");
                return Redirect("https://twosportshop.vercel.app/payment-cancel");
            }
            return Redirect("https://twosportshop.vercel.app/payment-error");
        }
    }
}
