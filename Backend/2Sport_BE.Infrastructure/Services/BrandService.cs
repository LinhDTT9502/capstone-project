using _2Sport_BE.Repository.Interfaces;
using _2Sport_BE.Repository.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2Sport_BE.Infrastructure.Services
{
    public interface IBrandService
    {
        Task<IQueryable<Brand>> ListAllAsync();
        Task<int> NumberOfBrandsAsync();
        Task<IQueryable<Brand>> GetBrandsAsync(string brandName);
        Task<IQueryable<Brand>> GetBrandById(int? id);
        Task<IQueryable<Brand>> GetBrandsByCategoryAsync(string category);

        Task CreateANewBrandAsync(Brand brand);
        Task UpdateBrandAsync(Brand brand);
        Task DeleteBrandAsync(int id);
    }
    public class BrandService : IBrandService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly TwoSportDBContext _dBContext;
        public BrandService(IUnitOfWork unitOfWork, TwoSportDBContext dBContext)
        {
            _unitOfWork = unitOfWork;
            _dBContext = dBContext;
        }

        public async Task CreateANewBrandAsync(Brand brand)
        {
            await _unitOfWork.BrandRepository.InsertAsync(brand);
        }

        public async Task DeleteBrandAsync(int id)
        {
            var toDeleteObject = await _dBContext.Brands.FirstOrDefaultAsync(_ => _.Id == id);
            if (toDeleteObject != null)
            {
                await _unitOfWork.BrandRepository.DeleteAsync(toDeleteObject);
            }
        }

        public async Task<IQueryable<Brand>> GetBrandById(int? id)
        {
            IEnumerable<Brand> filter = await _unitOfWork.BrandRepository.GetAsync(_ => _.Id == id);
            return filter.AsQueryable();
        }

        public async Task<IQueryable<Brand>> GetBrandsAsync(string brandName)
        {
            IEnumerable<Brand> filter = await _unitOfWork.BrandRepository.GetAsync(_ => _.BrandName.ToUpper().Contains(brandName.ToUpper()));
            return filter.AsQueryable();
        }

        public Task<IQueryable<Brand>> GetBrandsByCategoryAsync(string category)
        {
            throw new NotImplementedException();
        }


        public async Task<IQueryable<Brand>> ListAllAsync()
        {
            IEnumerable<Brand> listAll = await _unitOfWork.BrandRepository.GetAllAsync();
            return listAll.AsQueryable();
        }

        public async Task<int> NumberOfBrandsAsync()
        {
            return await _unitOfWork.BrandRepository.CountAsync();
        }

        public async Task UpdateBrandAsync(Brand brand)
        {
            await _unitOfWork.BrandRepository.UpdateAsync(brand);
        }
    }
}
