using _2Sport_BE.DataContent;
using _2Sport_BE.Infrastructure.DTOs;
using _2Sport_BE.Infrastructure.Enums;
using _2Sport_BE.Infrastructure.Hubs;
using _2Sport_BE.Infrastructure.Services;
using _2Sport_BE.Service.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace _2Sport_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RentalOrderController : ControllerBase
    {
        private readonly IRentalOrderService _rentalOrderServices;
        private readonly ICartItemService _cartItemService;
        public RentalOrderController(IRentalOrderService rentalOrderServices,
            IPaymentService paymentService,
            ICartItemService cartItemService
            )
        {
            _rentalOrderServices = rentalOrderServices;
            _cartItemService = cartItemService;
        }

        [HttpGet]
        [Route("get-all-rental-orders")]
        public async Task<IActionResult> ListAllRentalOrders()
        {
            var response = await _rentalOrderServices.GetAllRentalOrderAsync();
            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
        [HttpGet]
        [Route("get-rental-order-detail")]
        public async Task<IActionResult> GetRentalOrderDetailsById(int orderId)
        {
            var response = await _rentalOrderServices.GetRentalOrderDetailsByIdAsync(orderId);
            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
        [HttpGet]
        [Route("get-rental-order-by-user")]
        public async Task<IActionResult> GetOrdersByUserId(int userId)
        {
            var response = await _rentalOrderServices.GetOrdersByUserIdAsync(userId);
            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
        [HttpGet]
        [Route("get-rental-by-parent-code")]
        public async Task<IActionResult> GetOrderByParentCode(string parentCode)
        {
            var response = await _rentalOrderServices.GetRentalOrderByParentCodeAsync(parentCode);
            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
        [HttpGet]
        [Route("get-rental-orders-by-status")]
        public async Task<IActionResult> GetOrdersByStatus(int? orderStatus, int? paymentStatus)
        {
            var response = await _rentalOrderServices.GetRentalOrdersByStatusAsync(orderStatus, paymentStatus);
            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
        [HttpGet]
        [Route("get-rental-order-by-orderCode")]
        public async Task<IActionResult> GetOrdersByOrderCode(string orderCode)
        {
            var response = await _rentalOrderServices.GetRentalOrderByOrderCodeAsync(orderCode);
            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
        [HttpGet]
        [Route("get-orders-by-branch")]
        public async Task<IActionResult> GetOrdersByBranchId(int branchId)
        {
            var response = await _rentalOrderServices.GetRentalOrdersByBranchAsync(branchId);
            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
        /*        [HttpGet]
                [Route("get-orders-by-date-and-status")]
                public async Task<IActionResult> GetOrdersByOrderCode(DateTime startDate, DateTime endDate, int status)
                {
                    var response = await _orderService.GetOrdersByMonthAndStatus(startDate, endDate, status);
                    if (response.IsSuccess)
                    {
                        return Ok(response);
                    }
                    return BadRequest(response);
                }*/
        [HttpPost("create")]
        public async Task<IActionResult> AddRentalOrder([FromBody] RentalOrderCM rentalOrderCM)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid request data.");
            }
            var response = await _rentalOrderServices.CreateRentalOrderAsync(rentalOrderCM);
            if (!response.IsSuccess)
            {
                return StatusCode(500, response);
            }
            foreach (var item in rentalOrderCM.ProductInformations)
            {
                if (item.CartItemId.HasValue && item.CartItemId.Value != Guid.Empty)
                { 
                
                    await _cartItemService.DeleteCartItem(item.CartItemId.Value);
                }
            }
            return Ok(response);
        }
        [HttpPut("request-extend")]
        public async Task<IActionResult> RequestExtendRentalPeriod([FromBody] ExtendRentalModel extendRentalModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid request data.");
            }
            var response = await _rentalOrderServices.ProcessExtendRentalPeriod(extendRentalModel);
            if (!response.IsSuccess)
            {
                return StatusCode(500, response);
            }

            return Ok(response);
        }
        [HttpPut("update")]
        public async Task<IActionResult> EditRentalOrder([FromQuery] int orderId, [FromBody] RentalOrderUM orderUM)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid request data.");
            }
            var response = await _rentalOrderServices.UpdateRentalOrderAsync(orderId, orderUM);
            if (!response.IsSuccess)
            {
                return StatusCode(500, response);
            }
            return Ok(response);
        }
        [HttpPut("return")]
        public async Task<IActionResult> ProcessReturn([FromBody] ParentOrderReturnModel returnData)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid request data.");
            }
            var response = await _rentalOrderServices.ReturnOrder(returnData);
            if (!response.IsSuccess)
            {
                return StatusCode(500, response);
            }
            return Ok(response);
        }

        [HttpPut("update-rental-order-status")]
        public async Task<IActionResult> EditRentalOrderStatus(int orderId, int status)
        {
            var response = await _rentalOrderServices.UpdateRentalOrderStatusAsync(orderId, status);

            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
        /*        [HttpGet]
                [Route("revenue-summary")]
                public async Task<IActionResult> GetSalesOrdersByStatus([FromQuery] DateTime? startDate,
                                                                        [FromQuery] DateTime? endDate,
                                                                        [FromQuery] int? status)
                {
                    try
                    {
                        var query = await _orderService.GetAllSaleOrderQueryableAsync();
                        if (startDate.HasValue)
                        {
                            query = query.Where(o => o.CreatedAt >= startDate.Value);
                        }
                        if (endDate.HasValue)
                        {
                            query = query.Where(o => o.CreatedAt <= endDate.Value);
                        }

                        // Lọc theo trạng thái đơn hàng
                        if (status.HasValue)
                        {
                            query = query.Where(o => o.OrderStatus == status.Value);
                        }

                        var totalRevenue = await query.SumAsync(o => o.TotalAmount);
                        var totalOrders = await query.CountAsync();

                        decimal totalRevenueInMonth = (decimal)query.Sum(_ => _.TotalAmount);
                        int ordersInMonth = query.Count();
                        int ordersInLastMonth = 1;
                        bool isIncrease;
                        double orderGrowthRatio = PercentageChange(ordersInMonth, ordersInLastMonth, out isIncrease);

                        SaleOrdersSales ordersSales = new SaleOrdersSales
                        {
                            SaleOrderGrowthRatio = totalOrders,
                            TotalIntoMoney = totalRevenue,
                            TotalSaleOrders = 0,
                            IsIncrease = true
                        };

                        ResponseModel<SaleOrdersSales> response = new ResponseModel<SaleOrdersSales>
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

        [HttpPut]
        [Route("assign-branch")]
        public async Task<IActionResult> AssignBranch(int orderId, int branchId)
        {
            var response = await _rentalOrderServices.UpdateBranchForRentalOrder(orderId, branchId);

            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return BadRequest(response);

        }
        [HttpPost]
        [Route("{orderId}/approve")]
        public async Task<IActionResult> ApproveRentalOrder(int orderId)
        {
            var response = await _rentalOrderServices.ApproveRentalOrderAsync(orderId);
            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
        [HttpPost]
        [Route("{orderId}/reject")]
        public async Task<IActionResult> RejectRentalOrder(int orderId)
        {
            var response = await _rentalOrderServices.RejectRentalOrderAsync(orderId);
            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
        [HttpDelete]
        [Route("remove/{orderId}")]
        public async Task<IActionResult> RemoveSaleOrder(int orderId)
        {
            var response = await _rentalOrderServices.DeleteRentalOrderAsync(orderId);
            if (response.IsSuccess) return Ok(response);
            return BadRequest(response);
        }

    }
}
