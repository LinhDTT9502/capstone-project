using _2Sport_BE.Repository.Interfaces;
using _2Sport_BE.Repository.Models;
using _2Sport_BE.Infrastructure.DTOs;

namespace _2Sport_BE.Infrastructure.Services
{
    public interface IRoleService
    {
        Task<Role> GetRoleById(int? id);
        Task<ResponseDTO<List<RoleVM>>> GetAllRoles();
    }
    public class RoleService : IRoleService
    {
        public readonly IUnitOfWork _unitOfWork;
        public RoleService(IUnitOfWork unitOfWork)
        {
        _unitOfWork = unitOfWork;
        }

        public async Task<ResponseDTO<List<RoleVM>>> GetAllRoles()
        {
            var response = new ResponseDTO<List<RoleVM>>();
            try
            {
                var query = await _unitOfWork.RoleRepository.GetAllAsync();
                if(query == null || !query.Any())
                {
                    response.IsSuccess = true;
                    response.Message = "Roles are not found";
                    return response;
                }
                var data = new List<RoleVM>();
                foreach (var role in query)
                {
                    var roleVM = new RoleVM()
                    {
                        RoleId = Convert.ToInt32(role["ID"]),
                        RoleName = Convert.ToString(role["RoleName"]),
                        Description = Convert.ToString(role["Description"]),
                        CreatedDate = Convert.ToDateTime(role["CreatedAt"])
                    };
                    data.Add(roleVM);
                }

                response.IsSuccess = true;
                response.Message = "Query Successfully";
                response.Data = data;
                return response;
            }
            catch(Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
                return response;
            }
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
