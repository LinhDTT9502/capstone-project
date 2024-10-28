using _2Sport_BE.Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace _2Sport_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerDetailService;
        public CustomerController(ICustomerService customerDetailService)
        {
            _customerDetailService = customerDetailService;
        }

        [HttpGet]
        [Route("get-loyal-points")]
        public async Task<IActionResult> GetPointByUserId([FromQuery] int userId)
        {
            var response = await _customerDetailService.GetPointByUserId(userId);
            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
        [HttpPost("add-points")]
        public async Task<IActionResult> AddPoints(string phoneNumber, int points)
        {
            if (string.IsNullOrEmpty(phoneNumber) || points <= 0)
            {
                return BadRequest("Invalid phone number or points.");
            }

            var result = await _customerDetailService.AddMemberPointByPhoneNumber(phoneNumber, points);

            if (!result.IsSuccess)
            {
                return BadRequest(result.Message);
            }

            return Ok(result);
        }
    }
}
