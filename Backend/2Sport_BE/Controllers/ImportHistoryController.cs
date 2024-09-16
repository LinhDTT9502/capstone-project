using _2Sport_BE.DataContent;
using _2Sport_BE.Repository.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using _2Sport_BE.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using _2Sport_BE.Service.Services;
using _2Sport_BE.ViewModels;
using AutoMapper;
using System.Text;

namespace _2Sport_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImportHistoryController : ControllerBase
    {
        private readonly IImportHistoryService _importService;
        private readonly IWarehouseService _warehouseService;
        private readonly IProductService _productService;
        private readonly ISupplierService _supplierService;
        private readonly IMapper _mapper;
        private static readonly char[] characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789".ToCharArray();
        public ImportHistoryController(IImportHistoryService importService, 
                                IWarehouseService warehouseService,
                                IProductService productService,
                                ISupplierService supplierService,
                                IMapper mapper)
        {
            _importService = importService;
            _warehouseService = warehouseService;
            _productService = productService;
            _supplierService = supplierService;
            _mapper = mapper;
        }
        [HttpGet]
        [Route("list-all-import-histories")]
        public async Task<IActionResult> ListAllAsync()
        {
            try
            {
                var query = (await _importService.ListAllAsync()).Include(_ => _.Product).ToList();
                foreach (var item in query)
                {
                    item.Product = await _productService.GetProductById((int)item.ProductId);
                    item.Supplier = (await _supplierService.GetSupplierById((int)item.SupplierId)).FirstOrDefault();
                }
                var result = _mapper.Map<List<ImportVM>>(query);
                return Ok(new { total = result.Count(), data = result });
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        [HttpDelete]
        [Route("delete-import-histories/{productId}")]
        public async Task<IActionResult> DeleteImportHistories(int productId)
        {
            try
            {
                var deletedImportHistories = (await _importService.GetImportHistorysAsync(productId)).ToList();
                if (deletedImportHistories.Count <= 0)
                {
                    return BadRequest($"Cannot find import histories with product id: {productId}");
                }
                await _importService.DeleteImportHistories(deletedImportHistories);
                return Ok("Delete import histories successfully!");
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }

        }
    }
}
