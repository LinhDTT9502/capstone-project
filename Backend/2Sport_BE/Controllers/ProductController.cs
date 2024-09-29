﻿using _2Sport_BE.Helpers;
using _2Sport_BE.Infrastructure.Services;
using _2Sport_BE.Repository.Interfaces;
using _2Sport_BE.Repository.Models;
using _2Sport_BE.Service.Services;
using _2Sport_BE.Services;
using _2Sport_BE.ViewModels;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Bcpg.OpenPgp;
using System.Linq;
using System.Net.WebSockets;
using System.Security.Claims;

namespace _2Sport_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IBrandService _brandService;
        private readonly IBranchService _branchService;
        private readonly ICategoryService _categoryService;
        private readonly ISportService _sportService;
        private readonly ILikeService _likeService;
        private readonly IReviewService _reviewService;
        private readonly IImageService _imageService;
        private readonly IWarehouseService _warehouseService;
        private readonly IImageVideosService _imageVideosService;
        private readonly IImportHistoryService _importHistoryService;
        private readonly IUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;

        public ProductController(IProductService productService, 
                                IBrandService brandService, 
                                IBranchService branchService, 
                                ICategoryService categoryService,
                                IUnitOfWork unitOfWork,
								ISportService sportService,
								ILikeService likeService,
								IReviewService reviewService,
                                IWarehouseService warehouseService,
                                IImageService imageService,
                                IImageVideosService imageVideosService,
                                IImportHistoryService importHistoryService,
                                IMapper mapper)
        {
            _productService = productService;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _brandService = brandService;
            _branchService = branchService;
            _categoryService = categoryService;
            _sportService = sportService;
            _likeService = likeService;
            _reviewService = reviewService;
            _warehouseService = warehouseService;
            _imageService = imageService;
            _imageVideosService = imageVideosService;
            _importHistoryService = importHistoryService;
        }

        [HttpGet]
        [Route("get-product/{productId}")]
        public async Task<IActionResult> GetProductById(int productId)
        {
            try
            {
                var product = await _productService.GetProductById(productId);
                var productVM = _mapper.Map<ProductVM>(product);
				var reviews = await _reviewService.GetReviewsOfProduct(product.Id);
				productVM.Reviews = reviews.ToList();
				var numOfLikes = await _likeService.CountLikeOfProduct(productId);
				productVM.Likes = numOfLikes;
				return Ok(productVM);
            } catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet]
        [Route("list-products")]
        public async Task<IActionResult> GetProducts([FromQuery] DefaultSearch defaultSearch)
        {
            try
            {
                var query = await _productService.GetProducts(_ => _.Status == true, null, "", defaultSearch.currentPage, defaultSearch.perPage);
                var products = query.ToList();
                foreach(var product in products)
                {
                    var brand = await _brandService.GetBrandById(product.BrandId);
                    product.Brand = brand.FirstOrDefault();
                    var category = await _categoryService.GetCategoryById(product.CategoryId);
                    product.Category = category;
					var sport = await _sportService.GetSportById(product.SportId);
					product.Sport = sport;
                }
                var result = products.Select(_ => _mapper.Map<Product, ProductVM>(_)).ToList();
                foreach (var product in result)
                {
					var reviews = await _reviewService.GetReviewsOfProduct(product.Id);
					product.Reviews = reviews.ToList();
					var numOfLikes = await _likeService.CountLikeOfProduct(product.Id);
                    product.Likes = numOfLikes;
                }
                return Ok(new { total = result.Count, data = result });
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet]
        [Route("filter-sort-products")]
        public async Task<IActionResult> FilterSortProducts([FromQuery]DefaultSearch defaultSearch, [FromQuery] string? size, 
                                                            [FromQuery] decimal minPrice, [FromQuery] decimal maxPrice,
                                                        [FromQuery] int sportId, [FromQuery] int[] brandIds, [FromQuery] int[] categoryIds)
        {
            try
            {
                var query = await _productService.GetProducts(_ => _.Status == true, "", defaultSearch.currentPage, defaultSearch.perPage);
                if (sportId != 0)
                {
                    query = query.Where(_ => _.SportId == sportId);
                }
                if (!String.IsNullOrEmpty(size))
                {
                    query = query.Where(_ => _.Size.ToLower().Equals(size.ToLower()));
                }
                if (minPrice >= 0 && maxPrice > 0)
                {
                    if (minPrice < maxPrice)
                    {
                        query = query.Where(_ => _.Price > minPrice && _.Price < maxPrice);
                    }
                    else
                    {
                        return BadRequest("Invalid query!");
                    }
                }
                // Apply brandIds filter if provided
                if (brandIds.Length > 0)
                {
                    query = query.Where(_ => brandIds.ToList().Contains((int)_.BrandId));
                }

                // Apply categoryIds filter if provided
                if (categoryIds.Length > 0)
                {
                    query = query.Where(_ => categoryIds.ToList().Contains((int)_.CategoryId));
                }

                var products = query.ToList();

                foreach (var product in products)
				{
					var brand = await _brandService.GetBrandById(product.BrandId);
					product.Brand = brand.FirstOrDefault();
					var category = await _categoryService.GetCategoryById(product.CategoryId);
					product.Category = category;
					var sport = await _sportService.GetSportById(product.SportId);
					product.Sport = sport;
                }

				var result = query.Sort(defaultSearch.sortBy, defaultSearch.isAscending)
                                  .Select(_ => _mapper.Map<Product, ProductVM>(_))
                                  .ToList();

				foreach (var product in result)
				{
                    var reviews = await _reviewService.GetReviewsOfProduct(product.Id);
                    product.Reviews = reviews.ToList();
					var numOfLikes = await _likeService.CountLikeOfProduct(product.Id);
					product.Likes = numOfLikes;
				}

				return Ok(new { total = result.Count, data = result });
            } catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet]
        [Route("sort-products-by-like")]
        public async Task<IActionResult> SortProductsByLike([FromQuery] int pageIndex, [FromQuery] int pageSize)
        {
            try
            {
                var query = await _productService.GetProducts(_ => _.Status == true, "", pageIndex, pageSize);
                var products = query.ToList();
                foreach (var product in products)
                {
                    var brand = await _brandService.GetBrandById(product.BrandId);
                    product.Brand = brand.FirstOrDefault();
                    var category = await _categoryService.GetCategoryById(product.CategoryId);
                    product.Category = category;
                    var sport = await _sportService.GetSportById(product.SportId);
                    product.Sport = sport;

                }

                var result = query.Select(_ => _mapper.Map<Product, ProductVM>(_)).ToList().AsQueryable();

                foreach (var product in result)
                {
                    var reviews = await _reviewService.GetReviewsOfProduct(product.Id);
                    product.Reviews = reviews.ToList();
                    var numOfLikes = await _likeService.CountLikeOfProduct(product.Id);
                    product.Likes = numOfLikes;
                }

                var finalResult = result.Sort("likes", false).ToList();

                return Ok(new { total = finalResult.Count, data = finalResult });
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }


        [HttpGet]
		[Route("search-products")]
		public async Task<IActionResult> SearchProducts([FromQuery] string keywords, [FromQuery] DefaultSearch defaultSearch)
		{
			try
			{
				var query = await _productService.GetProducts(_ => _.Status == true && 
                                                                (_.ProductName.ToLower().Contains(keywords.ToLower()) ||
                                                                _.ProductCode.ToLower().Contains(keywords.ToLower()))
                                                                , "", defaultSearch.currentPage, defaultSearch.perPage);

				var products = query.ToList();
				foreach (var product in products)
				{
					var brand = await _brandService.GetBrandById(product.BrandId);
					product.Brand = brand.FirstOrDefault();
					var category = await _categoryService.GetCategoryById(product.CategoryId);
					product.Category = category;
					var sport = await _sportService.GetSportById(product.SportId);
					product.Sport = sport;
                }

				var result = query.Sort(defaultSearch.sortBy, defaultSearch.isAscending)
								  .Select(_ => _mapper.Map<Product, ProductVM>(_))
								  .ToList();

				foreach (var product in result)
				{
					var reviews = await _reviewService.GetReviewsOfProduct(product.Id);
					product.Reviews = reviews.ToList();
					var numOfLikes = await _likeService.CountLikeOfProduct(product.Id);
					product.Likes = numOfLikes;
				}

				return Ok(new { total = result.Count, data = result });
			}
			catch (Exception ex)
			{
				return BadRequest(ex);
			}
		}

        [HttpPost]
        [Route("import-product")]
        public async Task<IActionResult> ImportProduct(ProductCM productCM)
        {
            var existedProduct = await _productService.GetProductByProductCode(productCM.ProductCode);


            var product = _mapper.Map<Product>(productCM);
            product.CreateAt = DateTime.Now;
            product.Status = true;
            try
            {
                var userId = GetCurrentUserIdFromToken();

                if (userId == 0)
                {
                    return Unauthorized();
                }

                if (existedProduct == null)
                {
                    if (productCM.MainImage != null)
                    {
                        var uploadResult = await _imageService.UploadImageToCloudinaryAsync(productCM.MainImage);
                        if (uploadResult != null && uploadResult.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            product.ImgAvatarPath = uploadResult.SecureUrl.AbsoluteUri;
                        }
                        else
                        {
                            return BadRequest("Something wrong!");
                        }
                    }
                    else
                    {
                        product.ImgAvatarName = "";
                        product.ImgAvatarPath = "";
                    }

                    var addedProduct = await _productService.AddProduct(product);

                    //Add product's images into ImageVideo table
                    if (productCM.ProductImages.Length > 0)
                    {
                        foreach (var image in productCM.ProductImages)
                        {
                            var uploadResult = await _imageService.UploadImageToCloudinaryAsync(image);
                            if (uploadResult != null && uploadResult.StatusCode == System.Net.HttpStatusCode.OK)
                            {
                                var imageObject = new ImagesVideo()
                                {
                                    ProductId = product.Id,
                                    ImageUrl = uploadResult.SecureUri.AbsoluteUri,
                                    CreateAt = DateTime.Now,
                                    VideoUrl = null,
                                    BlogId = null,
                                };
                                await _imageVideosService.AddImage(imageObject);
                            }
                            else
                            {
                                return BadRequest("Something wrong!");
                            }
                        }
                    }
                }

                //Import product into warehouse
                if (existedProduct != null)
                {
                    var existedWarehouse = (await _warehouseService.GetWarehouseByProductIdAndBranchId(existedProduct.Id,
                                                                                   productCM.BranchId))
                                                                                   .FirstOrDefault();
                    if (existedWarehouse != null)
                    {
                        existedWarehouse.Quantity += productCM.Quantity;
                        await _warehouseService.UpdateWarehouseAsync(existedWarehouse);
                    }
                    product = existedProduct;
                } 
                else
                {
                    var warehouse = new Warehouse()
                    {
                        BranchId = productCM.BranchId,
                        ProductId = product.Id,
                        Quantity = productCM.Quantity,
                    };
                    await _warehouseService.CreateANewWarehouseAsync(warehouse);
                }



                //Save import history
                var importedBranch = await _branchService.GetBranchById(productCM.BranchId);
                var importHistory = new ImportHistory()
                {
                    UserId = userId,
                    ProductId = product.Id,
                    Content = $@"{importedBranch.BranchName}: Import {productCM.Quantity} {productCM.ProductName} ({productCM.ProductCode})",
                    ImportDate = DateTime.Now,
                    Quantity = productCM.Quantity,
                    SupplierId = productCM.SupplierId,
                    LotCode = productCM.LotCode,
                };
                await _importHistoryService.CreateANewImportHistoryAsync(importHistory);

                return Ok("Add product successfully!");
            }
                
            catch (Exception e)
            {
                return BadRequest(e);
            }

        }

        [HttpPost]
        [Route("import-product-list")]
        public async Task<IActionResult> ImportProductList(List<ProductCM> productList)
        {
            try
            {
                foreach (var productCM in productList)
                {
                    var existedProduct = await _productService.GetProductByProductCode(productCM.ProductCode);

                    var product = _mapper.Map<Product>(productCM);
                    product.CreateAt = DateTime.Now;
                    product.Status = true;
                    try
                    {
                        var userId = GetCurrentUserIdFromToken();

                        if (userId == 0)
                        {
                            return Unauthorized();
                        }

                        if (existedProduct == null)
                        {
                            if (productCM.MainImage != null)
                            {
                                var uploadResult = await _imageService.UploadImageToCloudinaryAsync(productCM.MainImage);
                                if (uploadResult != null && uploadResult.StatusCode == System.Net.HttpStatusCode.OK)
                                {
                                    product.ImgAvatarPath = uploadResult.SecureUrl.AbsoluteUri;
                                }
                                else
                                {
                                    return BadRequest("Something wrong!");
                                }
                            }
                            else
                            {
                                product.ImgAvatarName = "";
                                product.ImgAvatarPath = "";
                            }

                            var addedProduct = await _productService.AddProduct(product);

                            //Add product's images into ImageVideo table
                            if (productCM.ProductImages.Length > 0)
                            {
                                foreach (var image in productCM.ProductImages)
                                {
                                    var uploadResult = await _imageService.UploadImageToCloudinaryAsync(image);
                                    if (uploadResult != null && uploadResult.StatusCode == System.Net.HttpStatusCode.OK)
                                    {
                                        var imageObject = new ImagesVideo()
                                        {
                                            ProductId = product.Id,
                                            ImageUrl = uploadResult.SecureUri.AbsoluteUri,
                                            CreateAt = DateTime.Now,
                                            VideoUrl = null,
                                            BlogId = null,
                                        };
                                        await _imageVideosService.AddImage(imageObject);
                                    }
                                    else
                                    {
                                        return BadRequest("Something wrong!");
                                    }
                                }
                            }
                        }

                        //Import product into warehouse
                        if (existedProduct != null)
                        {
                            var existedWarehouse = (await _warehouseService.GetWarehouseByProductIdAndBranchId(existedProduct.Id,
                                                                                           productCM.BranchId))
                                                                                           .FirstOrDefault();
                            if (existedWarehouse != null)
                            {
                                existedWarehouse.Quantity += productCM.Quantity;
                                await _warehouseService.UpdateWarehouseAsync(existedWarehouse);
                            }
                        }
                        else
                        {
                            var warehouse = new Warehouse()
                            {
                                BranchId = productCM.BranchId,
                                ProductId = product.Id,
                                Quantity = productCM.Quantity,
                            };
                            await _warehouseService.CreateANewWarehouseAsync(warehouse);
                        }

                        //Save import history
                        var importedBranch = await _branchService.GetBranchById(productCM.BranchId);
                        var importHistory = new ImportHistory()
                        {
                            UserId = userId,
                            ProductId = product.Id,
                            Content = $@"{importedBranch.BranchName}: Import {productCM.Quantity} {productCM.ProductName} ({productCM.ProductCode})",
                            ImportDate = DateTime.Now,
                            Quantity = productCM.Quantity,
                            SupplierId = productCM.SupplierId,
                            LotCode = productCM.LotCode,
                        };
                        await _importHistoryService.CreateANewImportHistoryAsync(importHistory);

                    }

                    catch (Exception e)
                    {
                        return BadRequest(e);
                    }
                }
                
                return Ok("Add products successfully!");

            }
            catch (Exception e)
            {
                return BadRequest(e);
            }

        }

        [HttpPut]
        [Route("update-product/{productId}")]
        public async Task<IActionResult> UpdateProduct(int productId, ProductUM productUM)
        {
            try
            {
                var updatedProduct = await _productService.GetProductById(productId);
                if (updatedProduct == null)
                {
                    return BadRequest($"Cannot find the product with id: {productId}");
                }
                else
                {
                    var userId = GetCurrentUserIdFromToken();

                    if (userId == 0)
                    {
                        return Unauthorized();
                    }

                    if (productUM.MainImage != null)
                    {
                        var uploadResult = await _imageService.UploadImageToCloudinaryAsync(productUM.MainImage);
                        if (uploadResult != null && uploadResult.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            updatedProduct.ImgAvatarPath = uploadResult.SecureUrl.AbsoluteUri;
                        }
                        else
                        {
                            return BadRequest("Something wrong!");
                        }
                    }

                    updatedProduct.ProductName = productUM.ProductName;
                    updatedProduct.ProductCode = productUM.ProductCode;
                    updatedProduct.BrandId = (int)productUM.BrandId;
                    updatedProduct.CategoryId = (int)productUM.CategoryId;
                    updatedProduct.SportId = (int)productUM.SportId;
                    await _productService.UpdateProduct(updatedProduct);

                    //Add product's images into ImageVideo table
                    if (productUM.ProductImages.Length > 0)
                    {
                        foreach (var image in productUM.ProductImages)
                        {
                            var uploadResult = await _imageService.UploadImageToCloudinaryAsync(image);
                            if (uploadResult != null && uploadResult.StatusCode == System.Net.HttpStatusCode.OK)
                            {

                                var imageObject = new ImagesVideo()
                                {
                                    ProductId = updatedProduct.Id,
                                    ImageUrl = uploadResult.SecureUri.AbsoluteUri,
                                    CreateAt = DateTime.Now,
                                    VideoUrl = null,
                                    BlogId = null,
                                };
                                await _imageVideosService.AddImage(imageObject);
                            }
                            else
                            {
                                return BadRequest("Something wrong!");
                            }
                        }
                    }


                    //Save import history
                    var importedBranch = await _branchService.GetBranchById(productUM.BranchId);
                    var importHistory = new ImportHistory()
                    {
                        UserId = userId,
                        ProductId = updatedProduct.Id,
                        Content = $@"{importedBranch.BranchName}: Updated {productUM.ProductName} ({productUM.ProductCode})",
                        ImportDate = DateTime.Now,
                        Quantity = productUM.Quantity,
                        SupplierId = productUM.SupplierId,
                        LotCode = productUM.LotCode,
                    };
                    await _importHistoryService.CreateANewImportHistoryAsync(importHistory);
                    return Ok($"Update product with id: {productId}");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
            
        }

        [HttpDelete]
        [Route("delete-product/{productId}")]
        public async Task<IActionResult> DeleteProduct(int productId)
        {
            try
            {
                await _productService.DeleteProductById(productId);
                return Ok("Delete product successfully!");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete]
        [Route("active-deactive-product/{productId}")]
        public async Task<IActionResult> ActiveDeactiveProduct(int productId)
        {
            try
            {
                var deletedProduct = await _productService.GetProductById(productId);
                deletedProduct.Status = false;
                await _productService.UpdateProduct(deletedProduct);
                _unitOfWork.Save();
                return Ok("Delete product successfully!");
            } catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpGet]
        [Route("get-number-of-products")]
        public async Task<IActionResult> GetNumberOfProducts()
        {
            try
            {
                var totalProducts = (await _productService.GetAllProducts()).Count();
                return Ok(totalProducts);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
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
