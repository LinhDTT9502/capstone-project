using _2Sport_BE.Infrastructure.Services;
using _2Sport_BE.Repository.Interfaces;
using _2Sport_BE.Repository.Models;
using _2Sport_BE.Service.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace _2Sport_BE.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class LikeController : ControllerBase
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly ILikeService _likeService;
		private readonly IUserService _userService;
		private readonly IProductService _productService;
		private readonly IBlogService _blogService;

        public LikeController(IUnitOfWork unitOfWork, 
							  ILikeService likeService, 
							  IUserService userService, 
							  IBlogService	blogService,
                              IProductService productService)
		{
			_unitOfWork = unitOfWork;
			_likeService = likeService;
			_userService = userService;
			_productService = productService;
			_blogService = blogService;
        }

		[HttpGet]
		[Route("get-likes-of-product")]
		public async Task<IActionResult> GetLikesOfProduct()
		{
			try
			{
				return Ok(await _likeService.GetLikesOfProduct());
			}
			catch (Exception ex)
			{
				return BadRequest(ex);
			}
		}

        [HttpPost]
		[Route("like-product/{productCode}")]
		public async Task<IActionResult> LikeProduct(string productCode)
		{
			try
			{
				var userId = GetCurrentUserIdFromToken();

				if (userId == 0)
				{
					return Unauthorized();
				}
				var user = await _userService.GetUserById(userId);
				var products = await _productService.GetProductsByProductCode(productCode);
				foreach (var product in products)
				{
                    var addedLike = new Like
                    {
                        UserId = userId,
                        ProductId = product.Id,
                        //User = user,
                        //Product = product,
                    };
					await _likeService.LikeProduct(addedLike);
                }

                _unitOfWork.Save();
				return Ok("Like product successfully!");

			}
			catch (Exception ex)
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
