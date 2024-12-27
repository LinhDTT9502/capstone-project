using _2Sport_BE.Enums;
using _2Sport_BE.Helpers;
using _2Sport_BE.Infrastructure.Services;
using _2Sport_BE.Repository.Data;
using _2Sport_BE.Repository.Interfaces;
using _2Sport_BE.Repository.Models;
using _2Sport_BE.Service.Services;
using _2Sport_BE.Services;
using _2Sport_BE.ViewModels;
using AutoMapper;
using CloudinaryDotNet.Actions;
using EntityFrameworkCore.SqlServer.SimpleBulks.BulkInsert;
using ExcelDataReader;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Net.WebSockets;
using System.Runtime.InteropServices;
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
        private readonly IManagerService _managerService;
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
                                IManagerService managerService,
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
            _managerService = managerService;
        }

        [HttpGet]
        [Route("get-product/{productId}")]
        public async Task<IActionResult> GetProductById(int productId)
        {
            try
            {
                var product = await _productService.GetProductById(productId);

                var productVM = _mapper.Map<ProductVM>(product);

                var brand = await _brandService.GetBrandById(productVM.BrandId);
                productVM.BrandName = brand.FirstOrDefault().BrandName;
                var category = await _categoryService.GetCategoryById(productVM.CategoryID);
                productVM.CategoryName = category.CategoryName;
                var sport = await _sportService.GetSportById(productVM.SportId);
                productVM.SportName = sport.Name;
                var reviews = await _reviewService.GetReviewsOfProduct(product.ProductCode);
                productVM.Reviews = reviews.ToList();
                var numOfLikes = await _likeService.CountLikesOfProduct(product.ProductCode);
                productVM.Likes = numOfLikes;
                productVM.ListImages.Add(productVM.ImgAvatarPath);
                productVM.ListImages.Reverse();
                return Ok(productVM);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet]
        [Route("get-product-by-product-code/{productCode}")]
        public async Task<IActionResult> GetProductByProductCode(string productCode, string? color, string? size, int? condition)
        {
            try
            {
                var productsSameProductCode = await _productService.GetProductsByProductCode(productCode);
                if (!string.IsNullOrEmpty(color))
                {
                    productsSameProductCode = productsSameProductCode.Where(_ => _.Color.ToLower().Equals(color.ToLower()));
                }
                if (!string.IsNullOrEmpty(size))
                {
                    productsSameProductCode = productsSameProductCode.Where(_ => _.Size.ToLower().Equals(size.ToLower()));
                }
                if (condition > -1)
                {
                    productsSameProductCode = productsSameProductCode.Where(_ => _.Condition == condition);
                }
                var trueCount = 0;
                foreach (var product in productsSameProductCode)
                {
                    if (product.Status == true)
                    {
                        trueCount++;
                        break;
                    }
                }
                if (trueCount == 0)
                {
                    return Ok("Sold out!");
                }
                var productVMs = _mapper.Map<List<ProductVM>>(productsSameProductCode.ToList());
                foreach (var productVM in productVMs)
                {
                    var brand = await _brandService.GetBrandById(productVM.BrandId);
                    productVM.BrandName = brand.FirstOrDefault().BrandName;
                    var category = await _categoryService.GetCategoryById(productVM.CategoryID);
                    productVM.CategoryName = category.CategoryName;
                    var sport = await _sportService.GetSportById(productVM.SportId);
                    productVM.SportName = sport.Name;
                    var reviews = await _reviewService.GetReviewsOfProduct(productCode);
                    productVM.Reviews = reviews.ToList();
                    var numOfLikes = await _likeService.CountLikesOfProduct(productCode);
                    productVM.Likes = numOfLikes;
                    productVM.ListImages.Add(productVM.ImgAvatarPath);
                    productVM.ListImages.Reverse();
                }

                return Ok(productVMs);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet]
        [Route("list-colors-of-product/{productCode}")]
        public async Task<IActionResult> GetColorsOfProduct(string productCode)
        {
            var colors = await _productService.GetColorsOfProduct(productCode);
            return Ok(new { total = colors.Count, data = colors }); 
        }


        [HttpGet]
        [Route("list-sizes-of-product/{productCode}")]
        public async Task<IActionResult> GetSizesOfProduct(string productCode, string color)
        {
            var sizes = await _productService.GetSizesOfProduct(productCode, color);
            return Ok(new { total = sizes.Count, data = sizes });
        }

        [HttpGet]
        [Route("list-conditions-of-product/{productCode}")]
        public async Task<IActionResult> GetConditionsOfProduct(string productCode, string color, string size)
        {
            var conditions = await _productService.GetConditionsOfProduct(productCode, color, size);
            return Ok(new { total = conditions.Count, data = conditions });
        }


        [HttpGet]
        [Route("list-products")]
        public async Task<IActionResult> GetProducts([FromQuery] DefaultSearch defaultSearch)
        {
            try
            {
                var query = await _productService.GetProducts(_ => _.Id > 0, null, "ImagesVideos", defaultSearch.currentPage, defaultSearch.perPage);
                
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
                var result = _mapper.Map<List<Product>, List<ProductVM>>(query.ToList());
                foreach (var product in result)
                {
                    var numOfLikes = await _likeService.CountLikesOfProduct(product.ProductCode);
                    product.Likes = numOfLikes;
                    product.ListImages.Add(product.ImgAvatarPath);
                    product.ListImages.Reverse();
                }
                return Ok(new { total = result.Count, data = result });
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet]
        [Route("list-all-products")]
        public async Task<IActionResult> GetAllProducts([FromQuery] DefaultSearch defaultSearch)
        {
            try
            {
                var query = await _productService.GetAllProducts();

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
                var result = _mapper.Map<List<Product>, List<ProductVM>>(query.ToList());
                foreach (var product in result)
                {
                    var numOfLikes = await _likeService.CountLikesOfProduct(product.ProductCode);
                    product.Likes = numOfLikes;
                    product.ListImages.Add(product.ImgAvatarPath);
                    product.ListImages.Reverse();
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
        public async Task<IActionResult> FilterSortProducts([FromQuery] DefaultSearch defaultSearch, [FromQuery] string? size,
                                                            [FromQuery] decimal minPrice, [FromQuery] decimal maxPrice,
                                                        [FromQuery] int sportId, [FromQuery] int[] brandIds, [FromQuery] int[] categoryIds)
        {
            try
            {
                var query = await _productService.GetProducts(_ => _.Status == true, "", defaultSearch.currentPage, defaultSearch.perPage);

                query = query.GroupBy(_ => _.ProductCode).Select(_ => _.FirstOrDefault());
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
                    var reviews = await _reviewService.GetReviewsOfProduct(product.ProductCode);
                    product.Reviews = reviews.ToList();
                    var numOfLikes = await _likeService.CountLikesOfProduct(product.ProductCode);
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
                    var reviews = await _reviewService.GetReviewsOfProduct(product.ProductCode);
                    product.Reviews = reviews.ToList();
                    var numOfLikes = await _likeService.CountLikesOfProduct(product.ProductCode);
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
                var query = await _productService.SearchProducts(_ => _.Status == true &&
                                                                (_.ProductName.ToLower().Contains(keywords.ToLower()) ||
                                                                _.ProductCode.ToLower().Contains(keywords.ToLower())));

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
                    var reviews = await _reviewService.GetReviewsOfProduct(product.ProductCode);
                    product.Reviews = reviews.ToList();
                    var numOfLikes = await _likeService.CountLikesOfProduct(product.ProductCode);
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
        public async Task<IActionResult> ImportProduct([FromForm] ProductCM productCM)
        {
            var existedProduct = (await _productService.GetProductByProductCodeAndColor(productCM.ProductCode,
                                                                                       productCM.Color)).FirstOrDefault();


            var product = _mapper.Map<Product>(productCM);
            product.CreateAt = DateTime.Now;
            product.Status = true;
            product.IsRent = false;
            product.RentPrice = 0;
            if (productCM.Condition >= 80)
            {
                product.IsRent = true;
                product.RentPrice = Math.Round((decimal)(product.Price * (decimal)0.1 * productCM.Condition / 100));
            }
            try
            {
                var userId = GetCurrentUserIdFromToken();

                if (userId == 0)
                {
                    return Unauthorized();
                }

                var managerDetail = await _managerService.GetManagerDetailsByUserIdAsync(userId);


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
                                };
                                await _imageVideosService.AddImage(imageObject);
                            }
                            else
                            {
                                return BadRequest("Something wrong!");
                            }
                        }
                    }

                    //Import product into warehouse
                    var warehouse = new Warehouse()
                    {
                        BranchId = managerDetail.Data.BranchId,
                        ProductId = product.Id,
                        TotalQuantity = productCM.Quantity,
                        AvailableQuantity = productCM.Quantity
                    };
                    await _warehouseService.CreateANewWarehouseAsync(warehouse);

                }
                else
                {

                    //Import a new product if it is different size color and condition
                    var existedProductWithSizeColorCodition = (await _productService
                                                                .GetProductByProductCodeSizeColorCondition
                                                                (existedProduct.ProductCode,
                                                                productCM.Size,
                                                                productCM.Color,
                                                                productCM.Condition)).FirstOrDefault();
                    if (existedProductWithSizeColorCodition == null)
                    {
                        var newProduct = new Product(existedProduct);
                        newProduct.Id = 0;
                        newProduct.Size = productCM.Size;
                        newProduct.Color = productCM.Color;
                        newProduct.Condition = productCM.Condition;
                        newProduct.Price = productCM.Price;
                        newProduct.Height = productCM.Height;
                        newProduct.Weight = productCM.Weight;
                        newProduct.Length = productCM.Length;
                        newProduct.Width = productCM.Width;

                        var existedProductWithProductCodeAndColor = (await _productService
                                                            .GetProducts(_ => _.ProductCode.Equals(existedProduct.ProductCode)
                                                            && _.Color.ToLower().Equals((productCM.Color.ToLower()))))
                                                            .FirstOrDefault();

                        if (existedProductWithProductCodeAndColor is null)
                        {
                            if (!string.IsNullOrEmpty(productCM.MainImage.ToString()))
                            {
                                var imgURL = await UploadAvaImgFile(productCM.MainImage.ToString());
                                return Ok($"{productCM.MainImage.ToString()} - {imgURL}");
                                if (imgURL != null)
                                {
                                    newProduct.ImgAvatarPath = imgURL;
                                }
                                else
                                {
                                    return StatusCode(500, "Something wrong!");
                                }
                            }
                            else
                            {
                                return BadRequest("Main Image is required!");
                            }
                        }
                        else
                        {
                            newProduct.ImgAvatarPath = existedProductWithProductCodeAndColor.ImgAvatarPath;
                        }
                        newProduct = await _productService.AddProduct(newProduct);


                        if (existedProductWithProductCodeAndColor is null)
                        {
                            var firstImgValue = productCM.ProductImages[0].ToString();
                            var secondImgValue = productCM.ProductImages[1].ToString();
                            var thirdImgValue = productCM.ProductImages[2].ToString();
                            var fourthImgValue = productCM.ProductImages[3].ToString();
                            var fifthImgValue = productCM.ProductImages[4].ToString();
                            var isSuccess = await UploadProductImages(newProduct.Id, firstImgValue, secondImgValue, thirdImgValue,
                                                        fourthImgValue, fifthImgValue);

                            if (!isSuccess)
                            {
                                return StatusCode(500, "Something wrong!");
                            }

                        }
                        else
                        {
                            var images = await _imageVideosService.GetAsyncs(
                                                        _ => _.ProductId == existedProduct.Id);
                            foreach (var image in images)
                            {
                                var imageObject = new ImagesVideo()
                                {
                                    ProductId = newProduct.Id,
                                    ImageUrl = image.ImageUrl,
                                    CreateAt = DateTime.Now,
                                    VideoUrl = null,
                                };
                                await _imageVideosService.AddImage(imageObject);
                            }

                        }

                        var warehouse = new Warehouse()
                        {
                            BranchId = managerDetail.Data.BranchId,
                            ProductId = newProduct.Id,
                            TotalQuantity = productCM.Quantity,
                            AvailableQuantity = productCM.Quantity
                        };
                        await _warehouseService.CreateANewWarehouseAsync(warehouse);
                    }
                    else
                    {

                        var existedWarehouse = (await _warehouseService.GetWarehouseByProductIdAndBranchId(
                                                                                existedProductWithSizeColorCodition.Id,
                                                                                managerDetail.Data.BranchId))
                                                                                .FirstOrDefault();

                        existedWarehouse.TotalQuantity += productCM.Quantity;
                        existedWarehouse.AvailableQuantity += productCM.Quantity;
                        await _warehouseService.UpdateWarehouseAsync(existedWarehouse);
                    }

                }

                if (existedProduct != null)
                {
                    product = existedProduct;
                }

                //Save import history
                var importedBranch = await _branchService.GetBranchById(managerDetail.Data.BranchId);
                var importHistory = new ImportHistory()
                {
                    ManagerId = managerDetail.Data.Id,
                    ProductId = product.Id,
                    ProductName = product.ProductName,
                    ProductCode = product.ProductCode,
                    Price = product.Price,
                    RentPrice = product.RentPrice,
                    Color = product.Color,
                    Size = product.Size,
                    Condition = product.Condition,
                    Action = $@"{importedBranch.BranchName}: Import {productCM.Quantity} {productCM.ProductName} ({productCM.ProductCode})",
                    ImportDate = DateTime.Now,
                    Quantity = productCM.Quantity,
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
        public async Task<IActionResult> ImportProductList([FromForm] List<ProductCM> productList)
        {
            try
            {
                foreach (var productCM in productList)
                {
                    var existedProduct = (await _productService.GetProductByProductCodeAndColor(productCM.ProductCode,
                                                                                       productCM.Color)).FirstOrDefault();


                    var product = _mapper.Map<Product>(productCM);
                    product.CreateAt = DateTime.Now;
                    product.Status = true;
                    product.IsRent = false;
                    product.RentPrice = 0;
                    if (productCM.Condition >= 80)
                    {
                        product.IsRent = true;
                        product.RentPrice = Math.Round((decimal)(product.Price * (decimal)0.1 * productCM.Condition / 100));
                    }
                    try
                    {
                        var userId = GetCurrentUserIdFromToken();

                        if (userId == 0)
                        {
                            return Unauthorized();
                        }

                        var managerDetail = await _managerService.GetManagerDetailsByUserIdAsync(userId);


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
                                        };
                                        await _imageVideosService.AddImage(imageObject);
                                    }
                                    else
                                    {
                                        return BadRequest("Something wrong!");
                                    }
                                }
                            }

                            //Import product into warehouse
                            var warehouse = new Warehouse()
                            {
                                BranchId = managerDetail.Data.BranchId,
                                ProductId = product.Id,
                                TotalQuantity = productCM.Quantity,
                                AvailableQuantity = productCM.Quantity
                            };
                            await _warehouseService.CreateANewWarehouseAsync(warehouse);

                        }
                        else
                        {

                            //Import a new product if it is different size color and condition
                            var existedProductWithSizeColorCodition = (await _productService
                                                                        .GetProductByProductCodeSizeColorCondition
                                                                        (existedProduct.ProductCode,
                                                                        productCM.Size,
                                                                        productCM.Color,
                                                                        productCM.Condition)).FirstOrDefault();
                            if (existedProductWithSizeColorCodition == null)
                            {
                                var newProduct = new Product(existedProduct);
                                newProduct.Id = 0;
                                newProduct.Size = productCM.Size;
                                newProduct.Color = productCM.Color;
                                newProduct.Condition = productCM.Condition;
                                newProduct.Price = productCM.Price;
                                newProduct.Height = productCM.Height;
                                newProduct.Weight = productCM.Weight;
                                newProduct.Length = productCM.Length;
                                newProduct.Width = productCM.Width;

                                var existedProductWithProductCodeAndColor = (await _productService
                                                                    .GetProducts(_ => _.ProductCode.Equals(existedProduct.ProductCode)
                                                                    && _.Color.ToLower().Equals((productCM.Color.ToLower()))))
                                                                    .FirstOrDefault();

                                if (existedProductWithProductCodeAndColor is null)
                                {
                                    if (!string.IsNullOrEmpty(productCM.MainImage.ToString()))
                                    {
                                        var imgURL = await UploadAvaImgFile(productCM.MainImage.ToString());
                                        if (imgURL != null)
                                        {
                                            newProduct.ImgAvatarPath = imgURL;
                                        }
                                        else
                                        {
                                            return StatusCode(500, "Something wrong!");
                                        }
                                    }
                                    else
                                    {
                                        return BadRequest("Main Image is required!");
                                    }
                                }
                                else
                                {
                                    newProduct.ImgAvatarPath = existedProductWithProductCodeAndColor.ImgAvatarPath;
                                }
                                newProduct = await _productService.AddProduct(newProduct);


                                if (existedProductWithProductCodeAndColor is null)
                                {
                                    var firstImgValue = productCM.ProductImages[0].ToString();
                                    var secondImgValue = productCM.ProductImages[1].ToString();
                                    var thirdImgValue = productCM.ProductImages[2].ToString();
                                    var fourthImgValue = productCM.ProductImages[3].ToString();
                                    var fifthImgValue = productCM.ProductImages[4].ToString();
                                    var isSuccess = await UploadProductImages(newProduct.Id, firstImgValue, secondImgValue, thirdImgValue,
                                                                fourthImgValue, fifthImgValue);

                                    if (!isSuccess)
                                    {
                                        return StatusCode(500, "Something wrong!");
                                    }

                                }
                                else
                                {
                                    var images = await _imageVideosService.GetAsyncs(
                                                                _ => _.ProductId == existedProduct.Id);
                                    foreach (var image in images)
                                    {
                                        var imageObject = new ImagesVideo()
                                        {
                                            ProductId = newProduct.Id,
                                            ImageUrl = image.ImageUrl,
                                            CreateAt = DateTime.Now,
                                            VideoUrl = null,
                                        };
                                        await _imageVideosService.AddImage(imageObject);
                                    }

                                }

                                var warehouse = new Warehouse()
                                {
                                    BranchId = managerDetail.Data.BranchId,
                                    ProductId = newProduct.Id,
                                    TotalQuantity = productCM.Quantity,
                                    AvailableQuantity = productCM.Quantity
                                };
                                await _warehouseService.CreateANewWarehouseAsync(warehouse);
                            }
                            else
                            {

                                var existedWarehouse = (await _warehouseService.GetWarehouseByProductIdAndBranchId(
                                                                                        existedProductWithSizeColorCodition.Id,
                                                                                        managerDetail.Data.BranchId))
                                                                                        .FirstOrDefault();

                                existedWarehouse.TotalQuantity += productCM.Quantity;
                                existedWarehouse.AvailableQuantity += productCM.Quantity;
                                await _warehouseService.UpdateWarehouseAsync(existedWarehouse);
                            }

                        }

                        if (existedProduct != null)
                        {
                            product = existedProduct;
                        }

                        //Save import history
                        var importedBranch = await _branchService.GetBranchById(managerDetail.Data.BranchId);
                        var importHistory = new ImportHistory()
                        {
                            ManagerId = managerDetail.Data.Id,
                            ProductId = product.Id,
                            ProductName = product.ProductName,
                            ProductCode = product.ProductCode,
                            Price = product.Price,
                            RentPrice = product.RentPrice,
                            Color = product.Color,
                            Size = product.Size,
                            Condition = product.Condition,
                            Action = $@"{importedBranch.BranchName}: Import {productCM.Quantity} {productCM.ProductName} ({productCM.ProductCode})",
                            ImportDate = DateTime.Now,
                            Quantity = productCM.Quantity,
                        };
                        await _importHistoryService.CreateANewImportHistoryAsync(importHistory);

                        return Ok("Add product successfully!");
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


        [HttpPost]
        [Route("import-product-from-excel")]
        public async Task<IActionResult> ImportProductFromExcel(IFormFile importFile)
        {
            try
            {
                var userId = GetCurrentUserIdFromToken();

                if (userId == 0)
                {
                    return Unauthorized();
                }

                System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
                if (importFile == null || importFile.Length == 0)
                {
                    // Handle invalid file input
                    return BadRequest("Cannot find import file!");
                }
                var importProductStatus = await ImportProductsIntoDB(importFile, userId);
                if (!importProductStatus.Equals("Import product successfully"))
                {
                    return BadRequest(importProductStatus);
                }
                //using (var dbct = new TwoSportCapstoneDbContext())
                //{
                //    dbct.BulkInsert(products);
                //}
                return Ok("Import product successfullu!");
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }

        }

        [NonAction]
        protected async Task<string> ImportProductsIntoDB(IFormFile importFile, int managerId)
        {

            using var fileStream = importFile.OpenReadStream();
            using var reader = ExcelReaderFactory.CreateReader(fileStream);

            do
            {
                if (reader.VisibleState != "visible")
                {
                    continue;
                }

                var rowIndex = 0;
                while (reader.Read())
                {
                    if (rowIndex < 3)
                    {
                        rowIndex++;
                        continue;
                    }

                    // Check if any mandatory fields are null
                    var brandValue = reader.GetValue(1)?.ToString();
                    var categoryValue = reader.GetValue(2)?.ToString();
                    var sportValue = reader.GetValue(3)?.ToString();
                    var productNameValue = reader.GetValue(4)?.ToString();
                    var productCodeValue = reader.GetValue(5)?.ToString();
                    var quantityValue = reader.GetValue(6)?.ToString();
                    var priceValue = reader.GetValue(7)?.ToString();
                    var sizeValue = reader.GetValue(8)?.ToString();
                    var colorValue = reader.GetValue(9)?.ToString();
                    var conditionValue = reader.GetValue(10)?.ToString();
                    var lengthValue = reader.GetValue(11)?.ToString();
                    var widthValue = reader.GetValue(12)?.ToString();
                    var heightValue = reader.GetValue(13)?.ToString();
                    var weightValue = reader.GetValue(14)?.ToString();
                    var avaImgValue = reader.GetValue(15)?.ToString();

                    // Check for null or empty mandatory fields
                    if (string.IsNullOrEmpty(brandValue) ||
                        string.IsNullOrEmpty(categoryValue) ||
                        string.IsNullOrEmpty(sportValue) ||
                        string.IsNullOrEmpty(productNameValue) ||
                        string.IsNullOrEmpty(productCodeValue) ||
                        string.IsNullOrEmpty(quantityValue) ||
                        string.IsNullOrEmpty(priceValue) ||
                        string.IsNullOrEmpty(sizeValue) ||
                        string.IsNullOrEmpty(colorValue) ||
                        string.IsNullOrEmpty(conditionValue) ||
                        string.IsNullOrEmpty(avaImgValue) ||
                        string.IsNullOrEmpty(heightValue)||
                        string.IsNullOrEmpty(weightValue) ||
                        string.IsNullOrEmpty(lengthValue) ||
                        string.IsNullOrEmpty(widthValue))
                    {
                        return "There is any value null!";
                    }

                    try
                    {
                        var managerDetail = await _managerService.GetManagerDetailsByUserIdAsync(managerId);

                        // Assuming product creation logic if all fields are valid
                        var brand = (await _brandService.GetBrandsAsync(brandValue)).FirstOrDefault();
                        if (brand == null)
                        {
                            return "There is no brand";
                        }

                        var sport = (await _sportService.GetSportByName(sportValue)).FirstOrDefault();
                        if (sport == null)
                        {
                            return "There is no sport";
                        }

                        var category = (await _categoryService.GetCategoryByName(categoryValue)).FirstOrDefault();
                        if (category == null)
                        {
                            return "There is no category";
                        }

                        var existedProduct = await _productService.GetProductByProductCode(productCodeValue);

                        var product = new Product
                        {
                            BrandId = brand.Id,
                            CategoryId = category.Id,
                            SportId = sport.Id,
                            ProductName = productNameValue,
                            ProductCode = productCodeValue,
                            Price = decimal.TryParse(priceValue, out var price) ? price : 0,
                            Size = sizeValue,
                            Color = colorValue,
                            Condition = int.Parse(conditionValue),
                            RentPrice = 0,
                            CreateAt = DateTime.Now,
                            Status = true,
                            Height = decimal.TryParse(heightValue, out var height) ? height : 0,
                            Weight = decimal.TryParse(weightValue, out var weigth) ? weigth : 0,
                            Length = decimal.TryParse(lengthValue, out var length) ? length : 0,
                            Width = decimal.TryParse(widthValue, out var width) ? width : 0,
                        };

                        //If there is a existed product with product code
                        if (existedProduct == null)
                        {
                            if (!string.IsNullOrEmpty(avaImgValue))
                            {
                                var imgURL = await UploadAvaImgFile(avaImgValue);
                                return $"{avaImgValue} - {imgURL}" ;
                                if (imgURL != null) {
                                    product.ImgAvatarPath = imgURL;
                                }
                                else
                                {
                                    return "Upload image failed!";
                                }
                            }
                            else
                            {
                                return "There is no image file!";
                            }

                            if (product.Condition >= 80)
                            {
                                product.IsRent = true;
                                product.RentPrice = Math.Round((decimal)(product.Price * (decimal)0.1 * product.Condition / 100));
                            }
                            else
                            {
                                product.IsRent = false;
                            }

                            await _productService.AddProduct(product);

                            //Add product's images into ImageVideo table
                            var firstImgValue = reader.GetValue(16)?.ToString();
                            var secondImgValue = reader.GetValue(17)?.ToString();
                            var thirdImgValue = reader.GetValue(18)?.ToString();
                            var fourthImgValue = reader.GetValue(19)?.ToString();
                            var fifthImgValue = reader.GetValue(20)?.ToString();
                            var isSuccess = await UploadProductImages(product.Id,firstImgValue, secondImgValue, thirdImgValue,
                                                        fourthImgValue, fifthImgValue);
                            
                            if (!isSuccess)
                            {
                                return "Upload image failed!";
                            }

                            //Import product into warehouse
                            var warehouse = new Warehouse()
                            {
                                BranchId = managerDetail.Data.BranchId,
                                ProductId = product.Id,
                                TotalQuantity = int.Parse(quantityValue),
                                AvailableQuantity = int.Parse(quantityValue),
                            };
                            await _warehouseService.CreateANewWarehouseAsync(warehouse);
                        }
                        else
                        {
                            //Import a new product if it is different size color and condition
                            var existedProductWithSizeColorCodition = (await _productService
                                                                        .GetProductByProductCodeSizeColorCondition
                                                                        (existedProduct.ProductCode,
                                                                        sizeValue,
                                                                        colorValue,
                                                                        int.Parse(conditionValue))).FirstOrDefault();
                            if (existedProductWithSizeColorCodition == null)
                            {
                                var newProduct = new Product(existedProduct)
                                {
                                    Id = 0,
                                    Size = sizeValue,
                                    Color = colorValue,
                                    Condition = int.Parse(conditionValue),
                                    RentPrice = 0,
                                    Price = decimal.Parse(priceValue),
                                    CreateAt = DateTime.Now
                                };
                                if (newProduct.IsRent)
                                {
                                    newProduct.RentPrice = Math.Round((decimal)(product.Price * (decimal)0.1 * product.Condition / 100));
                                }

                                var existedProductWithProductCodeAndColor = (await _productService
                                                            .GetProducts(_ => _.ProductCode.Equals(existedProduct.ProductCode)
                                                            && _.Color.ToLower().Equals((colorValue.ToLower()))))
                                                            .FirstOrDefault();

                                if (existedProductWithProductCodeAndColor is null)
                                {
                                    if (!string.IsNullOrEmpty(avaImgValue))
                                    {
                                        var imgURL = await UploadAvaImgFile(avaImgValue);
                                        if (imgURL != null)
                                        {
                                            newProduct.ImgAvatarPath = imgURL;
                                        }
                                        else
                                        {
                                            return "Upload image failed!";
                                        }
                                    }
                                    else
                                    {
                                        return "There is no image file!";
                                    }
                                } else
                                {
                                    newProduct.ImgAvatarPath = existedProductWithProductCodeAndColor.ImgAvatarPath;
                                }
                                newProduct = await _productService.AddProduct(newProduct);


                                if (existedProductWithProductCodeAndColor is null)
                                {
                                    //Add product's images into ImageVideo table
                                    var firstImgValue = reader.GetValue(16)?.ToString();
                                    var secondImgValue = reader.GetValue(17)?.ToString();
                                    var thirdImgValue = reader.GetValue(18)?.ToString();
                                    var fourthImgValue = reader.GetValue(19)?.ToString();
                                    var fifthImgValue = reader.GetValue(20)?.ToString();
                                    var isSuccess = await UploadProductImages(newProduct.Id, firstImgValue, secondImgValue, thirdImgValue,
                                                                fourthImgValue, fifthImgValue);

                                    if (!isSuccess)
                                    {
                                        return "Upload image failed!";
                                    }

                                } else
                                {
                                    var images = await _imageVideosService.GetAsyncs(
                                                                _ => _.ProductId == existedProduct.Id);
                                    foreach (var image in images)
                                    {
                                        var imageObject = new ImagesVideo()
                                        {
                                            ProductId = newProduct.Id,
                                            ImageUrl = image.ImageUrl,
                                            CreateAt = DateTime.Now,
                                            VideoUrl = null,
                                        };
                                        await _imageVideosService.AddImage(imageObject);
                                    }
                                    
                                }

                                var warehouse = new Warehouse()
                                {
                                    BranchId = managerDetail.Data.BranchId,
                                    ProductId = newProduct.Id,
                                    TotalQuantity = int.Parse(quantityValue),
                                    AvailableQuantity = int.Parse(quantityValue),
                                };
                                await _warehouseService.CreateANewWarehouseAsync(warehouse);
                                product = newProduct;
                            }
                            else
                            {

                                var existedWarehouse = (await _warehouseService.GetWarehouseByProductIdAndBranchId(
                                                                                        existedProductWithSizeColorCodition.Id,
                                                                                        managerDetail.Data.BranchId))
                                                                                        .FirstOrDefault();
                                if (existedWarehouse == null)
                                {
                                    var newWarehouse = new Warehouse
                                    {
                                        BranchId = managerDetail.Data.BranchId,
                                        ProductId = existedProductWithSizeColorCodition.Id,
                                        TotalQuantity = int.Parse(quantityValue),
                                        AvailableQuantity = int.Parse(quantityValue),
                                    };
                                    await _warehouseService.CreateANewWarehouseAsync(newWarehouse);
                                    product = existedProductWithSizeColorCodition;
                                }
                                else
                                {
                                    existedWarehouse.TotalQuantity += int.Parse(quantityValue);
                                    existedWarehouse.AvailableQuantity += int.Parse(quantityValue);
                                    await _warehouseService.UpdateWarehouseAsync(existedWarehouse);
                                    product = existedProductWithSizeColorCodition;
                                }
                            }
                        }

                        //Save import history
                        var manager = await _managerService.GetManagerDetailsByUserIdAsync(managerId);
                        var importedBranch = await _branchService.GetBranchById(manager.Data.BranchId);
                        var importHistory = new ImportHistory()
                        {
                            ManagerId = manager.Data.Id,
                            ProductId = product.Id,
                            ProductName = product.ProductName,
                            ProductCode = product.ProductCode,
                            Price = product.Price,
                            RentPrice = product.RentPrice,
                            Color = product.Color,
                            Size = product.Size,
                            Condition = product.Condition,
                            Action = $@"{importedBranch.BranchName}: Import {int.Parse(quantityValue)} {productNameValue} ({productCodeValue}) {colorValue} {sizeValue} {conditionValue}",
                            ImportDate = DateTime.Now,
                            Quantity = int.Parse(quantityValue),
                        };
                        await _importHistoryService.CreateANewImportHistoryAsync(importHistory);
                    }
                    catch (Exception ex)
                    {
                        return ex.Message;
                    }


                    rowIndex++;
                }

                break;

            } while (reader.NextResult());

            //using (var dbct = new TwoSportCapstoneDbContext())
            //{
            //    dbct.BulkInsert(products);
            //}

            return "Import product successfully";
        }

        [NonAction]
        private async Task<bool> UploadProductImages(int productId, string? firstImgValue, 
                        string? secondImgValue, string? thirdImgValue, string? fourthImgValue, string? fifthImgValue)
        {
            var listProductImages = new List<string>();
            if (!string.IsNullOrEmpty(firstImgValue))
            {
                listProductImages.Add(firstImgValue);
            }
            if (!string.IsNullOrEmpty(secondImgValue))
            {
                listProductImages.Add(secondImgValue);
            }
            if (!string.IsNullOrEmpty(thirdImgValue))
            {
                listProductImages.Add(thirdImgValue);
            }
            if (!string.IsNullOrEmpty(fourthImgValue))
            {
                listProductImages.Add(fourthImgValue);
            }
            if (!string.IsNullOrEmpty(fifthImgValue))
            {
                listProductImages.Add(fifthImgValue);
            }

            if (listProductImages.Count > 0)
            {
                foreach (var imagePath in listProductImages)
                {
                    var imageFile = ConvertToIFormFile(imagePath);
                    var uploadResult = await _imageService.UploadImageToCloudinaryAsync(imageFile);
                    if (uploadResult != null && uploadResult.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        var imageObject = new ImagesVideo()
                        {
                            ProductId = productId,
                            ImageUrl = uploadResult.SecureUri.AbsoluteUri,
                            CreateAt = DateTime.Now,
                            VideoUrl = null,
                        };
                        await _imageVideosService.AddImage(imageObject);
                    }
                    else
                    {
                        return false;

                    }
                }
            }
            return true;
        }

        [NonAction]
        private async Task<string> UploadAvaImgFile(string avaImgValue)
        {
            var avaImgFile = ConvertToIFormFile(avaImgValue);
            return avaImgFile;
            //var uploadResult = await _imageService.UploadImageToCloudinaryAsync(avaImgFile);
            //if (uploadResult != null && uploadResult.StatusCode == System.Net.HttpStatusCode.OK)
            //{
            //    return uploadResult.SecureUrl.AbsoluteUri;
            //}
            //else
            //{
            //    return null;
            //}
        }

        [NonAction]
        public string ConvertToIFormFile(string filePath)
        {
            // Check if the filePath contains a prefix like "/app/" and remove it
            if (filePath.StartsWith("/app/"))
            {
                filePath = filePath.Substring("/app/".Length); // Remove the "/app/" prefix
            }

            var fileInfo = new FileInfo(filePath);
            return $"{filePath} - {fileInfo}";
            //var fileBytes = System.IO.File.ReadAllBytes(filePath); // Read file into memory
            //var stream = new MemoryStream(fileBytes);             // Create memory stream from file bytes

            //IFormFile formFile = new FormFile(stream, 0, fileInfo.Length, null, fileInfo.Name)
            //{
            //    Headers = new HeaderDictionary(),
            //    ContentType = "image/jpeg"
            //};

            //return formFile;
        }


        [HttpPost]
        [Route("add-product")]
        public async Task<IActionResult> AddProduct(ProductCM productCM)
        {
            try
            {
                int userId = GetCurrentUserIdFromToken();
                if (userId == 0)
                {
                    return Unauthorized();
                }
                var getProductByProductCodeAndDifferentName = (await _productService
                                                                    .GetProducts(_ => _.ProductCode.ToLower()
                                                                    .Equals(productCM.ProductCode.ToLower()) &&
                                                                    !_.ProductName.ToLower()
                                                                    .Equals(productCM.ProductName.ToLower())))
                                                                    .FirstOrDefault();
                if (getProductByProductCodeAndDifferentName is not null)
                {
                    return BadRequest("There is a product had this product code!");
                }
                var existedProduct = (await _productService
                                                    .GetProducts(_ => _.ProductCode.ToLower()
                                                    .Equals(productCM.ProductCode.ToLower()) &&
                                                    _.ProductName.ToLower()
                                                    .Equals(productCM.ProductName.ToLower()) &&
                                                    _.Color.ToLower()
                                                    .Equals(productCM.Color) &&
                                                    _.Size.ToLower()
                                                    .Equals(productCM.Size) &&
                                                    _.Condition == productCM.Condition))
                                                    .FirstOrDefault();
                if (existedProduct is not null)
                {
                    return BadRequest("The product has already existed!");
                }

                var product = _mapper.Map<Product>(productCM);
                product.Status = true;
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

                await _productService.AddProduct(product);

                //Add product's images into ImageVideo table
                if (productCM.ProductImages is not null)
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
                            };
                            await _imageVideosService.AddImage(imageObject);
                        }
                        else
                        {
                            return BadRequest("Something wrong!");
                        }
                    }
                }
                return Ok(product);

            } catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut]
        [Route("update-product/{productId}")]
        public async Task<IActionResult> UpdateProduct(int productId, ProductUM productUM)
        {
            try
            {
                var userId = GetCurrentUserIdFromToken();

                if (userId == 0)
                {
                    return Unauthorized();
                }

                var updatedProduct = await _productService.GetProductById(productId);
                if (updatedProduct == null)
                {
                    return BadRequest($"Cannot find the product with id: {productId}");
                }
                else
                {
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

                    updatedProduct.Color = productUM.Color;
                    updatedProduct.Size = productUM.Size;
                    updatedProduct.Condition = productUM.Condition;
                    updatedProduct.Height = productUM.Height;
                    updatedProduct.Length = productUM.Length;
                    updatedProduct.Width = productUM.Width;
                    updatedProduct.Weight = productUM.Weight;
                    updatedProduct.Price = productUM.Price;
                    updatedProduct.ProductName = productUM.ProductName;
                    updatedProduct.ProductCode = productUM.ProductCode;
                    updatedProduct.BrandId = (int)productUM.BrandId;
                    updatedProduct.CategoryId = (int)productUM.CategoryId;
                    updatedProduct.SportId = (int)productUM.SportId;
                    if (!productUM.IsRent)
                    {
                        updatedProduct.IsRent = productUM.IsRent;
                        updatedProduct.RentPrice = 0;
                    } else
                    {
                        updatedProduct.IsRent = productUM.IsRent;
                        updatedProduct.RentPrice = productUM.RentPrice;
                    }

                    await _productService.UpdateProduct(updatedProduct);

                    //Add product's images into ImageVideo table
                    if (productUM.ProductImages is not null)
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
                    var manager = await _managerService.GetManagerDetailsByIdAsync(userId);
                    var importedBranch = await _branchService.GetBranchById(manager.Data.BranchId);
                    var importHistory = new ImportHistory()
                    {
                        ManagerId = manager.Data.Id,
                        ProductId = updatedProduct.Id,
                        Action = $@"{importedBranch.BranchName}: Updated {productUM.ProductName} ({productUM.ProductCode})",
                        ImportDate = DateTime.Now,
                        Quantity = 0,
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
                var activeDeactiveProduct = await _productService.GetProductById(productId);
                var tmpProduct = await _productService.GetProductById(productId);
                activeDeactiveProduct.Status = !tmpProduct.Status;
                await _productService.UpdateProduct(activeDeactiveProduct);
                _unitOfWork.Save();
                return Ok("Active/Deactive product successfully!");
            }
            catch (Exception ex)
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


        [HttpPut]
        [Route("edit-description-of-product/{productCode}")]
        public async Task<IActionResult> EditDescriptionOfPrduct (string productCode, string description)
        {
            try
            {
                var userId = GetCurrentUserIdFromToken();
                if (userId == null)
                {
                    return Unauthorized();
                }

                var products = await _productService.GetProductsByProductCode(productCode);
                foreach (var product in products)
                {
                    product.Description = description;
                }
                await _productService.UpdateProducts(products);
                return Ok("save successfully!");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Something wrong!");
            }
        }



        [HttpPut]
        [Route("edit-offers-of-product/{categoryId}")]
        public async Task<IActionResult> EditOffersnOfPrduct(int categoryId, string offers)
        {
            try
            {
                var userId = GetCurrentUserIdFromToken();
                if (userId == null)
                {
                    return Unauthorized();
                }

                var products = await _productService.GetProductsByCategoryId(categoryId);
                foreach (var product in products)
                {
                    product.Offers = offers;
                }
                await _productService.UpdateProducts(products);
                return Ok("save successfully!");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Something wrong!");
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