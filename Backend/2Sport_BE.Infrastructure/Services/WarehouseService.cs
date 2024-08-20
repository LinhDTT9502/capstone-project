using _2Sport_BE.Repository.Interfaces;
using _2Sport_BE.Repository.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace _2Sport_BE.Infrastructure.Services
{
    public interface IWarehouseService
    {
        Task<IQueryable<Warehouse>> ListAllAsync();
        Task<IQueryable<Warehouse>> GetWarehouse(Expression<Func<Warehouse, bool>> filter = null);
        Task<IQueryable<Warehouse>> GetWarehouseById(int? id);
        Task<IQueryable<Warehouse>> GetWarehouseByProductId(int? productId);
        Task CreateANewWarehouseAsync(Warehouse warehouse);
        Task UpdateWarehouseAsync(Warehouse warehouse);
        Task DeleteWarehouseAsync(int id);
    }
    public class WarehouseService : IWarehouseService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly TwoSportDBContext _dBContext;
        public WarehouseService(IUnitOfWork unitOfWork, TwoSportDBContext dBContext)
        {
            _unitOfWork = unitOfWork;
            _dBContext = dBContext;
        }

        public async Task CreateANewWarehouseAsync(Warehouse warehouse)
        {
            await _unitOfWork.WarehouseRepository.InsertAsync(warehouse);
        }

        public async Task DeleteWarehouseAsync(int id)
        {
            var toDeleteObject = await _dBContext.Warehouses.FirstOrDefaultAsync(_ => _.Id == id);
            if (toDeleteObject != null)
            {
                await _unitOfWork.WarehouseRepository.DeleteAsync(toDeleteObject);
            }
        }

        public async Task<IQueryable<Warehouse>> GetWarehouse(Expression<Func<Warehouse, bool>> filter = null)
        {
            var query = await _unitOfWork.WarehouseRepository.GetAsync(filter);
            return query.AsQueryable();
        }

        public async Task<IQueryable<Warehouse>> GetWarehouseById(int? id)
        {
            IEnumerable<Warehouse> filter = await _unitOfWork.WarehouseRepository.GetAsync(_ => _.Id == id);
            return filter.AsQueryable();
        }

        public async Task<IQueryable<Warehouse>> GetWarehouseByProductId(int? productId)
        {
            IEnumerable<Warehouse> filter = await _unitOfWork.WarehouseRepository.GetAsync(_ => _.ProductId == productId);
            return filter.AsQueryable();
        }

        public async Task<IQueryable<Warehouse>> ListAllAsync()
        {
            IEnumerable<Warehouse> listAll = await _unitOfWork.WarehouseRepository.GetAllAsync();
            return listAll.AsQueryable();
        }

        public async Task UpdateWarehouseAsync(Warehouse warehouse)
        {
            await _unitOfWork.WarehouseRepository.UpdateAsync(warehouse);
        }
    }
}
