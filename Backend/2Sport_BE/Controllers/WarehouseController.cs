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
    public class WarehouseController : ControllerBase
    {
        private readonly IWarehouseService _warehouseService;
        private readonly IProductService _productService;
        private readonly IBranchService _branchService;
        private readonly IMapper _mapper;
        public WarehouseController(IWarehouseService warehouseService,
                                   IProductService productService,
                                   IBranchService branchService,
                                   IMapper mapper)
        {
            _productService = productService;
            _warehouseService = warehouseService;
            _branchService = branchService;
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
                    if (item.ProductId > 0)
                    {
                        item.Product = await _productService.GetProductById((int)item.ProductId);
                    }

                    if (item.BranchId > 0)
                    {
                        item.Branch = await _branchService.GetBranchById((int)item.BranchId);
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
        [Route("list-products-of-branch/{branchId}")]
        public async Task<IActionResult> GetProductsOfBranch(int branchId)
        {
            var query = (await _warehouseService.GetProductsOfBranch(branchId)).ToList();
            foreach (var item in query)
            {
                if (item.ProductId > 0)
                {
                    item.Product = await _productService.GetProductById((int)item.ProductId);
                }

                if (item.BranchId > 0)
                {
                    item.Branch = await _branchService.GetBranchById((int)item.BranchId);
                }
            }
            var result = _mapper.Map<List<WarehouseVM>>(query.ToList());
            return Ok(new { total = result.Count, data = result });
        }

        [HttpGet]
        [Route("quantity-of-product/{productId}")]
        public async Task<IActionResult> GetQuantityOfProducts(int productId)
        {
            var query = (await _warehouseService.GetWarehouseByProductId(productId)).ToList();
            var availableQuantity = 0;
            var totalQuantity = 0;
            foreach (var item in query)
            {
                availableQuantity += (int)item.AvailableQuantity;
                totalQuantity += (int)item.TotalQuantity;
            }

            var product = await _productService.GetProductById(productId);
            var result = _mapper.Map<ProductVM>(product);
            return Ok(new { data = result, TotalQuantity = totalQuantity, AvailableQuantity = availableQuantity });
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

        [HttpDelete]
        [Route("delete-warehouse/{productId}")]
        public async Task<IActionResult> DeleteWarehouse(int productId)
        {
            try
            {
                var deletedWarehouses = (await _warehouseService.GetWarehouseByProductId(productId)).ToList();
                if (deletedWarehouses.Count <= 0)
                {
                    return BadRequest($"Cannot find warehouses with product id: {productId}");
                }
                await _warehouseService.DeleteWarehouseAsync(deletedWarehouses);
                return Ok("Delete warehouse successfully!");
            } catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }

        }


        [HttpPut]
        [Route("update-quantity-of-warehouse/{warehouseId}")]
        public async Task<IActionResult> UpdateQuantityOfWarehouse(int warehouseId, int quantity)
        {
            try
            {
                var updatedWarehouses = (await _warehouseService.GetWarehouseById(warehouseId)).FirstOrDefault();
                if (updatedWarehouses is null)
                {
                    return BadRequest($"Cannot find warehouses with warehouse id: {warehouseId}");
                }
                var distanceQuantity = updatedWarehouses.TotalQuantity - updatedWarehouses.AvailableQuantity;

                updatedWarehouses.TotalQuantity = quantity;
                updatedWarehouses.AvailableQuantity = quantity - distanceQuantity;
                await _warehouseService.UpdateWarehouseAsync(updatedWarehouses);
                return Ok("Update warehouse successfully!");
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }

        }
    }
}
