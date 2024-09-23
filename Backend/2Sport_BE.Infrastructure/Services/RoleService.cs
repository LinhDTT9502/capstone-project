using _2Sport_BE.Repository.Interfaces;
using _2Sport_BE.Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2Sport_BE.Service.Services
{
    public interface IRoleService
    {
        Task<Role> GetRoleById(int? id);
    }
    public class RoleService : IRoleService
    {
        public readonly IUnitOfWork _unitOfWork;
        public RoleService(IUnitOfWork unitOfWork)
        {
        _unitOfWork = unitOfWork;
        }
        public async Task<Role> GetRoleById(int? id)
        {
            var query = await _unitOfWork.RoleRepository.GetObjectAsync(r => r.Id == id);
            if (query == null)
            {
                return null;
            }
            return query;
        }
    }
}
