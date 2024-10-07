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
        private readonly ISupplierService _supplierService;
        private readonly ISportService _sportService;
        private readonly ILikeService _likeService;
        private readonly IReviewService _reviewService;
        private readonly IImageService _imageService;
        private readonly IWarehouseService _warehouseService;
        private readonly IImageVideosService _imageVideosService;
        private readonly IImportHistoryService _importHistoryService;
        private readonly IEmployeeDetailService _employeeDetailService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ProductController(IProductService productService,
                                IBrandService brandService,
                                IBranchService branchService,
                                ICategoryService categoryService,
                                ISupplierService supplierService,
                                IUnitOfWork unitOfWork,
                                ISportService sportService,
                                ILikeService likeService,
                                IReviewService reviewService,
                                IWarehouseService warehouseService,
                                IImageService imageService,
                                IImageVideosService imageVideosService,
                                IImportHistoryService importHistoryService,
                                IEmployeeDetailService employeeDetailService,
                                IMapper mapper)
        {
            _productService = productService;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _brandService = brandService;
            _branchService = branchService;
            _categoryService = categoryService;
            _supplierService = supplierService;
            _sportService = sportService;
            _likeService = likeService;
            _reviewService = reviewService;
            _warehouseService = warehouseService;
            _imageService = imageService;
            _imageVideosService = imageVideosService;
            _importHistoryService = importHistoryService;
            _employeeDetailService = employeeDetailService;
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
            }
            catch (Exception ex)
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
                foreach (var product in products)
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

                var employeeDetail = await _employeeDetailService.GetEmployeeDetailByEmployeeId(userId);


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

                    //Import product into warehouse
                    var warehouse = new Warehouse()
                    {
                        BranchId = employeeDetail.BranchId,
                        ProductId = product.Id,
                        Quantity = productCM.Quantity,
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
                        newProduct.ListedPrice = productCM.ListedPrice;
                        newProduct.Price = productCM.Price;
                        await _productService.AddProduct(newProduct);

                        var warehouse = new Warehouse()
                        {
                            BranchId = employeeDetail.BranchId,
                            ProductId = newProduct.Id,
                            Quantity = productCM.Quantity,
                        };
                        await _warehouseService.CreateANewWarehouseAsync(warehouse);
                    }
                    else
                    {

                        var existedWarehouse = (await _warehouseService.GetWarehouseByProductIdAndBranchId(
                                                                                existedProductWithSizeColorCodition.Id,
                                                                                employeeDetail.BranchId))
                                                                                .FirstOrDefault();
                        
                        existedWarehouse.Quantity += productCM.Quantity;
                        await _warehouseService.UpdateWarehouseAsync(existedWarehouse);
                    }

                }

                if (existedProduct != null)
                {
                    product = existedProduct;
                }

                //Save import history
                var importedBranch = await _branchService.GetBranchById(employeeDetail.BranchId);
                var importHistory = new ImportHistory()
                {
                    EmployeeId = userId,
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

                        var employeeDetail = await _employeeDetailService.GetEmployeeDetailByEmployeeId(userId);


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

                            //Import product into warehouse
                            var warehouse = new Warehouse()
                            {
                                BranchId = employeeDetail.BranchId,
                                ProductId = product.Id,
                                Quantity = productCM.Quantity,
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
                                newProduct.ListedPrice = productCM.ListedPrice;
                                newProduct.Price = productCM.Price;
                                await _productService.AddProduct(newProduct);
                            }
                            else
                            {
                                var existedWarehouse = (await _warehouseService.GetWarehouseByProductIdAndBranchId(
                                                                                        existedProductWithSizeColorCodition.Id,
                                                                                        employeeDetail.BranchId))
                                                                                        .FirstOrDefault();
                                if (existedWarehouse != null)
                                {
                                    existedWarehouse.Quantity += productCM.Quantity;
                                    await _warehouseService.UpdateWarehouseAsync(existedWarehouse);
                                }
                                else
                                {
                                    //var branch = (await _branchService.GetBranchByManagerId(userId)).FirstOrDefault();
                                    var warehouse = new Warehouse()
                                    {
                                        BranchId = employeeDetail.BranchId,
                                        ProductId = existedProductWithSizeColorCodition.Id,
                                        Quantity = productCM.Quantity,
                                    };
                                    await _warehouseService.CreateANewWarehouseAsync(warehouse);
                                }
                            }

                        }

                        //Save import history
                        var importedBranch = await _branchService.GetBranchById(employeeDetail.BranchId);
                        var importHistory = new ImportHistory()
                        {
                            EmployeeId = userId,
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
                    var supplierValue = reader.GetValue(5)?.ToString();
                    var productNameValue = reader.GetValue(7)?.ToString();
                    var productCodeValue = reader.GetValue(8)?.ToString();
                    var quantityValue = reader.GetValue(9)?.ToString();
                    var lotCodeValue = reader.GetValue(10)?.ToString();
                    var listedPriceValue = reader.GetValue(11)?.ToString();
                    var priceValue = reader.GetValue(12)?.ToString();
                    var rentPriceValue = reader.GetValue(13)?.ToString();
                    var sizeValue = reader.GetValue(14)?.ToString();
                    var colorValue = reader.GetValue(15)?.ToString();
                    var conditionValue = reader.GetValue(16)?.ToString();
                    var avaImgValue = reader.GetValue(18)?.ToString();
                    var isRent = reader.GetValue(17)?.ToString();

                    // Check for null or empty mandatory fields
                    if (string.IsNullOrEmpty(brandValue) ||
                        string.IsNullOrEmpty(categoryValue) ||
                        string.IsNullOrEmpty(sportValue) ||
                        string.IsNullOrEmpty(supplierValue) ||
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
                        //Check if brand is not exist, add new brand
                        #region Add new brand
                        var existedBrand = (await _brandService.GetBrandsAsync(brandValue)).FirstOrDefault();
                        if (existedBrand == null)
                        {
                            var newBrand = new Brand()
                            {
                                BrandName = brandValue,
                            };

                            var brandImg = reader.GetValue(2)?.ToString();
                            if (!string.IsNullOrEmpty(brandImg))
                            {
                                var brandImgFile = ConvertToIFormFile(brandImg);
                                var uploadResult = await _imageService.UploadImageToCloudinaryAsync(brandImgFile);
                                if (uploadResult != null && uploadResult.StatusCode == System.Net.HttpStatusCode.OK)
                                {

                                    newBrand.Logo = uploadResult.SecureUrl.AbsoluteUri;
                                    await _brandService.CreateANewBrandAsync(newBrand);
                                }
                                else
                                {
                                    return (int)ProductErrors.NotExcepted;
                                }
                            }
                        }
                        #endregion

                        //Check if sport is not exist, add a new sport
                        #region Add new sport
                        var existedSport = (await _sportService.GetSportByName(sportValue)).FirstOrDefault();
                        if (existedSport == null)
                        {
                            var newSport = new Sport()
                            {
                                Name = sportValue,
                            };
                            await _sportService.AddSport(newSport);
                        }
                        #endregion

                        //Check if category is not exist, add a new category
                        #region Add new category
                        var existedCategory = (await _categoryService.GetCategoryByName(categoryValue)).FirstOrDefault();
                        var sport = (await _sportService.GetSportByName(sportValue)).FirstOrDefault();
                        if (existedCategory == null)
                        {
                            var newCategory = new Category()
                            {
                                CategoryName = categoryValue,
                                SportId = sport.Id,
                                Description = "",
                            };
                            await _categoryService.AddCategory(newCategory);
                        }
                        #endregion

                        //Check if supplier is not exist, add a new supplier
                        #region Add new supplier
                        var existedSupplier = (await _supplierService.GetSuppliersAsync(supplierValue)).FirstOrDefault();
                        if (existedSupplier == null)
                        {
                            var supplierLocation = reader.GetValue(6)?.ToString();
                            var newSupplier = new Supplier()
                            {
                                SupplierName = supplierValue,
                                Location = supplierLocation,
                            };
                            await _supplierService.CreateANewSupplierAsync(newSupplier);
                        }
                        #endregion

                        // Assuming product creation logic if all fields are valid
                        var brand = (await _brandService.GetBrandsAsync(brandValue)).FirstOrDefault();
                        if (brand == null)
                        {
                            return (int)ProductErrors.BrandNameError;
                        }

                        var category = (await _categoryService.GetCategoryByName(categoryValue)).FirstOrDefault();
                        if (category == null)
                        {
                            return (int)ProductErrors.CategoryNameError;
                        }

                        var supplier = (await _supplierService.GetSuppliersAsync(supplierValue)).FirstOrDefault();
                        if (supplier == null)
                        {
                            return (int)ProductErrors.SupplierNameError;
                        }
                        var existedProduct = await _productService.GetProductByProductCode(productCodeValue);

                        var employeeDetail = await _employeeDetailService.GetEmployeeDetailByEmployeeId(managerId);
                        var product = new Product
                        {
                            BrandId = brand.Id,
                            CategoryId = category.Id,
                            SportId = sport.Id,
                            ProductName = productNameValue,
                            ProductCode = productCodeValue,
                            ListedPrice = decimal.TryParse(listedPriceValue, out var listedPrice) ? listedPrice : 0,
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
                            var firstImgValue = reader.GetValue(19)?.ToString();
                            var secondImgValue = reader.GetValue(20)?.ToString();
                            var thirdImgValue = reader.GetValue(21)?.ToString();
                            var fourthImgValue = reader.GetValue(22)?.ToString();
                            var fifthImgValue = reader.GetValue(23)?.ToString();
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
                                            BlogId = null,
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
                                BranchId = employeeDetail.BranchId,
                                ProductId = product.Id,
                                Quantity = int.Parse(quantityValue),
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
                                newProduct.ListedPrice = decimal.Parse(listedPriceValue);
                                newProduct.Price = decimal.Parse(priceValue);
                                await _productService.AddProduct(newProduct);

                                var warehouse = new Warehouse()
                                {
                                    BranchId = employeeDetail.BranchId,
                                    ProductId = newProduct.Id,
                                    Quantity = int.Parse(quantityValue),
                                };
                                await _warehouseService.CreateANewWarehouseAsync(warehouse);
                            }
                            else
                            {

                                var existedWarehouse = (await _warehouseService.GetWarehouseByProductIdAndBranchId(
                                                                                        existedProductWithSizeColorCodition.Id,
                                                                                        employeeDetail.BranchId))
                                                                                        .FirstOrDefault();

                                existedWarehouse.Quantity += int.Parse(quantityValue);
                                await _warehouseService.UpdateWarehouseAsync(existedWarehouse);
                            }
                        }
                        if (existedProduct != null)
                        {
                            product = existedProduct;
                        }

                        //Save import history
                        var employee = await _employeeDetailService.GetEmployeeDetailByEmployeeId(managerId);
                        var importedBranch = await _branchService.GetBranchById(employee.BranchId);
                        var importHistory = new ImportHistory()
                        {
                            EmployeeId = managerId,
                            ProductId = product.Id,
                            Content = $@"{importedBranch.BranchName}: Import {int.Parse(quantityValue)} {productNameValue} ({productCodeValue})",
                            ImportDate = DateTime.Now,
                            Quantity = int.Parse(quantityValue),
                            SupplierId = supplier.Id,
                            LotCode = lotCodeValue,
                        };
                        await _importHistoryService.CreateANewImportHistoryAsync(importHistory);
                    } catch (Exception ex)
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
                        EmployeeId = userId,
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