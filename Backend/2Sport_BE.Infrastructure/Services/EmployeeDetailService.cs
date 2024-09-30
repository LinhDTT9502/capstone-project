using _2Sport_BE.Repository.Interfaces;
using _2Sport_BE.Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2Sport_BE.Service.Services
{
    public interface IEmployeeDetailService
    {
        Task<EmployeeDetail> GetEmployeeDetail(int id);
    }
    public class EmployeeDetailService : IEmployeeDetailService
    {
        private readonly IUnitOfWork _unitOfWork;
        public EmployeeDetailService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<EmployeeDetail> GetEmployeeDetail(int id)
        {
            var query = await _unitOfWork.EmployeeDetailRepository
                .GetObjectAsync(ed => ed.Id == id);
            if (query == null) return null;

            return query;
        }
    }
}
