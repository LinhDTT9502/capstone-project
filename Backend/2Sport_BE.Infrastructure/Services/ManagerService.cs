using _2Sport_BE.Repository.Interfaces;
using _2Sport_BE.Infrastructure.DTOs;
using _2Sport_BE.Repository.Models;
using AutoMapper;
using _2Sport_BE.Infrastructure.Enums;
using Microsoft.CodeAnalysis.Semantics;
using Microsoft.IdentityModel.Tokens;
namespace _2Sport_BE.Infrastructure.Services
{
    public interface IManagerService
    {
        //CRUD
        Task<ResponseDTO<List<ManagerVM>>> GetAllManagerAsync();
        Task<ResponseDTO<List<ManagerVM>>> GetManagerDetailsByBranchIdAsync(int branchId);
        Task<ResponseDTO<ManagerVM>> GetManagerDetailsByIdAsync(int managerId);
        Task<ResponseDTO<ManagerVM>> GetManagerDetailsByUserIdAsync(int userId);

        Task<ResponseDTO<ManagerVM>> CreateManagerAsync(ManagerCM managerCM);
        Task<ResponseDTO<ManagerVM>> UpdateManagerAsync(int managerId, ManagerUM managerUM);
        Task<ResponseDTO<int>> DeleteManagerAsync(int managerId);


        Task<ResponseDTO<ManagerVM>> ConvertStaffToManager(int userId, int roleId, int branchId);
    }
    public class ManagerService : IManagerService
    {
        private readonly IUnitOfWork  _unitOfWork;
        private readonly IMapper _mapper;
        public ManagerService(IUnitOfWork unitOfWork,
                                IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ResponseDTO<ManagerVM>> CreateManagerAsync(ManagerCM managerCM)
        {
            var response = new ResponseDTO<ManagerVM>();
            try
            {
                var user = await _unitOfWork.UserRepository.GetObjectAsync(u => u.Id ==  managerCM.UserId);
                if(user == null)
                {
                    response.IsSuccess = false;
                    response.Message = $"User with userId = {managerCM.UserId} cannot found";
                    return response;
                }
                if(user.RoleId != (int)UserRole.Manager)
                {
                    response.IsSuccess = false;
                    response.Message = $"User with roleId = {user.RoleId} cannot be a manager";
                    return response;
                }
                var branch = await _unitOfWork.BranchRepository.GetObjectAsync(b => b.Id == managerCM.BranchId);
                if(branch == null)
                {
                    response.IsSuccess = false;
                    response.Message = $"Branch with branchId = {managerCM.UserId} cannot found";
                    return response;
                }
                var managers = await _unitOfWork.ManagerRepository. GetObjectAsync(m => m .BranchId == branch.Id && (m.IsActive == true || m.EndDate == null) );

                if (managers != null)
                {
                    response.IsSuccess = false;
                    response.Message = $"The Branch with branchId {managerCM.BranchId} already has a manager";
                    return response;
                }
                var manager = await _unitOfWork.ManagerRepository
                    .GetObjectAsync(m => m.UserId == managerCM.UserId);
                if(manager != null)
                {
                    response.IsSuccess = false;
                    response.Message = $"The manager with userId {manager.UserId} are existed";
                    response.Data = null;
                }
                if (manager == null)
                {
                    manager = new Manager()
                    {
                        UserId = managerCM.UserId,
                        BranchId = managerCM.BranchId,
                        StartDate = managerCM.StartDate,
                        IsActive = true
                    };
                    await _unitOfWork.ManagerRepository.InsertAsync(manager);
                    await _unitOfWork.SaveChanges();
                    //Return
                    var result = _mapper.Map<ManagerVM>(manager);
                    response.IsSuccess = true;
                    response.Message = "Inserted successfully";
                    response.Data = result;

                }
                
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }
            return response;
        }
        public async Task<ResponseDTO<ManagerVM>> UpdateManagerAsync(int managerId, ManagerUM managerUM)
        {
            var response = new ResponseDTO<ManagerVM>();
            try
            {
                var manager = await _unitOfWork.ManagerRepository
                    .GetObjectAsync(m => m.Id == managerId);
                if (manager != null)
                {
                    
                    manager = _mapper.Map<Manager>(managerUM);
                    manager.Id = managerId;
                    await _unitOfWork.ManagerRepository.UpdateAsync(manager);
                    //Return
                    var result = _mapper.Map<ManagerVM>(manager);
                    response.IsSuccess = true;
                    response.Message = "Updated successfully";
                    response.Data = result;

                }
                else
                {
                    response.IsSuccess = false;
                    response.Message = "Error updated";
                    response.Data = null;
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }
            return response;
        }
        public async Task<ResponseDTO<List<ManagerVM>>> GetAllManagerAsync()
        {
            var response = new ResponseDTO<List<ManagerVM>>();
            try
            {
                var query = await _unitOfWork.ManagerRepository.GetAllAsync(new string[] {"User", "Branch"});
                if(query.Count() > 0)
                {
                    var result = query.Select(m =>
                    {
                        var managerVM = _mapper.Map<ManagerVM>(m);
                        managerVM.UserVM = _mapper.Map<UserVM>(m.User);
                        managerVM.BranchName = m.Branch != null ? m.Branch.BranchName : "N/A";
                        return managerVM;
                    }).ToList();

                    response.IsSuccess = true;
                    response.Message = "Query Successfully";
                    response.Data = result;
                }
                else
                {
                    response.IsSuccess = false;
                    response.Message = "Query failed";
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;

            }
            return response;
        }
        public async Task<ResponseDTO<List<ManagerVM>>> GetManagerDetailsByBranchIdAsync(int branchId)
        {
            var response = new ResponseDTO<List<ManagerVM>>();
            try
            {
                var query = await _unitOfWork.ManagerRepository.GetAndIncludeAsync(m => m.BranchId == branchId, new string[] { "User" });
                if (query != null)
                {
                    var result = query.Select(m =>
                    {
                        var managerVM = _mapper.Map<ManagerVM>(m);
                        managerVM.UserVM = _mapper.Map<UserVM>(m.User);
                        managerVM.BranchName = m.Branch != null ? m.Branch.BranchName : "N/A";

                        return managerVM;
                    }).ToList() ;

                    response.IsSuccess = true;
                    response.Message = "Query Successfully";
                    response.Data = result;
                }
                else
                {
                    response.IsSuccess = false;
                    response.Message = "Query failed";
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;

            }
            return response;
        }
        public async Task<ResponseDTO<ManagerVM>> GetManagerDetailsByIdAsync(int managerId)
        {
            var response = new ResponseDTO<ManagerVM>();
            try
            {
                var query = await _unitOfWork.ManagerRepository.GetObjectAsync(m => m.Id == managerId, new string[] { "User" });
                if (query != null)
                {
                    var managerVM = _mapper.Map<ManagerVM>(query);
                    var userVM = _mapper.Map<UserVM>(query.User);
                    managerVM.UserVM = userVM;
                    managerVM.BranchName = query.Branch != null ? query.Branch.BranchName : "N/A";


                    response.IsSuccess = true;
                    response.Message = "Query Successfully";
                    response.Data = managerVM;
                }
                else
                {
                    response.IsSuccess = false;
                    response.Message = "Query failed";
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;

            }
            return response;
        }
        public async Task<ResponseDTO<ManagerVM>> GetManagerDetailsByUserIdAsync(int userId)
        {
            var response = new ResponseDTO<ManagerVM>();
            try
            {
                var query = await _unitOfWork.ManagerRepository.GetObjectAsync(m => m.UserId == userId, new string[] { "User" });
                if (query != null)
                {
                    var managerVM = _mapper.Map<ManagerVM>(query);
                    var userVM = _mapper.Map<UserVM>(query.User);
                    managerVM.UserVM = userVM;
                    managerVM.BranchName = query.Branch != null ? query.Branch.BranchName : "N/A";


                    response.IsSuccess = true;
                    response.Message = "Query Successfully";
                    response.Data = managerVM;
                }
                else
                {
                    response.IsSuccess = false;
                    response.Message = "Query failed";
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;

            }
            return response;
        }
        public async Task<ResponseDTO<ManagerVM>> ConvertStaffToManager(int userId, int roleId, int branchId)
        {
            var response = new ResponseDTO<ManagerVM>();
            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    var user = _unitOfWork.UserRepository.FindObject(_ => _.Id == userId);
                    user.RoleId = roleId;
                    await _unitOfWork.UserRepository.UpdateAsync(user);

                    if (roleId == (int)UserRole.Staff)
                    {
                        var staff = _unitOfWork.StaffRepository.FindObject(_ => _.UserId == user.Id && _.BranchId == branchId);
                        var branch = _unitOfWork.BranchRepository.FindObject(_ => _.Id == branchId);
                        if (staff != null) await _unitOfWork.StaffRepository.DeleteAsync(staff);

                        Manager manager = new Manager()
                        {
                            UserId = user.Id,
                            BranchId = branch.Id,
                            StartDate = DateTime.Now,
                            IsActive = true,
                            
                        };
                        await _unitOfWork.ManagerRepository.InsertAsync(manager);
                        await _unitOfWork.SaveChanges();

                        var result = _mapper.Map<ManagerVM>(manager);
                        response.IsSuccess = true;
                        response.Message = "Inserted successfully";
                        response.Data = result;
                    }
                }
                catch (Exception ex)
                {
                    response.IsSuccess = false;
                    response.Message = ex.Message;
                }
                return response;
            }
        }
        public async Task<ResponseDTO<int>> DeleteManagerAsync(int managerId)
        {
            var response = new ResponseDTO<int>();
            try
            {
                var toDeleted = await _unitOfWork.ManagerRepository
                                        .GetObjectAsync(m => m.Id == managerId);
                if (toDeleted == null)
                {
                    response.IsSuccess = false;
                    response.Message = "Error Deleted";
                    response.Data = 0;

                }
                else
                {
                    var listStaff = await _unitOfWork.StaffRepository.GetAsync(s => s.ManagerId == managerId);
                    if (listStaff.Any())
                    {
                        foreach (var staff in listStaff)
                        {
                            staff.ManagerId = null;
                            await _unitOfWork.StaffRepository.UpdateAsync(staff);
                        }
                    }
                    await _unitOfWork.ManagerRepository.DeleteAsync(toDeleted);
                    await _unitOfWork.SaveChanges();
                    response.IsSuccess = true;
                    response.Message = "Deleted Successfully";
                    response.Data = 1;

                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
                response.Data = 0;

            }
            return response;
        }
    }
}
