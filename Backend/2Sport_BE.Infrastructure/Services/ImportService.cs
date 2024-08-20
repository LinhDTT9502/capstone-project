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
    public interface IImportHistoryService
    {
        Task<IQueryable<ImportHistory>> ListAllAsync();
        Task<IQueryable<ImportHistory>> GetImportHistory(Expression<Func<ImportHistory, bool>> filter = null,
                                string includeProperties = "");
        Task<IQueryable<ImportHistory>> GetImportHistorysAsync(string supplierName);
        Task<IQueryable<ImportHistory>> GetImportHistoryById(int? id);

        Task CreateANewImportHistoryAsync(ImportHistory import);
        Task UpdateImportHistoryAsync(ImportHistory import);
        Task DeleteImportHistoryAsync(int id);
    }
    public class ImportHistoryService : IImportHistoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly TwoSportDBContext _dBContext;
        public ImportHistoryService(IUnitOfWork unitOfWork, TwoSportDBContext dBContext)
        {
            _unitOfWork = unitOfWork;
            _dBContext = dBContext;
        }

        public async Task CreateANewImportHistoryAsync(ImportHistory supplier)
        {
            await _unitOfWork.ImportHistoryRepository.InsertAsync(supplier);
        }

        public async Task DeleteImportHistoryAsync(int id)
        {
            var toDeleteObject = await _dBContext.ImportHistories.FirstOrDefaultAsync(_ => _.Id == id);
            if (toDeleteObject != null)
            {
                await _unitOfWork.ImportHistoryRepository.DeleteAsync(toDeleteObject);
            }
        }

        public async Task<IQueryable<ImportHistory>> GetImportHistory(Expression<Func<ImportHistory, bool>> filter = null, string includeProperties = "")
        {
            return (await _unitOfWork.ImportHistoryRepository.GetAsync(filter, null, includeProperties, null, null)).AsQueryable();
        }

        public async Task<IQueryable<ImportHistory>> GetImportHistoryById(int? id)
        {
            IEnumerable<ImportHistory> filter = await _unitOfWork.ImportHistoryRepository.GetAsync(_ => _.Id == id);
            return filter.AsQueryable();
        }

        public async Task<IQueryable<ImportHistory>> GetImportHistorysAsync(string supplierName)
        {
            IEnumerable<ImportHistory> filter = await _unitOfWork.ImportHistoryRepository.GetAsync(_ => _.Supplier.SupplierName.ToUpper().Contains(supplierName.ToUpper()));
            return filter.AsQueryable();
        }

        public async Task<IQueryable<ImportHistory>> ListAllAsync()
        {
            IEnumerable<ImportHistory> listAll = await _unitOfWork.ImportHistoryRepository.GetAllAsync();
            return listAll.AsQueryable();
        }

        public async Task UpdateImportHistoryAsync(ImportHistory supplier)
        {
            await _unitOfWork.ImportHistoryRepository.UpdateAsync(supplier);
        }
    }
}
