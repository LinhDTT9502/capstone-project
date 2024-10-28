using _2Sport_BE.Repository.Interfaces;
using _2Sport_BE.Repository.Models;
using _2Sport_BE.Infrastructure.DTOs;
using _2Sport_BE.Infrastructure.Enums;
using AutoMapper;
using System.Security.Cryptography;
using System.Text;

namespace _2Sport_BE.Infrastructure.Services
{
    public interface IAdminService
    {
        Task<ResponseDTO<AdminVM>> CreateAdminAsync(AdminCM adminCM);
        Task<ResponseDTO<AdminVM>> UpDateAdminAsync(int adminId, AdminUM adminUM);
        Task<ResponseDTO<int>> DeleteAdminAsync(int adminId);
        Task<ResponseDTO<List<AdminVM>>> GetAllAdminAsync();
        Task<ResponseDTO<AdminVM>> GetAdminDetailAsync(int adminId);
    }
    public class AdminService : IAdminService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public AdminService()
        {

        }
        public AdminService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        private string HashPassword(string password)
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
        public async Task<ResponseDTO<AdminVM>> CreateAdminAsync(AdminCM adminCM)
        {
            var response = new ResponseDTO<AdminVM>();
            try
            {
                var admin = await _unitOfWork.UserRepository
                    .GetObjectAsync(m => (m.UserName == adminCM.Username && m.HashPassword == HashPassword(adminCM.Password)) || m.Email == adminCM.Email);
                if (admin == null)
                {
                    admin = new User()
                    {
                        UserName = adminCM.Username,
                        HashPassword = HashPassword(adminCM.Password),
                        RoleId = (int)UserRole.Admin,
                        Email = adminCM.Email,
                        IsActived = true,
                        CreatedAt = DateTime.Now,
                        FullName= adminCM.FullName,
                    };
                    await _unitOfWork.UserRepository.InsertAsync(admin);

                    //Return
                    var result = new AdminVM()
                    {
                        AdminId = admin.Id,
                        Email = admin.Email,
                        FullName = admin.FullName,
                        Username = admin.UserName
                    };

                    response.IsSuccess = true;
                    response.Message = "Inserted successfully";
                    response.Data = result;
                }
                else
                {
                    response.IsSuccess = false;
                    response.Message = "Error inserted";
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

        public async Task<ResponseDTO<int>> DeleteAdminAsync(int adminId)
        {
            var response = new ResponseDTO<int>();
            try
            {
                var toDeleted = await _unitOfWork.UserRepository
                                        .GetObjectAsync(m => m.Id == adminId);
                if (toDeleted == null)
                {
                    response.IsSuccess = false;
                    response.Message = "Error Deleted";
                    response.Data = 0;
                }
                else
                {
                    await _unitOfWork.UserRepository.DeleteAsync(toDeleted);

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

        public async Task<ResponseDTO<AdminVM>> GetAdminDetailAsync(int adminId)
        {
            var response = new ResponseDTO<AdminVM>();
            try
            {
                var admin = await _unitOfWork.UserRepository
                                        .GetObjectAsync(m => m.Id == adminId);
                if (admin == null)
                {
                    response.IsSuccess = false;
                    response.Message = "Error Query";
                }
                else
                {
                    var result = new AdminVM()
                    {
                        AdminId = admin.Id,
                        Email = admin.Email,
                        FullName = admin.FullName,
                        Username = admin.UserName
                    };

                    response.IsSuccess = true;
                    response.Message = "Query Successfully";
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

        public async Task<ResponseDTO<List<AdminVM>>> GetAllAdminAsync()
        {
            var response = new ResponseDTO<List<AdminVM>>();
            try
            {
                var query = await _unitOfWork.UserRepository
                                        .GetAsync(ad => ad.RoleId == (int)UserRole.Admin);
                if (query == null)
                {
                    response.IsSuccess = false;
                    response.Message = "Error Query";
                }
                else
                {
                    var result = query.Select(_ => new AdminVM()
                    {
                        AdminId = _.Id,
                        Email = _.Email,
                        FullName = _.FullName,
                        Username = _.UserName
                    }).ToList();

                    response.IsSuccess = true;
                    response.Message = "Query Successfully";
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

        public async Task<ResponseDTO<AdminVM>> UpDateAdminAsync(int adminId, AdminUM adminUM)
        {
            var response = new ResponseDTO<AdminVM>();
            try
            {
                var toUpdate = await _unitOfWork.UserRepository
                                        .GetObjectAsync(m => m.Id == adminId);
                if (toUpdate == null)
                {
                    response.IsSuccess = false;
                    response.Message = "Error Updated";
                }
                else
                {
                    toUpdate.UserName = adminUM.Username;
                    toUpdate.FullName = adminUM.FullName;
                    toUpdate.Email = adminUM.Email;
                    toUpdate.HashPassword = HashPassword(adminUM.Password);

                    await _unitOfWork.UserRepository.UpdateAsync(toUpdate);

                    //Return
                    var result = new AdminVM()
                    {
                        AdminId = toUpdate.Id,
                        Email = toUpdate.Email,
                        FullName = toUpdate.FullName,
                        Username = toUpdate.UserName,
                       
                    };
                    response.IsSuccess = true;
                    response.Message = "Updated Successfully";
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
}
