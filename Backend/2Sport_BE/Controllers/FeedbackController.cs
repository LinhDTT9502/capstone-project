using _2Sport_BE.Repository.Interfaces;
using _2Sport_BE.Repository.Models;
using _2Sport_BE.Service.Services;
using _2Sport_BE.ViewModels;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace _2Sport_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeedbackController : ControllerBase
    {
        private readonly IFeedbackService _feedbackService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public FeedbackController(IFeedbackService feedbackService, IMapper mapper, IUnitOfWork unitOfWork)
        {
            _feedbackService = feedbackService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        [Route("get-all-feedbacks")]
        public async Task<IActionResult> GetFeedbacks()
        {
            try
            {
                var query = await _feedbackService.GetFeedbacks();
                var result = _mapper.Map<List<FeedbackVM>>(query.ToList());
                return Ok(new { total = result.Count, data = result });
            } catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        [Route("get-feedback-by-id/{feedbackId}")]
        public async Task<IActionResult> GetFeedbackById(int feedbackId)
        {
            try
            {
                var query = (await _feedbackService.GetFeedbackById(feedbackId)).FirstOrDefault();
                var result = _mapper.Map<FeedbackVM>(query);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        [Route("send-feedback")]
        public async Task<IActionResult> SendFeedback(FeedbackCM feedbackCM)
        {
            try
            {
                int userId = GetCurrentUserIdFromToken();
                var addedFeedback = _mapper.Map<Feedback>(feedbackCM); 
                var isSuccess = await _feedbackService.SendFeedback(userId, addedFeedback);
                if (isSuccess == 1)
                {
                    return Ok("Send feedback successfully!");
                } else
                {
                    return BadRequest("Send feedback failed!");
                }
            } catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete]
        [Route("remove-feedback/{feedbackId}")]
        public async Task<IActionResult> RemoveFeedback(int feedbackId)
        {
            try
            {
                var isSuccess = await _feedbackService.RemoveFeedback(feedbackId);
                if (isSuccess == 1)
                {
                    return Ok("Remove feedback successfully!");
                } else
                {
                    return BadRequest("Remove feedback failed!");
                }
            } catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        protected int GetCurrentUserIdFromToken()
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

    }
}
