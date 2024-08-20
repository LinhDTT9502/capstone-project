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
    public class ImportController : ControllerBase
    {
        private readonly IImportHistoryService _importService;
        private readonly IWarehouseService _warehouseService;
        private readonly IProductService _productService;
        private readonly ISupplierService _supplierService;
        private readonly IMapper _mapper;
        private static readonly char[] characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789".ToCharArray();
        public ImportController(IImportHistoryService importService, 
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

        [HttpPost]
        [Route("import-product")]
        public async Task<IActionResult> ImportProduct(ImportCM importCM)
        {
            var addedImport = _mapper.Map<ImportHistory>(importCM);
            addedImport.ImportDate = DateTime.Now;
            var randomText = GenerateRandomText(10);
            addedImport.LotCode = $"LOT_{DateTime.Now}_{randomText}";
            addedImport.ImportCode = $"IM_{DateTime.Now}_{randomText}";
            var existedWareHouse = (await _warehouseService.GetWarehouseByProductId(importCM.ProductId)).FirstOrDefault();
            try
            {
                if (existedWareHouse == null)
                {
                    var newWareHouse = new Warehouse()
                    {
                        ProductId = importCM.ProductId,
                        Quantity = importCM.Quantity,
                    };
                    await _warehouseService.CreateANewWarehouseAsync(newWareHouse);
                }
                else
                {
                    existedWareHouse.Quantity += importCM.Quantity;
                    await _warehouseService.UpdateWarehouseAsync(existedWareHouse);
                }
                await _importService.CreateANewImportHistoryAsync(addedImport);
                return Ok("Import product successfully!");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Method to generate random text
        [NonAction]
        public static string GenerateRandomText(int length)
        {
            if (length <= 0)
                throw new ArgumentException("Length must be a positive integer.");

            // StringBuilder is more efficient for concatenating multiple characters
            StringBuilder result = new StringBuilder(length);

            // Create an instance of the Random class
            Random random = new Random();

            // Generate each character randomly
            for (int i = 0; i < length; i++)
            {
                // Pick a random character from the characters array
                char randomChar = characters[random.Next(characters.Length)];
                result.Append(randomChar);
            }

            return result.ToString();
        }
    }
}
