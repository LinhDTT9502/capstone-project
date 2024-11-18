using _2Sport_BE.Infrastructure.DTOs;
using _2Sport_BE.Repository.Interfaces;
using _2Sport_BE.Repository.Models;
using _2Sport_BE.Service.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2Sport_BE.Infrastructure.Services
{
    public interface IRefundRequestService
    {
        ResponseDTO<List<RefundRequest>> GetAllRefundRequest();
        ResponseDTO<RefundRequestVM> CreateRefundRequest(RefundRequestCM refundRequestCM);
        
    }
    public class RefundRequestService : IRefundRequestService
    {
        private readonly IUnitOfWork _unitOfwork;
        private readonly ISaleOrderService _saleOrderService;
        private readonly IRentalOrderService _rentalOrderService;
        public RefundRequestService(IUnitOfWork unitOfwork,
                                    ISaleOrderService saleOrderService,
                                    IRentalOrderService rentalOrderService)
        {
            _unitOfwork = unitOfwork;
            _saleOrderService = saleOrderService;
            _rentalOrderService = rentalOrderService;
        }
        public ResponseDTO<RefundRequestVM> CreateRefundRequest(RefundRequestCM refundRequestCM)
        {
            /*var response = new ResponseDTO<RefundRequestVM>();
            using(var transaction =  await _unitOfwork.BeginTransactionAsync())
            {
                try
                {
                    if (refundRequestCM.IsAgreementAccepted == false)
                    {
                        response.IsSuccess = false;
                        response.Message = "IsAggreementAccepted is false. Requiring true value";
                        return response;
                    }
                    if(refundRequestCM.OrderType == 1)
                    {
                        var saleOrder = await _saleOrderService.FindSaleOrderByCode(refundRequestCM.OrderCode);
                        if (saleOrder == null)
                        {
                            response.IsSuccess = false;
                            response.Message = "The Order is not found";
                            return response;
                        }
                        if (!saleOrder.BranchId.HasValue)
                        {
                            response.IsSuccess = false;
                            response.Message = "The Order do not belong to any branch, so it cannot be processed";
                            return response;
                        }
                        var refundRequest = new RefundRequest()
                        {
                            SaleOrderCode = saleOrder.OrderCode,
                            SaleOrderID = saleOrder.Id,
                            BranchId = saleOrder.BranchId.Value,
                            IsAgreementAccepted = refundRequestCM.IsAgreementAccepted,
                            Reason = refundRequestCM.Reason,
                            Notes = refundRequestCM.Notes,
                            
                        };
                    }
                    else
                    {
                        var rentalOrder = await _rentalOrderService.FindRentalOrderByOrderCode(refundRequestCM.OrderCode);
                        if (rentalOrder == null)
                        {
                            response.IsSuccess = false;
                            response.Message = "Order is not found";
                            return response;
                        }

                    }
                }
                catch (Exception ex)
                {

                    throw;
                }
            }*/
            return null;
        }

        public ResponseDTO<List<RefundRequest>> GetAllRefundRequest()
        {
            throw new NotImplementedException();
        }
    }
}
