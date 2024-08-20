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
        Task DeleteCategoryById(int id);
        Task UpdateCategory(Category newCategory);
    }
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task AddCategories(IEnumerable<Category> categories)
        {
            await _unitOfWork.CategoryRepository.InsertRangeAsync(categories);
        }

        public async Task DeleteCategoryById(int id)
        {
            await _unitOfWork.CategoryRepository.DeleteAsync(id);
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
    }
}
