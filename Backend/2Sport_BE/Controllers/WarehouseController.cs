using _2Sport_BE.DataContent;
using _2Sport_BE.Repository.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using _2Sport_BE.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using _2Sport_BE.Service.Services;
using _2Sport_BE.ViewModels;
using AutoMapper;

namespace _2Sport_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WarehouseController : ControllerBase
    {
        private readonly IWarehouseService _warehouseService;
        private readonly IProductService _productService;
        private readonly ISupplierService _supplierService;
        private readonly IMapper _mapper;
        public WarehouseController(IWarehouseService warehouseService,
                                   IProductService productService,
                                   ISupplierService supplierService,
                                   IMapper mapper)
        {
            _productService = productService;
            _supplierService = supplierService;
            _warehouseService = warehouseService;
            _mapper = mapper;
        }

        [HttpGet]
        [Route("list-all")]
        public async Task<IActionResult> ListAllAsync()
        {
            try
            {
                var query = (await _warehouseService.ListAllAsync()).Include(_ => _.Product).ToList();
                foreach (var item in query)
                {
                    if (item.ProductId != null)
                    {
                        item.Product = await _productService.GetProductById((int)item.ProductId);
                    }
                }
                var result = _mapper.Map<List<WarehouseVM>>(query);
                return Ok(new { total = result.Count(), data = result });
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        [HttpGet]
        [Route("search-by-product")]
        public async Task<IActionResult> SearchByProductName([FromQuery] string productName)
        {
            try
            {
                var query = await _warehouseService.GetWarehouse
                                                    (_ => _.Product.ProductName.ToLower().Contains(productName.ToLower()));
                var warehouses = query.Include(_ => _.Product).ToList();
                foreach (var item in warehouses)
                {
                    item.Product = await _productService.GetProductById((int)item.ProductId);
                }
                var result = _mapper.Map<List<WarehouseVM>>(warehouses);
                return Ok(new { total = result.Count(), data = result });
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        [HttpGet]
        [Route("get-by-product-id/{productId}")]
        public async Task<IActionResult> GetByProductId([FromQuery] int productId)
        {
            try
            {
                var query = await _warehouseService.GetWarehouse
                                                    (_ => _.Product.Id == productId);
                var warehouses = query.Include(_ => _.Product).ToList();
                foreach (var item in warehouses)
                {
                    item.Product = await _productService.GetProductById((int)item.ProductId);
                }
                var result = _mapper.Map<List<WarehouseVM>>(warehouses);
                return Ok(new { total = result.Count(), data = result });
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }
    }
}
