using _2Sport_BE.Repository.Models;
using _2Sport_BE.Service.Services;
using _2Sport_BE.ViewModels;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace _2Sport_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderDetailController : Controller
    {
        private readonly IOrderDetailService _orderDetailService;
        private readonly IProductService _productService;
        private readonly IMapper _mapper;

        public OrderDetailController(IOrderDetailService orderDetailService, 
                                     IProductService productService, 
                                     IMapper mapper)
        {
            _orderDetailService = orderDetailService;
            _productService = productService;
            _mapper = mapper;
        }

        [HttpGet("{orderId}")]
        public async Task<IActionResult> GetOrderFromUser(int orderId)
        {
            var order = await _orderDetailService.GetOrderDetailByOrderIdAsync(orderId) ;

            if (order == null)
            {
                return NotFound();
            }
            List<OrderDetailDTO> result = new List<OrderDetailDTO>();
            for (int i = 0; i < order.Count; i++)
            {
                var product = await _productService.GetProductById((int)order[i].ProductId);
                result.Add(new OrderDetailDTO()
                {
                    Id = order[i].Id,
                    OrderId = order[i].SaleOrderId,
                    ProductId = order[i].ProductId,
                    ProductName = product.ProductName,
                    Quantity = order[i].Quantity,
                    Price = order[i].UnitPrice

                });
            }
            return Ok(result);
        }
    }
}
