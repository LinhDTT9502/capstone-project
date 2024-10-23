using _2Sport_BE.Repository.Implements;
using _2Sport_BE.Repository.Interfaces;
using _2Sport_BE.Repository.Models;
using _2Sport_BE.Service.DTOs;
using _2Sport_BE.Service.Enums;
using AutoMapper;
using Azure.Core;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Linq.Expressions;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace _2Sport_BE.Service.Services
{
    public interface IEmployeeService
    {
        Task<ResponseDTO<TokenModel>> LoginEmployeeAsync(UserLogin login);
        Task<ResponseDTO<string>> LogoutEmployeeAsync(TokenModel request);
        Task<ResponseDTO<List<EmployeeVM>>> GetAllEmployees();
        Task<ResponseDTO<List<EmployeeVM>>> GetEmployeesByBranch(int branchId);
        Task<ResponseDTO<List<EmployeeVM>>> GetEmployeeWithConditionAsync(Expression<Func<Employee, bool>> where, string? includes = "");
        Task<ResponseDTO<EmployeeVM>> GetEmployeeDetailsById(int id);
        Task<ResponseDTO<int>> AddEmployeeAsync(EmployeeCM employeeCM);
        Task<ResponseDTO<bool>> UpdateEmployeeAsync (int id, EmployeeUM employeeUM);
        Task<ResponseDTO<bool>> RemoveEmployeeAsync(int id);
        Task<ResponseDTO<bool>> DisableEmployeeAsync(int id);
        Task<ResponseDTO<bool>> UpdatePasswordAsync(int id, ChangePasswordVM changePasswordVM);

        Task<ResponseDTO<EmployeeVM>> CreateANewAdminAccount(EmployeeCM employeeCM);
    }
    public class EmployeeService : IEmployeeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly AuthService _auth;
        private readonly IMapper _mapper;
        private readonly IRoleService _roleService;
        public EmployeeService(IUnitOfWork unitOfWork, AuthService auth, IMapper mapper, IRoleService roleService)
        {
            _unitOfWork = unitOfWork;
            _auth = auth;
            _mapper = mapper;
            _roleService = roleService;
        }
        public string HashPassword(string password)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));

                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
        public async Task<ResponseDTO<int>> AddEmployeeAsync(EmployeeCM employeeCM)
        {
            var response = new ResponseDTO<int>();

            var existingEmployee = await _unitOfWork.EmployeeRepository
                .GetObjectAsync(u => u.Email == employeeCM.Email || u.UserName == employeeCM.UserName);
            if (existingEmployee != null)
            {
                response.IsSuccess = false;
                response.Message = "UserName or Email already exists!";
                return response;
            }

            var branch = await _unitOfWork.BranchRepository.GetObjectAsync(b => b.Id == employeeCM.BranchId);
            if (branch == null)
            {
                response.IsSuccess = false;
                response.Message = "Branch not found!";
                return response;
            }

            if (employeeCM.SupervisorId != null)
            {
                var supervisor = await _unitOfWork.EmployeeRepository.GetObjectAsync(m => m.EmployeeId == employeeCM.SupervisorId);
                if (supervisor == null)
                {
                    response.IsSuccess = false;
                    response.Message = "Supervisor not found!";
                    return response;
                }
            }

            try
            {
                // Add Employee
                var newEmployee = new Employee
                {
                    UserName = employeeCM.UserName,
                    Email = employeeCM.Email,
                    Password = HashPassword(employeeCM.Password),
                    FullName = employeeCM.FullName,
                    Address = employeeCM.Address,
                    Phone = employeeCM.Phone,
                    AvatarUrl = employeeCM.AvatarUrl,
                    Gender = employeeCM.Gender,
                    DOB = employeeCM.DOB,
                    CreatedDate = DateTime.Now,
                    IsActive = true,
                    RoleId = employeeCM.RoleId,
                    PasswordResetToken = string.Empty,
                };
                await _unitOfWork.EmployeeRepository.InsertAsync(newEmployee);

                // Add Employee Detail
                var employeeDetail = new Staff
                {
                    BranchId = employeeCM.BranchId,
                    HireDate = employeeCM.HireDate,
                    EmployeeId = newEmployee.EmployeeId,
                    Position = employeeCM.Position ?? string.Empty,
                    SupervisorId = employeeCM.SupervisorId
                };
                await _unitOfWork.EmployeeDetailRepository.InsertAsync(employeeDetail);

                await _unitOfWork.SaveChanges();

                response.IsSuccess = true;
                response.Message = "Employee added successfully";
                response.Data = newEmployee.EmployeeId;
            }
            catch (DbUpdateException)
            {
                response.IsSuccess = false;
                response.Message = "An error occurred while saving the employee. Please try again.";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = $"Unexpected error: {ex.Message}";
            }

            return response;
        }
        public async Task<ResponseDTO<bool>> DisableEmployeeAsync(int id)
        {
            var response = new ResponseDTO<bool>();

            try
            {
                var existingUser = await _unitOfWork.EmployeeRepository.GetObjectAsync(u => u.EmployeeId == id);
                if (existingUser == null)
                {
                    response.IsSuccess = false;
                    response.Message = "Employee not found!";
                    return response;
                }

                existingUser.IsActive = !existingUser.IsActive;
                await _unitOfWork.EmployeeRepository.UpdateAsync(existingUser);

                response.IsSuccess = true;
                response.Message = "Employee disabled successfully.";
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
        public async Task<ResponseDTO<List<EmployeeVM>>> GetAllEmployees()
        {
            var response = new ResponseDTO<List<EmployeeVM>>();

            try
            {
                var employees = await _unitOfWork.EmployeeRepository.GetAllAsync();

                if (employees == null || !employees.Any())
                {
                    response.IsSuccess = false;
                    response.Message = "No employees found!";
                    return response;
                }
                var employeeVMList = new List<EmployeeVM>();

                foreach (var employee in employees)
                {
                    var empDetail = await _unitOfWork.EmployeeDetailRepository.GetObjectAsync(ed => ed.EmployeeId == employee.EmployeeId);

                    var employeeVM = new EmployeeVM()
                    {
                        EmployeeId = employee.EmployeeId,
                        Address = employee.Address,
                        AvatarUrl = employee.AvatarUrl,
                        CreatedDate = employee.CreatedDate,
                        DOB = employee.DOB,
                        Email = employee.Email,
                        FullName = employee.FullName,
                        Gender = employee.Gender,
                        Phone = employee.Phone,
                        UserName = employee.UserName,
                        IsActive = employee.IsActive,
                        LastUpdate = (DateTime)employee.LastUpdate,
                        RoleId = (int)employee.RoleId,
                        BranchId = (int)empDetail.BranchId,
                        HireDate = (DateTime)empDetail.HireDate,
                        Position = empDetail.Position,
                        SupervisorId = (int)empDetail.SupervisorId
                    };

                    employeeVMList.Add(employeeVM);
                }

                response.IsSuccess = true;
                response.Message = "Employees retrieved successfully";
                response.Data = employeeVMList;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = $"An error occurred: {ex.Message}";
            }

            return response;
        }
        public async Task<ResponseDTO<EmployeeVM>> GetEmployeeDetailsById(int id)
        {
            var response = new ResponseDTO<EmployeeVM>();
            try
            {
                var employee = await _unitOfWork.EmployeeRepository.GetObjectAsync(u => u.EmployeeId == id);

                if (employee == null)
                {
                    response.IsSuccess = false;
                    response.Message = "Employee not found!";
                    return response;
                }

                var empDetail = await _unitOfWork.EmployeeDetailRepository.GetObjectAsync(ed => ed.EmployeeId == employee.EmployeeId);

                var result = new EmployeeVM()
                {
                    EmployeeId = employee.EmployeeId,
                    Address = employee.Address,
                    AvatarUrl = employee.AvatarUrl,
                    CreatedDate = employee.CreatedDate,
                    DOB = employee.DOB,
                    Email = employee.Email,
                    FullName = employee.FullName,
                    Gender = employee.Gender,
                    Phone = employee.Phone,
                    UserName = employee.UserName,
                    IsActive = employee.IsActive,
                    LastUpdate = (DateTime)employee.LastUpdate,
                    RoleId = (int)employee.RoleId,
                    BranchId = empDetail.BranchId,
                    HireDate = (DateTime)empDetail.HireDate,
                    Position = empDetail.Position,
                    SupervisorId = (int)empDetail.SupervisorId
                };

                response.IsSuccess = true;
                response.Message = "Query Successfully";
                response.Data = result;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = $"An error occurred: {ex.Message}";
            }

            return response;
        }
        public async Task<ResponseDTO<List<EmployeeVM>>> GetEmployeesByBranch(int branchId)
        {
            var response = new ResponseDTO<List<EmployeeVM>>();
            try
            {
                var employeeDetails = await _unitOfWork.EmployeeDetailRepository
                                          .GetAsync(ed => ed.BranchId == branchId, "Employee");

                var employeeDetailList = employeeDetails.ToList();

                if (!employeeDetailList.Any())
                {
                    response.IsSuccess = false;
                    response.Message = "No Employees Found!";
                    return response;
                }

                var employees = employeeDetailList.Select(e => new EmployeeVM
                {
                    EmployeeId = e.Employee.EmployeeId,
                    FullName = e.Employee.FullName,
                    Email = e.Employee.Email,
                    Address = e.Employee.Address,
                    AvatarUrl = e.Employee.AvatarUrl,
                    BranchId = e.BranchId,
                    CreatedDate = e.Employee.CreatedDate,
                    DOB = e.Employee.DOB,
                    Gender = e.Employee.Gender,
                    HireDate = (DateTime) e.HireDate,
                    IsActive = e.Employee.IsActive,
                    LastUpdate = (DateTime)e.Employee.LastUpdate,
                    Phone = e.Employee.Phone,
                    Position = e.Position,
                    RoleId = (int)e.Employee.RoleId,
                    SupervisorId = (int)e.SupervisorId,
                    UserName = e.Employee.UserName
                }).ToList();

                response.IsSuccess = true;
                response.Message = "Query successfully!";
                response.Data = employees;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = $"An error occurred: {ex.Message}";
            }

            return response;
        }
        public async Task<ResponseDTO<List<EmployeeVM>>> GetEmployeeWithConditionAsync(Expression<Func<Employee, bool>> where, string? includes = "")
        {
            var response = new ResponseDTO<List<EmployeeVM>>();

            try
            {
                var employees = await _unitOfWork.EmployeeRepository.GetAsync(where, includes);

                if (employees == null || !employees.Any())
                {
                    response.IsSuccess = false;
                    response.Message = "No employees found!";
                    return response;
                }
                var employeeVMList = new List<EmployeeVM>();
                foreach (var employee in employees)
                {
                    var empDetail = await _unitOfWork.EmployeeDetailRepository.GetObjectAsync(ed => ed.EmployeeId == employee.EmployeeId);
                    var employeeVM = new EmployeeVM()
                    {
                        EmployeeId = employee.EmployeeId,
                        Address = employee.Address,
                        AvatarUrl = employee.AvatarUrl,
                        CreatedDate = employee.CreatedDate,
                        DOB = employee.DOB,
                        Email = employee.Email,
                        FullName = employee.FullName,
                        Gender = employee.Gender,
                        Phone = employee.Phone,
                        UserName = employee.UserName,
                        IsActive = employee.IsActive,
                        LastUpdate = (DateTime)employee.LastUpdate,
                        RoleId = (int)employee.RoleId,
                        BranchId = empDetail?.BranchId,
                        HireDate = (DateTime)empDetail.HireDate,
                        Position = empDetail?.Position,
                        SupervisorId = (int)empDetail.SupervisorId
                    };

                    employeeVMList.Add(employeeVM);
                }

                response.IsSuccess = true;
                response.Message = "Employees retrieved successfully";
                response.Data = employeeVMList;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = $"An error occurred: {ex.Message}";
            }

            return response;
        }
        public async Task<ResponseDTO<TokenModel>> LoginEmployeeAsync(UserLogin requestUser)
        {
            ResponseDTO<TokenModel> response = new ResponseDTO<TokenModel>();
            try
            {
                var loginUser = await _unitOfWork.EmployeeRepository
                    .GetObjectAsync(_ => _.UserName == requestUser.UserName && _.Password == HashPassword(requestUser.Password));

                if (loginUser == null)
                {
                    response.IsSuccess = false;
                    response.Message = "Invalid Username And Password";
                    return response;
                }

                if (loginUser != null && loginUser.IsActive != true)
                {
                    response.IsSuccess = false;
                    response.Message = "Not Permission";
                    return response;
                }
                var authenticationResult = await _auth.AuthenticateAsync(loginUser);

                if (authenticationResult != null && authenticationResult.Success)
                {
                    response.Message = "Query successfully";
                    response.IsSuccess = true;
                    response.Data = new TokenModel() { UserId = loginUser.EmployeeId, Token = authenticationResult.Token, RefreshToken = authenticationResult.RefreshToken };
                }
                else
                {
                    response.Message = "Something went wrong!";
                    response.IsSuccess = false;
                }

                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<ResponseDTO<string>> LogoutEmployeeAsync(TokenModel request)
        {
            var result = new ResponseDTO<string>();
            var token = await _unitOfWork.RefreshTokenRepository.GetObjectAsync(_ => _.Token == request.RefreshToken);
            if (token == null)
            {
                result.IsSuccess = false;
                result.Message = "Token is not match or not found";
                result.Data = string.Empty;
                return result;
            }
            else
            {
                await _unitOfWork.RefreshTokenRepository.DeleteAsync(token);
                _unitOfWork.Save();

                result.IsSuccess = true;
                result.Message = "Sign out successfully";
                result.Data = string.Empty;
                return result;
            }
        }
        public async Task<ResponseDTO<bool>> RemoveEmployeeAsync(int id)
        {
            var response = new ResponseDTO<bool>();
            try
            {
                var user = await _unitOfWork.EmployeeRepository.GetObjectAsync(u => u.EmployeeId == id);
                if (user == null)
                {
                    response.IsSuccess = false;
                    response.Message = "Employee not found!";
                    return response;
                }
                await _unitOfWork.UserRepository.DeleteAsync(id);

                response.IsSuccess = true;
                response.Message = "Employee removed successfully.";
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
        public async Task<ResponseDTO<bool>> UpdateEmployeeAsync(int id, EmployeeUM employeeUM)
        {
            var response = new ResponseDTO<bool>();
            try
            {
                var employee = await _unitOfWork.EmployeeRepository.GetObjectAsync(u => u.EmployeeId == id);

                if (employee == null)
                {
                    response.IsSuccess = false;
                    response.Message = "Employee not found!";
                    return response;
                }

                employee.FullName = employeeUM.FullName;
                employee.Gender = employeeUM.Gender;
                employee.Phone = employeeUM.Phone;
                employee.Address = employeeUM.Address;
                employee.DOB = employeeUM.DOB;
                employee.AvatarUrl = employeeUM.AvatarUrl;
                employee.IsActive = employeeUM.IsActive;
                employee.RoleId = employeeUM?.RoleId;
                employee.LastUpdate = DateTime.Now;

                var employeeDetail = await _unitOfWork.EmployeeDetailRepository.GetObjectAsync(ed => ed.EmployeeId == id);
                if (employeeDetail != null)
                {
                    employeeDetail.BranchId = employeeUM.BranchId;
                    employeeDetail.HireDate = employeeUM.HireDate;
                    employeeDetail.Position = employeeUM.Position;
                    employeeDetail.SupervisorId = employeeUM.SupervisorId;
                }

                await _unitOfWork.EmployeeRepository.UpdateAsync(employee);

                if (employeeDetail != null)
                {
                    await _unitOfWork.EmployeeDetailRepository.UpdateAsync(employeeDetail);
                }

                await _unitOfWork.SaveChanges();

                response.IsSuccess = true;
                response.Message = "Employee updated successfully!";
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
        public async Task<ResponseDTO<bool>> UpdatePasswordAsync(int id, ChangePasswordVM changePasswordVM)
        {
            var response = new ResponseDTO<bool>();
            try
            {
                var existingUser = await _unitOfWork.EmployeeRepository.GetObjectAsync(u => u.EmployeeId == id);
                if (existingUser == null)
                {
                    response.IsSuccess = false;
                    response.Message = "Employee not found!";
                    return response;
                }

                existingUser.Password = HashPassword(changePasswordVM.NewPassword);
                await _unitOfWork.EmployeeRepository.UpdateAsync(existingUser);

                response.IsSuccess = true;
                response.Message = "Password updated successfully.";
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

        public async Task<ResponseDTO<EmployeeVM>> CreateANewAdminAccount(EmployeeCM employeeCM)
        {
            var response = new ResponseDTO<EmployeeVM>();

            var existingEmployee = await _unitOfWork.EmployeeRepository
                .GetObjectAsync(u => u.Email == employeeCM.Email || u.UserName == employeeCM.UserName);
            if (existingEmployee != null)
            {
                response.IsSuccess = false;
                response.Message = "UserName or Email already exists!";
                return response;
            }

            using(var trasaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    // Add Employee
                    var newEmployee = new Employee
                    {
                        UserName = employeeCM.UserName,
                        Email = employeeCM.Email,
                        Password = HashPassword(employeeCM.Password),
                        FullName = employeeCM.FullName,
                        Address = employeeCM.Address,
                        Phone = employeeCM.Phone,
                        AvatarUrl = employeeCM.AvatarUrl,
                        Gender = employeeCM.Gender,
                        DOB = employeeCM.DOB,
                        CreatedDate = DateTime.Now,
                        IsActive = true,
                        RoleId = (int)UserRole.Admin,
                        PasswordResetToken = string.Empty,
                    };
                    await _unitOfWork.EmployeeRepository.InsertAsync(newEmployee);

                    // Add Employee Detail
                    var employeeDetail = new Staff
                    {
                        HireDate = employeeCM.HireDate,
                        EmployeeId = newEmployee.EmployeeId,
                        Position = employeeCM.Position ?? "Administrator",
                    };
                    await _unitOfWork.EmployeeDetailRepository.InsertAsync(employeeDetail);

                    await _unitOfWork.SaveChanges();
                    await trasaction.CommitAsync();

                    var result = new EmployeeVM()
                    {
                        EmployeeId = newEmployee.EmployeeId,
                        Address = newEmployee.Address,
                        AvatarUrl = newEmployee.AvatarUrl,
                        CreatedDate = newEmployee.CreatedDate,
                        DOB = newEmployee.DOB,
                        Email = newEmployee.Email,
                        FullName = newEmployee.FullName,
                        Gender = newEmployee.Gender,
                        Phone = newEmployee.Phone,
                        UserName = newEmployee.UserName,
                        IsActive = newEmployee.IsActive,
                        LastUpdate = (DateTime)newEmployee.LastUpdate,
                        RoleId = (int)newEmployee.RoleId,
                        BranchId = employeeDetail.BranchId,
                        HireDate = (DateTime)employeeDetail.HireDate,
                        Position = employeeDetail.Position,
                        SupervisorId = (int)employeeDetail.SupervisorId
                    };

                    response.IsSuccess = true;
                    response.Message = "Admin added successfully";
                    response.Data = result;
                    return response;
                }
                catch (DbUpdateException)
                {
                    await trasaction.RollbackAsync();
                    response.IsSuccess = false;
                    response.Message = "An error occurred while saving the employee. Please try again.";
                }
                catch (Exception ex)
                {
                    await trasaction.RollbackAsync();
                    response.IsSuccess = false;
                    response.Message = $"Unexpected error: {ex.Message}";
                }

            }
            return response;
        }
    }
}
