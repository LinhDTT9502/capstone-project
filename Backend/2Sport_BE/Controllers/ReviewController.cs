using _2Sport_BE.Repository.Interfaces;
using _2Sport_BE.Repository.Models;
using _2Sport_BE.Service.Services;
using _2Sport_BE.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace _2Sport_BE.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ReviewController : ControllerBase
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IReviewService _reviewService;

		public ReviewController(IUnitOfWork unitOfWork, IReviewService reviewService)
		{
			_unitOfWork = unitOfWork;
			_reviewService = reviewService;
		}

		[HttpGet]
		[Route("get-all-reviews")]
		public async Task<IActionResult> GetAllReviews()
		{
			try
			{
				var allReviews = await _reviewService.GetAllReviews();
				return Ok(allReviews.ToList());
			} catch (Exception ex)
			{
				return BadRequest(ex);
			}
		}

		[HttpPost]
		[Route("add-review/{productId}")]
		public async Task<IActionResult> AddReview(int productId, ReviewCM reviewCM)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			try
			{
				var userId = GetCurrentUserIdFromToken();

				if (userId == 0)
				{
					return Unauthorized();
				}
				var addedReview = new Review
				{
					Star = reviewCM.Star,
					Review1 = reviewCM.Review1,
					Status = true,
					UserId = userId,
					ProductId = productId,
				};
				await _reviewService.AddReview(addedReview);
				_unitOfWork.Save();
				return Ok(addedReview);
			} catch (Exception ex)
			{
				return BadRequest(ex);
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
