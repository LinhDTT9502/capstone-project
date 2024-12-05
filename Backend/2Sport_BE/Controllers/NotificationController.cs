using _2Sport_BE.Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace _2Sport_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;
        public NotificationController(INotificationService notificationService)
        {
                _notificationService = notificationService;
        }
        [HttpPut("update-status")]
        public async Task<IActionResult> UpdateNotificationStatus([FromQuery] int id, [FromQuery]bool isRead)
        {
            try
            {
                var response = await _notificationService.UpdateNotificationStatus(id, isRead);

                if (response.IsSuccess)
                {
                    return Ok(response);
                }

                return BadRequest(response);
            }
            catch (Exception ex)
            {
                 return StatusCode(500, new { message = "An error occurred while updating the notification status." });
            }
        }

        [HttpGet("get-by-user/{userId}")]
        public async Task<IActionResult> GetNotificationsByUserId(int userId)
        {
            try
            {
                var response = await _notificationService.GetNotificationByUserId(userId);

                if (response.IsSuccess)
                {
                    return Ok(response);
                }

                return NotFound(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while fetching notifications." });
            }
        }
    }
}
