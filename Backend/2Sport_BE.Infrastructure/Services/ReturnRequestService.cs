using _2Sport_BE.Infrastructure.DTOs;
using _2Sport_BE.Infrastructure.Enums;
using _2Sport_BE.Infrastructure.Helpers;
using _2Sport_BE.Infrastructure.Services;
using _2Sport_BE.Repository.Interfaces;
using _2Sport_BE.Repository.Models;
using _2Sport_BE.Service.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Semantics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2Sport_BE.Service.Services
{
    public interface IReturnRequestService
    {
        Task<ResponseDTO<List<ReturnRequest>>> GetAllReturn();
        Task<ResponseDTO<List<ReturnRequest>>> GetReturnByBranchId(int branchId);
        Task<ResponseDTO<ReturnResponseDto>> CreateReturnAsync(ReturnRequestDTO request, string videoUrl);
        Task<ResponseDTO<ReturnRequest>> UpdateReturnRequest(int returnId, ReturnRequestUM updatedRequest);
        Task<ResponseDTO<List<ReturnRequest>>> GetReturnByOrderId(int? saleOrderId, int? rentalOrderId);
        Task<ResponseDTO<bool>> DeleteReturnRequest(int returnId);
    }
    public class ReturnRequestService : IReturnRequestService
    {
        private readonly ISaleOrderService _saleOrderService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMethodHelper _methodHelper;
        public ReturnRequestService(ISaleOrderService saleOrderService, IUnitOfWork unitOfWork, IMethodHelper methodHelper)
        {
            _saleOrderService = saleOrderService;
            _unitOfWork = unitOfWork;
            _methodHelper = methodHelper;
        }

        public async Task<ResponseDTO<ReturnResponseDto>> CreateReturnAsync(ReturnRequestDTO request, string videoUrl)
        {
            var order = await _saleOrderService.FindSaleOrderById(request.OrderId);
            if (order == null || order.BranchId == null)
            {
                return new ResponseDTO<ReturnResponseDto>
                {
                    IsSuccess = false,
                    Message = "Không tìm thấy đơn hàng.",
                };
            }

            var orderDetail = order.OrderDetails.FirstOrDefault(x => x.ProductCode == request.ProductCode &&
                                                                        x.Size == request.Size &&
                                                                        x.Color == request.Color &&
                                                                        x.Condition == request.Condition);
            if (orderDetail == null)
            {
                return new ResponseDTO<ReturnResponseDto>
                {
                    IsSuccess = false,
                    Message = "Sản phẩm không tìm thấy trong đơn hàng.",
                };
            }

            if (order.OrderStatus < (int)OrderStatus.DELIVERED)
            {
                return new ResponseDTO<ReturnResponseDto>
                {
                    IsSuccess = false,
                    Message = "Đơn hàng chưa trong trạng thái trả hàng, hoàn tiền.",
                };
            }

            var returnEntity = new ReturnRequest
            {
                SaleOrderID = request.OrderId,
                ProductCode = request.ProductCode,
                Size = request.Size,
                Color = request.Color,
                Condition = request.Condition,
                ReturnAmount = request.ReturnAmount,
                Reason = request.Reason,
                Notes = request.Notes,
                VideoUrl = videoUrl,
                Status = "PENDING",
                CreatedAt = DateTime.UtcNow,
                BranchId = order.BranchId.Value, 

            };
            await _unitOfWork.ReturnRequestRepository.InsertAsync(returnEntity);

            return new ResponseDTO<ReturnResponseDto>
            {
                IsSuccess = true,
                Message = "Yêu cầu trả hàng, hoàn tiền đã được tạo",
                Data = new ReturnResponseDto
                {
                    ReturnId = returnEntity.ReturnID,
                    OrderId = returnEntity.SaleOrderID.Value,
                    ProductCode = returnEntity.ProductCode,
                    Status = returnEntity.Status,
                    CreatedAt = returnEntity.CreatedAt,
                    Color = returnEntity.Color,
                    Condition = returnEntity.Condition,
                    Size = returnEntity.Size,
                }
            };
 
        }

        public async Task<ResponseDTO<List<ReturnRequest>>> GetAllReturn()
        {
            try
            {
                var result = await _unitOfWork.ReturnRequestRepository
                    .GetAllAsync(new string[] { "SaleOrder", "RentalOrder" });

                if (result == null || result.ToList().Count == 0)
                {
                    return new ResponseDTO<List<ReturnRequest>>
                    {
                        IsSuccess = false,
                        Message = "No return requests found.",
                        Data = null
                    };
                }
                return new ResponseDTO<List<ReturnRequest>>
                {
                    IsSuccess = true,
                    Message = "Return requests retrieved successfully.",
                    Data = result.ToList()
                };
            }
            catch (Exception ex)
            {
                return new ResponseDTO<List<ReturnRequest>>
                {
                    IsSuccess = false,
                    Message = $"An error occurred: {ex.Message}",
                    Data = null
                };
            }
        }

        public async Task<ResponseDTO<List<ReturnRequest>>> GetReturnByBranchId(int branchId)
        {
            try
            {
                var result = await _unitOfWork.ReturnRequestRepository
                    .GetAndIncludeAsync(r => r.BranchId == branchId, new string[] { "SaleOrder", "RentalOrder" });

                if (result == null || result.ToList().Count == 0)
                {
                    return new ResponseDTO<List<ReturnRequest>>
                    {
                        IsSuccess = true,
                        Message = "No return requests found for the given Branch ID.",
                        Data = null
                    };
                }
                return new ResponseDTO<List<ReturnRequest>>
                {
                    IsSuccess = true,
                    Message = "Return requests retrieved successfully.",
                    Data = result.ToList()
                };
            }
            catch (Exception ex)
            {
                return new ResponseDTO<List<ReturnRequest>>
                {
                    IsSuccess = false,
                    Message = $"An error occurred: {ex.Message}",
                    Data = null
                };
            }
        }

        public async Task<ResponseDTO<List<ReturnRequest>>> GetReturnByOrderId(int? saleOrderId, int? rentalOrderId)
        {
            var result = await _unitOfWork.ReturnRequestRepository
                 .GetAndIncludeAsync(r =>
           (saleOrderId.HasValue && r.SaleOrderID == saleOrderId) ||
           (rentalOrderId.HasValue && r.RentalOrderID == rentalOrderId), new string[] { "SaleOrder", "RentalOrder" });

            if (result == null)
            {
                return new ResponseDTO<List<ReturnRequest>>
                {
                    IsSuccess = false,
                    Message = "No return request found for the given Order ID."
                };
            }

            return new ResponseDTO<List<ReturnRequest>>
            {
                IsSuccess = true,
                Data = result.ToList(),
                Message = "Return request retrieved successfully."
            };
        }
        public async Task<ResponseDTO<ReturnRequest>> UpdateReturnRequest(int returnId, ReturnRequestUM updatedRequest)
        {
            try
            {
                var existingRequest = await _unitOfWork.ReturnRequestRepository.GetObjectAsync(r => r.ReturnID == returnId);
                if (existingRequest == null)
                {
                    return new ResponseDTO<ReturnRequest>
                    {
                        IsSuccess = false,
                        Message = "Return request not found.",
                        Data = null
                    };
                }

                existingRequest.ProductCode = updatedRequest.ProductCode;
                existingRequest.Size = updatedRequest.Size;
                existingRequest.Color = updatedRequest.Color;
                existingRequest.Condition = updatedRequest.Condition;
                existingRequest.ReturnAmount = updatedRequest.ReturnAmount;
                existingRequest.Reason = updatedRequest.Reason;
                existingRequest.Notes = updatedRequest.Notes;
                existingRequest.ProcessedBy = updatedRequest.ProcessedBy;
                existingRequest.Status = updatedRequest.Status;
                existingRequest.VideoUrl = updatedRequest.VideoUrl;
                existingRequest.UpdatedAt = _methodHelper.GetTimeInUtcPlus7();

                await _unitOfWork.ReturnRequestRepository.UpdateAsync(existingRequest);

                return new ResponseDTO<ReturnRequest>
                {
                    IsSuccess = true,
                    Message = "Return request updated successfully.",
                    Data = existingRequest
                };
            }
            catch (Exception ex)
            {
                return new ResponseDTO<ReturnRequest>
                {
                    IsSuccess = false,
                    Message = $"An error occurred: {ex.Message}",
                    Data = null
                };
            }
        }
        public async Task<ResponseDTO<bool>> DeleteReturnRequest(int returnId)
        {
            try
            {
                var existingRequest = await _unitOfWork.ReturnRequestRepository.GetObjectAsync(r => r.ReturnID == returnId);
                if (existingRequest == null)
                {
                    return new ResponseDTO<bool>
                    {
                        IsSuccess = false,
                        Message = "Return request not found.",
                        Data = false
                    };
                }

                await _unitOfWork.ReturnRequestRepository.DeleteAsync(existingRequest);
                await _unitOfWork.SaveChanges();

                return new ResponseDTO<bool>
                {
                    IsSuccess = true,
                    Message = "Return request deleted successfully.",
                    Data = true
                };
            }
            catch (Exception ex)
            {
                return new ResponseDTO<bool>
                {
                    IsSuccess = false,
                    Message = $"An error occurred: {ex.Message}",
                    Data = false
                };
            }
        }

    }
}
