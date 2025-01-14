﻿using _2Sport_BE.Infrastructure.DTOs;
using _2Sport_BE.Repository.Interfaces;
using _2Sport_BE.Repository.Models;
using _2Sport_BE.Service.DTOs;
using _2Sport_BE.Service.Enums;
using AutoMapper;

namespace _2Sport_BE.Infrastructure.Services
{
    public interface IRefundRequestService
    {
        Task<ResponseDTO<List<RefundRequestVM>>> GetAllRefundRequest();
        Task<ResponseDTO<RefundRequestVM>> CreateRefundRequest(RefundRequestCM refundRequestCM);
        Task<ResponseDTO<bool>> UpdateRefundRequest(int refundRequestId, RefundRequestUM refundRequestUM);
        Task<ResponseDTO<bool>> DeleteRefundRequest(int refundRequestId);
        Task<ResponseDTO<List<RefundRequestVM>>> GetAllSaleRefundRequests(string status = null, int? branchId = null);
        Task<ResponseDTO<List<RefundRequestVM>>> GetAllRentalRefundRequests(string status = null, int? branchId = null);

    }
    public class RefundRequestService : IRefundRequestService
    {
        private readonly IUnitOfWork _unitOfwork;
        private readonly ISaleOrderService _saleOrderService;
        private readonly IRentalOrderService _rentalOrderService;
        private readonly IMapper _mapper;
        public RefundRequestService(IUnitOfWork unitOfwork,
                                    ISaleOrderService saleOrderService,
                                    IRentalOrderService rentalOrderService,
                                    IMapper mapper)
        {
            _unitOfwork = unitOfwork;
            _saleOrderService = saleOrderService;
            _rentalOrderService = rentalOrderService;
            _mapper = mapper;
        }
        public async Task<ResponseDTO<RefundRequestVM>> CreateRefundRequest(RefundRequestCM refundRequestCM)
        {
            var response = new ResponseDTO<RefundRequestVM>();

            using (var transaction = await _unitOfwork.BeginTransactionAsync())
            {
                try
                {
                    RefundRequest refundRequest = null;

                    if (refundRequestCM.OrderType == 1) 
                    {
                        var saleOrder = await _saleOrderService.FindSaleOrderByCode(refundRequestCM.OrderCode);
                        if (saleOrder == null)
                        {
                            response.IsSuccess = false;
                            response.Message = "Sale order not found.";
                            return response;
                        }

                        //if (!saleOrder.BranchId.HasValue)
                        //{
                        //    response.IsSuccess = false;
                        //    response.Message = "The Sale Order does not belong to any branch.";
                        //    return response;
                        //}
                        var checkExist = await _unitOfwork.RefundRequestRepository.GetObjectAsync(_ => _.SaleOrderCode == refundRequestCM.OrderCode);
                        if (checkExist != null)
                        {
                            response.IsSuccess = false;
                            response.Message = "This sale order is existed in refund request.";
                            return response;
                        }
                        refundRequest = new RefundRequest
                        {
                            SaleOrderCode = saleOrder.SaleOrderCode,
                            SaleOrderID = saleOrder.Id,
                            BranchId = saleOrder.BranchId != null ? saleOrder.BranchId.Value : 0,
                            IsAgreementAccepted = refundRequestCM.IsAgreementAccepted,
                            Reason = refundRequestCM.Reason,
                            Status = RefundStatus.Pending.ToString(),
                            Notes = refundRequestCM.Notes, 
                            CreatedAt = DateTime.UtcNow
                        };
                    }
                    else // Refund for Rental Order
                    {
                        var rentalOrder = await _rentalOrderService.FindRentalOrderByOrderCode(refundRequestCM.OrderCode);
                        if (rentalOrder == null)
                        {
                            response.IsSuccess = false;
                            response.Message = "Rental order not found.";
                            return response;
                        }

                        //if (!rentalOrder.BranchId.HasValue)
                        //{
                        //    response.IsSuccess = false;
                        //    response.Message = "The Rental Order does not belong to any branch.";
                        //    return response;
                        //}
                        var checkExist = await _unitOfwork.RefundRequestRepository.GetObjectAsync(_ => _.RentalOrderCode == refundRequestCM.OrderCode);
                        if (checkExist != null)
                        {
                            response.IsSuccess = false;
                            response.Message = "This rental order is existed in refund request.";
                            return response;
                        }
                        refundRequest = new RefundRequest
                        {
                            RentalOrderCode = rentalOrder.RentalOrderCode,
                            RentalOrderID = rentalOrder.Id,
                            BranchId = rentalOrder.BranchId != null ? rentalOrder.BranchId.Value : 0,
                            IsAgreementAccepted = refundRequestCM.IsAgreementAccepted,
                            Reason = refundRequestCM.Reason,
                            Notes = refundRequestCM.Notes,
                            Status = RefundStatus.Pending.ToString(),
                            CreatedAt = DateTime.Now,
                        };

                    }
                    await _unitOfwork.RefundRequestRepository.InsertAsync(refundRequest);

                    response.Data = _mapper.Map<RefundRequestVM>(refundRequest);
                    response.IsSuccess = true;
                    response.Message = "Refund request created successfully.";

                    await transaction.CommitAsync();
                    return response;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();

                    response.IsSuccess = false;
                    response.Message = $"An error occurred: {ex.Message}";
                    return response;
                }
            }
        }

        public async Task<ResponseDTO<bool>> DeleteRefundRequest(int refundRequestId)
        {
            var response = new ResponseDTO<bool>();

            try
            {
                var refundRequest = _unitOfwork.RefundRequestRepository.FindObject(_ => _.RefundID == refundRequestId);
                if (refundRequest == null)
                {
                    response.IsSuccess = false;
                    response.Message = "Refund request not found.";
                    return response;
                }

                if (refundRequest.Status == RefundStatus.Completed.ToString())
                {
                    response.IsSuccess = false;
                    response.Message = "Cannot delete a completed refund request.";
                    return response;
                }

                await _unitOfwork.RefundRequestRepository.DeleteAsync(refundRequest);

                response.IsSuccess = true;
                response.Message = "Refund request deleted successfully.";
                response.Data = true;
                return response;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = $"An error occurred: {ex.Message}";
                return response;
            }
        }

        public async Task<ResponseDTO<List<RefundRequestVM>>> GetAllRefundRequest()
        {
            var response = new ResponseDTO<List<RefundRequestVM>>();

            try
            {
                var refundRequests = await _unitOfwork.RefundRequestRepository.GetAllAsync();

                if (refundRequests == null || !refundRequests.Any())
                {
                    response.IsSuccess = true;
                    response.Message = "No refund requests found.";
                    response.Data = new List<RefundRequestVM>();
                    return response;
                }

                var refundRequestVMs = _mapper.Map<List<RefundRequestVM>>(refundRequests);

                response.IsSuccess = true;
                response.Message = "Refund requests retrieved successfully.";
                response.Data = refundRequestVMs;
                return response;
            }
            catch (Exception ex)
            {
                // Bắt lỗi và trả về phản hồi
                response.IsSuccess = false;
                response.Message = $"An error occurred while retrieving refund requests: {ex.Message}";
                response.Data = null;
                return response;
            }
        }


        public async Task<ResponseDTO<List<RefundRequestVM>>> GetAllSaleRefundRequests(string status = null, int? branchId = null)
        {
            var response = new ResponseDTO<List<RefundRequestVM>>();

            try
            {
                var refundRequests = await _unitOfwork.RefundRequestRepository.GetAsync(_ => _.SaleOrderID != null || _.SaleOrderCode != null);

                if (!string.IsNullOrEmpty(status))
                {
                    refundRequests = refundRequests.Where(r => r.Status == status).ToList();
                }
                if (branchId.HasValue)
                {
                    refundRequests = refundRequests.Where(r => r.BranchId == branchId.Value).ToList();
                }

                var refundRequestVMs = refundRequests.Select(r => _mapper.Map<RefundRequestVM>(r)).ToList();

                response.IsSuccess = true;
                response.Message = "Refund requests retrieved successfully.";
                response.Data = refundRequestVMs;
                return response;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = $"An error occurred: {ex.Message}";
                return response;
            }
        }

        public async Task<ResponseDTO<List<RefundRequestVM>>> GetAllRentalRefundRequests(string status = null, int? branchId = null)
        {
            var response = new ResponseDTO<List<RefundRequestVM>>();

            try
            {
                var refundRequests = await _unitOfwork.RefundRequestRepository.GetAsync(_ => _.RentalOrderID != null || _.RentalOrderCode != null);

                if (!string.IsNullOrEmpty(status))
                {
                    refundRequests = refundRequests.Where(r => r.Status == status).ToList();
                }
                if (branchId.HasValue)
                {
                    refundRequests = refundRequests.Where(r => r.BranchId == branchId.Value).ToList();
                }

                var refundRequestVMs = refundRequests.Select(r => _mapper.Map<RefundRequestVM>(r)).ToList();

                response.IsSuccess = true;
                response.Message = "Refund requests retrieved successfully.";
                response.Data = refundRequestVMs;
                return response;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = $"An error occurred: {ex.Message}";
                return response;
            }
        }
        public async Task<ResponseDTO<bool>> UpdateRefundRequest(int refundRequestId, RefundRequestUM refundRequestUM)
        {
            var response = new ResponseDTO<bool>();

            try
            {
                var refundRequest = await _unitOfwork.RefundRequestRepository.GetObjectAsync(_ => _.RefundID == refundRequestUM.RefundRequestId);
                if (refundRequest == null)
                {
                    response.IsSuccess = false;
                    response.Message = "Refund request not found.";
                    return response;
                }

                // Cập nhật thông tin
                refundRequest.Status = refundRequestUM.Status;
                refundRequest.RefundAmount = refundRequestUM.RefundAmount;
                refundRequest.RefundMethod = refundRequestUM.RefundMethod;
                refundRequest.StaffNotes = refundRequestUM.StaffNotes;
                refundRequest.UpdatedAt = DateTime.UtcNow;
                refundRequest.ProcessedBy = refundRequestUM.ProcessedBy;
                refundRequest.StaffName = refundRequestUM.StaffName;
                refundRequest.RefundAmount = refundRequestUM.RefundAmount;
                refundRequest.PaymentGatewayTransactionID = string.IsNullOrEmpty(refundRequestUM.PaymentGatewayTransactionID) != false ? refundRequestUM.PaymentGatewayTransactionID : "UNKNOWM";
                // Lưu vào cơ sở dữ liệu
                await _unitOfwork.RefundRequestRepository.UpdateAsync(refundRequest);

                response.IsSuccess = true;
                response.Message = "Refund request updated successfully.";
                response.Data = true;
                return response;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = $"An error occurred: {ex.Message}";
                return response;
            }
        }
    }
}
