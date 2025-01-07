using _2Sport_BE.Infrastructure.DTOs;
using _2Sport_BE.Infrastructure.Services;
using _2Sport_BE.Repository.Interfaces;
using _2Sport_BE.Repository.Models;
using _2Sport_BE.Service.Services;
using _2Sport_BE.ViewModels;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using System.Security.Claims;

namespace _2Sport_BE.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ReviewController : ControllerBase
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;
        private readonly IReviewService _reviewService;
		private readonly ISaleOrderService _saleOrderService;

        public ReviewController(IUnitOfWork unitOfWork, 
								IReviewService reviewService,
								ISaleOrderService saleOrderService,
								IMapper mapper)
		{
			_unitOfWork = unitOfWork;
			_reviewService = reviewService;
			_saleOrderService = saleOrderService;
			_saleOrderService = saleOrderService;
			_mapper = mapper;
        }

        [HttpGet]
		[Route("get-all-reviews")]
		public async Task<IActionResult> GetAllReviews()
		{
			try
			{
				var allReviews = await _reviewService.GetAllReviews();
				var result = _mapper.Map<List<ReviewVM>>(allReviews.ToList());
				return Ok(result);
			} catch (Exception ex)
			{
				return BadRequest(ex);
			}
		}

        [HttpGet]
        [Route("get-all-reviews-of-product/{productCode}")]
        public async Task<IActionResult> GetAllReviewsOfProoduct(string productCode)
        {
            try
            {
                var allReviews = await _reviewService.GetReviewsOfProduct(productCode);
                var result = _mapper.Map<List<ReviewVM>>(allReviews.ToList());
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet]
        [Route("check-is-review/{saleOrderId}")]
        public async Task<IActionResult> CheckIsReivew(int saleOrderId)
        {
            try
            {
				var reviewsInSaleOrder = (await _saleOrderService.GetSaleOrderDetailsByIdAsync(saleOrderId))
															.Data
															.Reviews;
				if (reviewsInSaleOrder.Count > 0)
				{
					return Ok(true);
				}
                return Ok(false);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPost]
		[Route("add-review/{saleOrderId}")]
		public async Task<IActionResult> AddReview(int saleOrderId, ReviewCM reviewCM)
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

                var userInSaleOrder = (await _saleOrderService.GetSaleOrderDetailsByIdAsync(saleOrderId))
                                                            .Data
                                                            .UserId;

				if (userId != userInSaleOrder)
				{
					return Unauthorized("You are not allow to review other sale order!");
				}

				var addedReview = new Review
				{
					Star = reviewCM.Star,
					ReviewContent = reviewCM.Review,
					Status = true,
					UserId = userId,
					SaleOrderId = saleOrderId,
					ProductCode = (await _unitOfWork.ProductRepository.FindAsync(reviewCM.ProductId)).ProductCode,
                    ProductId = reviewCM.ProductId,
                    CreatedAt = DateTime.Now,
                };
				
				await _reviewService.AddReview(addedReview);
				_unitOfWork.Save();
				return Ok("Add review successfuly!");
			} catch (Exception ex)
			{
				return BadRequest(ex);
			}
		}

		[HttpDelete]
		[Route("delete-review/{reviewId}")]
		public async Task<IActionResult> DeleteReview(int reviewId)
		{
			try
			{
				var deletedReview = await _reviewService.DeleteReview(reviewId);
				return Ok("Delete successfully!");
			} catch (Exception e)
			{
				return StatusCode(500, e.Message);
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
