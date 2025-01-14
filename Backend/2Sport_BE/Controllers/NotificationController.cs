using _2Sport_BE.Infrastructure.Services;
using _2Sport_BE.Repository.Models;
using _2Sport_BE.Services.Caching;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace _2Sport_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;
        private readonly IRedisCacheService _redisCacheService;
        private readonly string _notificationsKey;
        public NotificationController(INotificationService notificationService,
                                      IRedisCacheService redisCacheService,
                                      IConfiguration configuration)
        {
            _notificationService = notificationService;
            _redisCacheService = redisCacheService;
            _notificationsKey = configuration.GetValue<string>("RedisKeys:Notifications");
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

        [HttpDelete]
        [Route("reset-notifications")]
        public async Task<IActionResult> ResetNotifications()
        {
            try
            {
                var listNotifications = _redisCacheService.GetData<List<Notification>>(_notificationsKey)
                                                       ?? new List<Notification>();

                listNotifications.Clear();
                _redisCacheService.SetData(_notificationsKey, listNotifications, TimeSpan.FromDays(30));
                return Ok($"Reset notifications successfully!");
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
    }
}
