using _2Sport_BE.Repository.Data;
using _2Sport_BE.Repository.Interfaces;
using _2Sport_BE.Repository.Models;
using _2Sport_BE.Service.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Twilio.Rest.Api.V2010.Account;

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
        private readonly TwoSportCapstoneDbContext _dBContext;
        private readonly IProductService _productService;
        public BrandService(IUnitOfWork unitOfWork, 
                            TwoSportCapstoneDbContext dBContext, 
                            IProductService productService)
        {
            _unitOfWork = unitOfWork;
            _dBContext = dBContext;
            _productService = productService;
        }

        public async Task CreateANewBrandAsync(Brand brand)
        {
            brand.CreatedAt = DateTime.Now;
            brand.Status = true;
            await _unitOfWork.BrandRepository.InsertAsync(brand);
        }

        public async Task DeleteBrandAsync(int id)
        {
            var products = await _unitOfWork.ProductRepository.GetAsync(_ => _.BrandId == id);
            foreach (var product in products)
            {
                await _productService.DeleteProductById(product.Id);
            }

            await _unitOfWork.BrandRepository.DeleteAsync(id);
        }

        public async Task<IQueryable<Brand>> GetBrandById(int? id)
        {
            IEnumerable<Brand> filter = await _unitOfWork.BrandRepository.GetAsync(_ => _.Id == id);
            return filter.AsQueryable();
        }

        public async Task<IQueryable<Brand>> GetBrandsAsync(string brandName)
        {
            IEnumerable<Brand> filter = await _unitOfWork.BrandRepository.GetAsync(_ => _.BrandName.ToUpper().Equals(brandName.ToUpper()));
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
