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
    public interface ISupplierService
    {
        Task<IQueryable<Supplier>> ListAllAsync();
        Task<int> NumberOfSuppliersAsync();
        Task<IQueryable<Supplier>> GetSuppliersAsync(string supplierName);
        Task<IQueryable<Supplier>> GetSupplierById(int? id);
        Task<IQueryable<Supplier>> GetSuppliersByCategoryAsync(string category);

        Task CreateANewSupplierAsync(Supplier supplier);
        Task UpdateSupplierAsync(Supplier supplier);
        Task DeleteSupplierAsync(int id);
    }
    public class SupplierService : ISupplierService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly TwoSportDBContext _dBContext;
        public SupplierService(IUnitOfWork unitOfWork, TwoSportDBContext dBContext)
        {
            _unitOfWork = unitOfWork;
            _dBContext = dBContext;
        }

        public async Task CreateANewSupplierAsync(Supplier supplier)
        {
            await _unitOfWork.SupplierRepository.InsertAsync(supplier);
        }

        public async Task DeleteSupplierAsync(int id)
        {
            var toDeleteObject = await _dBContext.Suppliers.FirstOrDefaultAsync(_ => _.Id == id);
            if (toDeleteObject != null)
            {
                await _unitOfWork.SupplierRepository.DeleteAsync(toDeleteObject);
            }
        }

        public async Task<IQueryable<Supplier>> GetSupplierById(int? id)
        {
            IEnumerable<Supplier> filter = await _unitOfWork.SupplierRepository.GetAsync(_ => _.Id == id);
            return filter.AsQueryable();
        }

        public async Task<IQueryable<Supplier>> GetSuppliersAsync(string supplierName)
        {
            IEnumerable<Supplier> filter = await _unitOfWork.SupplierRepository.GetAsync(_ => _.SupplierName.ToUpper().Contains(supplierName.ToUpper()));
            return filter.AsQueryable();
        }

        public Task<IQueryable<Supplier>> GetSuppliersByCategoryAsync(string category)
        {
            throw new NotImplementedException();
        }


        public async Task<IQueryable<Supplier>> ListAllAsync()
        {
            IEnumerable<Supplier> listAll = await _unitOfWork.SupplierRepository.GetAllAsync();
            return listAll.AsQueryable();
        }

        public async Task<int> NumberOfSuppliersAsync()
        {
            return await _unitOfWork.SupplierRepository.CountAsync();
        }

        public async Task UpdateSupplierAsync(Supplier supplier)
        {
            await _unitOfWork.SupplierRepository.UpdateAsync(supplier);
        }
    }
}
