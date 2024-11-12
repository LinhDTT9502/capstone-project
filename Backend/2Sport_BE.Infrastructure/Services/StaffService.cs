using _2Sport_BE.Repository.Interfaces;
using _2Sport_BE.Repository.Models;
using _2Sport_BE.Infrastructure.DTOs;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2Sport_BE.Infrastructure.Services
{
    public interface IStaffService
    {
        Task<ResponseDTO<StaffVM>> CreateStaffAsync(StaffCM staffCM);
        Task<ResponseDTO<StaffVM>> UpdateStaffAsync(int staffId, StaffUM staffUM);
        Task<ResponseDTO<int>> DeleteStaffAsync(int staffId);
        Task<ResponseDTO<List<StaffVM>>> GetAllStaffsAsync();
        Task<ResponseDTO<StaffVM>> GetStaffDetailAsync(int staffId);
        Task<ResponseDTO<List<StaffVM>>> GetStaffsByBranchId(int branchId);
    }
    public class StaffService : IStaffService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public StaffService()
        {
            
        }
        public StaffService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ResponseDTO<StaffVM>> CreateStaffAsync(StaffCM staffCM)
        {
            var response = new ResponseDTO<StaffVM>();
            try
            {
                var isExisted = await _unitOfWork.StaffRepository
                    .GetObjectAsync(m => m.UserId == staffCM.UserId);
                var manager = await _unitOfWork.ManagerRepository
                    .GetObjectAsync(m => m.Id == staffCM.ManagerId);
                
                if (isExisted == null)
                {
                    var staff = new Staff()
                    {
                        ManagerId = manager != null ? manager.Id : null,
                        BranchId = staffCM.BranchId,
                        StartDate = DateTime.Now,
                        EndDate = staffCM.EndDate,
                        Position = staffCM.Position,
                        UserId = staffCM.UserId,
                    };
                    await _unitOfWork.StaffRepository.InsertAsync(staff);
                    await _unitOfWork.SaveChanges();
                    //Return
                    var result = _mapper.Map<StaffVM>(staff);

                    response.IsSuccess = true;
                    response.Message = "Inserted successfully";
                    response.Data = result;

                }
                else
                {
                    response.IsSuccess = false;
                    response.Message = $"Staff with userId{staffCM.UserId} is existed";
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<ResponseDTO<int>> DeleteStaffAsync(int staffId)
        {
            var response = new ResponseDTO<int>();
            try
            {
                var toDeleted = await _unitOfWork.StaffRepository
                                        .GetObjectAsync(m => m.StaffId == staffId);
                if (toDeleted == null)
                {
                    response.IsSuccess = false;
                    response.Message = "Error Deleted";
                    response.Data = 0;

                }
                else
                {
                    await _unitOfWork.StaffRepository.DeleteAsync(toDeleted);
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

        public async Task<ResponseDTO<List<StaffVM>>> GetAllStaffsAsync()
        {
            var response = new ResponseDTO<List<StaffVM>>();
            try
            {
                var query = await _unitOfWork.StaffRepository.GetAllAsync();
                if (query.Count() > 0)
                {
                    var result = query.Select(m => new StaffVM()
                    {
                        BranchId = m.BranchId,
                        EndDate = m.EndDate,
                        StaffId = m.StaffId,
                        StartDate = m.StartDate,
                        UserId = m.UserId,
                        Position = m.Position,
                        ManagerId = m.ManagerId
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

        public async Task<ResponseDTO<StaffVM>> GetStaffDetailAsync(int staffId)
        {
            var response = new ResponseDTO<StaffVM>();
            try
            {
                var query = await _unitOfWork.StaffRepository.GetObjectAsync(m => m.StaffId == staffId, new string[] { "User" });
                if (query != null)
                {
                    var staffVM = _mapper.Map<StaffVM>(query);
                    var userVM = _mapper.Map<UserVM>(query.User);
                    staffVM.UserVM = userVM;

                    response.IsSuccess = true;
                    response.Message = "Query Successfully";
                    response.Data = staffVM;
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

        public async Task<ResponseDTO<List<StaffVM>>> GetStaffsByBranchId(int branchId)
        {
            var response = new ResponseDTO<List<StaffVM>>();
            try
            {
                var query = await _unitOfWork.StaffRepository.GetAsync(s => s.BranchId == branchId);
                if (query.Count() > 0)
                {
                    var result = query.Select(m => new StaffVM()
                    {
                        BranchId = m.BranchId,
                        EndDate = m.EndDate,
                        StaffId = m.StaffId,
                        StartDate = m.StartDate,
                        UserId = m.UserId,
                        Position = m.Position,
                        ManagerId = m.ManagerId
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

        public async Task<ResponseDTO<StaffVM>> UpdateStaffAsync(int staffId, StaffUM staffUM)
        {
            var response = new ResponseDTO<StaffVM>();
            try
            {
                var staff = await _unitOfWork.StaffRepository
                    .GetObjectAsync(m => m.StaffId == staffId);
                if (staff != null)
                {
                    staff = _mapper.Map<Staff>(staffUM);
                    await _unitOfWork.StaffRepository.UpdateAsync(staff);
                    await _unitOfWork.SaveChanges();
                    //Return
                    var result = _mapper.Map<StaffVM>(staff);
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
    }
}
