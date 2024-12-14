using _2Sport_BE.Repository.Interfaces;
using _2Sport_BE.Repository.Models;
using _2Sport_BE.Service.DTOs;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace _2Sport_BE.Service.Services
{
    public interface IProductService
    {
        Task<IQueryable<Product>> GetAllProducts();
        Task<IQueryable<Product>> GetProducts(Expression<Func<Product, bool>> filter = null);

        Task<IQueryable<Product>> GetProducts(Expression<Func<Product, bool>> filter = null,
                                                    string includeProperties = "",
                                                    int? pageIndex = null, int? pageSize = null);

        Task<IQueryable<Product>> GetProducts(Expression<Func<Product, bool>> filter = null,
                                  Func<IQueryable<Product>, IOrderedQueryable<Product>> orderBy = null,
                                  string includeProperties = "",
                                  int? pageIndex = null,
                                  int? pageSize = null);

        Task<Product> GetProductById(int id);
        Task<Product> GetProductByProductCode(string productCode);
        Task<Product> AddProduct(Product product);
        Task AddProducts(IEnumerable<Product> products);
        Task DeleteProductById(int id);
        Task UpdateProduct(Product newProduct);
        Task<IQueryable<Product>> GetProductByProductCodeSizeColorCondition(string productCode, string size, string color, int condition);
        Task<IQueryable<Product>> GetProductsByProductCode(string productCode);
        Task<List<ColorStatusDTO>> GetColorsOfProduct(string productCode);
        Task<List<SizeStatusDTO>> GetSizesOfProduct(string productCode, string color);
        Task<List<ConditionStatusDTO>> GetConditionsOfProduct(string productCode, string color, string size);
        Task<IQueryable<Product>> GetProductByProductCodeAndColor(string productCode, string color);
    }
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Product> AddProduct(Product product)
        {
            try
            {
                await _unitOfWork.ProductRepository.InsertAsync(product);
                return product;
            } catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }

        }

        public async Task AddProducts(IEnumerable<Product> products)
        {
            await _unitOfWork.ProductRepository.InsertRangeAsync(products);
        }

        public async Task DeleteProductById(int id)
        {
            try
            {
                var deletedProduct = await _unitOfWork.ProductRepository.FindAsync(id);
                deletedProduct.Status = false;
                await _unitOfWork.ProductRepository.UpdateAsync(deletedProduct);

                //delete warehouses include deleted products
                var warehouses = await _unitOfWork.WarehouseRepository.GetAsync(_ => _.ProductId == deletedProduct.Id);
                foreach (var item in warehouses)
                {
                    item.AvailableQuantity = 0;
                    item.TotalQuantity = 0;
                    await _unitOfWork.WarehouseRepository.UpdateAsync(item);
                }

                //delete likes include deleted products
                var likes = await _unitOfWork.LikeRepository.GetAsync(_ => _.ProductCode.ToLower()
                                                        .Equals(deletedProduct.ProductCode.ToLower()));
                await _unitOfWork.LikeRepository.DeleteRangeAsync(likes);

                //delete reviews include deleted products
                var reviews = await _unitOfWork.ReviewRepository.GetAsync(_ => _.ProductCode.ToLower()
                                                        .Equals(deletedProduct.ProductCode.ToLower()));
                await _unitOfWork.ReviewRepository.DeleteRangeAsync(reviews);

                //delete imagesVideos include deleted products
                var imagesVideos = await _unitOfWork.ImagesVideoRepository.GetAsync(_ => _.ProductId == deletedProduct.Id);
                await _unitOfWork.ImagesVideoRepository.DeleteRangeAsync(imagesVideos);

            } catch(Exception ex) {
                Console.WriteLine(ex.Message);
            }
            
        }

        public async Task<IQueryable<Product>> GetAllProducts()
        {
            var query = await _unitOfWork.ProductRepository.GetAllAsync();
            return query.AsQueryable();
        }

        public async Task<List<ColorStatusDTO>> GetColorsOfProduct(string productCode)
        {
            var query = (await _unitOfWork.ProductRepository.GetAsync(_ => _.ProductCode
                                                       .ToLower().Equals(productCode.ToLower())))
                                                            .GroupBy(_ => _.Color)
                                                            .Select(_ => _.First());
            
            var listColorsAndStatus = new List<ColorStatusDTO>();

            foreach (var product in query)
            {
                var sizes = await GetSizesOfProduct(productCode, product.Color);
                var status = false;
                foreach (var size in sizes)
                {
                    if (size.Status)
                    {
                        status = true;
                        break;
                    }
                }
                if (!string.IsNullOrEmpty(product.Color) && 
                    !listColorsAndStatus.Any(cs => cs.Color == product.Color && cs.Status == product.Status))
                {
                    listColorsAndStatus.Add(new ColorStatusDTO() { Color = product.Color, Status = status });
                }
            }
            return listColorsAndStatus;

        }

        public async Task<List<ConditionStatusDTO>> GetConditionsOfProduct(string productCode, string color, string size)
        {
            var query = (await _unitOfWork.ProductRepository
                .GetAsync(_ => _.ProductCode.ToLower().Equals(productCode.ToLower())
                            && _.Color.ToLower().Equals(color.ToLower())
                            && _.Size.ToLower().Equals(size.ToLower())));

            var listConditions = new List<ConditionStatusDTO>();

            foreach (var product in query)
            {
                var warehouse = await _unitOfWork.WarehouseRepository.GetAsync(_ => _.ProductId == product.Id);

                var status = true;
                if (warehouse.FirstOrDefault().AvailableQuantity == 0)
                {
                    status = false;
                }
                var conditionStatusPair = new ConditionStatusDTO() { Status = status, Condition = product.Condition };
                if (!listConditions.Any(cs => cs.Condition == conditionStatusPair.Condition &&
                                              cs.Status == conditionStatusPair.Status))
                {
                    listConditions.Add(conditionStatusPair);
                }
            }

            return listConditions;
        }


        public async Task<List<SizeStatusDTO>> GetSizesOfProduct(string productCode, string color)
        {
            var query = (await _unitOfWork.ProductRepository
                .GetAsync(_ => _.ProductCode.ToLower().Equals(productCode.ToLower())
                            && _.Color.ToLower().Equals(color.ToLower())))
                .GroupBy(_ => _.Size)
                .Select(_ => _.First());

            var listSizes = new List<SizeStatusDTO>();

            foreach (var product in query)
            {
                var condiditions = await GetConditionsOfProduct(productCode, color, product.Size);
                var status = false;
                foreach (var condition in condiditions)
                {
                    if (condition.Status)
                    {
                        status = true;
                        break;
                    }
                }
                
                var sizeStatusPair = new SizeStatusDTO() { Status = status, Size = product.Size };
                if (!listSizes.Any(ss => ss.Size == sizeStatusPair.Size &&
                                         ss.Status == sizeStatusPair.Status))
                {
                    listSizes.Add(sizeStatusPair);
                }
            }

            return listSizes;
        }


        public async Task<Product> GetProductById(int id)
        {
            return (await _unitOfWork.ProductRepository.GetAsync(_ => _.Id == id, "ImagesVideos")).FirstOrDefault();
        }

        public async Task<Product> GetProductByProductCode(string productCode)
        {
            return (await _unitOfWork.ProductRepository.GetAsync(_ => _.Status == true && _.ProductCode
                                                       .ToLower().Equals(productCode.ToLower()))).FirstOrDefault();
        }

        public async Task<IQueryable<Product>> GetProductByProductCodeSizeColorCondition(string productCode, string size, string color, int condition)
        {
            var product = await _unitOfWork.ProductRepository.GetAsync(_ => _.ProductCode.ToLower()
                                                                        .Equals(productCode.ToLower()) &&
                                                                        _.Size.ToLower().Equals(size.ToLower()) &&
                                                                        _.Color.ToLower().Equals(color.ToLower()) &&
                                                                        _.Condition == condition);
            return product.AsQueryable();
        }

        public async Task<IQueryable<Product>> GetProducts(Expression<Func<Product, bool>> filter = null)
        {
            var query = await _unitOfWork.ProductRepository.GetAsync(filter);
            return query.AsQueryable();
        }

        public async Task<IQueryable<Product>> GetProducts(Expression<Func<Product, bool>> filter = null,
                                                            int? pageIndex = null,
                                                            int? pageSize = null)
        {
            var query = await _unitOfWork.ProductRepository.GetAsync(filter, null, null, pageIndex, pageSize);
            return query.AsQueryable();
        }

        public async Task<IQueryable<Product>> GetProducts(Expression<Func<Product, bool>> filter = null,
                                                    Func<IQueryable<Product>,
                                                    IOrderedQueryable<Product>> orderBy = null, string includeProperties = "",
                                                    int? pageIndex = null, int? pageSize = null)
        {
            try
            {
                var query = await _unitOfWork.ProductRepository.GetAsync(filter, orderBy, includeProperties, null, null);
                // Group by ProductCode and ProductName, then select the first item in each group
                var distinctProducts = query
                    .GroupBy(p => new { p.ProductCode, p.ProductName })
                    .Select(g => g.First())
                    .Skip((int)(pageIndex * pageSize)).Take((int)pageSize);
                return distinctProducts.AsQueryable();
            } catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
            
            

        }

        public async Task<IQueryable<Product>> GetProducts(Expression<Func<Product, bool>> filter = null,
                                                    string includeProperties = "",
                                                    int? pageIndex = null, int? pageSize = null)
        {
            var query = await _unitOfWork.ProductRepository.GetAsync(filter, includeProperties, pageIndex, pageSize);
            return query.AsQueryable();
        }

        public async Task<IQueryable<Product>> GetProductsByProductCode(string productCode)
        {
            var query = await _unitOfWork.ProductRepository.GetAsync(_ =>_.Status == true && _.ProductCode
                                                       .ToLower().Equals(productCode.ToLower()), "ImagesVideos");
            return query.AsQueryable();
        }

        public async Task UpdateProduct(Product newProduct)
        {
            await _unitOfWork.ProductRepository.UpdateAsync(newProduct);
        }

        public async Task<IQueryable<Product>> GetProductByProductCodeAndColor(string productCode, string color)
        {
            var query = await _unitOfWork.ProductRepository.GetAsync(_ => _.Status == true && _.ProductCode
                                                       .ToLower().Equals(productCode.ToLower()) &&
                                                       _.Color.ToLower().Equals(color.ToLower()));
            return query.AsQueryable();
        }
    }
}
