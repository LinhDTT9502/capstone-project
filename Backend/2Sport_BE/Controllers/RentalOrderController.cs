using _2Sport_BE.Infrastructure.DTOs;
using _2Sport_BE.Infrastructure.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace _2Sport_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RentalOrderController : ControllerBase
    {
        /*[HttpPost("checkout-rental-order")]
        public async Task<IActionResult> HandleCheckoutRental([FromBody] RentalOrderCM rentalOrderCM)
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

            var response = await _rentalOrderService.CreateRentalOrderForCustomer(rentalOrderCM);
            if (!response.IsSuccess)
            {
                return StatusCode(500, response);
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
        }*/

       /* [HttpPut("update-rental-orderr")]
        public async Task<IActionResult> UpdateRentalOrder([FromQuery] int orderId, [FromBody] RentalOrderUM orderUM)
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
        }*/
    }
}
