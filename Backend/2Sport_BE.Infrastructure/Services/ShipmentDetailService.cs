using _2Sport_BE.Repository.Data;
using _2Sport_BE.Repository.Interfaces;
using _2Sport_BE.Repository.Models;
using Microsoft.VisualBasic;

namespace _2Sport_BE.Service.Services
{
    public interface IShipmentDetailService
    {
        Task<IQueryable<ShipmentDetail>> GetAllShipmentDetails(int userId);
        Task<ShipmentDetail> GetShipmentDetailById(int Id);
        Task AddShipmentDetails(IEnumerable<ShipmentDetail> shipmentDetails);
        Task AddShipmentDetail(ShipmentDetail shipmentDetail);
        Task DeleteShipmentDetailById(int id);
        Task UpdateShipmentDetail(ShipmentDetail shipmentDetail);
    }
    public class ShipmentDetailService : IShipmentDetailService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly TwoSportCapstoneDbContext dbContext;
        public ShipmentDetailService(IUnitOfWork unitOfWork, TwoSportCapstoneDbContext dbContext)
        {
            _unitOfWork = unitOfWork;
            this.dbContext = dbContext;
        }

        public async Task AddShipmentDetail(ShipmentDetail shipmentDetail)
        {
            await _unitOfWork.ShipmentDetailRepository.InsertAsync(shipmentDetail);
            await _unitOfWork.SaveChanges();
        }

        public async Task AddShipmentDetails(IEnumerable<ShipmentDetail> shipmentDetails)
        {
           await _unitOfWork.ShipmentDetailRepository.InsertRangeAsync(shipmentDetails);
        }

        public async Task DeleteShipmentDetailById(int id)
        {
            var result = await _unitOfWork.ShipmentDetailRepository.GetObjectAsync(_ => _.Id == id);
            if(result != null)
            {
                await _unitOfWork.ShipmentDetailRepository.DeleteAsync(result);
                await _unitOfWork.SaveChanges();
            }
        }

        public async Task<IQueryable<ShipmentDetail>> GetAllShipmentDetails(int userId)
        {
            var result = await _unitOfWork.ShipmentDetailRepository.GetAsync(_ => _.UserId == userId);
            return result.AsQueryable();
        }

        public async Task<ShipmentDetail> GetShipmentDetailById(int id)
        {
            var resutl = await _unitOfWork.ShipmentDetailRepository.GetAsync(_ => _.Id == id);
            return resutl.FirstOrDefault();
        }

        public async Task UpdateShipmentDetail(ShipmentDetail shipmentDetail)
        {
            await _unitOfWork.ShipmentDetailRepository.UpdateAsync(shipmentDetail);
            await _unitOfWork.SaveChanges();
        }
    }
}
