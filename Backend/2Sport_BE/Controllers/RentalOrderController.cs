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

        [HttpGet("get-all-rental-orders")]
        public async Task<IActionResult> ListAllRentalOrders()
        {
            var response = await _rentalOrderServices.GetAllRentalOrderAsync();
            if (response.IsSuccess) return Ok(response);
            return BadRequest(response);
        }

        [HttpGet("get-rental-order-detail")]
        public async Task<IActionResult> GetRentalOrderDetailsById(int orderId)
        {
            var response = await _rentalOrderServices.GetRentalOrderDetailsByIdAsync(orderId);
            if (response.IsSuccess) return Ok(response);
            return BadRequest(response);
        }

        [HttpGet("get-rental-order-by-user")]
        public async Task<IActionResult> GetOrdersByUserId(int userId)
        {
            var response = await _rentalOrderServices.GetOrdersByUserIdAsync(userId);
            if (response.IsSuccess) return Ok(response);
            return BadRequest(response);
        }

        [HttpGet("get-rental-by-parent-code")]
        public async Task<IActionResult> GetOrderByParentCode(string parentCode)
        {
            var response = await _rentalOrderServices.GetRentalOrderByParentCodeAsync(parentCode);
            if (response.IsSuccess) return Ok(response);
            return BadRequest(response);
        }

        [HttpGet("get-rental-orders-by-status")]
        public async Task<IActionResult> GetOrdersByStatus(int? orderStatus, int? paymentStatus)
        {
            var response = await _rentalOrderServices.GetRentalOrdersByStatusAsync(orderStatus, paymentStatus);
            if (response.IsSuccess) return Ok(response);
            return BadRequest(response);
        }

        [HttpGet("get-rental-order-by-orderCode")]
        public async Task<IActionResult> GetOrdersByOrderCode(string orderCode)
        {
            var response = await _rentalOrderServices.GetRentalOrderByOrderCodeAsync(orderCode);
            if (response.IsSuccess) return Ok(response);
            return BadRequest(response);
        }

        [HttpGet("get-orders-by-branch")]
        public async Task<IActionResult> GetOrdersByBranchId(int branchId)
        {
            var response = await _rentalOrderServices.GetRentalOrdersByBranchAsync(branchId);
            if (response.IsSuccess) return Ok(response);
            return BadRequest(response);
        }

        [HttpGet("get-extension-orders/{extensionStatus}")]
        public async Task<IActionResult> GetExtensionOrders(int extensionStatus,[FromQuery]int branchId)
        {
            var response = await _rentalOrderServices.GetRentalOrdersByBranchAndExtensionStatus(branchId, extensionStatus);
            if (response.IsSuccess) return Ok(response);
            return BadRequest(response);
        }

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
            if (!ModelState.IsValid) return BadRequest("Invalid request data.");
            var response = await _rentalOrderServices.ReturnOrder(returnData);
            if (!response.IsSuccess) return StatusCode(500, response);
            return Ok(response);
        }

        [HttpPost("request-cancel/{rentalOrderId}")]
        public async Task<IActionResult> RequestCancelOrder(int rentalOrderId, [FromQuery] string reason)
        {
            var response = await _rentalOrderServices.CancelRentalOrderAsync(rentalOrderId, reason);
            if (response.IsSuccess) return Ok(response);
            return BadRequest(response);
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
  
        [HttpPut("assign-branch")]
        public async Task<IActionResult> AssignBranch(int orderId, int branchId)
        {
            var response = await _rentalOrderServices.UpdateBranchForRentalOrder(orderId, branchId);
            if (response.IsSuccess) return Ok(response);
            return BadRequest(response);

        }

        [HttpPost("{orderId}/approve")]
        public async Task<IActionResult> ApproveRentalOrder(int orderId)
        {
            var response = await _rentalOrderServices.ApproveRentalOrderAsync(orderId);
            if (response.IsSuccess) return Ok(response);
            return BadRequest(response);
        }

        [HttpPost("{orderId}/reject")]
        public async Task<IActionResult> RejectRentalOrder(int orderId)
        {
            var response = await _rentalOrderServices.RejectRentalOrderAsync(orderId);
            if (response.IsSuccess) return Ok(response);
            return BadRequest(response);
        }

        [HttpDelete("remove/{orderId}")]
        public async Task<IActionResult> RemoveSaleOrder(int orderId)
        {
            var response = await _rentalOrderServices.DeleteRentalOrderAsync(orderId);
            if (response.IsSuccess) return Ok(response);
            return BadRequest(response);
        }


        [HttpPost("request-extension")]
        public async Task<IActionResult> RequestExtension(ExtensionRequestModel model)
        {
            var response = await _rentalOrderServices.RequestExtensionAsync(model);
            if (response.IsSuccess) return Ok(response);
            return BadRequest(response);
        }

        [HttpPost("approve-extension/{rentalOrderCode}")]
        public async Task<IActionResult> ApproveExtension(string rentalOrderCode)
        {
            var response = await _rentalOrderServices.ApproveExtensionAsync(rentalOrderCode);
            if (response.IsSuccess) return Ok(response);
            return BadRequest(response);
        }

        [HttpPost("reject-extension/{rentalOrderCode}")]
        public async Task<IActionResult> RejectExtension(string rentalOrderCode, [FromQuery] string rejectionReason)
        {
            var response = await _rentalOrderServices.RejectExtensionAsync(rentalOrderCode, rejectionReason);
            if (response.IsSuccess) return Ok(response);
            return BadRequest(response);
        }

    }
}
