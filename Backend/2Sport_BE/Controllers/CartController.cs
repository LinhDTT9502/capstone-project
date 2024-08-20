using _2Sport_BE.Helpers;
using _2Sport_BE.Infrastructure.Services;
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
    public class CartController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICartItemService _cartItemService;
		private readonly IWarehouseService _warehouseService;
        private readonly IMapper _mapper;

        public CartController(IUnitOfWork unitOfWork, ICartItemService cartItemService,
                              IWarehouseService warehouse,
                              IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _cartItemService = cartItemService;
            _warehouseService = warehouse;
            _mapper = mapper;
        }

        [HttpGet]
        [Route("get-cart")]
        public async Task<IActionResult> GetCarts([FromQuery]DefaultSearch defaultSearch)
        {
            try
            {
				var userId = GetCurrentUserIdFromToken();

				if (userId == 0)
				{
					return Unauthorized();
				}

				var query = await _cartItemService.GetCartItems(userId, defaultSearch.currentPage, defaultSearch.perPage);
                if (query != null)
                {
					var cartItems = query.Select(_ => _mapper.Map<CartItem, CartItemVM>(_)).ToList();
					if (cartItems != null)
					{
						foreach (var carItem in cartItems)
						{
							var product = await _unitOfWork.ProductRepository.FindAsync(carItem.ProductId);
							carItem.ProductName = product.ProductName;
							carItem.MainImageName = product.MainImageName;
							carItem.MainImagePath = product.MainImagePath;
						}
						return Ok(new { total = cartItems.Count(), data = cartItems });
					}
					return BadRequest();
				}
                
				return BadRequest();
            } catch (Exception ex)
            {
                return BadRequest(ex);
            }

        }

        [HttpGet]
        [Route("get-cart-item/{cartItemId}")]
        public async Task<IActionResult> GetCartItem(int cartItemId)
        {
            try
            {
                var cartItem = await _cartItemService.GetCartItemById(cartItemId);
                if (cartItem != null)
                {
                    return Ok(cartItem);
                }
                return BadRequest($"Cannot find cart item with id: {cartItemId}");
            } catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        [HttpPost]
        [Route("add-to-cart")]
        public async Task<IActionResult> AddToCart(CartItemCM cartItemCM)
        {
            try
            {
                var userId = GetCurrentUserIdFromToken();

                if (userId == 0)
                {
                    return Unauthorized();
                }

				var newCartItem = _mapper.Map<CartItemCM, CartItem>(cartItemCM);
				var quantityOfProduct = (await _warehouseService.GetWarehouseByProductId(cartItemCM.ProductId))
										.FirstOrDefault().Quantity;
				if (cartItemCM.Quantity > quantityOfProduct)
				{
					return BadRequest($"Xin lỗi! Chúng tôi chỉ còn {quantityOfProduct} sản phẩm");
				}
                var addedCartItem = await _cartItemService.AddCartItem(userId, newCartItem);
                if (addedCartItem != null)
                {
                    return Ok(addedCartItem);
                } else
                {
                    return BadRequest("Add to cart failed");
                }
            } catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

		[HttpPut]
		[Route("reduce-cart/{cartItemId}")]
		public async Task<IActionResult> ReduceCart(int cartItemId)
		{
			try
			{
				var userId = GetCurrentUserIdFromToken();

				if (userId == 0)
				{
					return Unauthorized();
				}

                await _cartItemService.ReduceCartItem(cartItemId);
                _unitOfWork.Save();
                return Ok($"Reduce cart item with id: {cartItemId}");
			}
			catch (Exception ex)
			{
				return BadRequest(ex);
			}
		}

		[HttpPut]
		[Route("update-quantity-cart-item/{cartItemId}")]
		public async Task<IActionResult> UpdateQuantityOfCart(int cartItemId, [FromQuery] int quantity)
		{
			try
			{
				var userId = GetCurrentUserIdFromToken();

				if (userId == 0)
				{
					return Unauthorized();
				}
				var cartItem = await _cartItemService.GetCartItemById(cartItemId);
                var quantityOfProduct = (await _warehouseService.GetWarehouseByProductId(cartItem.ProductId))
                        .FirstOrDefault().Quantity;
                if (quantity > quantityOfProduct)
                {
                    return BadRequest($"Xin lỗi! Chúng tôi chỉ còn {quantityOfProduct} sản phẩm");
                }
                await _cartItemService.UpdateQuantityOfCartItem(cartItemId, quantity);
				_unitOfWork.Save();
				return Ok($"Update quantity cart item with id: {cartItemId}");
			}
			catch (Exception ex)
			{
				return BadRequest(ex);
			}
		}

		[HttpDelete]
		[Route("delete-cart-item/{cartItemId}")]
		public async Task<IActionResult> DeleteCartItem(int cartItemId)
		{
			try
			{
				var userId = GetCurrentUserIdFromToken();

				if (userId == 0)
				{
					return Unauthorized();
				}

				await _cartItemService.DeleteCartItem(cartItemId);
				_unitOfWork.Save();
				return Ok($"Delete cart item with id: {cartItemId}");
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
