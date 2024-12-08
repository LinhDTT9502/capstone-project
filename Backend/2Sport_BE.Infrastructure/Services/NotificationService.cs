﻿using _2Sport_BE.Infrastructure.DTOs;
using _2Sport_BE.Infrastructure.Enums;
using _2Sport_BE.Infrastructure.Hubs;
using _2Sport_BE.Repository.Interfaces;
using _2Sport_BE.Repository.Models;
using Microsoft.AspNetCore.SignalR;
using Twilio.Rest.Api.V2010.Account;

namespace _2Sport_BE.Infrastructure.Services
{
    public interface INotificationService
    {
        Task NotifyForCreatingNewOrderAsync(string orderCode, bool isRentalOrder, int? brachId = null);
        Task NotifyForRejectOrderAsync(string orderCode, int branchId);
        Task NotifyPaymentCancellation(string orderCode, bool isRentalOrder, int? branchId = null);
        Task NotifyPaymentPaid(string orderCode, bool isRentalOrder, int? branchId = null);
        Task SendRentalOrderExpirationNotificationAsync(string customerId, string orderId, DateTime rentalEndDate);

        //
        Task<ResponseDTO<List<Notification>>> GetNotificationByUserId(int userId);
        Task<ResponseDTO<Notification>> UpdateNotificationStatus(int notificationId, bool isRead);
        Task<bool> NotifyForComment(int currUserId, List<User> coordinators, Product product);
        Task<bool> NotifyForReplyComment(string currUserId, Product product);
    }
    public class NotificationService : INotificationService
    {
        private readonly INotificationHub _notificationHub;
        private readonly IUnitOfWork _unitOfWork;
        public NotificationService(INotificationHub notificationHub, IUnitOfWork unitOfWork)
        {
            _notificationHub = notificationHub;
            _unitOfWork = unitOfWork;
        }
        public async Task NotifyForCreatingNewOrderAsync(string orderCode, bool isRentalOrder, int? branchId = null)
        {
            var message = isRentalOrder
                ? $"Đơn hàng thuê có mã là T-{orderCode} vừa được tạo."
                : $"Đơn hàng có mã là S-{orderCode} vừa được tạo.";

            List<int> userIdList = new List<int>();

            try
            {
                if (branchId.HasValue)
                {
                    var staffs = await _unitOfWork.StaffRepository.GetAsync(s => s.BranchId == branchId);
                    var managers = await _unitOfWork.ManagerRepository.GetAsync(m => m.BranchId == branchId);

                    if (staffs != null && staffs.Any())
                    {
                        userIdList.AddRange(staffs.Select(s => s.UserId.Value));
                    }

                    if (managers != null && managers.Any())
                    {
                        userIdList.AddRange(managers.Select(m => m.UserId.Value));
                    }

                    await _notificationHub.SendMessageToGroup($"Branch_{branchId}", message);
                }
                else
                {
                    var coordinators = await _unitOfWork.UserRepository.GetAsync(u => u.RoleId == (int)UserRole.OrderCoordinator);
                    if (coordinators != null && coordinators.Any())
                    {
                        userIdList.AddRange(coordinators.Select(c => c.Id));
                    }

                    await _notificationHub.SendMessageToGroup("Coordinator", message);
                }

                // Lưu thông báo vào cơ sở dữ liệu
                if (userIdList.Any())
                {
                    var notifications = userIdList.Select(userId => new Notification
                    {
                        UserId = userId,
                        Message = message,
                        Type = "Created Order Noti",
                        CreatedAt = DateTime.UtcNow,
                        IsRead = false
                    }).ToList();

                    await _unitOfWork.NotificationRepository.InsertRangeAsync(notifications);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
        public async Task NotifyForRejectOrderAsync(string orderCode, int branchId)
        {
            var message = $"Đơn hàng {orderCode} đã bị từ chối tại chi nhánh {branchId}.";

            try
            {
                
                List<int> userIdList = new List<int>();

                var coordinators = await _unitOfWork.UserRepository.GetAsync(u => u.RoleId == (int)UserRole.OrderCoordinator);
                if (coordinators != null && coordinators.Any())
                {
                    userIdList.AddRange(coordinators.Select(c => c.Id));
                }

                await _notificationHub.SendMessageToGroup("Coordinator", message);

                if (userIdList.Any())
                {
                    var notifications = userIdList.Select(userId => new Notification
                    {
                        UserId = userId,
                        Message = message,
                        Type = "Rejected Order Noti",
                        CreatedAt = DateTime.UtcNow,
                        IsRead = false
                    }).ToList();

                    await _unitOfWork.NotificationRepository.InsertRangeAsync(notifications);
                }
            }
            catch (Exception ex)
            { 
                Console.WriteLine($"Error in NotifyForRejectOrderAsync: {ex.Message}", ex);
            }
        }
        public async Task NotifyPaymentCancellation(string orderCode, bool isRentalOrder, int? branchId = null)
        {
            string message = isRentalOrder
                ? $"Thanh toán cho đơn hàng thuê T-{orderCode} đã bị hủy."
                : $"Thanh toán cho đơn hàng S-{orderCode} đã bị hủy.";

            try
            {
                List<int> userIdList = new List<int>();

                if (branchId != null)
                {
                    var staffs = await _unitOfWork.StaffRepository.GetAsync(s => s.BranchId == branchId);
                    var managers = await _unitOfWork.ManagerRepository.GetAsync(m => m.BranchId == branchId);

                    if (staffs != null && staffs.Any())
                    {
                        userIdList.AddRange(staffs.Select(s => s.UserId.Value));
                    }

                    if (managers != null && managers.Any())
                    {
                        userIdList.AddRange(managers.Select(m => m.UserId.Value));
                    }

                    var toBranch = $"Branch_{branchId}";
                    await _notificationHub.SendMessageToGroup(toBranch, message);
                }
                else
                {
                    var coordinators = await _unitOfWork.UserRepository.GetAsync(u => u.RoleId == (int)UserRole.OrderCoordinator);
                    if (coordinators != null && coordinators.Any())
                    {
                        userIdList.AddRange(coordinators.Select(c => c.Id));
                    }

                    await _notificationHub.SendMessageToGroup("Coordinator", message);
                }

                if (userIdList.Any())
                {
                    var notifications = userIdList.Select(userId => new Notification
                    {
                        UserId = userId,
                        Message = message,
                        Type = "Payment Cancellation Noti",
                        CreatedAt = DateTime.UtcNow,
                        IsRead = false
                    }).ToList();

                    await _unitOfWork.NotificationRepository.InsertRangeAsync(notifications);
                }
            }
            catch (Exception ex)
            {
               Console.WriteLine($"Error in NotifyPaymentCancellation: {ex.Message}", ex);
            }
        }
        public async Task NotifyPaymentPaid(string orderCode, bool isRentalOrder, int? branchId = null)
        {
            // Xây dựng thông báo dựa trên loại đơn hàng
            string message = isRentalOrder
                ? $"Thanh toán cho đơn hàng thuê T-{orderCode} thành công."
                : $"Thanh toán cho đơn hàng S-{orderCode} thành công.";

            try
            {
                List<int> userIdList = new List<int>();

                if (branchId != null)
                {
                    var staffs = await _unitOfWork.StaffRepository.GetAsync(s => s.BranchId == branchId);
                    var managers = await _unitOfWork.ManagerRepository.GetAsync(m => m.BranchId == branchId);

                    if (staffs != null && staffs.Any())
                    {
                        userIdList.AddRange(staffs.Select(s => s.UserId.Value));
                    }

                    if (managers != null && managers.Any())
                    {
                        userIdList.AddRange(managers.Select(m => m.UserId.Value));
                    }

                    var toBranch = $"Branch_{branchId}";
                    await _notificationHub.SendMessageToGroup(toBranch, message);
                }
                else
                {
 
                    var coordinators = await _unitOfWork.UserRepository.GetAsync(u => u.RoleId == (int)UserRole.OrderCoordinator);
                    if (coordinators != null && coordinators.Any())
                    {
                        userIdList.AddRange(coordinators.Select(c => c.Id));
                    }

                    await _notificationHub.SendMessageToGroup("Coordinator", message);
                }

                if (userIdList.Any())
                {
                    var notifications = userIdList.Select(userId => new Notification
                    {
                        UserId = userId,
                        Message = message,
                        Type = "Payment Paid Noti",
                        CreatedAt = DateTime.UtcNow,
                        IsRead = false 
                    }).ToList();

                    await _unitOfWork.NotificationRepository.InsertRangeAsync(notifications);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in NotifyPaymentPaid: {ex.Message}", ex);
            }
        }
        public async Task SendRentalOrderExpirationNotificationAsync(string customerId, string orderCode, DateTime rentalEndDate)
        {
            var message = $"Đơn thuê T-{orderCode} sẽ hết hạn vào ngày {rentalEndDate:dd/MM/yyyy}.";
            var notifications = new Notification()
            {
                UserId = int.Parse(customerId),
                Message = message,
                Type = "Payment Paid Noti",
                CreatedAt = DateTime.UtcNow,
                IsRead = false
            };
            await _unitOfWork.NotificationRepository.InsertAsync(notifications);

            await _notificationHub.SendNotificationToCustomer(customerId, message);
        }

        public async Task<ResponseDTO<Notification>> UpdateNotificationStatus(int notificationId, bool isRead)
        {
            var response = new ResponseDTO<Notification>();

            try
            {
                var notification = _unitOfWork.NotificationRepository.FindObject(_ => _.Id == notificationId);

                if (notification == null)
                {
                    response.IsSuccess = false;
                    response.Message = "Notification not found.";
                    return response;
                }
                notification.IsRead = isRead;
                notification.ReadAt = DateTime.UtcNow;

                await _unitOfWork.NotificationRepository.UpdateAsync(notification);
                response.IsSuccess = true;
                response.Message = "Notification status updated successfully.";
                response.Data = notification;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "An error occurred while updating the notification status.";
            }

            return response;
        }

        public async Task<ResponseDTO<List<Notification>>> GetNotificationByUserId(int userId)
        {
            var response = new ResponseDTO<List<Notification>>();

            try
            {
                var notifications = await _unitOfWork.NotificationRepository.GetAsync(n => n.UserId == userId);

                if (notifications == null || !notifications.Any())
                {
                    response.IsSuccess = false;
                    response.Message = "No notifications found for this user.";
                    return response;
                }

                response.IsSuccess = true;
                response.Message = "Notifications fetched successfully.";
                response.Data = notifications.ToList();
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "An error occurred while fetching notifications.";
            }

            return response;
        }

        public async Task<bool> NotifyForComment(int currUserId, List<User> coordinators, Product product)
        {
            try
            {
                var commentedUser = await _unitOfWork.UserRepository.FindAsync(currUserId);

                foreach (var coordinator in coordinators)
                {
                    var message = $"{commentedUser.UserName} đã đặt câu hỏi trong sản phẩm {product.ProductName}";
                    var notifications = new Notification()
                    {
                        UserId = coordinator.Id,
                        Message = message,
                        Type = "Comment Noti",
                        CreatedAt = DateTime.UtcNow,
                        IsRead = false,
                        ReferenceLink = $"localhost:5173/product/{product.ProductCode}"
                    };
                    await _unitOfWork.NotificationRepository.InsertAsync(notifications);

                    await _notificationHub.SendMessageToGroup("Coordinator", message);
                }
                return true;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public async Task<bool> NotifyForReplyComment(string currUserId, Product product)
        {
            try
            {
                var message = $"Quản trị viên đã trả lời câu hỏi của bạn trong sản phẩm {product.ProductName}";
                var notifications = new Notification()
                {
                    UserId = int.Parse(currUserId),
                    Message = message,
                    Type = "Comment Noti",
                    CreatedAt = DateTime.UtcNow,
                    IsRead = false,
                    ReferenceLink = $"localhost:5173/product/{product.ProductCode}"
                };
                await _unitOfWork.NotificationRepository.InsertAsync(notifications);

                await _notificationHub.SendNotificationToCustomer(currUserId, message);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }

        }
    }
}
