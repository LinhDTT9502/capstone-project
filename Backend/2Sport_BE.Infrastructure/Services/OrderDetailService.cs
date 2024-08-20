using _2Sport_BE.Repository.Interfaces;
using _2Sport_BE.Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2Sport_BE.Service.Services
{
    public interface IOrderDetailService
    {
        Task<List<OrderDetail>> GetOrderDetailByOrderIdAsync(int orderId);
    }
    public class OrderDetailService : IOrderDetailService
    {
        private readonly IUnitOfWork _unitOfWork;
        public OrderDetailService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<List<OrderDetail>> GetOrderDetailByOrderIdAsync(int orderId)
        {
            var result = await _unitOfWork.OrderDetailRepository.GetAsync(_ => _.OrderId == orderId);
            return result.ToList();
        }
    }
}
