using _2Sport_BE.Repository.Interfaces;
using _2Sport_BE.Repository.Models;
using Microsoft.Extensions.Hosting;
using System.Linq.Expressions;

namespace _2Sport_BE.Service.Services
{
    public interface ISportService
    {
        Task<IQueryable<Sport>> GetAllSports();
        Task<IQueryable<Sport>> GetSports(Expression<Func<Sport, bool>> filter = null);

        Task<IQueryable<Sport>> GetSports(Expression<Func<Sport, bool>> filter = null,
                                  Func<IQueryable<Sport>, IOrderedQueryable<Sport>> orderBy = null,
                                  string includeProperties = "",
                                  int? pageIndex = null,
                                  int? pageSize = null);

        Task<Sport> GetSportById(int? id);
        Task AddSports(IEnumerable<Sport> sports);
        Task DeleteSportById(int id);
        Task UpdateSport(Sport newSport);
        Task<IQueryable<Sport>> GetSportByName(string sportName);
        Task AddSport(Sport newSport);
    }
    public class SportService : ISportService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICategoryService _categoryService;
        private readonly IProductService _productService;
        public SportService(IUnitOfWork unitOfWork, 
                            ICategoryService categoryService,
                            IProductService productService)
        {
            _unitOfWork = unitOfWork;
            _categoryService = categoryService;
            _productService = productService;
        }

        public async Task AddSport(Sport newSport)
        {
            await _unitOfWork.SportRepository.InsertAsync(newSport);
        }

        public async Task AddSports(IEnumerable<Sport> sports)
        {
            await _unitOfWork.SportRepository.InsertRangeAsync(sports);
        }

        public async Task DeleteSportById(int id)
        {
            try
            {
                var products = await _unitOfWork.ProductRepository.GetAsync(_ => _.SportId == id);
                foreach (var product in products)
                {
                    await _productService.DeleteProductById(product.Id);
                }

                var categories = await _unitOfWork.CategoryRepository.GetAsync(_ => _.SportId == id);
                foreach(var category in categories)
                {
                    await _categoryService.DeleteCategoryById(category.Id);
                }


            await _unitOfWork.SportRepository.DeleteAsync(id);
            } catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public async Task<IQueryable<Sport>> GetAllSports()
        {
            var query = await _unitOfWork.SportRepository.GetAllAsync();
            return query.AsQueryable();
        }

        public async Task<Sport> GetSportById(int? id)
        {
            return await _unitOfWork.SportRepository.FindAsync(id);
        }

        public async Task<IQueryable<Sport>> GetSportByName(string sportName)
        {
            var query = await _unitOfWork.SportRepository.GetAsync(_ => _.Name.ToLower().Equals(sportName.ToLower()));
            return query.AsQueryable();
        }

        public async Task<IQueryable<Sport>> GetSports(Expression<Func<Sport, bool>> filter = null)
        {
            var query = await _unitOfWork.SportRepository.GetAsync(filter);
            return query.AsQueryable();
        }

        public async Task<IQueryable<Sport>> GetSports(Expression<Func<Sport, bool>> filter = null, int? pageIndex = null, int? pageSize = null)
        {
            var query = await _unitOfWork.SportRepository.GetAsync(filter, null, null, pageIndex, pageSize);
            return query.AsQueryable();
        }

        public async Task<IQueryable<Sport>> GetSports(Expression<Func<Sport, bool>> filter = null, 
                                                    Func<IQueryable<Sport>, 
                                                    IOrderedQueryable<Sport>> orderBy = null, string includeProperties = "", 
                                                    int? pageIndex = null, int? pageSize = null)
        {
            var query = await _unitOfWork.SportRepository.GetAsync(filter, orderBy, includeProperties, pageIndex, pageSize);
            return query.AsQueryable();
        }

        public async Task UpdateSport(Sport newSport)
        {
            await _unitOfWork.SportRepository.UpdateAsync(newSport);
        }

    }
}
