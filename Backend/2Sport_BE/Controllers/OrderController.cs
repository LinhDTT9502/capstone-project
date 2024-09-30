using _2Sport_BE.DataContent;
using _2Sport_BE.Infrastructure.Services;
using _2Sport_BE.Repository.Models;
using _2Sport_BE.Service.DTOs;
using _2Sport_BE.Service.Enums;
using _2Sport_BE.Service.Services;
using _2Sport_BE.ViewModels;
using AutoMapper;
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
        public OrderController(IOrderService orderService, ICartService cartService)
        {
            _orderService = orderService;
            _cartService = cartService;
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
        [HttpPost("checkout-for-customer")]
        public async Task<IActionResult> HandleCheckoutForCustomer([FromBody] OrderCM orderCM, [FromQuery] int paymentMethodId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid request data.");
            }
            var userId = GetCurrentUserIdFromToken();
            if(userId != orderCM.userID)
            {
                return Unauthorized();
            }

            var cart = await _cartService.GetCartByUserId(orderCM.userID);
            if (cart == null || !cart.CartItems.Any())
            {
                return NotFound("Your Cart is empty");
            }

            var response = await _orderService.ProcessCheckoutOrder(orderCM);
            if (response.IsSuccess)
            {

                return Ok(response);
            }
            return BadRequest(response);
        }
        /* [HttpPost("create-order")]
         public async Task<IActionResult> PostOrder([FromBody] OrderCM orderCM)
         {
             if (orderCM == null)
             {
                 return BadRequest();
             }
             int userId =  GetCurrentUserIdFromToken();
             User user = await _userService.GetUserById(userId);
             var order = _mapper.Map<OrderCM, Order>(orderCM);
             order.UserId = userId;
             order.User = user;
             var result = await _orderService.AddOrderAsync(order);

             if (result == null)
             {
                 return NotFound();
             }
             var orderVm = _mapper.Map<Order, OrderVM>(result);
             return Ok(orderVm);
         }*/
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
    }
}
