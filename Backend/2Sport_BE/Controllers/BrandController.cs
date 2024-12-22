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
using System.Security.Claims;

namespace _2Sport_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BrandController : ControllerBase
    {
        private readonly IBrandService _brandService;
        private readonly IProductService _productService;
        private readonly IWarehouseService _warehouseService;
        private readonly IUserService _userService;
        private readonly IImageService _imageService;
        private readonly IAdminService _adminService;
        private readonly IManagerService _managerService;
        private readonly IMapper _mapper;
        public BrandController(IBrandService brandService, IProductService productService,
                               IWarehouseService warehouseService,
                               IImageService imageService,
                               IUserService userService,
                               IManagerService managerService,
                               IAdminService adminService,
                               IMapper mapper)
        {
            _brandService = brandService;
            _productService = productService;
            _warehouseService = warehouseService;
            _imageService = imageService;
            _managerService = managerService;
            _adminService = adminService;
            _mapper = mapper;
            _userService = userService;
        }
        [HttpGet]
        [Route("list-all")]
        public async Task<IActionResult> ListAllAsync()
        {
            try
            {
                var brands = await _brandService.ListAllAsync();
                var warehouses = (await _warehouseService.GetAvailableWarehouse());
                var products = new List<Product>();
                foreach (var item in warehouses)
                {
                    products.Add(await _productService.GetProductByProductCode(item.ProductCode));
                }

                foreach (var item in brands.ToList())
                {
                    item.Quantity = 0;
                    foreach (var product in products)
                    {
                        if (product.BrandId == item.Id)
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


        [HttpGet]
        [Route("get-brand-by-id/{brandId}")]
        public async Task<IActionResult> GetBrandById(int brandId)
        {
            try
            {

                var brand = (await _brandService.GetBrandById(brandId)).FirstOrDefault();
                if (brand is not null)
                {
                    var warehouses = (await _warehouseService.GetAvailableWarehouse());
                    var products = new List<Product>();
                    foreach (var item in warehouses)
                    {
                        products.Add(await _productService.GetProductByProductCode(item.ProductCode));
                    }
                    brand.Quantity = 0;
                    foreach (var product in products)
                    {
                        if (product.BrandId == brand.Id)
                        {
                            brand.Quantity += 1;
                        }
                    }
                    var result = _mapper.Map<BrandVM>(brand);
                    return Ok(result);
                }
                return Ok(new { data = new Brand() });
                
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

                var userId = GetCurrentUserIdFromToken();

                if (userId == 0)
                {
                    return Unauthorized();
                }

                var manager = await _managerService.GetManagerDetailsByIdAsync(userId);
                var admin = await _adminService.GetAdminDetailAsync(userId);


                //Add brand under manager role
                if (manager.Data != null || admin.Data != null)
                {
                    var existedBrand = (await _brandService.GetBrandsAsync(Name)).FirstOrDefault();
                    if (existedBrand != null)
                    {
                        return BadRequest("The brand already exists!");
                    }

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

                }
                else
                {
                    return Unauthorized("You are not allowed to add brand!");
                }

                return Ok("Add brand successfully!");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
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

        [HttpDelete]
        [Route("delete-brand/{brandId}")]
        public async Task<IActionResult> DeleteBrand(int brandId)
        {
            try
            {
                var deletedBrand =  (await _brandService.GetBrandById(brandId)).FirstOrDefault();
                if (deletedBrand != null)
                {
                    deletedBrand.Status = !deletedBrand.Status;
                    await _brandService.UpdateBrandAsync(deletedBrand);
                    var deletedProducts = await _productService.GetProducts(_ => _.BrandId == brandId);
                    if (deletedProducts != null)
                    {
                        foreach (var product in deletedProducts)
                        {
                            await _productService.DeleteProductById(product.Id);
                        }
                    }
                    return Ok($"Delete brand with id: {brandId}!");
                }
                return BadRequest($"Cannot find brand with id {brandId}!");
            } catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
            
        }

        protected int GetCurrentUserIdFromToken()
        {
            int UserId = 0;
            try
            {
                if (HttpContext.User.Identity.IsAuthenticated)
                {
                    var identity = HttpContext.User.Identity as ClaimsIdentity;
                    if (identity != null)
                    {
                        IEnumerable<Claim> claims = identity.Claims;
                        string strUserId = identity.FindFirst("UserId").Value;
                        int.TryParse(strUserId, out UserId);

                    }
                }
                return UserId;
            }
            catch
            {
                return UserId;
            }
        }
    }
}
