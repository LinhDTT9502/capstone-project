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
using EntityFrameworkCore.SqlServer.SimpleBulks.BulkInsert;
using ExcelDataReader;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
                var productsSameProductCode = await _productService.GetProductsByProductCode(product.ProductCode);

                var productVMs = _mapper.Map<List<ProductVM>>(productsSameProductCode.ToList());
                foreach (var productVM in productVMs)
                {
                    var brand = await _brandService.GetBrandById(productVM.BrandId);
                    productVM.BrandName = brand.FirstOrDefault().BrandName;
                    var category = await _categoryService.GetCategoryById(productVM.CategoryID);
                    productVM.CategoryName = category.CategoryName;
                    var sport = await _sportService.GetSportById(productVM.SportId);
                    productVM.SportName = sport.Name;
                    var reviews = await _reviewService.GetReviewsOfProduct(product.Id);
                    productVM.Reviews = reviews.ToList();
                    var numOfLikes = await _likeService.CountLikeOfProduct(productId);
                    productVM.Likes = numOfLikes;
                }
                
                return Ok(productVMs);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet]
        [Route("get-product-by-product-code/{productCode}")]
        public async Task<IActionResult> GetProductByProductCode(string productCode)
        {
            try
            {
                var productsSameProductCode = await _productService.GetProductsByProductCode(productCode);

                var productVMs = _mapper.Map<List<ProductVM>>(productsSameProductCode.ToList());
                foreach (var productVM in productVMs)
                {
                    var brand = await _brandService.GetBrandById(productVM.BrandId);
                    productVM.BrandName = brand.FirstOrDefault().BrandName;
                    var category = await _categoryService.GetCategoryById(productVM.CategoryID);
                    productVM.CategoryName = category.CategoryName;
                    var sport = await _sportService.GetSportById(productVM.SportId);
                    productVM.SportName = sport.Name;
                    var reviews = await _reviewService.GetReviewsOfProductByProductCode(productCode);
                    productVM.Reviews = reviews.ToList();
                    var numOfLikes = await _likeService.CountLikeOfProductByProductCode(productCode);
                    productVM.Likes = numOfLikes;
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
        [Route("list-conditions-of-product/{productCode}")]
        public async Task<IActionResult> GetConditionsOfProduct(string productCode)
        {
            var conditions = await _productService.GetConditionsOfProduct(productCode);
            return Ok(new { total = conditions.Count, data = conditions });
        }

        [HttpGet]
        [Route("list-sizes-of-product/{productCode}")]
        public async Task<IActionResult> GetSizesOfProduct(string productCode)
        {
            var sizes = await _productService.GetSizesOfProduct(productCode);
            return Ok(new { total = sizes.Count, data = sizes });
        }

        [HttpGet]
        [Route("list-products")]
        public async Task<IActionResult> GetProducts([FromQuery] DefaultSearch defaultSearch)
        {
            try
            {
                var query = await _productService.GetProducts(_ => _.Status == true, null, "", defaultSearch.currentPage, defaultSearch.perPage);
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
        public async Task<IActionResult> FilterSortProducts([FromQuery] DefaultSearch defaultSearch, [FromQuery] string? size,
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

                var managerDetail = await _managerService.GetManagerDetailByIdAsync(userId);


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
                        var newProduct = existedProduct;
                        newProduct.Id = 0;
                        newProduct.Size = productCM.Size;
                        newProduct.Color = productCM.Color;
                        newProduct.Condition = productCM.Condition;
                        newProduct.Price = productCM.Price;
                        await _productService.AddProduct(newProduct);

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
                    StaffId = userId,
                    ProductId = product.Id,
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

                        var managerDetail = await _managerService.GetManagerDetailByIdAsync(userId);


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
                                AvailableQuantity = productCM.Quantity,
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
                                var newProduct = existedProduct;
                                newProduct.Size = productCM.Size;
                                newProduct.Color = productCM.Color;
                                newProduct.Condition = productCM.Condition;
                                newProduct.Price = productCM.Price;
                                await _productService.AddProduct(newProduct);
                            }
                            else
                            {
                                var existedWarehouse = (await _warehouseService.GetWarehouseByProductIdAndBranchId(
                                                                                        existedProductWithSizeColorCodition.Id,
                                                                                        managerDetail.Data.BranchId))
                                                                                        .FirstOrDefault();
                                if (existedWarehouse != null)
                                {
                                    existedWarehouse.TotalQuantity += productCM.Quantity;
                                    existedWarehouse.AvailableQuantity += productCM.Quantity;
                                    await _warehouseService.UpdateWarehouseAsync(existedWarehouse);
                                }
                                else
                                {
                                    //var branch = (await _branchService.GetBranchByManagerId(userId)).FirstOrDefault();
                                    var warehouse = new Warehouse()
                                    {
                                        BranchId = managerDetail.Data.BranchId,
                                        ProductId = existedProductWithSizeColorCodition.Id,
                                        TotalQuantity = productCM.Quantity,
                                        AvailableQuantity = productCM.Quantity,
                                    };
                                    await _warehouseService.CreateANewWarehouseAsync(warehouse);
                                }
                            }

                        }

                        //Save import history
                        var importedBranch = await _branchService.GetBranchById(managerDetail.Data.BranchId);
                        var importHistory = new ImportHistory()
                        {
                            StaffId = userId,
                            ProductId = product.Id,
                            Action = $@"{importedBranch.BranchName}: Import {productCM.Quantity} {productCM.ProductName} ({productCM.ProductCode})",
                            ImportDate = DateTime.Now,
                            Quantity = productCM.Quantity,
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
                if (importProductStatus == (int)ProductErrors.NullError)
                {
                    return BadRequest("One or more field are required");
                }
                if (importProductStatus == (int)ProductErrors.NotExcepted)
                {
                    return StatusCode(500, "Unknown error");
                }
                if (importProductStatus == (int)ProductErrors.BrandNameError)
                {
                    return BadRequest("Brand name seems wrong please check again");
                }
                if (importProductStatus == (int)ProductErrors.CategoryNameError)
                {
                    return BadRequest("Category name seems wrong please check again");
                }
                if (importProductStatus == (int)ProductErrors.SportNameError)
                {
                    return BadRequest("Sport name seems wrong please check again");
                }
                if (importProductStatus == (int)ProductErrors.SupplierNameError)
                {
                    return BadRequest("Supplier name seems wrong please check again");
                }
                //using (var dbct = new TwoSportCapstoneDbContext())
                //{
                //    dbct.BulkInsert(products);
                //}
                return Ok("Import product successfull!");
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }

        }

        [NonAction]
        protected async Task<int> ImportProductsIntoDB(IFormFile importFile, int managerId)
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
                    var categoryValue = reader.GetValue(3)?.ToString();
                    var sportValue = reader.GetValue(4)?.ToString();
                    var productNameValue = reader.GetValue(5)?.ToString();
                    var productCodeValue = reader.GetValue(6)?.ToString();
                    var quantityValue = reader.GetValue(7)?.ToString();
                    var lotCodeValue = reader.GetValue(8)?.ToString();
                    var listedPriceValue = reader.GetValue(9)?.ToString();
                    var priceValue = reader.GetValue(10)?.ToString();
                    var rentPriceValue = reader.GetValue(11)?.ToString();
                    var sizeValue = reader.GetValue(12)?.ToString();
                    var colorValue = reader.GetValue(13)?.ToString();
                    var conditionValue = reader.GetValue(14)?.ToString();
                    var avaImgValue = reader.GetValue(16)?.ToString();
                    var isRent = reader.GetValue(15)?.ToString();

                    // Check for null or empty mandatory fields
                    if (string.IsNullOrEmpty(brandValue) ||
                        string.IsNullOrEmpty(categoryValue) ||
                        string.IsNullOrEmpty(sportValue) ||
                        string.IsNullOrEmpty(productNameValue) ||
                        string.IsNullOrEmpty(productCodeValue) ||
                        string.IsNullOrEmpty(quantityValue) ||
                        string.IsNullOrEmpty(lotCodeValue) ||
                        string.IsNullOrEmpty(listedPriceValue) ||
                        string.IsNullOrEmpty(priceValue) ||
                        string.IsNullOrEmpty(sizeValue) ||
                        string.IsNullOrEmpty(colorValue) ||
                        string.IsNullOrEmpty(conditionValue) ||
                        string.IsNullOrEmpty(avaImgValue) ||
                        string.IsNullOrEmpty(isRent))
                    {
                        return (int)ProductErrors.NullError;
                    }

                    try
                    {
                        var managerDetail = await _managerService.GetManagerDetailByIdAsync(managerId);

                        //Check if brand is not exist, add new brand
                        #region Add new brand
                        var existedBrand = await _brandService.GetBrandsAsync(brandValue);
                        if (existedBrand == null)
                        {
                            var brandImg = reader.GetValue(2)?.ToString();
                            var brandImgFile = ConvertToIFormFile(brandImg);
                            if (!string.IsNullOrEmpty(brandImg))
                            {
                                var uploadResult = await _imageService.UploadImageToCloudinaryAsync(brandImgFile);
                                if (uploadResult != null && uploadResult.StatusCode == System.Net.HttpStatusCode.OK)
                                {
                                    var newBrand = new Brand()
                                    {
                                        BrandName = brandValue,
                                        Logo = uploadResult.SecureUrl.AbsoluteUri,
                                    };
                                    var newBrandBranch = new BrandBranch()
                                    {
                                        BranchId = managerDetail.Data.BranchId
                                    };
                                    await _brandService.CreateANewBrandAsync(newBrand);
                                }
                                else
                                {
                                    return (int)ProductErrors.NotExcepted;
                                }
                            }
                            else
                            {
                                return (int)ProductErrors.NullError;
                            }
                        }
                        #endregion

                        // Assuming product creation logic if all fields are valid
                        var brand = (await _brandService.GetBrandsAsync(brandValue)).FirstOrDefault();
                        if (brand == null)
                        {
                            return (int)ProductErrors.BrandNameError;
                        }

                        var sport = (await _sportService.GetSportByName(sportValue)).FirstOrDefault();
                        if (sport == null)
                        {
                            return (int)ProductErrors.SportNameError;
                        }

                        var category = (await _categoryService.GetCategoryByName(categoryValue)).FirstOrDefault();
                        if (category == null)
                        {
                            return (int)ProductErrors.CategoryNameError;
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
                        };
                        if (existedProduct == null)
                        {
                            if (!string.IsNullOrEmpty(avaImgValue))
                            {
                                var avaImgFile = ConvertToIFormFile(avaImgValue);
                                var uploadResult = await _imageService.UploadImageToCloudinaryAsync(avaImgFile);
                                if (uploadResult != null && uploadResult.StatusCode == System.Net.HttpStatusCode.OK)
                                {
                                    product.ImgAvatarPath = uploadResult.SecureUrl.AbsoluteUri;
                                }
                                else
                                {
                                    return (int)ProductErrors.NotExcepted;
                                }
                            }
                            else
                            {
                                return (int)ProductErrors.NullError;
                            }



                            if (isRent.ToLower().Equals("yes"))
                            {
                                product.IsRent = true;
                                if (!string.IsNullOrEmpty(rentPriceValue))
                                {
                                    product.RentPrice = decimal.Parse(rentPriceValue);
                                }
                            }
                            else
                            {
                                product.IsRent = false;
                            }

                            await _productService.AddProduct(product);

                            //Add product's images into ImageVideo table
                            var listProductImages = new List<string>();
                            var firstImgValue = reader.GetValue(17)?.ToString();
                            var secondImgValue = reader.GetValue(18)?.ToString();
                            var thirdImgValue = reader.GetValue(19)?.ToString();
                            var fourthImgValue = reader.GetValue(20)?.ToString();
                            var fifthImgValue = reader.GetValue(21)?.ToString();
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
                                            ProductId = product.Id,
                                            ImageUrl = uploadResult.SecureUri.AbsoluteUri,
                                            CreateAt = DateTime.Now,
                                            VideoUrl = null,
                                        };
                                        await _imageVideosService.AddImage(imageObject);
                                    }
                                    else
                                    {
                                        return (int)ProductErrors.NotExcepted;

                                    }
                                }
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
                                var newProduct = existedProduct;
                                newProduct.Id = 0;
                                newProduct.Size = sizeValue;
                                newProduct.Color = colorValue;
                                newProduct.Condition = int.Parse(conditionValue);
                                newProduct.Price = decimal.Parse(priceValue);
                                await _productService.AddProduct(newProduct);

                                var warehouse = new Warehouse()
                                {
                                    BranchId = managerDetail.Data.BranchId,
                                    ProductId = newProduct.Id,
                                    TotalQuantity = int.Parse(quantityValue),
                                    AvailableQuantity = int.Parse(quantityValue),
                                };
                                await _warehouseService.CreateANewWarehouseAsync(warehouse);
                            }
                            else
                            {

                                var existedWarehouse = (await _warehouseService.GetWarehouseByProductIdAndBranchId(
                                                                                        existedProductWithSizeColorCodition.Id,
                                                                                        managerDetail.Data.BranchId))
                                                                                        .FirstOrDefault();

                                existedWarehouse.TotalQuantity += int.Parse(quantityValue);
                                existedWarehouse.AvailableQuantity += int.Parse(quantityValue);
                                await _warehouseService.UpdateWarehouseAsync(existedWarehouse);
                            }
                        }
                        if (existedProduct != null)
                        {
                            product = existedProduct;
                        }

                        //Save import history
                        var manager = await _managerService.GetManagerDetailByIdAsync(managerId);
                        var importedBranch = await _branchService.GetBranchById(manager.Data.BranchId);
                        var importHistory = new ImportHistory()
                        {
                            StaffId = managerId,
                            ProductId = product.Id,
                            Action = $@"{importedBranch.BranchName}: Import {int.Parse(quantityValue)} {productNameValue} ({productCodeValue})",
                            ImportDate = DateTime.Now,
                            Quantity = int.Parse(quantityValue),
                        };
                        await _importHistoryService.CreateANewImportHistoryAsync(importHistory);
                    }
                    catch (Exception ex)
                    {
                        return (int)ProductErrors.NotExcepted;
                    }


                    rowIndex++;
                }

                break;

            } while (reader.NextResult());

            //using (var dbct = new TwoSportCapstoneDbContext())
            //{
            //    dbct.BulkInsert(products);
            //}

            return 1;
        }

        [NonAction]
        public IFormFile ConvertToIFormFile(string filePath)
        {
            var fileInfo = new FileInfo(filePath);
            var stream = new FileStream(filePath, FileMode.Open);

            IFormFile formFile = new FormFile(stream, 0, fileInfo.Length, null, fileInfo.Name)
            {
                Headers = new HeaderDictionary(),
                ContentType = "image/jpeg"
            };

            return formFile;
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
                        StaffId = userId,
                        ProductId = updatedProduct.Id,
                        Action = $@"{importedBranch.BranchName}: Updated {productUM.ProductName} ({productUM.ProductCode})",
                        ImportDate = DateTime.Now,
                        Quantity = productUM.Quantity,
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

        [HttpPut]
        [Route("edit-description-of-product/{productId}")]
        public async Task<IActionResult> EditDescriptionOfProduct(int productId, string description)
        {
            try
            {
                var editedProduct = await _productService.GetProductById(productId);
                if (editedProduct == null)
                {
                    return BadRequest($"There is no any products with id {productId}");
                }
                editedProduct.Description = description;
                await _productService.UpdateProduct(editedProduct);
                return Ok("Save successfully!");
            } catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
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