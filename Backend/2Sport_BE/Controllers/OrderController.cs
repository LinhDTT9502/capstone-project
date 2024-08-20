using _2Sport_BE.DataContent;
using _2Sport_BE.Infrastructure.Services;
using _2Sport_BE.Repository.Models;
using _2Sport_BE.Service.Enums;
using _2Sport_BE.Service.Services;
using _2Sport_BE.ViewModels;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Security.Claims;

namespace _2Sport_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : Controller
    {

        private readonly IOrderService _orderService;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly IProductService _productService;
        public OrderController(IOrderService orderService, IMapper mapper, IUserService userService, IProductService productService)
        {
            _orderService = orderService;
            _mapper = mapper;
            _userService = userService;
            _productService = productService;
        }

        // GET: api/Orders
        [HttpGet]
        [Route("get-all-orders")]
        public async Task<IActionResult> GetOrders()
        {
            var orders = await _orderService.GetOrdersAsync();
            var ordersInfo = new List<OrderResponse>();

            foreach (var order in orders)
            {
                if (order != null)
                {
                    
                    var user = await _userService.FindAsync((int)order.UserId);
                    if (user != null)
                    {
                        var orderDetails = order.OrderDetails ?? new List<OrderDetail>();
                        OrderResponse orderInfo = new OrderResponse()
                        {
                            Id = order.Id,
                            CreateDate = order.ReceivedDate.HasValue ? order.ReceivedDate.Value.ToString("MM/dd/yyyy") : null,
                            Amount = order.IntoMoney.ToString(),
                            CustomerName = order.User.FullName,
                            OrderCode = order.OrderCode,
                            Status = Enum.GetName(typeof(OrderStatus), order.Status)?.Replace('_', ' '),
                            OrderDetails = (await Task.WhenAll(orderDetails.Select(async od => new OrderDetailResponse
                            {
                                ProductName = (await _productService.GetProductById((int)od.ProductId)).ProductName,
                                Quantity = od.Quantity,
                                TotalPrice = ((decimal)od.Quantity * (decimal)od.Price).ToString()
                            }))).ToList()
                        };
                        ordersInfo.Add(orderInfo);
                    }
                }
            }
            return Ok(ordersInfo);
        }

        [HttpGet]
        [Route("get-orders-sales-by-status")]
        public async Task<IActionResult> GetSalesOrdersByStatus(int month,int status)
        {
            try
            {
                List<Order> orders = await _orderService.GetOrdersByMonthAndStatus(month, status);
                List<Order> ordersLastMonth = await _orderService.GetOrdersByMonthAndStatus(month - 1, status);
                decimal totalRevenueInMonth = (decimal)orders.Sum(_ => _.IntoMoney);
                int ordersInMonth = orders.Count();
                int ordersInLastMonth = ordersLastMonth.Count();
                bool isIncrease;
                double orderGrowthRatio = PercentageChange(ordersInMonth, ordersInLastMonth, out isIncrease);

                OrdersSales ordersSales = new OrdersSales
                {
                    TotalOrders = ordersInMonth,
                    TotalIntoMoney = totalRevenueInMonth,
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
        }
        [HttpGet]
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
        }
        [HttpGet]
        [Route("history-orders")]
        public async Task<IActionResult> GetHistoryOrders()
        {
            int userId = GetCurrentUserIdFromToken();
            ResponseModel<List<Order>> response = new ResponseModel<List<Order>>();
            if (userId == 0 || userId.ToString() == string.Empty) {
                response.IsSuccess = false;
                response.Message = "You don't have permission";
                return BadRequest(response);
            }
            var orders = await  _orderService.ListAllOrderByUseIdAsync(userId);
            if (orders.Count() > 0)
            {
                response.IsSuccess = true;
                response.Message = "Query successfully!";
                response.Data = orders.ToList();

                return Ok(response);
            }
            else
            {
                response.IsSuccess = true;
                response.Message = "Orders are not found!";
                response.Data = null;
                return NotFound(response);
            }
        }

        // PUT: api/Orders/5
        [HttpPost("create-order")]
        public async Task<IActionResult> PostOrder([FromBody] OrderCM orderCM)
        {
            if (orderCM == null)
            {
                return BadRequest();
            }
            int userId =  GetCurrentUserIdFromToken();
            User user = await _userService.FindAsync(userId);
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
        }
        // PUT: api/Orders/5
        [HttpPut("update-order-status/{id}")]
        public async Task<IActionResult> ChangeOrderStatus(int id, int status)
        {
            if (id == null || status == null)
            {
                return BadRequest("Requird parameters!");
            }

            var result = await _orderService.UpdateOrderAsync(id, status);

            if (!result)
            {
                return NotFound("Change status is false!");
            }
            Order order = await _orderService.GetOrderByIdAsync(id);
            ResponseModel<Order> orderModel = new ResponseModel<Order>()
            {
                IsSuccess = true,
                Message = "Update successfully",
                Data = order
            };
            return StatusCode(200, orderModel);
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
        [NonAction]
        private double PercentageChange(int current, int previous, out bool isIncrease)
        {
            if (previous == 0)
            {
                isIncrease = current > 0; // Trả về true nếu có tăng trưởng, ngược lại là false
                return current == 0 ? 0 : 100;
            }

            double change = ((double)(current - previous) / previous) * 100;
            isIncrease = change >= 0; // Trả về true nếu có tăng trưởng, ngược lại là false
            return change;
        }

    }
}
