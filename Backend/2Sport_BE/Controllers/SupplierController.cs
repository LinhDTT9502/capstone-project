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
    public class SupplierController : ControllerBase
    {
        private readonly ISupplierService _supplierService;
        private readonly IProductService _productService;
        private readonly IMapper _mapper;
        public SupplierController(ISupplierService supplierService, IProductService productService, IMapper mapper)
        {
            _supplierService = supplierService;
            _productService = productService;
            _mapper = mapper;
        }
        [HttpGet]
        [Route("list-all")]
        public async Task<IActionResult> ListAllAsync()
        {
            try
            {
                var result = await _supplierService.ListAllAsync();
                return Ok(new { total = result.Count(), data = result });
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        [HttpPost]
        [Route("add-supplier")]
        public async Task<IActionResult> Addsupplier(SupplierCM supplierCM)
        {
            var addedSupplier = _mapper.Map<Supplier>(supplierCM);
            try
            {
                await _supplierService.CreateANewSupplierAsync(addedSupplier);
                return Ok("Add supplier successfully!");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
