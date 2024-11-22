using _2Sport_BE.Infrastructure.Services;
using _2Sport_BE.Service.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace _2Sport_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RefundRequestController : ControllerBase
    {
        private readonly IRefundRequestService _refundRequestService;
        public RefundRequestController(IRefundRequestService refundRequestService)
        {
            _refundRequestService = refundRequestService;
        }
        [HttpGet("getAll")]
        public async Task<IActionResult> GetAllRefundRequests([FromQuery] string status = null, [FromQuery] int? branchId = null)
        {
            var result = await _refundRequestService.GetAllRefundRequests(status, branchId);

            if (result.IsSuccess)
                return Ok(result);
            else
                return BadRequest(result);
        }
        [HttpPost("create")]
        public async Task<IActionResult> CreateRefundRequest([FromBody] RefundRequestCM refundRequestCM)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid input data.");

            var result = await _refundRequestService.CreateRefundRequest(refundRequestCM);

            if (result.IsSuccess)
                return Ok(result);
            else
                return BadRequest(result);
        }
        [HttpPut("update")]
        public async Task<IActionResult> UpdateRefundRequest([FromBody] RefundRequestUM refundRequestUM)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid input data.");

            var result = await _refundRequestService.UpdateRefundRequest(refundRequestUM.RefundRequestId, refundRequestUM);

            if (result.IsSuccess)
                return Ok(result);
            else
                return BadRequest(result);
        }
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteRefundRequest(int id)
        {
            if (id <= 0)
                return BadRequest("Invalid refund request ID.");

            var result = await _refundRequestService.DeleteRefundRequest(id);

            if (result.IsSuccess)
                return Ok(result);
            else
                return BadRequest(result);
        }
    }
}
