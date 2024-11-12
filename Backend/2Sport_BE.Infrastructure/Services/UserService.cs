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
        Task<ResponseDTO<bool>> UpdatePasswordAsync(int id, ChangePasswordVM changePasswordVM);
        Task<string> VerifyPhoneNumber(string from, string to);

        Task<User> FindUserByPhoneNumber(string phoneNumber);
        //Functions to manage managers and staffs (belong to administrator)
        Task<ResponseDTO<List<UserVM>>> GetUserByRoleIdAndBranchId(int roleId, int branchId);
        Task<ResponseDTO<UserUM>> UpdateAvatarAsync(int userId, IFormFile formFile);
    }
    public class UserService : IUserService
    {
        private IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
       /* private readonly IImageService*/
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
                existingUser = _mapper.Map(profile, existingUser);
                existingUser.UpdatedAt = DateTime.UtcNow;
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
        public async Task<string> VerifyPhoneNumber(string from, string to)
        {
            /*var credentials = Credentials.FromApiKeyAndSecret("a985b2e1", "45WqlXONkSho55Yf");

            var vonageClient = new VonageClient(credentials);

            var response = await vonageClient.SmsClient.SendAnSmsAsync(new Vonage.Messaging.SendSmsRequest()
            {
                To = "+84384394372",
                From = "+84366819078",
                Text = "A text message sent using the Vonage SMS API"
            });*/
            var credentials = new Nexmo.Api.Request.Credentials()
            {
                ApiKey = "a985b2e1",
                ApiSecret = "45WqlXONkSho55Yf"

            };
            //var credentials = Credentials.FromApiKeyAndSecret(VONAGE_API_KEY, VONAGE_API_SECRET);           
            var results = SMS.Send(new SMS.SMSRequest
            {
                from = from,
                to = to,
                text = "2sport v3"
            }, credentials);


            /*var VONAGE_API_KEY = "a985b2e1";
            var VONAGE_API_SECRET = "45WqlXONkSho55Yf";*/
            /* var client = new SmsClient(credentials);
             var request = new SendSmsRequest { To = to, From = from, Text = "2sport ver2" };
             var response = await client.SendAnSmsAsync(request);
             response.Messages[0].MessageId.ToString();*/


            return "response.Messages.ToString()";
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

        public Task<ResponseDTO<UserUM>> UpdateAvatarAsync(int userId, IFormFile formFile)
        {
            throw new NotImplementedException();
        }
    }
}
