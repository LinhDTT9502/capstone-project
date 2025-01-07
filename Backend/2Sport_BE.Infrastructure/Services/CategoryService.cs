using _2Sport_BE.Repository.Interfaces;
using _2Sport_BE.Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace _2Sport_BE.Service.Services
{
    public interface ICategoryService
    {
        Task<IQueryable<Category>> GetAllCategories();
        Task<IQueryable<Category>> GetCategories(Expression<Func<Category, bool>> filter = null);

        Task<IQueryable<Category>> GetCategories(Expression<Func<Category, bool>> filter = null,
                                  Func<IQueryable<Category>, IOrderedQueryable<Category>> orderBy = null,
                                  string includeProperties = "",
                                  int? pageIndex = null,
                                  int? pageSize = null);

        Task<Category> GetCategoryById(int? id);
        Task AddCategories(IEnumerable<Category> categories);
        Task AddCategory(Category category);
        Task DeleteCategoryById(int id);
        Task DeleteCategory(Category category);
        Task UpdateCategory(Category newCategory);
        Task<IQueryable<Category>> GetCategoryByName(string categoryName);
    }
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IProductService _productService;
        public CategoryService(IUnitOfWork unitOfWork,
                               IProductService productService)
        {
            _unitOfWork = unitOfWork;
            _productService = productService;
        }

        public async Task AddCategories(IEnumerable<Category> categories)
        {
            foreach (var category in categories)
            {
                category.Status = true;
                category.Quantity = 0;
            }
            await _unitOfWork.CategoryRepository.InsertRangeAsync(categories);
        }

        public async Task DeleteCategoryById(int id)
        {
            try
            {
                var deletedProducts = await _unitOfWork.ProductRepository.GetAsync(_ => _.CategoryId == id);
                foreach (var product in deletedProducts)
                {
                    await _productService.DeleteProductById(product.Id);
                }

                await _unitOfWork.CategoryRepository.DeleteAsync(id);

            } catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public async Task<IQueryable<Category>> GetAllCategories()
        {
            var query = await _unitOfWork.CategoryRepository.GetAllAsync();
            return query.AsQueryable();
        }

        public async Task<Category> GetCategoryById(int? id)
        {
            return await _unitOfWork.CategoryRepository.FindAsync(id);
        }

        public async Task<IQueryable<Category>> GetCategories(Expression<Func<Category, bool>> filter = null)
        {
            var query = await _unitOfWork.CategoryRepository.GetAsync(filter);
            return query.AsQueryable();
        }

        public async Task<IQueryable<Category>> GetCategories(Expression<Func<Category, bool>> filter = null, 
                                                            int? pageIndex = null, 
                                                            int? pageSize = null)
        {
            var query = await _unitOfWork.CategoryRepository.GetAsync(filter, null, null, pageIndex, pageSize);
            return query.AsQueryable();
        }

        public async Task<IQueryable<Category>> GetCategories(Expression<Func<Category, bool>> filter = null,
                                                    Func<IQueryable<Category>,
                                                    IOrderedQueryable<Category>> orderBy = null, string includeProperties = "",
                                                    int? pageIndex = null, int? pageSize = null)
        {
            var query = await _unitOfWork.CategoryRepository.GetAsync(filter, orderBy, includeProperties, pageIndex, pageSize);
            return query.AsQueryable();
        }

        public async Task UpdateCategory(Category newCategory)
        {
            await _unitOfWork.CategoryRepository.UpdateAsync(newCategory);
        }

        public async Task AddCategory(Category category)
        {
            category.CreatedAt = DateTime.Now;
            category.Status = true;
            category.Quantity = 0;
            await _unitOfWork.CategoryRepository.InsertAsync(category);
        }

        public async Task<IQueryable<Category>> GetCategoryByName(string categoryName)
        {
            var query = await _unitOfWork.CategoryRepository.GetAsync(_ => _.CategoryName.ToLower()
                                                                    .Equals(categoryName.ToLower()));
            return query.AsQueryable();
        }

        public async Task DeleteCategory(Category category)
        {
            await _unitOfWork.CategoryRepository.DeleteAsync(category);
        }
    }
}
