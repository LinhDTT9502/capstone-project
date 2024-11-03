using _2Sport_BE.Repository.Data;
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
        Task DeleteWarehouseAsync(List<Warehouse> warehouses);
        Task<IQueryable<Warehouse>> GetWarehouseByProductIdAndBranchId(int productId, int? branchId);
        Task UpdateWarehouseStock(Warehouse productInWarehouse, int quantity);
        Task AdjustStockLevel(Warehouse productInWarehouse, int? quantity, bool isRestock);

        bool IsStockAvailable(Warehouse productInWarehouse, int quantity);
    }
    public class WarehouseService : IWarehouseService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly TwoSportCapstoneDbContext _dBContext;
        public WarehouseService(IUnitOfWork unitOfWork, TwoSportCapstoneDbContext dBContext)
        {
            _unitOfWork = unitOfWork;
            _dBContext = dBContext;
        }

        public async Task AdjustStockLevel(Warehouse productInWarehouse, int? quantity, bool isRestock)
        {
            if (isRestock) //Tang quantity lai
            {
                productInWarehouse.AvailableQuantity += quantity;
                productInWarehouse.TotalQuantity += quantity;

            }
            else
            {
                if (productInWarehouse.AvailableQuantity >= quantity)
                {
                    productInWarehouse.AvailableQuantity -= quantity;
                    productInWarehouse.TotalQuantity -= quantity;
                }
                else
                {
                    throw new InvalidOperationException("Insufficient stock available.");
                }
            }

            await _unitOfWork.WarehouseRepository.UpdateAsync(productInWarehouse);
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

        public async Task DeleteWarehouseAsync(List<Warehouse> warehouses)
        {
            await _unitOfWork.WarehouseRepository.DeleteRangeAsync(warehouses);
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

        public async Task<IQueryable<Warehouse>> GetWarehouseByProductIdAndBranchId(int productId, int? branchId)
        {
            IEnumerable<Warehouse> filter = await _unitOfWork.WarehouseRepository.GetAsync(_ => _.ProductId == productId
                                                                                            && _.BranchId == branchId);
            return filter.AsQueryable();
        }

        public bool IsStockAvailable(Warehouse productInWarehouse, int quantity)
        {
            return productInWarehouse != null &&
                    productInWarehouse.TotalQuantity >= quantity &&
                    productInWarehouse.AvailableQuantity >= quantity;
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

        public async Task UpdateWarehouseStock(Warehouse productInWarehouse, int quantity)
        {
            productInWarehouse.AvailableQuantity -= quantity;
            await _unitOfWork.WarehouseRepository.UpdateAsync(productInWarehouse);
        }
    }
}
