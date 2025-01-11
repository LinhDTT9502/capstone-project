using _2Sport_BE.Infrastructure.DTOs;
using _2Sport_BE.Infrastructure.Services;
using _2Sport_BE.Repository.Interfaces;
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
        private readonly IUnitOfWork _unitOfWork;
        public RefundRequestController(IRefundRequestService refundRequestService, IUnitOfWork unitOfWork)
        {
            _refundRequestService = refundRequestService;
            _unitOfWork = unitOfWork;
        }
        [HttpGet("getAll")]
        public async Task<IActionResult> GetAllRefundRequests([FromQuery] string? orderType, [FromQuery] string? status = null, [FromQuery] int? branchId = null)
        {
            var response = new ResponseDTO<List<RefundRequestVM>>();
            if(orderType == "1")
            {
                response = await _refundRequestService.GetAllSaleRefundRequests(status, branchId);
            }else if(orderType == "2")
            {
                response = await _refundRequestService.GetAllRentalRefundRequests(status, branchId);
            }
            else
            {
                response = await _refundRequestService.GetAllRefundRequest();
            }
            
            if (response.IsSuccess) return Ok(response);
            return BadRequest(response);
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
