using _2Sport_BE.Helpers;
using _2Sport_BE.Infrastructure.Services;
using _2Sport_BE.Repository.Interfaces;
using _2Sport_BE.Repository.Models;
using _2Sport_BE.Service.Services;
using _2Sport_BE.Services.Caching;
using _2Sport_BE.ViewModels;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NuGet.Configuration;
using System.Security.Claims;

namespace _2Sport_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartWithRedisController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICartItemService _cartItemService;
        private readonly IWarehouseService _warehouseService;
        private readonly IProductService _productService;
        private readonly IRedisCacheService _redisCacheService;
        private readonly IMapper _mapper;
        private readonly string _cartItemsKey;
        public CartWithRedisController(IUnitOfWork unitOfWork,
                                       ICartItemService cartItemService,
                                       IWarehouseService warehouse,
                                       IProductService productService,
                                       IRedisCacheService redisCacheService,
                                       IConfiguration configuration,
                                       IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _cartItemService = cartItemService;
            _productService = productService;
            _redisCacheService = redisCacheService;
            _warehouseService = warehouse;
            _cartItemsKey = configuration.GetValue<string>("RedisKeys:CartItems");
            _mapper = mapper;
        }

        [HttpGet]
        [Route("get-cart")]
        public async Task<IActionResult> GetCarts([FromQuery] DefaultSearch defaultSearch)
        {
            try
            {
                var userId = GetCurrentUserIdFromToken();

                if (userId == 0)
                {
                    return Unauthorized();
                }
                var query = _redisCacheService.GetData<List<CartItem>>(_cartItemsKey)
                                                        ?? new List<CartItem>();
                //var query = await _cartItemService.GetCartItems(userId, defaultSearch.currentPage, defaultSearch.perPage);
                if (query != null)
                {
                    var cartItems = query.Select(_ => _mapper.Map<CartItem, CartItemVM>(_)).ToList();
                    if (cartItems != null)
                    {
                        foreach (var carItem in cartItems)
                        {
                            if (carItem.Quantity == 0)
                            {
                                var deleteCartItem = query.Find(_ => _.CartItemId.Equals(carItem.CartItemId));
                                query.Remove(deleteCartItem);
                                _redisCacheService.SetData(_cartItemsKey, query);
                                cartItems = query.Select(_ => _mapper.Map<CartItem, CartItemVM>(_)).ToList();
                            }
                            var product = await _unitOfWork.ProductRepository.FindAsync(carItem.ProductId);
                            carItem.ProductName = product.ProductName;
                            carItem.ProductCode = product.ProductCode;
                            carItem.Color = product.Color;
                            carItem.Size = product.Size;
                            carItem.Price = (decimal)product.Price;
                            carItem.Condition = (int)product.Condition;
                            carItem.ImgAvatarPath = product.ImgAvatarPath;
                            carItem.RentPrice = (decimal)product.RentPrice;
                        }
                        return Ok(new { total = cartItems.Count(), data = cartItems });
                    }
                    return BadRequest();
                }

                return BadRequest();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }

        }

        [HttpGet]
        [Route("get-cart-item/{cartItemId}")]
        public async Task<IActionResult> GetCartItem(Guid cartItemId)
        {
            try
            {
                var listCartItems = _redisCacheService.GetData<List<CartItem>>(_cartItemsKey) 
                                                            ?? new List<CartItem>();
                var cartItem = listCartItems.Find(_ => _.CartItemId.Equals(cartItemId));
                if (cartItem != null)
                {
                    var result = _mapper.Map<CartItemVM>(cartItem);
                    var product = await _unitOfWork.ProductRepository.FindAsync(cartItem.ProductId);
                    result.ProductName = product.ProductName;
                    result.ProductCode = product.ProductCode;
                    result.Color = product.Color;
                    result.Size = product.Size;
                    result.Price = (decimal)product.Price;
                    result.Condition = (int)product.Condition;
                    result.ImgAvatarPath = product.ImgAvatarPath;
                    result.RentPrice = (decimal)product.RentPrice;
                    return Ok(result);
                }
                return BadRequest($"Cannot find cart item with id: {cartItemId}");
            }
            catch (Exception e)
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
                var productInWarehouse = (await _warehouseService.GetWarehouseByProductId(cartItemCM.ProductId))
                                        .FirstOrDefault();
                var addedProduct = (await _productService.GetProductById((int)cartItemCM.ProductId));
                if (productInWarehouse == null)
                {
                    return BadRequest("That product is not in warehouse!");
                }
                var quantityOfProduct = productInWarehouse.AvailableQuantity;
                var addedCartItemQuantity = cartItemCM.Quantity;
                var listCartItemsInCache = _redisCacheService.GetData<List<CartItem>>(_cartItemsKey) 
                                                                        ?? new List<CartItem>();
                var existedCartItem = listCartItemsInCache.FirstOrDefault(_ => _.UserId == userId
                                                                && _.ProductId == (int)cartItemCM.ProductId);
                //var existedCartItem = await _cartItemService.GetCartItemByUserIdAndProductId(userId, (int)cartItemCM.ProductId);
                var addedCartItem = new CartItem();
                if (existedCartItem != null)
                {
                    addedCartItemQuantity += existedCartItem.Quantity;
                    if (addedCartItemQuantity > quantityOfProduct)
                    {
                        return BadRequest($"Xin lỗi! Chúng tôi chỉ còn {quantityOfProduct} sản phẩm");
                    }
                    existedCartItem.Quantity = addedCartItemQuantity;
                    existedCartItem.UnitPrice = addedCartItemQuantity;
                    _redisCacheService.SetData(_cartItemsKey, listCartItemsInCache);
                    return Ok(existedCartItem);
                    //addedCartItem = await _cartItemService.AddExistedCartItem(existedCartItem);

                }
                else
                {
                    if (addedCartItemQuantity > quantityOfProduct)
                    {
                        return BadRequest($"Xin lỗi! Chúng tôi chỉ còn {quantityOfProduct} sản phẩm");
                    }
                    newCartItem.CartItemId = Guid.NewGuid();
                    newCartItem.UserId = userId;
                    listCartItemsInCache.Add(newCartItem);
                    _redisCacheService.SetData(_cartItemsKey, listCartItemsInCache);
                    return Ok(newCartItem);
                    //addedCartItem = await _cartItemService.AddNewCartItem(userId, newCartItem);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPut]
        [Route("reduce-cart/{cartItemId}")]
        public async Task<IActionResult> ReduceCart(Guid cartItemId)
        {
            try
            {
                var userId = GetCurrentUserIdFromToken();

                if (userId == 0)
                {
                    return Unauthorized();
                }
                var listCartItems = _redisCacheService.GetData<List<CartItem>>(_cartItemsKey)
                                                                    ?? new List<CartItem>();
                var reducedCartItem = listCartItems.Find(_ => _.CartItemId.Equals(cartItemId));
                if (reducedCartItem != null)
                {
                    var product = (await _unitOfWork.ProductRepository
                                                        .GetAsync(_ => _.Id == reducedCartItem.ProductId))
                                                        .FirstOrDefault();
                    reducedCartItem.Quantity -= 1;
                    if (reducedCartItem.Quantity == 0)
                    {
                        listCartItems.Remove(reducedCartItem);
                    }
                    _redisCacheService.SetData(_cartItemsKey, listCartItems);
                }
                return Ok($"Reduce cart item with id: {cartItemId}");
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPut]
        [Route("update-quantity-cart-item/{cartItemId}")]
        public async Task<IActionResult> UpdateQuantityOfCart(Guid cartItemId, [FromQuery] int quantity)
        {
            try
            {
                var userId = GetCurrentUserIdFromToken();

                if (userId == 0)
                {
                    return Unauthorized();
                }
                var listCartItems = _redisCacheService.GetData<List<CartItem>>(_cartItemsKey)
                                                              ?? new List<CartItem>();
                var updatedCartItem = listCartItems.Find(_ => _.CartItemId.Equals(cartItemId));
                if (updatedCartItem == null)
                {
                    return BadRequest("The cart is not exist!");
                }
                if ((await _warehouseService.GetWarehouseByProductId(updatedCartItem.ProductId)) is null)
                {
                    return BadRequest("The warehouse is not exist!");
                }
                var quantityOfProduct = (await _warehouseService.GetWarehouseByProductId(updatedCartItem.ProductId))
                        .FirstOrDefault().AvailableQuantity;
                if (quantity > quantityOfProduct)
                {
                    return BadRequest($"Xin lỗi! Chúng tôi chỉ còn {quantityOfProduct} sản phẩm");
                }

                var product = await _unitOfWork.ProductRepository.FindAsync(updatedCartItem.ProductId);
                if (updatedCartItem.Quantity == 0)
                {
                    listCartItems.Remove(updatedCartItem);
                }
                else
                {
                    updatedCartItem.Quantity = quantity;
                    updatedCartItem.UnitPrice = product.Price;
                }
                _redisCacheService.SetData(_cartItemsKey, listCartItems);
                return Ok($"Update quantity cart item with id: {cartItemId}");
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPut]
        [Route("update-product-cart-item/{cartItemId}")]
        public async Task<IActionResult> UpdateProductOfCart(Guid cartItemId, [FromQuery] int productId)
        {
            try
            {
                var userId = GetCurrentUserIdFromToken();

                if (userId == 0)
                {
                    return Unauthorized();
                }
                var listCartItems = _redisCacheService.GetData<List<CartItem>>(_cartItemsKey)
                                                                ?? new List<CartItem>();

                var cartItem = listCartItems.Find(_ => _.CartItemId.Equals(cartItemId));
                if (cartItem is null)
                {
                    return BadRequest("The cart is not exist!");
                }
                if ((await _warehouseService.GetWarehouseByProductId(cartItem.ProductId)) is null)
                {
                    return BadRequest("The warehouse is not exist!");
                }

                var quantityOfProduct = (await _warehouseService.GetWarehouseByProductId(cartItem.ProductId))
                        .FirstOrDefault().AvailableQuantity;
                if (cartItem.Quantity > quantityOfProduct)
                {
                    return BadRequest($"Xin lỗi! Chúng tôi chỉ còn {quantityOfProduct} sản phẩm");
                }
                var existedProduct = await _productService.GetProductById(productId);
                if (existedProduct == null)
                {
                    return BadRequest("This product is not existed!");
                }
                await _cartItemService.UpdateProductIdOfCartItem(cartItemId, productId);
                cartItem.ProductId = productId;
                _redisCacheService.SetData(_cartItemsKey, listCartItems);
                return Ok($"Updated productId in cart item with id: {cartItemId}");
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpDelete]
        [Route("delete-cart-item/{cartItemId}")]
        public async Task<IActionResult> DeleteCartItem(Guid cartItemId)
        {
            try
            {
                var userId = GetCurrentUserIdFromToken();

                if (userId == 0)
                {
                    return Unauthorized();
                }
                var listCartItems = _redisCacheService.GetData<List<CartItem>>(_cartItemsKey)
                                                       ?? new List<CartItem>();
                var deletedCartItem = listCartItems.Find(_ => _.CartItemId.Equals(cartItemId));
                if (deletedCartItem == null)
                {
                    return NotFound("There is not cart item!");
                } 
                listCartItems.Remove(deletedCartItem);
                _redisCacheService.SetData(_cartItemsKey, listCartItems);
                await _cartItemService.DeleteCartItem(cartItemId);
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
