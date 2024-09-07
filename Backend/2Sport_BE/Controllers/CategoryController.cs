using _2Sport_BE.Infrastructure.Services;
using _2Sport_BE.Repository.Interfaces;
using _2Sport_BE.Repository.Models;
using _2Sport_BE.Service.Services;
using _2Sport_BE.ViewModels;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace _2Sport_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        private readonly IProductService _productService;
        private readonly IWarehouseService _warehouseService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public CategoryController(ICategoryService categoryService, IUnitOfWork unitOfWork,
									IProductService productService,
                                    IWarehouseService warehouseService,
                                    IMapper mapper)
        {
            _categoryService = categoryService;
            _productService = productService;
            _warehouseService = warehouseService;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet]
        [Route("list-categories")]
        public async Task<IActionResult> GetCategories()
        {
            try
            {
                var query = await _categoryService.GetAllCategories();
                var warehouses = (await _warehouseService.GetWarehouse(_ => _.Quantity > 0)).Include(_ => _.Product).ToList();
                foreach (var item in warehouses)
                {
                    item.Product = await _productService.GetProductById((int)item.ProductId);
                }
                foreach (var item in query.ToList())
				{
                    item.Quantity = 0;
                    foreach (var productInWarehouse in warehouses)
                    {
                        if (productInWarehouse.Product.CategoryId == item.Id)
                        {
                            item.Quantity += 1;
                        }
                    }
                }
				var categories = _mapper.Map<List<CategoryVM>>(query.ToList());
                return Ok(new { total = categories.Count, data = categories });
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPost]
        [Route("add-categories")]
        public async Task<IActionResult> AddCategories(List<Category> newCategories)
        {
            try
            {
                await _categoryService.AddCategories(newCategories);
                await _unitOfWork.SaveChanges();
                return Ok("Add new sports successfully!");
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPut]
        [Route("update-category")]
        public async Task<IActionResult> UpdateCategory(CategoryUM category)
        {
            try
            {
                var updatedCategory = _mapper.Map<CategoryUM, Category>(category);
                await _categoryService.UpdateCategory(updatedCategory);
                await _unitOfWork.SaveChanges();
                return Ok(updatedCategory);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpDelete]
        [Route("delete-category")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            try
            {
                await _categoryService.DeleteCategoryById(id);
                return Ok("Removed successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
    }
}
