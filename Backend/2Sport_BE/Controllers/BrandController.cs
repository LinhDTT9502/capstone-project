using _2Sport_BE.DataContent;
using _2Sport_BE.Repository.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using _2Sport_BE.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using _2Sport_BE.Service.Services;
using _2Sport_BE.ViewModels;
using AutoMapper;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace _2Sport_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BrandController : ControllerBase
    {
        private readonly IBrandService _brandService;
        private readonly IProductService _productService;
        private readonly IWarehouseService _warehouseService;
        private readonly IMapper _mapper;
        public BrandController(IBrandService brandService, IProductService productService, 
                               IWarehouseService warehouseService,
                               IMapper mapper)
        {
            _brandService = brandService;
            _productService = productService;
            _warehouseService = warehouseService;
            _mapper = mapper;

        }
        [HttpGet]
        [Route("list-all")]
        public async Task<IActionResult> ListAllAsync()
        {
            try
            {
                var brands = await _brandService.ListAllAsync();
                var warehouses = (await _warehouseService.GetWarehouse(_ => _.Quantity > 0)).Include(_ => _.Product).ToList();
                foreach (var item in warehouses)
                {
                    item.Product = await _productService.GetProductById((int)item.ProductId);
                }
                
                foreach (var item in brands.ToList())
                {
                    item.Quantity = 0;
                    foreach (var productInWarehouse in warehouses)
                    {
                        if (productInWarehouse.Product.BrandId == item.Id)
                        {
                            item.Quantity += 1;
                        }
                    }
                }
                var result = _mapper.Map<List<BrandVM>>(brands.ToList());
                return Ok(new { total = result.Count(), data = result });
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        [HttpPost]
        [Route("add-brand")]
        public async Task<IActionResult> AddBrand(BrandCM brandCM)
        {
            var addedBrand = _mapper.Map<Brand>(brandCM);
            try
            {
                await _brandService.CreateANewBrandAsync(addedBrand);
                return Ok("Add brand successfully!");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
