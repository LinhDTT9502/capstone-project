using _2Sport_BE.Repository.Models;
using _2Sport_BE.Service.DTOs;
using _2Sport_BE.Service.Services;
using _2Sport_BE.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace _2Sport_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReturnRequestController : ControllerBase
    {
        private readonly IReturnRequestService _returnRequestService;
        private readonly IImageService _imageService;
        public ReturnRequestController(IReturnRequestService returnService, IImageService imageService)
        {
            _returnRequestService = returnService;
            _imageService = imageService;
        }
        [HttpGet("get-all")]
        public async Task<IActionResult> GetAllReturns()
        {
            try
            {
                var result = await _returnRequestService.GetAllReturn();
                if (result.IsSuccess)
                {
                    return Ok(result.Data);
                }
                return NotFound(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
        [HttpGet("branch/{branchId}")]
        public async Task<IActionResult> GetReturnsByBranchId(int branchId)
        {
            try
            {
                var result = await _returnRequestService.GetReturnByBranchId(branchId);
                if (result.IsSuccess)
                {
                    return Ok(result);
                }
                return NotFound(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateReturn([FromForm] ReturnRequestDTO request)
        {
            try
            {
                if (request == null || request.Video == null)
                {
                    return BadRequest(new { Message = "Invalid request data." });
                }

                string folderName = "return_videos";
                var uploadResult = await _imageService.UploadVideoToCloudinaryAsync(request.Video, folderName);

                if (uploadResult == null || uploadResult.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    return BadRequest(new { Message = "Failed to upload video." });
                }

                var videoUrl = uploadResult.Url.ToString();

                var result = await _returnRequestService.CreateReturnAsync(request, videoUrl);

                if (result.IsSuccess)
                {
                    return Ok(result);
                }

                return NotFound(new { Message = result.Message ?? "Return request not found." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred.", Details = ex.Message });
            }
        }

        [HttpPut("{returnId}")]
        public async Task<IActionResult> UpdateReturn(int returnId, [FromBody] ReturnRequestUM updatedRequest)
        {
            try
            {
                var result = await _returnRequestService.UpdateReturnRequest(returnId, updatedRequest);
                if (result.IsSuccess)
                {
                    return Ok(result);
                }
                return NotFound(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
        [HttpGet("order")]
        public async Task<IActionResult> GetReturnsByOrderId([FromQuery] int? saleOrderId, [FromQuery] int? rentalOrderId)
        {
            try
            {
                var result = await _returnRequestService.GetReturnByOrderId(saleOrderId, rentalOrderId);
                if (result.IsSuccess)
                {
                    return Ok(result);
                }
                return NotFound(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpDelete("{returnId}")]
        public async Task<IActionResult> DeleteReturnRequest(int returnId)
        {
            try
            {
                var result = await _returnRequestService.DeleteReturnRequest(returnId);

                if (result.IsSuccess)
                {
                    return Ok(new { Message = "Return request deleted successfully." });
                }
                return NotFound(new { Message = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

    }
}
