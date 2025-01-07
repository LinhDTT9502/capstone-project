using _2Sport_BE.Repository.Interfaces;
using _2Sport_BE.Repository.Models;
using _2Sport_BE.Infrastructure.DTOs;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Text;
using Nexmo.Api;
using static System.Net.Mime.MediaTypeNames;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.Design;
using _2Sport_BE.Infrastructure.Helpers;
using Newtonsoft.Json.Linq;
using System.Security.Claims;
using System.Linq;
using StackExchange.Redis;
using _2Sport_BE.Infrastructure.Enums;
namespace _2Sport_BE.Infrastructure.Services
{
    public interface IUserService
    {
        Task<ResponseDTO<List<UserVM>>> GetAllUsers();
        Task<ResponseDTO<List<UserVM>>> SearchUsers(string? fullName, string? username);
        Task<ResponseDTO<int>> CreateUserAsync(UserCM userCM);
        Task<ResponseDTO<UserVM>> GetUserDetailsById(int id);
        Task<User> GetUserById(int id);
        Task<IEnumerable<User>> GetUserWithConditionAsync(Expression<Func<User, bool>> where, string? includes = "");
        Task<ResponseDTO<bool>> UpdateUserAsync(int id, UserUM user);
        Task<ResponseDTO<bool>> UpdateUserAsync(int id, User user);
        Task<ResponseDTO<bool>> UpdateProfile(int id, ProfileUM profile);
        Task<ResponseDTO<UserVM>> UpdateStatus(int userId, bool isActive);
        Task<ResponseDTO<bool>> DeleteUserAsync(int id);
        Task<ResponseDTO<bool>> DisableUserAsync(int id);
        Task<ResponseDTO<bool>> UpdatePasswordAsync(int id, ChangePasswordVM changePasswordVM);
        Task<ResponseDTO<int>> UpdateEmailAsync(int userId, ResetEmailRequesrt resetEmailModel);
        Task<User> FindUserByPhoneNumber(string phoneNumber);
        Task<ResponseDTO<List<UserVM>>> GetUserByRoleIdAndBranchId(int roleId, int branchId);
        Task<ResponseDTO<List<UserVM>>> GetByRoleUsersWithoutBranch(int roleId);
    }
    public class UserService : IUserService
    {
        private IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IMethodHelper _methodeHelper;

        public UserService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IMethodHelper methodHelper
            )
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _methodeHelper = methodHelper;
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
        public async Task<ResponseDTO<List<UserVM>>> GetByRoleUsersWithoutBranch(int roleId)
        {
            IQueryable<User> query = _unitOfWork.UserRepository.GetQueryable();

            if (roleId == 2) // RoleId = 2 => Manager
            {
                var managerUserIds = _unitOfWork.ManagerRepository
                                        .GetQueryable()
                                        .Select(m => m.UserId);

                query = query.Where(u => u.RoleId == (int)UserRole.Manager && !managerUserIds.Contains(u.Id));
            }
            else if (roleId == 3) // RoleId = 3 => Staff
            {
                var staffUserIds = _unitOfWork.StaffRepository
                                        .GetQueryable()
                                        .Select(s => s.UserId);

                query = query.Where(u => u.RoleId == (int)UserRole.Staff && !staffUserIds.Contains(u.Id));
            }
            else
            {
                throw new ArgumentException("Invalid roleId. Supported values: 2 (Manager) or 3 (Staff).");
            }


            var result = _mapper.Map<List<User>, List<UserVM>>(query.ToList());

            var response = new ResponseDTO<List<UserVM>>
            {
                IsSuccess = true,
                Message = "Query Succesfully",
                Data = result
            };
            return response;
        }
        public async Task<ResponseDTO<List<UserVM>>> GetAllUsers()
        {

            var query = await _unitOfWork.UserRepository.GetAllAsync();
            var result = _mapper.Map<List<User>, List<UserVM>>(query.ToList());

            var response = new ResponseDTO<List<UserVM>>
            {
                IsSuccess = true,
                Message = "Query Succesfully",
                Data = result
            };
            return response;
        }
        public async Task<ResponseDTO<List<UserVM>>> SearchUsers(string? fullName, string? username)
        {

            var query = await _unitOfWork.UserRepository.GetAllAsync();
            if (!string.IsNullOrWhiteSpace(username))
            {
                username = username.ToLower();
                query = query.Where(x => x.UserName.ToLower().Contains(username));
            }
            if (!string.IsNullOrWhiteSpace(fullName))
            {
                fullName = fullName.ToLower();
                query = query.Where(x => x.FullName.ToLower().Contains(fullName));
            }
            var result = _mapper.Map<List<User>, List<UserVM>>(query.ToList());
            var response = new ResponseDTO<List<UserVM>>
            {
                IsSuccess = true,
                Message = "Query Succesfully",
                Data = result
            };
            return response;
        }
        public async Task<ResponseDTO<int>> CreateUserAsync(UserCM userCM)
        {
            var response = new ResponseDTO<int>();
            var newUser = await _unitOfWork.UserRepository
                .GetObjectAsync(u => u.Email.Equals(userCM.Email) || u.UserName.Equals(userCM.UserName));

            if (newUser != null)
            {
                response.IsSuccess = false;
                response.Message = "UserName or Email are existed!";
                return response;
            }

            try
            {
                newUser = _mapper.Map<UserCM, User>(userCM);
                newUser.HashPassword = HashPassword(userCM.HashPassword);
                newUser.CreatedAt = DateTime.Now;
                newUser.IsActived = true;
                newUser.EmailConfirmed = true;
                newUser.ImgAvatarPath = "";
                await _unitOfWork.UserRepository.InsertAsync(newUser);
                await _unitOfWork.SaveChanges();

                response.IsSuccess = true;
                response.Message = "Add user successfully";
                response.Data = newUser.Id;
                return response;
            }
            catch (DbUpdateException dbUpdateEx)
            {
                response.IsSuccess = false;
                response.Message = "An error occurred while saving the user. Please try again.";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = $"An unexpected error occurred: {ex.Message}";
            }
            return response;
        }
        public async Task<ResponseDTO<UserVM>> GetUserDetailsById(int id)
        {
            var response = new ResponseDTO<UserVM>();

            try
            {
                var query = await _unitOfWork.UserRepository.GetObjectAsync(u => u.Id == id, new string[] {"Staffs", "Managers", "Customers"});

                if (query == null)
                {
                    response.IsSuccess = false;
                    response.Message = "User not found!";
                    return response;
                }

                var result = _mapper.Map<User, UserVM>(query);
                result.StaffDetail = query.Staffs != null ? _mapper.Map<Staff, StaffVM>(query.Staffs.FirstOrDefault()) : null;
                result.ManagerDetail = query.Managers != null ? _mapper.Map<Manager, ManagerVM>(query.Managers.FirstOrDefault()) : null;
                result.CustomerDetail = query.Customers != null ? _mapper.Map<Customer, CustomerVM>(query.Customers.FirstOrDefault()) : null;

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
        public async Task<IEnumerable<User>> GetUserWithConditionAsync(Expression<Func<User, bool>> where, string? includes = "")
        {
            IEnumerable<User> users = await _unitOfWork.UserRepository.GetAsync(where, null, includes);
            return users;
        }
        public async Task<ResponseDTO<bool>> DeleteUserAsync(int id)
        {
            var response = new ResponseDTO<bool>();
            try
            {
                var user = await _unitOfWork.UserRepository.GetObjectAsync(u => u.Id == id, new string[] {"Customers", "Managers", "Staffs"});
                if (user == null)
                {
                    response.IsSuccess = false;
                    response.Message = "User not found!";
                    return response;
                }

                var customer = user.Customers.FirstOrDefault();
                var staff = user.Staffs.FirstOrDefault();
                var manager = user.Managers.FirstOrDefault();
                if(customer != null)
                {
                    await _unitOfWork.CustomerDetailRepository.DeleteAsync(customer);
                }
                if (staff != null)
                {
                    await _unitOfWork.StaffRepository.DeleteAsync(staff);
                }
                if (manager != null)
                {
                    await _unitOfWork.ManagerRepository.DeleteAsync(manager);
                }
                await _unitOfWork.UserRepository.DeleteAsync(id);
                await _unitOfWork.SaveChanges();  
                response.IsSuccess = true;
                response.Message = "User removed successfully.";
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
        public async Task<ResponseDTO<bool>> UpdateUserAsync(int id, UserUM userUM)
        {
            var response = new ResponseDTO<bool>();

            try
            {
                var existingUser = await _unitOfWork.UserRepository.GetObjectAsync(u => u.Id == id);
                if (existingUser == null)
                {
                    response.IsSuccess = false;
                    response.Message = "User not found!";
                    return response;
                }
                existingUser = _mapper.Map(userUM, existingUser);

                existingUser.UpdatedAt = DateTime.Now;
                await _unitOfWork.UserRepository.UpdateAsync(existingUser);
                response.IsSuccess = true;
                response.Message = "User updated successfully.";
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
        public async Task<ResponseDTO<bool>> UpdateProfile(int id, ProfileUM profile)
        {
            var response = new ResponseDTO<bool>();

            try
            {
                var existingUser = await _unitOfWork.UserRepository.GetObjectAsync(u => u.Id == id);
                if (existingUser == null)
                {
                    response.IsSuccess = false;
                    response.Message = "User not found!";
                    return response;
                }
                existingUser.FullName = profile.FullName;
                existingUser.Gender = profile.Gender;
                existingUser.DOB = profile.BirthDate;
                existingUser.Address = profile.Address;
                existingUser.UpdatedAt = DateTime.Now;

                await _unitOfWork.UserRepository.UpdateAsync(existingUser);
                response.IsSuccess = true;
                response.Message = "User updated successfully.";
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
        public async Task<ResponseDTO<bool>> DisableUserAsync(int id)
        {
            var response = new ResponseDTO<bool>();

            try
            {
                var existingUser = await _unitOfWork.UserRepository.GetObjectAsync(u => u.Id == id);
                if (existingUser == null)
                {
                    response.IsSuccess = false;
                    response.Message = "User not found!";
                    return response;
                }

                existingUser.IsActived = !existingUser.IsActived;
                await _unitOfWork.UserRepository.UpdateAsync(existingUser);

                response.IsSuccess = true;
                response.Message = "User disabled successfully.";
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
        public async Task<ResponseDTO<bool>> UpdateUserAsync(int id, User user)
        {
            var response = new ResponseDTO<bool>();
            try
            {
                var existingUser = _unitOfWork.UserRepository.FindObject(u => u.Id == id);
                if (existingUser == null)
                {
                    response.IsSuccess = false;
                    response.Message = "User not found!";
                    return response;
                }

                existingUser = user;
                await _unitOfWork.UserRepository.UpdateAsync(user);
                await _unitOfWork.SaveChanges();
                response.IsSuccess = true;
                response.Message = "User updated successfully.";
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
        public async Task<User> GetUserById(int id)
        {
            try
            {
                var query = await _unitOfWork.UserRepository.GetObjectAsync(u => u.Id == id);

                if (query == null)
                {
                    return null;
                }
                return query;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task<ResponseDTO<bool>> UpdatePasswordAsync(int id, ChangePasswordVM changePasswordVM)
        {
            var response = new ResponseDTO<bool>();
            try
            {
                var existingUser = await _unitOfWork.UserRepository.GetObjectAsync(u => u.Id == id);
                if (existingUser == null)
                {
                    response.IsSuccess = false;
                    response.Message = "User not found!";
                    return response;
                }
                if (existingUser.HashPassword != HashPassword(changePasswordVM.OldPassword))
                {
                    response.IsSuccess = false;
                    response.Message = "Old password is wrong!";
                    return response;
                }
                existingUser.HashPassword = HashPassword(changePasswordVM.NewPassword);
                await _unitOfWork.UserRepository.UpdateAsync(existingUser);

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
        public async Task<User> FindUserByPhoneNumber(string phoneNumber)
        {
            var user = await _unitOfWork.UserRepository.GetObjectAsync(U => U.PhoneNumber == phoneNumber);

            if (user == null) return null;
            return user;
        }
        //=========
        public async Task<ResponseDTO<List<UserVM>>> GetUserByRoleIdAndBranchId(int roleId, int branchId)
        {
            var response = new ResponseDTO<List<UserVM>>();

            try
            {
                var query = await _unitOfWork.UserRepository.GetAllAsync();
                if (roleId != 0)
                {
                    query = query.Where(x => x.RoleId == roleId);
                }
                if (branchId != 0)
                {
                    query = query.Where(x => x.RoleId == roleId);
                }
                var result = _mapper.Map<List<User>, List<UserVM>>(query.ToList());

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

        public async Task<ResponseDTO<UserVM>> UpdateStatus(int userId, bool isActive)
        {
            var response = new ResponseDTO<UserVM>();

            try
            {
                var query = await _unitOfWork.UserRepository.GetObjectAsync(u => u.Id == userId, new string[] { "Staffs", "Managers" });

                if (query == null)
                {
                    response.IsSuccess = false;
                    response.Message = "User not found!";
                    return response;
                }

                query.IsActived = isActive;

                var manager = query.Managers.FirstOrDefault();
                if (manager != null)
                {
                    manager.IsActive = isActive;
                    await _unitOfWork.ManagerRepository.UpdateAsync(manager);
                }

                var staff = query.Staffs.FirstOrDefault();
                if (staff != null)
                {
                    staff.IsActive = isActive;
                    await _unitOfWork.StaffRepository.UpdateAsync(staff);

                }
                await _unitOfWork.UserRepository.UpdateAsync(query);

                await _unitOfWork.SaveChanges();

                var result = _mapper.Map<User, UserVM>(query);

                response.IsSuccess = true;
                response.Message = "Updated Successfully";
                response.Data = result;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = $"An error occurred: {ex.Message}";
            }

            return response;
        }

        public async Task<ResponseDTO<int>> UpdateEmailAsync(int uId, ResetEmailRequesrt resetEmailModel)
        {
            var response = new ResponseDTO<int>();
            try
            {
                var principal = _methodeHelper.GetPrincipalFromToken(resetEmailModel.Token);

                if (principal != null)
                {
                    var userId = principal.FindFirst("UserId")?.Value;
                    var email = principal.FindFirst("Email")?.Value;
                    var otp = principal.FindFirst("OTP")?.Value;

                    var existingUser = await _unitOfWork.UserRepository.GetObjectAsync(u => uId.ToString().Equals(userId) && u.Id == uId);
                    if (existingUser == null)
                    {
                        response.IsSuccess = false;
                        response.Message = "User not found!";
                        response.Data = 0;

                        return response;
                    }
                    if(email != resetEmailModel.Email)
                    {
                        response.IsSuccess = false;
                        response.Message = "Email is not matched!";
                        response.Data = 0;

                        return response;
                    }
                    if(otp != resetEmailModel.OtpCode)
                    {
                        response.IsSuccess = false;
                        response.Message = "OTP is not matched!";
                        response.Data = 0;

                        return response;
                    }
                    existingUser.UpdatedAt = DateTime.Now;
                    existingUser.Email = resetEmailModel.Email;
                    await _unitOfWork.UserRepository.UpdateAsync(existingUser);

                    response.IsSuccess = true;
                    response.Message = "Email updated successfully.";
                    response.Data = 1;
                }
                else
                {
                    response.IsSuccess = false;
                    response.Message = "Token is invalid.";
                    response.Data = 0;
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = $"An error occurred: {ex.Message}";
                response.Data = 0;
            }

            return response;
        }
    }
}
