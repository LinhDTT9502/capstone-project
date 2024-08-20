using _2Sport_BE.Repository.Interfaces;
using _2Sport_BE.Repository.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Net.payOS;
using Net.payOS.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2Sport_BE.Service.Services
{
    public interface IOrderService
    {
        Task<IEnumerable<Order>> GetOrdersAsync();
        Task<List<Order>> ListAllOrderByUseIdAsync(int userId);
        Task<Order> GetOrderByIdAsync(int id);
        Task<Order> GetOrderByIdFromUserAsync(int orderId, int userId);
        Task<Order> GetOrderByOrderCode(string orderCode);
        Task<List<Order>> GetOrderByStatus(int status);
        Task<Order> AddOrderAsync(Order order);
        Task<bool> UpdateOrderAsync(int orderId, int status);
        Task<bool> DeleteOrderAsync(int id);

        Task<List<Order>> GetOrdersByMonth(int month);

        Task<List<Order>> GetOrdersByMonthAndStatus(int month, int status);
    }
    public class OrderService : IOrderService
    {
        private readonly TwoSportDBContext _context;
        private readonly IConfiguration _configuration;
        private readonly IUnitOfWork _unitOfWork;

        public OrderService(TwoSportDBContext context, IConfiguration configuration, IUnitOfWork unitOfWork)
        {
            _context = context;
            _configuration = configuration;
            _unitOfWork = unitOfWork;
        }
        public async Task<List<Order>> GetOrdersByMonth(int month)
        {

            var ordersInMonth = await _unitOfWork.OrderRepository.GetAsync(_ => _.ReceivedDate.Value.Month == month);
            return ordersInMonth.ToList();
        }
        public async Task<decimal> GetOrdersRevenueByMonth(int month)
        {
            
            var ordersInMonth = await _unitOfWork.OrderRepository.GetAsync(_ => _.ReceivedDate.Value.Month == month);

            var totalOrdersInMonth = ordersInMonth.ToList().Sum(_ => _.IntoMoney);

            return (decimal)totalOrdersInMonth;
        }
        public async Task<List<Order>> GetOrdersByMonthAndStatus(int month, int status)
        {
            var ordersInMonth = await _unitOfWork.OrderRepository.GetAsync(_ => _.ReceivedDate.Value.Month == month && _.Status == status);
            return ordersInMonth.ToList();
        }

        //cai nay chua biet
        public async Task<Order> GetOrderByIdAsync(int id)
        {
            return await _unitOfWork.OrderRepository.GetObjectAsync(_ => _.Id == id);
        }
        //For admin
        public async Task<List<Order>> GetOrderByStatus(int status)
        {
            var result = await _unitOfWork.OrderRepository.GetAsync(_ => _.Status == status);
            return result.ToList();
        }
        //For admin
        public async Task<IEnumerable<Order>> GetOrdersAsync()
        {
            return await _unitOfWork.OrderRepository.GetAllAsync("OrderDetails"); ;
        }
        //For user
        public async Task<List<Order>> ListAllOrderByUseIdAsync(int userId)
        {
            var result = await _unitOfWork.OrderRepository.GetAsync(_ => _.UserId == userId, "OrderDetails");
            return result.OrderBy(_ => _.ReceivedDate).ToList();
        }

        public async Task<Order> AddOrderAsync(Order order)
        {
            await _unitOfWork.OrderRepository.InsertAsync(order);
            return order;
        }

        public async Task<bool> UpdateOrderAsync(int orderId, int status)
        {
            var checkExist = await _unitOfWork.OrderRepository.GetObjectAsync(_ => _.Id == orderId);
            if(checkExist != null)
            {
                checkExist.Status = status;
                await _unitOfWork.OrderRepository.UpdateAsync(checkExist);
                return true;
            }
            return false;
        }

        public async Task<bool> DeleteOrderAsync(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return false;
            }

            await _unitOfWork.OrderRepository.DeleteAsync(id);
            return true;
        }

        public async Task<Order> GetOrderByIdFromUserAsync(int orderId, int userId)
        {
            return await _unitOfWork.OrderRepository.GetObjectAsync(_ => _.Id == orderId && _.UserId == userId);
        }

        public async Task<Order> GetOrderByOrderCode(string orderCode)
        {
            return await _unitOfWork.OrderRepository.GetObjectAsync(_ => _.OrderCode.Equals(orderCode.Trim()));
        }
    }

}
