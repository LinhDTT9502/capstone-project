using _2Sport_BE.DataContent;
using _2Sport_BE.Infrastructure.DTOs;
using _2Sport_BE.Infrastructure.Enums;
using _2Sport_BE.Infrastructure.Helpers;
using _2Sport_BE.Infrastructure.Services;
using _2Sport_BE.Repository.Models;
using _2Sport_BE.Service.Services;
using _2Sport_BE.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace _2Sport_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SaleOrderController : Controller
    {
        private readonly ISaleOrderService _orderService;
        private readonly ICartItemService _cartItemService;
        private readonly IImageService _imageService;

        public SaleOrderController(ISaleOrderService orderService,
                                ICartItemService cartItemService,
                                IImageService imageService)
        {
            _orderService = orderService;
            _cartItemService = cartItemService;
            _imageService = imageService;
        }
        [HttpGet("get-all-sale-orders")]
        public async Task<IActionResult> ListAllSaleOrder()
        {
            var response = await _orderService.GetAllSaleOrdersAsync();

            if (response.IsSuccess) return Ok(response);
            return BadRequest(response);
        }

        [HttpGet("get-sale-order-detail")]
        public async Task<IActionResult> GetOrderByOrderId(int orderId)
        {
            var response = await _orderService.GetSaleOrderDetailsByIdAsync(orderId);

            if (response.IsSuccess) return Ok(response);
            return BadRequest(response);
        }

        [HttpGet("get-orders-by-user")]
        public async Task<IActionResult> GetOrdersByUserId(int userId)
        {
            var response = await _orderService.GetSaleOrdersOfUserAsync(userId);

            if (response.IsSuccess) return Ok(response);
            return BadRequest(response);
        }

        [HttpGet("get-orders-by-status")]
        public async Task<IActionResult> GetOrdersByStatus(int? orderStatus, int? paymentStatus)
        {
            var response = await _orderService.GetSaleOrdersByStatus(orderStatus, paymentStatus);

            if (response.IsSuccess) return Ok(response);
            return BadRequest(response);
        }

        [HttpGet("get-order-by-code")]
        public async Task<IActionResult> GetOrdersByOrderCode(string orderCode)
        {
            var response = await _orderService.GetSaleOrderBySaleOrderCode(orderCode);

            if (response.IsSuccess) return Ok(response);
            return BadRequest(response);
        }

        [HttpGet("get-orders-by-branch")]
        public async Task<IActionResult> GetOrdersByBranchId(int branchId)
        {
            var response = await _orderService.GetSaleOrdersByBranchAsync(branchId);

            if (response.IsSuccess) return Ok(response);
            return BadRequest(response);
        }

        [HttpPost("create-sale-order")]
        public async Task<IActionResult> AddSaleOrder([FromBody] SaleOrderCM orderCM)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid request data.");
            }
            //Tao order
            var response = await _orderService.CreateSaleOrderAsync(orderCM);
            if (!response.IsSuccess)
            {
                return StatusCode(500, response);
            }
            foreach (var item in orderCM.ProductInformations)
            {
                if (item.CartItemId.HasValue && item.CartItemId.Value != Guid.Empty)
                {
                    await _cartItemService.DeleteCartItem(item.CartItemId.Value);
                }
            }
            return Ok(response);
        }

        [HttpPut("update-sale-order")]
        public async Task<IActionResult> EditSaleOrder([FromQuery] int orderId, [FromBody] SaleOrderUM orderUM)
        {
            if (!ModelState.IsValid) return BadRequest("Invalid request data.");

            var response = await _orderService.UpdateSaleOrderAsync(orderId, orderUM);

            if (response.IsSuccess) return Ok(response);
            return BadRequest(response);
        }

        [HttpPut("update-order-status/{orderId}")]
        public async Task<IActionResult> ChangeOrderStatus(int orderId, [FromQuery] int status)
        {
            var response = await _orderService.UpdateSaleOrderStatusAsync(orderId, status);
            if (response.IsSuccess) return Ok(response);
            return BadRequest(response);
        }

        [HttpPut("update-sale-payment-status/{orderId}")]
        public async Task<IActionResult> ChangeSlePaymentStatus(int orderId, [FromQuery] int paymentStatus)
        {
            var response = await _orderService.UpdateSalePaymentStatus(orderId, paymentStatus);
            if (response.IsSuccess) return Ok(response);
            return BadRequest(response);
        }

        [HttpPut("assign-branch")]
        public async Task<IActionResult> AssignBranch(int orderId, int branchId)
        {
            var response = await _orderService.UpdateBranchForSaleOrder(orderId, branchId);

            if (response.IsSuccess) return Ok(response);
            return BadRequest(response);
        }

        [HttpPost("request-cancel/{saleOrderId}")]
        public async Task<IActionResult> RequestCancelOrder(int saleOrderId, [FromQuery] string reason)
        {
            var response = await _orderService.CancelSaleOrderAsync(saleOrderId, reason);

            if (response.IsSuccess) return Ok(response);
            return BadRequest(response);
        }

        [HttpPost("{orderId}/approve")]
        public async Task<IActionResult> ApproveSaleOrder(int orderId)
        {
            var response = await _orderService.ApproveSaleOrderAsync(orderId);

            if (response.IsSuccess) return Ok(response);
            return BadRequest(response);
        }

        [HttpPost("{orderId}/reject")]
        public async Task<IActionResult> RejectSaleOrder(int orderId)
        {
            var response = await _orderService.RejectSaleOrderAsync(orderId);

            if (response.IsSuccess) return Ok(response);
            return BadRequest(response);
        }

        [HttpDelete("remove/{orderId}")]
        public async Task<IActionResult> RemoveSaleOrder(int orderId)
        {
            var response = await _orderService.DeleteSaleOrderAsync(orderId);

            if (response.IsSuccess) return Ok(response);
            return BadRequest(response);
        }

        [NonAction]
        public int GetCurrentUserIdFromToken()
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

        [HttpPost]
        [Route("upload-sale-order-image")]
        public async Task<IActionResult> UploadOrderImage(SaleOrderImageModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var order = await _orderService.FindSaleOrderById(model.SaleOrderId);
            if(order == null) return NotFound();
            else
            {
                if (model.OrderImage != null)
                {
                    var uploadResult = await _imageService.UploadImageToCloudinaryAsync(model.OrderImage);
                    if (uploadResult != null && uploadResult.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        order.OrderImage = uploadResult.SecureUrl.AbsoluteUri;
                        await _orderService.UpdateSaleOrder(order);
                        return Ok("Upload avatar successfully");
                    }
                    else
                    {
                        return BadRequest("Something wrong!");
                    }
                }
                else
                {
                    return Ok("No updated");
                }
            }
        }
    }
}
