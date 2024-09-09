using _2Sport_BE.Repository.Interfaces;
using _2Sport_BE.Repository.Models;
using _2Sport_BE.Service.DTOs;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Text;

namespace _2Sport_BE.Infrastructure.Services
{
    public interface IUserService
    {
        Task<ResponseDTO<List<UserVM>>> GetAllUsers();
        Task<ResponseDTO<List<UserVM>>> SearchUsers(string? fullName, string? username);
        Task<ResponseDTO<int>> AddUserAsync(UserCM userCM);
        Task<ResponseDTO<UserVM>> GetUserDetailsById(int id);
        Task<User> GetUserById(int id);
        Task<IEnumerable<User>> GetUserWithConditionAsync(Expression<Func<User, bool>> where, string? includes = "");
        Task<ResponseDTO<bool>> UpdateUserAsync(int id, UserUM user);
        Task<ResponseDTO<bool>> UpdateUserAsync(int id, User user);
        Task<ResponseDTO<bool>> UpdateProfile(int id, ProfileUM profile);
        Task<ResponseDTO<bool>> RemoveUserAsync(int id);
        Task<ResponseDTO<bool>> DisableUserAsync(int id);
    }
    public class UserService : IUserService
    {
        private IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public UserService(
            IUnitOfWork unitOfWork,
            IMapper mapper
            )
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
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
        public async Task<ResponseDTO<int>> AddUserAsync(UserCM userCM)
        {
            var response = new ResponseDTO<int>();
            var newUser = await _unitOfWork.UserRepository
                .GetObjectAsync(u => u.Email.Equals(userCM.Email) || u.UserName.Equals(userCM.Username));

            if (newUser != null)
            {
                response.IsSuccess = false;
                response.Message = "UserName or Email are existed!";
                return response;
            }

            try
            {
                newUser = _mapper.Map<UserCM, User>(userCM);
                newUser.Password = HashPassword(userCM.Password);
                newUser.CreatedDate = DateTime.Now;
                newUser.IsActive = true;

                await _unitOfWork.UserRepository.InsertAsync(newUser);
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
                var query = await _unitOfWork.UserRepository.GetObjectAsync(u => u.Id == id);

                if (query == null)
                {
                    response.IsSuccess = false;
                    response.Message = "User not found!";
                    return response;
                }

                var result = _mapper.Map<User, UserVM>(query);

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
        public async Task<ResponseDTO<bool>> RemoveUserAsync(int id)
        {
            var response = new ResponseDTO<bool>();
            try
            {
                var user = await _unitOfWork.UserRepository.GetObjectAsync(u => u.Id == id);
                if (user == null)
                {
                    response.IsSuccess = false;
                    response.Message = "User not found!";
                    return response;
                }
                await _unitOfWork.UserRepository.DeleteAsync(id);
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

                existingUser.LastUpdate = DateTime.Now;
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
                existingUser = _mapper.Map(profile, existingUser);
                existingUser.LastUpdate = DateTime.UtcNow;
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

                existingUser.IsActive = !existingUser.IsActive;
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
                var existingUser = await _unitOfWork.UserRepository.GetObjectAsync(u => u.Id == id);
                if (existingUser == null)
                {
                    response.IsSuccess = false;
                    response.Message = "User not found!";
                    return response;
                }

                existingUser = user;
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
    }
}
