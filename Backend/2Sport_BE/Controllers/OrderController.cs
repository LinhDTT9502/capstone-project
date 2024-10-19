using _2Sport_BE.DataContent;
using _2Sport_BE.Infrastructure.Services;
using _2Sport_BE.Repository.Models;
using _2Sport_BE.Service.DTOs;
using _2Sport_BE.Service.Enums;
using _2Sport_BE.Service.Services;
using _2Sport_BE.ViewModels;
using AutoMapper;
using MailKit.Search;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Security.Claims;
using Vonage.Users;

namespace _2Sport_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : Controller
    {

        private readonly IOrderService _orderService;
        private readonly ICartService _cartService;
        private readonly ICartItemService _cartItemService;
        private readonly IPaymentService _paymentService;
        private readonly IRentalOrderService _rentalOrderService;
        public OrderController(IOrderService orderService,
                                ICartService cartService,
                                ICartItemService cartItemService,
                                IPaymentService paymentService,
                                IRentalOrderService rentalOrderService)
        {
            _orderService = orderService;
            _cartService = cartService;
            _cartItemService = cartItemService;
            _paymentService = paymentService;
            _rentalOrderService = rentalOrderService;
        }
        [HttpGet]
        [Route("get-all-orders")]
        public async Task<IActionResult> GetOrders()
        {
            var response = await _orderService.GetAllOrdersAsync();
            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
        [HttpGet]
        [Route("get-order-by-orderId")]
        public async Task<IActionResult> GetOrderByOrderId(int orderId)
        {
            var response = await _orderService.GetOrderByIdAsync(orderId);
            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
        [HttpGet]
        [Route("get-orders-by-userId")]
        public async Task<IActionResult> GetOrdersByUserId(int userId)
        {
            var response = await _orderService.GetAllOrdersByUseIdAsync(userId);
            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
        [HttpGet]
        [Route("get-orders-by-status")]
        public async Task<IActionResult> GetOrdersByStatus(int status)
        {
            var response = await _orderService.GetOrdersByStatus(status);
            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
        [HttpGet]
        [Route("get-orders-by-orderCode")]
        public async Task<IActionResult> GetOrdersByOrderCode(string orderCode)
        {
            var response = await _orderService.GetOrderByOrderCode(orderCode);
            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
        [HttpGet]
        [Route("get-orders-by-date")]
        public async Task<IActionResult> GetOrdersByOrderCode(DateTime startDate, DateTime endDate)
        {
            var response = await _orderService.GetOrdersByMonth(startDate, endDate);
            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
        [HttpGet]
        [Route("get-orders-by-date-and-status")]
        public async Task<IActionResult> GetOrdersByOrderCode(DateTime startDate, DateTime endDate, int status)
        {
            var response = await _orderService.GetOrdersByMonthAndStatus(startDate, endDate, status);
            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
        [HttpPut("update-order-status")]
        public async Task<IActionResult> ChangeOrderStatus(int orderId, int status)
        {
            var response = await _orderService.ChangeOrderStatusAsync(orderId, status);

            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
        [HttpGet]
        [Route("revenue-summary")]
        public async Task<IActionResult> GetSalesOrdersByStatus([FromQuery] DateTime? startDate,
                                                                [FromQuery] DateTime? endDate,
                                                                [FromQuery] int? status)
        {
            try
            {
                var query = await _orderService.GetAllOrderQueryableAsync();
                if (startDate.HasValue)
                {
                    query = query.Where(o => o.CreateAt >= startDate.Value);
                }
                if (endDate.HasValue)
                {
                    query = query.Where(o => o.CreateAt <= endDate.Value);
                }

                // Lọc theo trạng thái đơn hàng
                if (status.HasValue)
                {
                    query = query.Where(o => o.Status == status.Value);
                }

                var totalRevenue = await query.SumAsync(o => o.IntoMoney);
                var totalOrders = await query.CountAsync();

                /*decimal totalRevenueInMonth = (decimal)orders.Sum(_ => _.IntoMoney);
                int ordersInMonth = orders.Count();
                int ordersInLastMonth = ordersLastMonth.Count();
                bool isIncrease;
                double orderGrowthRatio = PercentageChange(ordersInMonth, ordersInLastMonth, out isIncrease);*/

                OrdersSales ordersSales = new OrdersSales
                {
                    TotalOrders = totalOrders,
                    TotalIntoMoney = totalRevenue,
                    orderGrowthRatio = 0,
                    IsIncrease = true
                };

                ResponseModel<OrdersSales> response = new ResponseModel<OrdersSales>
                {
                    IsSuccess = true,
                    Message = "Query Successfully",
                    Data = ordersSales
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                ResponseModel<OrdersSales> response = new ResponseModel<OrdersSales>
                {
                    IsSuccess = false,
                    Message = "Something went wrong: " + ex.Message,
                    Data = null
                };
                return BadRequest(response);
            }
        }
        //==================================================================
        [HttpPost("checkout-sale-order-for-customer")]
        public async Task<IActionResult> HandleCheckoutForCustomer([FromBody] OrderCM orderCM)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid request data.");
            }
            var userId = GetCurrentUserIdFromToken();
            if (userId != orderCM.UserID || userId == 0 || orderCM.UserID == 0)
            {
                return Unauthorized();
            }

            var cart = await _cartService.GetCartByUserId(orderCM.UserID);
            if (cart == null || cart.CartItems.Count == 0)
            {
                return NotFound("Your Cart is empty");
            }

            //Tao order
            var response = await _orderService.ProcessCreatetOrder(orderCM);
            if (!response.IsSuccess)
            {
                return StatusCode(500, response);
            }

            //Xoa cart
            var isDeleteCartItem = await _cartItemService.DeleteCartItem(cart, orderCM.orderDetailCMs);
            if (!isDeleteCartItem)
            {
                return StatusCode(500, "Deleting cart item fail");
            }

            //Tao link payment
            var paymentLink = orderCM.PaymentMethodID == (int)OrderMethods.PayOS
                                  ? await _paymentService.PaymentWithPayOs(response.Data.OrderID)
                                  : "";
            if (paymentLink.Length == 0)
            {
                return BadRequest("Cannot create payment link");
            }
            response.Data.PaymentLink = paymentLink;
            return Ok(response);
        }
        [HttpPost("checkout-sale-order-for-guest")]
        public async Task<IActionResult> HandleCheckoutForGuest([FromBody] GuestOrderCM guestOrderCM, [FromQuery] int paymentMethodId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid request data.");
            }
            //Tao order
            var response = await _orderService.ProcessCreatetOrderForGuest(guestOrderCM);
            if (!response.IsSuccess)
            {
                return StatusCode(500, response);
            }
            //Tao link payment
            var paymentLink = guestOrderCM.PaymentMethodID == (int)OrderMethods.PayOS
                                  ? await _paymentService.PaymentWithPayOsForGuest(response.Data.OrderID)
                                  : "";
            if (paymentLink.Length == 0)
            {
                return BadRequest("Cannot create payment link");
            }
            response.Data.PaymentLink = paymentLink;
            return Ok(response);
        }
        [HttpPut("update-sale-order-for-customer")]
        public async Task<IActionResult> UpdateSaleOrder([FromQuery] int orderId, [FromBody] OrderUM orderUM)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid request data.");
            }
            var response = await _orderService.UpdateOrderOfCustomerAsync(orderId, orderUM);
            if (!response.IsSuccess)
            {
                return StatusCode(500, response);
            }
            return Ok(response);
        }
        [HttpPut("update-sale-order-for-guest")]
        public async Task<IActionResult> UpdateSaleOrderForGuest([FromQuery] int orderId, [FromBody] GuestOrderUM guestOrderUM)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid request data.");
            }
            var response = await _orderService.UpdateOrderOfGuestAsync(orderId, guestOrderUM);
            if (!response.IsSuccess)
            {
                return StatusCode(500, response);
            }
            return Ok(response);
        }
        //===================================================================

        [HttpPost("checkout-rental-order-for-customer")]
        public async Task<IActionResult> HandleCheckoutRentalForCustomer([FromBody] RentalOrderCM rentalOrderCM)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid request data.");
            }
            var userId = GetCurrentUserIdFromToken();
            if (userId != rentalOrderCM.UserID || userId == 0 || rentalOrderCM.UserID == 0)
            {
                return Unauthorized();
            }

            var cart = await _cartService.GetCartByUserId(rentalOrderCM.UserID);
            if (cart == null || cart.CartItems.Count == 0)
            {
                return NotFound("Your Cart is empty");
            }

            var response = await _rentalOrderService.CreateRentalOrderForCustomer(rentalOrderCM);
            if (!response.IsSuccess)
            {
                return StatusCode(500, response);
            }
            var isDeleteCartItem = await _cartItemService.DeleteCartItem(cart, rentalOrderCM.rentalOrderItems);
            if (!isDeleteCartItem)
            {
                return StatusCode(500, "Deleting cart item fail");
            }

            var paymentLink = rentalOrderCM.PaymentMethodID == (int)OrderMethods.PayOS
                                  ? await _paymentService.PaymentWithPayOs(response.Data.OrderID)
                                  : "";
            if (paymentLink.Length == 0)
            {
                return BadRequest("Cannot create payment link");
            }
            response.Data.PaymentLink = paymentLink;
            return Ok(response);
        }
        //Renting orders of Guest
        [HttpPost("checkout-rental-order-for-guest")]
        public async Task<IActionResult> HandleCheckoutRentalForGuest([FromBody] GuestRentalOrderCM guestRentalOrderCM)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid request data.");
            }

            var response = await _rentalOrderService.CreateRentalOrderForGuest(guestRentalOrderCM);
            if (!response.IsSuccess)
            {
                return StatusCode(500, response);
            }
            var paymentLink = guestRentalOrderCM.PaymentMethodID == (int)OrderMethods.PayOS
                                  ? await _paymentService.PaymentWithPayOsForGuest(response.Data.OrderID)
                                  : "";
            if (paymentLink.Length == 0)
            {
                return BadRequest("Cannot create payment link");
            }
            response.Data.PaymentLink = paymentLink;
            return Ok(response);
        }
        //Update Rental Order - Staff's function - for Customer
        [HttpPut("update-rental-order-for-customer")]
        public async Task<IActionResult> UpdateRentalOrderForCustomer([FromQuery] int orderId, [FromBody] RentalOrderUM orderUM)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid request data.");
            }
            var response = await _rentalOrderService.UpdateRentailOrderForCustomerAsync(orderId, orderUM);
            if (!response.IsSuccess)
            {
                return StatusCode(500, response);
            }
            return Ok(response);
        }
        [HttpPut("update-rental-order-for-guest")]
        public async Task<IActionResult> UpdateRentalOrderForGuest([FromQuery] int orderId, [FromBody] GuestRentalOrderUM orderUM)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid request data.");
            }
            var response = await _rentalOrderService.UpdateRentailOrderForGuestAsync(orderId, orderUM);
            if (!response.IsSuccess)
            {
                return StatusCode(500, response);
            }
            return Ok(response);
        }
        //=================================================================

        [HttpGet("cancel")]
        public async Task<IActionResult> HandleOrderCancel([FromQuery] PaymentResponse paymentResponse)
        {
            if (!ModelState.IsValid || AreAnyStringsNullOrEmpty(paymentResponse))
            {
                return BadRequest(new ResponseModel<object>
                {
                    IsSuccess = false,
                    Message = "Invalid request data.",
                    Data = null
                });
            }

            var result = await _orderService.ProcessCancelledOrder(paymentResponse);
            if (result.IsSuccess)
            {
                var redirectUrl = "https://twosport.vercel.app/order_cancel";
                return Redirect(redirectUrl);
            }
            return BadRequest(result);
        }
        [HttpGet("return")]
        public async Task<IActionResult> HandleOrderReturn([FromQuery] PaymentResponse paymentResponse)
        {
            if (!ModelState.IsValid || AreAnyStringsNullOrEmpty(paymentResponse))
            {
                return BadRequest(new ResponseModel<object>
                {
                    IsSuccess = false,
                    Message = "Invalid request data.",
                    Data = null
                });
            }

            var result = await _orderService.ProcessCompletedOrder(paymentResponse);
            if (result.IsSuccess)
            {
                var redirectUrl = "https://twosport.vercel.app/order_success";
                return Redirect(redirectUrl);
            }
            return BadRequest(result);
        }
        /*        [HttpGet]
            [Route("get-orders-sales")]
            public async Task<IActionResult> GetSalesOrders(int month)
            {
                try
                {
                    List<Order> orders = await _orderService.GetOrdersByMonth(month);
                    List<Order> ordersLastMonth = await _orderService.GetOrdersByMonth(month - 1);
                    decimal totalRevenueInMonth =(decimal) orders.Sum(_ => _.IntoMoney);
                    int ordersInMonth = orders.Count();
                    int ordersInLastMonth = ordersLastMonth.Count();
                    bool isIncrease;
                    double orderGrowthRatio = PercentageChange(ordersInMonth, ordersInLastMonth, out isIncrease);

                    OrdersSales ordersSales = new OrdersSales
                    {
                        TotalOrders = orders.Count(),
                        TotalIntoMoney = (decimal)orders.Sum(_ => _.IntoMoney),
                        orderGrowthRatio = orderGrowthRatio,
                        IsIncrease = isIncrease
                    };
                    ResponseModel<OrdersSales> response = new ResponseModel<OrdersSales>
                    {
                        IsSuccess = true,
                        Message = "Query Successfully",
                        Data = ordersSales
                    };

                    return Ok(response);
                }
                catch (Exception ex)
                {
                    ResponseModel<OrdersSales> response = new ResponseModel<OrdersSales>
                    {
                        IsSuccess = false,
                        Message = "Something went wrong: " + ex.Message,
                        Data = null
                    };
                    return BadRequest(response);
                }
            }*/
        [HttpGet]
        [Route("get-revenue")]
        public async Task<IActionResult> GetRevenue(
           [FromQuery] int? branchId,
           [FromQuery] int? orderType,
           [FromQuery] DateTime? dateFrom,
           [FromQuery] DateTime? dateTo,
           [FromQuery] int? status)
        {
            var response = await _orderService.GetOrdersRevenue(branchId, orderType, dateFrom, dateTo, status);
            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
        [NonAction]
        private double PercentageChange(int current, int previous, out bool isIncrease)
        {
            if (previous == 0)
            {
                isIncrease = current > 0;
                return current == 0 ? 0 : 100;
            }

            double change = ((double)(current - previous) / previous) * 100;
            isIncrease = change >= 0;
            return change;
        }
        [NonAction]
        private int GetCurrentUserIdFromToken()
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
        [NonAction]
        public bool AreAnyStringsNullOrEmpty(PaymentResponse response)
        {
            return string.IsNullOrEmpty(response.Status) ||
                   string.IsNullOrEmpty(response.Code) ||
                   string.IsNullOrEmpty(response.Id) ||
                   string.IsNullOrEmpty(response.OrderCode);
        }
    }
}
