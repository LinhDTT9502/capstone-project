using _2Sport_BE.Repository.Data;
using _2Sport_BE.Repository.Interfaces;
using _2Sport_BE.Repository.Models;
using Microsoft.EntityFrameworkCore;
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
            var toDeleteObject = await _dBContext.Branches.FirstOrDefaultAsync(_ => _.Id == id);
            if (toDeleteObject != null)
            {
                await _unitOfWork.BranchRepository.DeleteAsync(toDeleteObject);
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
