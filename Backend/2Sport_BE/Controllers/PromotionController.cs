using _2Sport_BE.Helpers;
using _2Sport_BE.Service.Services;
using _2Sport_BE.ViewModels;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace _2Sport_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PromotionController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IMapper _mapper;

        public PromotionController(IProductService productService, IMapper mapper)
        {
            _productService = productService;
            _mapper = mapper;
        }

        [HttpGet]
        [Route("get-all-promotions")]
        public async Task<IActionResult> GetAllPromotions([FromQuery] DefaultSearch defaultSearch)
        {
            try
            {
                var query = await _productService.GetProducts(_ => _.Discount > 0, null, "",
                                                                defaultSearch.currentPage, defaultSearch.perPage);

                // Group by ProductCode and ProductName, then select the first item in each group
                var distinctProducts = query
                    .GroupBy(p => new { p.ProductCode, p.ProductName })
                    .Select(g => g.First());
                var result = _mapper.Map<List<ProductVM>>(distinctProducts.ToList());
                return Ok(new { total = distinctProducts.Count(), data = result });
            } catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
            
        }

        [HttpPost]
        [Route("create-and-update-promotion/{productCode}")]
        public async Task<IActionResult> CreateAndUpdatePromotion(string productCode, int percentDiscount)
        {
            try
            {
                var query = await _productService.GetProductsByProductCode(productCode);

                foreach (var product in query)
                {
                    product.Discount = percentDiscount;
                    await _productService.UpdateProduct(product);
                }
                return Ok("Create/Update discount for product successfully!");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }
    }
}
