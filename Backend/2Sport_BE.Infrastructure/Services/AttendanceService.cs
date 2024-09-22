using _2Sport_BE.Repository.Interfaces;
using _2Sport_BE.Repository.Models;
using _2Sport_BE.Service.DTOs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2Sport_BE.Service.Services
{
    public interface IAttendanceService
    {
        Task<ResponseDTO<bool>> AddAttendance(AttendanceCM attendanceCM);
        Task<ResponseDTO<bool>> UpdateAttendance(AttendanceUM attendanceUM);
        Task<ResponseDTO<List<AttendanceVM>>> GetDailyAttendance(int branchId, DateTime date);
        //Task<List<AttendanceSummaryDTO>> GetMonthlyAttendanceSummary(int branchId, DateTime startDate, DateTime endDate);
    }
    public class AttendanceService : IAttendanceService
    {
        private readonly IUnitOfWork _unitOfWork;
        public AttendanceService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<ResponseDTO<List<AttendanceVM>>> GetDailyAttendance(int branchId, DateTime date)
        {
            var response = new ResponseDTO<List<AttendanceVM>>();
            try
            {
                var query = await _unitOfWork.AttendanceRepository
                    .GetAsync(a => a.BranchId == branchId && a.AttendanceDate.Date == date.Date);

                if (query != null && query.Any())
                {
                    var attendanceList = new List<AttendanceVM>();

                    foreach (var item in query)
                    {
                        var branch = await _unitOfWork.BranchRepository.GetObjectAsync(b => b.Id == item.BranchId);
                        var employee = await _unitOfWork.EmployeeRepository.GetObjectAsync(e => e.EmployeeId == item.EmployeeId);
                        var manager = await _unitOfWork.EmployeeRepository.GetObjectAsync(m => m.EmployeeId == item.CheckedBy);

                        var attendanceVM = new AttendanceVM()
                        {
                            BranchId = item.BranchId,
                            AttendanceDate = item.AttendanceDate,
                            CheckedBy = item.CheckedBy,
                            EmployeeId = item.EmployeeId,
                            Reason = item.Reason,
                            Status = item.Status,
                            BranchName = branch != null ? branch.BranchName : "Unknown",
                            EmployeeName = employee != null ? employee.FullName : "Unknown",
                            ManagerName = manager != null ? manager.FullName : "Unknown"
                        };

                        attendanceList.Add(attendanceVM);
                    }

                    response.IsSuccess = true;
                    response.Message = "Query successfully";
                    response.Data = attendanceList;
                }
                else
                {
                    response.IsSuccess = false;
                    response.Message = "No attendance records found for this date and branch.";
                    response.Data = new List<AttendanceVM>();
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = $"An error occurred: {ex.Message}";
                response.Data = new List<AttendanceVM>();
            }
            return response;
        }

        public async Task<ResponseDTO<bool>> AddAttendance(AttendanceCM attendanceCM)
        {
            var response = new ResponseDTO<bool>();
            var branch = await _unitOfWork.BranchRepository
                                .GetObjectAsync(br => br.Id == attendanceCM.BranchId);
            if(branch == null)
            {
                response.IsSuccess = false;
                response.Message = $"Branch with id = {attendanceCM.BranchId} is not found";
                response.Data = false;

                return response;
            }
            var employee = await _unitOfWork.EmployeeDetailRepository
                                .GetObjectAsync(e => e.EmployeeId ==  attendanceCM.EmployeeId && e.BranchId == branch.Id);
            var manager = await _unitOfWork.EmployeeDetailRepository
                                .GetObjectAsync(e => e.EmployeeId == attendanceCM.CheckedBy && e.BranchId == branch.Id);
            if (employee == null || manager == null)
            {
                response.IsSuccess = false;
                response.Message = $"Employee with id = {attendanceCM.EmployeeId} or Manager with id = {attendanceCM.CheckedBy} are not found";
                response.Data = false;

                return response;
            }
            try
            {
                var attendance = new Attendance
                {
                    EmployeeId = attendanceCM.EmployeeId,
                    BranchId = attendanceCM.BranchId,
                    AttendanceDate = attendanceCM.AttendanceDate,
                    Status = attendanceCM.Status,
                    Reason = attendanceCM.Reason,
                    CheckedBy = attendanceCM.CheckedBy
                };

                await _unitOfWork.AttendanceRepository.InsertAsync(attendance);

                response.IsSuccess = true;
                response.Message = "Attendance recorded successfully!";
                response.Data = true;
            }
            catch (DbUpdateException dbEx)
            {
                response.IsSuccess = false;
                response.Message = $"Database update error: {dbEx.Message}";
                response.Data = false;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = $"An unexpected error occurred: {ex.Message}";
                response.Data = false;
            }
            return response;
        }

        public async Task<ResponseDTO<bool>> UpdateAttendance(AttendanceUM attendanceUM)
        {
            var response = new ResponseDTO<bool>();
            var branch = await _unitOfWork.BranchRepository
                               .GetObjectAsync(br => br.Id == attendanceUM.BranchId);
            if (branch == null)
            {
                response.IsSuccess = false;
                response.Message = $"Branch with id = {attendanceUM.BranchId} is not found";
                response.Data = false;

                return response;
            }
            var employee = await _unitOfWork.EmployeeDetailRepository
                                .GetObjectAsync(e => e.EmployeeId == attendanceUM.EmployeeId && e.BranchId == branch.Id);
            var manager = await _unitOfWork.EmployeeDetailRepository
                                .GetObjectAsync(e => e.EmployeeId == attendanceUM.CheckedBy && e.BranchId == branch.Id);
            if (branch == null || manager == null)
            {
                response.IsSuccess = false;
                response.Message = $"Employee with id = {attendanceUM.EmployeeId} or Manager with id = {attendanceUM.CheckedBy} are not found";
                response.Data = false;

                return response;
            }
            try
            {
                var existingAttendance = await _unitOfWork.AttendanceRepository
                    .GetObjectAsync(a => a.EmployeeId == attendanceUM.EmployeeId &&
                                         a.AttendanceDate.Date == attendanceUM.AttendanceDate.Date);

                if (existingAttendance == null)
                {
                    response.IsSuccess = false;
                    response.Message = "Attendance not found for this employee on this date!";
                    response.Data = false;
                    return response;
                }

                existingAttendance.Status = attendanceUM.Status;
                existingAttendance.Reason = attendanceUM.Reason;
                existingAttendance.CheckedBy = attendanceUM.CheckedBy;

                await _unitOfWork.AttendanceRepository.UpdateAsync(existingAttendance);

                response.IsSuccess = true;
                response.Message = "Attendance updated successfully!";
                response.Data = true;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = $"An error occurred: {ex.Message}";
                response.Data = false;
            }

            return response;
        }
    }
}
