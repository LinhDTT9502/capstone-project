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
using _2Sport_BE.Services;

namespace _2Sport_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BrandController : ControllerBase
    {
        private readonly IBrandService _brandService;
        private readonly IProductService _productService;
        private readonly IWarehouseService _warehouseService;
        private readonly IImageService _imageService;
        private readonly IMapper _mapper;
        public BrandController(IBrandService brandService, IProductService productService, 
                               IWarehouseService warehouseService,
                               IImageService imageService,
                               IMapper mapper)
        {
            _brandService = brandService;
            _productService = productService;
            _warehouseService = warehouseService;
            _imageService = imageService;
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
                if (brandCM.LogoImage != null)
                {
                    var uploadResult = await _imageService.UploadImageToCloudinaryAsync(brandCM.LogoImage);
                    if (uploadResult != null && uploadResult.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        addedBrand.Logo = uploadResult.SecureUrl.AbsoluteUri;
                    }
                    else
                    {
                        return BadRequest("Something wrong!");
                    }
                }
                await _brandService.CreateANewBrandAsync(addedBrand);
                return Ok("Add brand successfully!");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("update-brand/{brandId}")]
        public async Task<IActionResult> UpdateBrand(int brandId, BrandUM brandUM)
        {
            var updatedBrand = (await _brandService.GetBrandById(brandId)).FirstOrDefault();
            if (updatedBrand != null)
            {
                updatedBrand.BrandName = brandUM.BrandName;
                try
                {
                    if (brandUM.LogoImage != null)
                    {
                        var uploadResult = await _imageService.UploadImageToCloudinaryAsync(brandUM.LogoImage);
                        if (uploadResult != null && uploadResult.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            updatedBrand.Logo = uploadResult.SecureUrl.AbsoluteUri;
                        }
                        else
                        {
                            return BadRequest("Something wrong!");
                        }
                    }
                    await _brandService.UpdateBrandAsync(updatedBrand);
                    return Ok("Update brand successfully!");
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
            return BadRequest($"Cannot find brand with id: {brandId}");

        }

        [HttpPost]
        [Route("active-deactive-brand/{brandId}")]
        public async Task<IActionResult> ActiveDeactiveBrand(int brandId)
        {
            var deletedBrand = await (await _brandService.GetBrandById(brandId)).FirstOrDefaultAsync();
            if (deletedBrand != null)
            {
                if (deletedBrand.Status == true)
                {
                    deletedBrand.Status = false;
                }
                else
                {
                    deletedBrand.Status = true;
                }
                return Ok($"Delete brand with id: {brandId}!");
            }
            return BadRequest($"Cannot find brand with id {brandId}!");
        }
    }
}
