using _2Sport_BE.Repository.Data;
using _2Sport_BE.Repository.Interfaces;
using _2Sport_BE.Repository.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2Sport_BE.Service.Services
{
    public interface IBranchService
    {
        Task<IQueryable<Branch>> ListAllAsync();
        Task<int> NumberOfBranchesAsync();
        Task<IQueryable<Branch>> GetBranchesAsync(string branchName);
        Task<Branch> GetBranchById(int? id);
        Task<IQueryable<Branch>> GetBranchesByCategoryAsync(string category);

        Task CreateANewBranchAsync(Branch branch);
        Task UpdateBranchAsync(Branch branch);
        Task DeleteBranchAsync(int id);
    }

    public class BranchService : IBranchService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly TwoSportCapstoneDbContext _dBContext;
        public BranchService(IUnitOfWork unitOfWork, TwoSportCapstoneDbContext dBContext)
        {
            _unitOfWork = unitOfWork;
            _dBContext = dBContext;
        }

        public async Task CreateANewBranchAsync(Branch branch)
        {
            branch.CreatedAt = DateTime.Now;
            branch.Status = true;
            await _unitOfWork.BranchRepository.InsertAsync(branch);
        }

        public async Task DeleteBranchAsync(int id)
        {
            try
            {
                //delete warehouses have BranchId same as Branch.Id
                var warehouses = await _unitOfWork.WarehouseRepository.GetAsync(_ => _.BranchId == id);
                await _unitOfWork.WarehouseRepository.DeleteRangeAsync(warehouses);

                //delete orders have BranchId same as Branch.Id
                var orders = await _unitOfWork.SaleOrderRepository.GetAsync(_ => _.BranchId == id);
                await _unitOfWork.SaleOrderRepository.DeleteRangeAsync(orders);

                //delete employees have BranchId same as Branch.Id
                var employees = await _unitOfWork.StaffRepository.GetAsync(_ => _.BranchId == id);
                await _unitOfWork.StaffRepository.DeleteRangeAsync(employees);

                //delete Branch
                await _unitOfWork.BranchRepository.DeleteAsync(id);
            } catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }

        public async Task<Branch> GetBranchById(int? id)
        {
            IEnumerable<Branch> filter = await _unitOfWork.BranchRepository.GetAsync(_ => _.Id == id);
            return filter.FirstOrDefault();
        }

        public async Task<IQueryable<Branch>> GetBranchesAsync(string branchName)
        {
            IEnumerable<Branch> filter = await _unitOfWork.BranchRepository.GetAsync(_ => _.BranchName.ToUpper().Contains(branchName.ToUpper()));
            return filter.AsQueryable();
        }

        public Task<IQueryable<Branch>> GetBranchesByCategoryAsync(string category)
        {
            throw new NotImplementedException();
        }


        public async Task<IQueryable<Branch>> ListAllAsync()
        {
            IEnumerable<Branch> listAll = await _unitOfWork.BranchRepository.GetAllAsync();
            return listAll.AsQueryable();
        }

        public async Task<int> NumberOfBranchesAsync()
        {
            return await _unitOfWork.BranchRepository.CountAsync();
        }

        public async Task UpdateBranchAsync(Branch branch)
        {
            await _unitOfWork.BranchRepository.UpdateAsync(branch);
        }
    }
}
