using _2Sport_BE.Repository.Interfaces;
using _2Sport_BE.Repository.Models;
using _2Sport_BE.Infrastructure.DTOs;

namespace _2Sport_BE.Infrastructure.Services
{
    public interface ICustomerService
    {
        Task<ResponseDTO<CustomerVM>> GetPointByUserId(int userId);
        Task<ResponseDTO<bool>> UpdateLoyaltyPoints(int orderId);
        Task<ResponseDTO<int>> AddMemberPointByPhoneNumber(string phoneNumber, int point);
    }
    public class CustomerService : ICustomerService
    {
        private readonly IUnitOfWork _unitOfWork;
        public CustomerService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ResponseDTO<CustomerVM>> GetPointByUserId(int userId)
        {
            var response = new ResponseDTO<CustomerVM>();
            try
            {
                var query = await _unitOfWork.CustomerDetailRepository.GetObjectAsync(cd => cd.UserId == userId);
                if (query != null)
                {
                    var result = new CustomerVM()
                    {
                        Id = query.Id,
                        UserId = query.UserId,
                        JoinedAt = query.JoinedAt,
                        LoyaltyPoints = query.LoyaltyPoints,
                        MembershipLevel = query.MembershipLevel
                    };

                    response.IsSuccess = true;
                    response.Message = "Query Successfully";
                    response.Data = result;
                }
                else
                {
                    response.IsSuccess = false;
                    response.Message = "Somethings are wrong when querying!";
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }
            return response;
        }
        public async Task<ResponseDTO<bool>> UpdateLoyaltyPoints(int orderId)
        {
            var response = new ResponseDTO<bool>();
            try
            {
                var order = await _unitOfWork.SaleOrderRepository.GetObjectAsync(o => o.Id == orderId);
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
                    customerDetail = new Customer()
                    {
                        JoinedAt = DateTime.UtcNow,
                        LoyaltyPoints = Convert.ToInt32(order.SubTotal),
                        MembershipLevel = "Normal_Member",
                        UserId = order.UserId
                    };
                    await _unitOfWork.CustomerDetailRepository.InsertAsync(customerDetail);
                }

                customerDetail.LoyaltyPoints += Convert.ToInt32(order.SubTotal);
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
        public async Task<ResponseDTO<int>> AddMemberPointByPhoneNumber(string phoneNumber, int point)
        {
            var response = new ResponseDTO<int>();
            try
            {
                var phone = Convert.ToInt32(phoneNumber);
                var user = await _unitOfWork.UserRepository.GetObjectAsync(_ => Convert.ToInt32(_.PhoneNumber).Equals(phone));
                if (user == null)
                {
                    response.IsSuccess = false;
                    response.Message = $"User with phoneNumber {phone} is not found";
                    return response;
                }
                var toUpdate = await _unitOfWork.CustomerDetailRepository.GetObjectAsync(u => u.UserId == user.Id);
                if (toUpdate == null)
                {
                    Customer customerDetail = new Customer()
                    {
                        JoinedAt = DateTime.Now,
                        MembershipLevel = "Normal_Member",
                        UserId = user.Id,
                        LoyaltyPoints = point
                    };
                    await _unitOfWork.CustomerDetailRepository.InsertAsync(customerDetail);
                }
                else
                {
                    toUpdate.LoyaltyPoints += point;
                    await _unitOfWork.CustomerDetailRepository.UpdateAsync(toUpdate);
                }
                response.IsSuccess = true;
                response.Message = "Your points add successfully";
                response.Data = (int)toUpdate.LoyaltyPoints;
                return response;

            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
                return response;
            }
        }
        private int CalculateLoyaltyPoints(decimal totalPrice)
        {
            const int pointConversionRate = 10000; // 1 điểm cho mỗi 10.000 VND chi tiêu
            return (int)(totalPrice / pointConversionRate);
        }
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
