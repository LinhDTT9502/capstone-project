using _2Sport_BE.Helpers;
using _2Sport_BE.Infrastructure.Services;
using _2Sport_BE.Repository.Interfaces;
using _2Sport_BE.Repository.Models;
using _2Sport_BE.Service.Services;
using _2Sport_BE.ViewModels;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Bcpg.OpenPgp;
using System.Linq;
using System.Net.WebSockets;

namespace _2Sport_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IBrandService _brandService;
        private readonly ICategoryService _categoryService;
        private readonly ISportService _sportService;
        private readonly ILikeService _likeService;
        private readonly IReviewService _reviewService;
        private readonly IUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;

        public ProductController(IProductService productService, 
                                IBrandService brandService, 
                                ICategoryService categoryService,
                                IUnitOfWork unitOfWork,
								ISportService sportService,
								ILikeService likeService,
								IReviewService reviewService,
                                IMapper mapper)
		{
            _productService = productService;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _brandService = brandService;
            _categoryService = categoryService;
            _sportService = sportService;
            _likeService = likeService;
            _reviewService = reviewService;
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
                    var classification = await _unitOfWork.ClassificationRepository.FindAsync(product.ClassificationId);
                    product.Classification = classification;
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
                    var classification = await _unitOfWork.ClassificationRepository.FindAsync(product.ClassificationId);
                    product.Classification = classification;
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
                    var classification = await _unitOfWork.ClassificationRepository.FindAsync(product.ClassificationId);
                    product.Classification = classification;

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
                    var classification = await _unitOfWork.ClassificationRepository.FindAsync(product.ClassificationId);
                    product.Classification = classification;
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

		[HttpPut]
        [Route("update-product/{productId}")]
        public async Task<IActionResult> UpdateProduct(int productId, ProductUM productUM)
        {
            try
            {
                var updatedProduct = await _productService.GetProductById(productId);
                if (updatedProduct != null)
                {
                    updatedProduct.ProductName = productUM.ProductName;
                    updatedProduct.ListedPrice = productUM.ListedPrice;
                    updatedProduct.Price = productUM.Price;
                    updatedProduct.Size = productUM.Size;
                    updatedProduct.Description = productUM.Description;
                    updatedProduct.Status = productUM.Status;
                    updatedProduct.Color = productUM.Color;
                    updatedProduct.Offers = productUM.Offers;
                    updatedProduct.MainImageName = productUM.MainImageName;
                    updatedProduct.MainImagePath = productUM.MainImagePath;
                    updatedProduct.CategoryId = productUM.CategoryId;
                    updatedProduct.BrandId = productUM.BrandId;
                    updatedProduct.SportId = productUM.SportId;
                    updatedProduct.ClassificationId = productUM.ClassificationId;
                    await _productService.UpdateProduct(updatedProduct);
                    return Ok(updatedProduct);
                } else
                {
                    return BadRequest("Update failed!");
                }
            } catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPost]
        [Route("add-product-list")]
        public async Task<IActionResult> AddProductList(List<ProductCM> productList)
        {
            try
            {
                var addedProducts = _mapper.Map<List<Product>>(productList);
				await _productService.AddProducts(addedProducts);
                return Ok("Add products successfully!");
                
			} catch (Exception e)
            {
                return BadRequest(e);
            }
            
        }

        [HttpDelete]
        [Route("delete-product/{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            try
            {
                await _productService.DeleteProductById(id);
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
    }
}
