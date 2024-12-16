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
        private readonly IManagerService _managerService;
        private readonly IMapper _mapper;
        private static readonly char[] characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789".ToCharArray();
        public ImportHistoryController(IImportHistoryService importService, 
                                IWarehouseService warehouseService,
                                IProductService productService,
                                IManagerService managerService,
                                IMapper mapper)
        {
            _importService = importService;
            _warehouseService = warehouseService;
            _productService = productService;
            _managerService = managerService;
            _mapper = mapper;
        }

        [HttpGet]
        [Route("list-all-import-histories")]
        public async Task<IActionResult> ListAllAsync()
        {
            try
            {
                var query = (await _importService.ListAllAsync()).Include("Product")
                                                                 .Include("Manager");

                var result = _mapper.Map<List<ImportVM>>(query);
                foreach (var item in result)
                {
                    item.ManagerName = (await _managerService.GetManagerDetailsByIdAsync(item.ManagerId))
                                            .Data.UserVM.FullName;
                }
                return Ok(new { total = result.Count(), data = result });
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        [HttpGet]
        [Route("list-all-import-histories-by-branchId/{branchId}")]
        public async Task<IActionResult> ListAllAsyncByBranchId(int branchId)
        {
            try
            {
                var query = (await _importService.ListImportHistoriesByBranchId(branchId))
                                                .Include(_ => _.Product).ToList();
                
                var result = _mapper.Map<List<ImportVM>>(query);
                foreach (var item in result)
                {
                    item.ManagerName = (await _managerService.GetManagerDetailsByIdAsync(item.ManagerId))
                                            .Data.UserVM.FullName;
                }
                return Ok(new { total = result.Count(), data = result });
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        [HttpDelete]
        [Route("delete-import-history/{importHistoryId}")]
        public async Task<IActionResult> DeleteImportHistories(int importHistoryId)
        {
            try
            {
                await _importService.DeleteImportHistoryAsync(importHistoryId);
                return Ok("Delete import histories successfully!");
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }

        }
    }
}
