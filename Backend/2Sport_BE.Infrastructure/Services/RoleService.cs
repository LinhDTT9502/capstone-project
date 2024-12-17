using _2Sport_BE.Repository.Interfaces;
using _2Sport_BE.Repository.Models;
using _2Sport_BE.Infrastructure.DTOs;
using AutoMapper;

namespace _2Sport_BE.Infrastructure.Services
{
    public interface IRoleService
    {
        Task<Role> GetRoleById(int? id);
        Task<ResponseDTO<List<RoleVM>>> GetAllRoles();
        Task<ResponseDTO<RoleVM>> GetRoleDetails(int id);
        Task<ResponseDTO<RoleVM>> CreateRole(RoleCM roleCM);
        Task<ResponseDTO<RoleVM>> UpdateRole(int roleId, RoleUM roleUM);
        Task<ResponseDTO<int>> DeleteRole(int roleId);
    }
    public class RoleService : IRoleService
    {
        public readonly IUnitOfWork _unitOfWork;
        public readonly IMapper _mapper;
        public RoleService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        
        public async Task<ResponseDTO<RoleVM>> CreateRole(RoleCM roleCM)
        {
            var response = new ResponseDTO<RoleVM>();
            try
            {
                var isExited = await _unitOfWork.RoleRepository.GetObjectAsync(r => r.RoleName == roleCM.RoleName);
                if (isExited != null)
                {
                    response.IsSuccess = false;
                    response.Message = $"Role - {roleCM.RoleName} is existed";
                    return response;
                }
                Role role = new Role()
                {
                    RoleName = roleCM.RoleName,
                    Description = roleCM.Description,
                    CreateAt = DateTime.UtcNow,
                };
                await _unitOfWork.RoleRepository.InsertAsync(role);

                RoleVM result = _mapper.Map<RoleVM>(role);

                response.IsSuccess = true;
                response.Message = "Created Successfully";
                response.Data = result;
                return response;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
                return response;
            }
        }

        public async Task<ResponseDTO<int>> DeleteRole(int roleId)
        {
            var response = new ResponseDTO<int>();
            try
            {
                var isExited = await _unitOfWork.RoleRepository.GetObjectAsync(r => r.Id == roleId);
                if (isExited == null)
                {
                    response.IsSuccess = false;
                    response.Message = $"Role - {roleId} is not existed";
                    return response;
                }
                
                await _unitOfWork.RoleRepository.DeleteAsync(isExited);

                response.IsSuccess = true;
                response.Message = "Deleted Successfully";
                response.Data = 1;
                return response;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
                response.Data = 0;
                return response;
            }
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

        public async Task<ResponseDTO<RoleVM>> GetRoleDetails(int id)
        {
            var response = new ResponseDTO<RoleVM>();
            try
            {
                var isExited = await _unitOfWork.RoleRepository.GetObjectAsync(r => r.Id == id);
                if (isExited == null)
                {
                    response.IsSuccess = false;
                    response.Message = $"Role - {id} is not found";
                    return response;
                }
                
                RoleVM result = _mapper.Map<RoleVM>(isExited);

                response.IsSuccess = true;
                response.Message = "Created Successfully";
                response.Data = result;
                return response;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
                return response;
            }
        }

        public async Task<ResponseDTO<RoleVM>> UpdateRole(int roleId, RoleUM roleUM)
        {
            var response = new ResponseDTO<RoleVM>();
            try
            {
                var isExited = _unitOfWork.RoleRepository.FindObject(r => r.Id == roleId);
                if (isExited == null)
                {
                    response.IsSuccess = false;
                    response.Message = $"Role - {roleId} is not existed";
                    return response;
                }
                isExited.RoleName = roleUM.RoleName;
                isExited.Description = roleUM.Description;

                await _unitOfWork.RoleRepository.UpdateAsync(isExited);

                RoleVM result = _mapper.Map<RoleVM>(isExited);

                response.IsSuccess = true;
                response.Message = "Updated Successfully";
                response.Data = result;
                return response;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
                return response;
            }
        }
    }
}
