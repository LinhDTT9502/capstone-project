using _2Sport_BE.Repository.Interfaces;
using _2Sport_BE.Service.DTOs;
using _2Sport_BE.Service.Enums;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2Sport_BE.Service.Services
{
    public interface ICustomerDetailService
    {
        Task<ResponseDTO<CustomerDetailVM>> GetCustomerDetailByUserId(int userId);
        Task<ResponseDTO<bool>> UpdateLoyaltyPoints(int orderId);
    }
    public class CustomerDetailService : ICustomerDetailService
    {
        private readonly IUnitOfWork _unitOfWork;
        public CustomerDetailService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ResponseDTO<CustomerDetailVM>> GetCustomerDetailByUserId(int userId)
        {
            var response = new ResponseDTO<CustomerDetailVM>();
            try
            {
                var query = await _unitOfWork.CustomerDetailRepository.GetObjectAsync(cd => cd.UserId == userId);
                if (query != null)
                {
                    var result = new CustomerDetailVM()
                    {
                        UserId = query.UserId,
                        JoinDate = query.JoinDate,
                        LoyaltyPoints = query.LoyaltyPoints,
                        MembershipLevel = query.MembershipLevel,
                    };

                    response.IsSuccess = true;
                    response.Message = "Query Successfully";
                    response.Data = result;

                    return response;
                }

                response.IsSuccess = false;
                response.Message = "Somethings are wrong when querying!";
                return response;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
                return response;
            }
        }

        public async Task<ResponseDTO<bool>> UpdateLoyaltyPoints(int orderId)
        {
            var response = new ResponseDTO<bool>();
            try
            {
                var order = await _unitOfWork.OrderRepository.GetObjectAsync(o => o.Id == orderId);
                if (order == null)
                {
                    response.IsSuccess = false;
                    response.Message = $"Order with id = {orderId} is not found!";
                    response.Data = false;

                    return response;
                }
                var customerDetail = await _unitOfWork.CustomerDetailRepository.GetObjectAsync(cd => cd.UserId == order.UserId);

                if (customerDetail == null)
                {
                    //add detail
                    customerDetail = new Repository.Models.CustomerDetail()
                    {
                        JoinDate = DateTime.UtcNow,
                        LoyaltyPoints = Convert.ToInt32(order.TotalPrice),
                        MembershipLevel = "Normal_Member",
                        UserId = order.UserId
                    };
                    await _unitOfWork.CustomerDetailRepository.InsertAsync(customerDetail);
                }

                customerDetail.LoyaltyPoints += Convert.ToInt32(order.TotalPrice);
                customerDetail.MembershipLevel = UpdateMembershipLevel(customerDetail.LoyaltyPoints);
                await _unitOfWork.CustomerDetailRepository.UpdateAsync(customerDetail);

                response.IsSuccess = true;
                response.Message = "Update successfully!";
                return response;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
                return response;
            }
        }
/*        private int CalculateLoyaltyPoints(decimal totalPrice)
        {
            const int pointConversionRate = 10000; // 1 điểm cho mỗi 10.000 VND chi tiêu
            return (int)(totalPrice / pointConversionRate);
        }*/
        private string UpdateMembershipLevel(int? loyaltyPoints)
        {
            if (loyaltyPoints >= 10000000)
            {
                return "Diamond_Member";
            }
            else if (loyaltyPoints >= 5000000)
            {
                return "Gold_Member";
            }
            else if (loyaltyPoints >= 3000000)
            {
                return "Silver_Member";
            }
            return "Normal_Member";
        }
    }

}
