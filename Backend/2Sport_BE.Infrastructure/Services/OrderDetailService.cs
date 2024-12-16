using _2Sport_BE.Repository.Interfaces;
using _2Sport_BE.Repository.Models;

namespace _2Sport_BE.Service.Services
{
    public interface IOrderDetailService
    {
        Task<List<OrderDetail>> GetOrderDetailByOrderIdAsync(int orderId);
        Task<bool> DeleteOrderDetailsByOrderId(int orderId);
    }
    public class OrderDetailService : IOrderDetailService
    {
        private readonly IUnitOfWork _unitOfWork;
        public OrderDetailService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> DeleteOrderDetailsByOrderId(int orderId)
        {
            using var transaction = await _unitOfWork.BeginTransactionAsync();

            try
            {
                var items = await _unitOfWork.OrderDetailRepository.GetAsync(_ => _.SaleOrderId == orderId);
                if (items == null || !items.Any()) 
                {
                    return false;
                }

                foreach (var item in items)
                {
                    await _unitOfWork.OrderDetailRepository.DeleteAsync(item);
                }

                await _unitOfWork.SaveChanges(); 

                await transaction.CommitAsync();

                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();  
                return false;
            }
        }


        public async Task<List<OrderDetail>> GetOrderDetailByOrderIdAsync(int orderId)
        {
            var result = await _unitOfWork.OrderDetailRepository.GetAsync(_ => _.SaleOrderId == orderId);
            return result.ToList();
        }
    }
}
