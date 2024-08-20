using _2Sport_BE.DataContent;
using _2Sport_BE.Infrastructure.Services;
using _2Sport_BE.Repository.Interfaces;
using _2Sport_BE.Repository.Models;
using _2Sport_BE.Service.Enums;
using _2Sport_BE.Service.Services;
using _2Sport_BE.ViewModels;
using MailKit.Search;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace _2Sport_BE.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : Controller
    {
        private readonly IPaymentService _paymentService;
        private readonly IOrderService _orderService;
        private readonly IOrderDetailService _orderDetailService;
        private readonly IUserService _userService;
        private readonly ICartItemService _cartItemService;
        private readonly ICartService _cartService;
        private readonly IShipmentDetailService _shipmentDetailService;
        private readonly IPaymentMethodService _paymentMethodService;
        private readonly IProductService _productService;
        private readonly IWarehouseService _warehouseService;
        private readonly IUnitOfWork _unitOfWork;

        public PaymentController(
            IPaymentService paymentService,
            IOrderService orderService,
            IUserService userService,
            ICartService cartService,
            ICartItemService cartItemService,
            IShipmentDetailService shipmentDetailService,
            IPaymentMethodService paymentMethodService,
            IProductService productService,
            IOrderDetailService orderDetailService,
            IWarehouseService warehouseService,
            IUnitOfWork unitOfWork)
        {
            _paymentService = paymentService;
            _orderService = orderService;
            _userService = userService;
            _cartService = cartService;
            _cartItemService = cartItemService;
            _shipmentDetailService = shipmentDetailService;
            _paymentMethodService = paymentMethodService;
            _productService = productService;
            _orderDetailService = orderDetailService;
            _warehouseService = warehouseService;
            _unitOfWork = unitOfWork;
        }
        [HttpPost("checkout-orders")]
        public async Task<IActionResult> CreatePayOsLink([FromBody] OrderCM orderCM, int orderMethodId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid request data.");
            }

            var user = await GetUserFromToken();
            if (user is null)
            {
                return Unauthorized("Invalid user!");
            }

            var cart = await _cartService.GetCartByUserId(user.Id);
            if (cart == null || !cart.CartItems.Any())
            {
                return NotFound("Your Cart is empty");
            }

            Order order = null;
            if (orderMethodId == (int)OrderMethods.PayOS || orderMethodId == (int)OrderMethods.COD)
            {
                order = await CreateOrder(orderCM, orderMethodId);
                if (order == null)
                {
                    return StatusCode(500, "Order creation failed.");
                }

                var check = await DeleteCartItem(cart, orderCM.OrderDetails);
                if (!check)
                {
                    return StatusCode(500, "Failed to delete cart items.");
                }

                var paymentLink = orderMethodId == (int)OrderMethods.PayOS
                                  ? await _paymentService.PaymentWithPayOs(order.Id)
                                  : null;

                var orderVM = new OrderVM()
                {
                    id = order.Id,
                    ShipmentDetailId = orderCM.ShipmentDetailId,
                    PaymentMethod = order.PaymentMethod.PaymentMethodName,
                    ReceivedDate = order.ReceivedDate,
                    TransportFee = order.TransportFee,
                    IntoMoney = order.IntoMoney,
                    Status = order.Status,
                    PaymentLink = paymentLink,
                    OrderDetails = order.OrderDetails.Select(item => new OrderDetailRequest
                    {
                        ProductId = item.ProductId,
                        Price = (decimal)item.Price,
                        Quantity = item.Quantity
                    }).ToList()
                };

                var responseModel = new ResponseModel<OrderVM>
                {
                    IsSuccess = true,
                    Message = "Query successfully!",
                    Data = orderVM
                };
                return Ok(responseModel);
            }

            return BadRequest("Invalid order method.");
        }

        [HttpGet("GetPaymentLinkInformation/{orderId}")]
        public async Task<IActionResult> GetPaymentLinkInformation(int orderId)
        {
            try
            {
                var paymentLinkInfo = await _paymentService.GetPaymentLinkInformationAsync(orderId);
                if (paymentLinkInfo == null)
                {
                    return NotFound();
                }
                return Ok(paymentLinkInfo);
            }
            catch (Exception ex)
            {

                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpPost("CancelPaymentLink")]
        public async Task<IActionResult> CancelPaymentLink([FromBody] CancelPaymentRequest request)
        {
            try
            {
                var checkOrderExist = await _orderService.GetOrderByIdFromUserAsync(request.OrderId, GetCurrentUserIdFromToken());
                if (checkOrderExist == null){
                    return BadRequest("You don't have permission in this function");
                }
                checkOrderExist.Status = (int) OrderStatus.CANCELLED;
                var cancelledPaymentLinkInfo = await _paymentService.CancelPaymentLink(request.OrderId, request.Reason);
                return Ok(cancelledPaymentLinkInfo);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpGet("cancel")]
        public async Task<IActionResult> HandleCancel([FromQuery] PaymentResponse paymentResponse)
        {
            if (!ModelState.IsValid || AreAnyStringsNullOrEmpty(paymentResponse))
            {
                return BadRequest(new ResponseModel<object>
                {
                    IsSuccess = false,
                    Message = "Invalid request data.",
                    Data = null
                });
            }
            Order order = await _orderService.GetOrderByOrderCode(paymentResponse.OrderCode);
            if (order == null)
            {
                return NotFound(new ResponseModel<object>
                {
                    IsSuccess = false,
                    Message = "Order not found.",
                    Data = null
                });
            }
            // Cập nhật trạng thái Order thành "Cancelled"
            var isUpdated = await _orderService.UpdateOrderAsync(order.Id, (int)OrderStatus.CANCELLED);
            if (!isUpdated)
            {
                return StatusCode(500, new ResponseModel<object>
                {
                    IsSuccess = false,
                    Message = "Failed to update order status.",
                    Data = null
                });
            }
            _unitOfWork.Save();
            var redirectUrl = "https://twosport.vercel.app/order_cancel";
            return Redirect(redirectUrl);
        }
        [HttpGet("return")]
        public async Task<IActionResult> HandleReturn([FromQuery] PaymentResponse paymentResponse)
        {
            if (!ModelState.IsValid || AreAnyStringsNullOrEmpty(paymentResponse))
            {
                return BadRequest(new ResponseModel<object>
                {
                    IsSuccess = false,
                    Message = "Invalid request data.",
                    Data = null
                });
            }
            Order order = await _orderService.GetOrderByOrderCode(paymentResponse.OrderCode);
            if (order == null)
            {
                return NotFound(new ResponseModel<object>
                {
                    IsSuccess = false,
                    Message = "Order not found.",
                    Data = null
                });
            }

            var isUpdated = await _orderService.UpdateOrderAsync(order.Id, (int)OrderStatus.PAID);
            if (!isUpdated)
            {
                return StatusCode(500, new ResponseModel<object>
                {
                    IsSuccess = false,
                    Message = "Failed to update order status.",
                    Data = null
                });
            }
            foreach(var item in order.OrderDetails)
            {
                if(item!= null)
                {
                  var productInWare = (await _warehouseService.GetWarehouseByProductId(item.ProductId)).FirstOrDefault();
                    productInWare.Quantity = productInWare.Quantity - item.Quantity;  
                }
            }
            _unitOfWork.Save();
            var redirectUrl = "https://twosport.vercel.app/order_success";
            return Redirect(redirectUrl);
        }

        [NonAction]
        public string GenerateOrderCode()
        {
            Random random = new Random();
            int randomDigits = random.Next(100000, 1000000);
            return randomDigits.ToString();
        }
        [NonAction]
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
        [NonAction]
        private async Task<User> GetUserFromToken()
        {
            var user = await _userService.GetAsync(_ => _.Id == GetCurrentUserIdFromToken());
            return user.FirstOrDefault();
        }
        [NonAction]
        protected async Task<Order> CreateOrder(OrderCM orderCM, int paymentMethodId)
        {
            var user = await GetUserFromToken();
            if (user == null)
            {
                throw new Exception("User not found");
            }
            var paymentMethod = await _paymentMethodService.GetPaymentMethodAsync(paymentMethodId);
            if (paymentMethod == null)
            {
                throw new Exception("Payment method not found");
            }
            var order = new Order
            {
                OrderCode = GenerateOrderCode(),
                Status = paymentMethodId == 1 ? (int?)OrderStatus.PROCESSING : (int?)OrderStatus.PENDING,
                TransportFee = orderCM.TransportFee,
                PaymentMethodId = paymentMethodId,
                PaymentMethod = paymentMethod,
                ShipmentDetailId = (int)orderCM.ShipmentDetailId,
                ReceivedDate = orderCM.ReceivedDate,
                UserId = GetCurrentUserIdFromToken(),
                User = user,
                OrderDetails = new List<OrderDetail>()
            };

            decimal totalPrice = 0;
            foreach (var item in orderCM.OrderDetails)
            {
                var product = await _productService.GetProductById((int)item.ProductId);
                if (product != null)
                {
                    var orderDetail = new OrderDetail
                    {
                        ProductId = product.Id,
                        Product = product,
                        Quantity = (int) item.Quantity,
                        Price = (int) item.Price,
                    };

                    order.OrderDetails.Add(orderDetail);
                    totalPrice += (decimal) (item.Price * item.Quantity);
                }
                else
                {
                    throw new Exception($"Product with ID {item.ProductId} not found");
                }
            }

            order.TotalPrice = totalPrice;
            order.IntoMoney = totalPrice + orderCM.TransportFee;
            await _orderService.AddOrderAsync(order);

            return order;
        }
        [NonAction]
        protected async Task<bool> DeleteCartItem(Cart cart, List<OrderDetailRequest> orderDetails)
        {
            if (orderDetails == null || !orderDetails.Any())
            {
                return false;
            }

            if (cart != null && cart.CartItems.Any())
            {
                bool allItemsDeleted = true;
                foreach (var orderDetail in orderDetails)
                {
                    var warehouses = await _warehouseService.GetWarehouseByProductId(orderDetail.ProductId);
                    Warehouse wareHouse = warehouses.FirstOrDefault();
                    if (wareHouse != null && wareHouse.Quantity >= orderDetail.Quantity)
                    {
                        var cartItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == orderDetail.ProductId && ci.Status == true);
                        if (cartItem != null)
                        {
                            await _cartItemService.DeleteCartItem(cartItem.Id);
                            /*wareHouse.Quantity -= orderDetail.Quantity;
                            await _warehouseService.UpdateWarehouseAsync(wareHouse);*/
                        }
                        else
                        {
                            allItemsDeleted = false;
                        }
                    }
                    else
                    {
                        allItemsDeleted = false;
                    }
                }

                if (allItemsDeleted)
                {
                    _unitOfWork.Save();
                    return true;
                }
            }
            return false;
        }
        [NonAction]
        public bool AreAnyStringsNullOrEmpty(PaymentResponse response)
        {
            return string.IsNullOrEmpty(response.Status) ||
                   string.IsNullOrEmpty(response.Code) ||
                   string.IsNullOrEmpty(response.Id) ||
                   string.IsNullOrEmpty(response.OrderCode);
        }
    }
}