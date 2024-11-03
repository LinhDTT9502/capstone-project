using _2Sport_BE.Repository.Interfaces;
using _2Sport_BE.Repository.Models;
using _2Sport_BE.Infrastructure.DTOs;
using _2Sport_BE.Infrastructure.Enums;
using _2Sport_BE.Infrastructure.Helpers;
using AutoMapper;
using Microsoft.AspNetCore.Routing.Tree;
using _2Sport_BE.Infrastructure.Hubs;
using Microsoft.AspNetCore.SignalR;
using _2Sport_BE.Service.Enums;


namespace _2Sport_BE.Infrastructure.Services
{
    public interface IRentalOrderService
    {
        Task<ResponseDTO<List<RentalOrderVM>>> GetAllRentalOrderAsync();
        Task<ResponseDTO<RentalOrderVM>> GetRentalOrderByIdAsync(int orderId);
        Task<ResponseDTO<List<RentalOrderVM>>> GetOrdersByUserIdAsync(int userId);
        Task<ResponseDTO<RentalOrderVM>> GetRentalOrderByOrderCodeAsync(string orderCode);
        Task<ResponseDTO<List<RentalOrderVM>>> GetRentalOrdersByStatusAsync(int? orderStatus, int? paymentStatus);
        Task<ResponseDTO<RentalOrderVM>> CreateRentalOrderAsync(RentalOrderCM rentalOrderCM);
        Task<ResponseDTO<RentalOrderVM>> UpdateRentalOrderAsync(int orderId, RentalOrderUM rentalOrderUM);
        Task<ResponseDTO<int>> ChangeStatusRentalOrderAsync(int orderId, int status);
        Task<ResponseDTO<int>> DeleteRentalOrderAsync(int rentalOrderId);

        Task<ResponseDTO<int>> CancelRentalOrderAsync(int orderId);
        Task<ResponseDTO<int>> RequestExtendRentalPeriod(string orderCode, int? quantity, int period);
        Task CheckRentalOrdersForExpiration();
    }

    public class RentalOrderService : IRentalOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMethodHelper _methodHelper;
        private readonly IWarehouseService _warehouseService;
        private readonly ICustomerService _customerService;
        private IMapper _mapper;
        private readonly INotificationService _notificationService;
        public RentalOrderService(
            IUnitOfWork unitOfWork,
            IMethodHelper methodHelper,
            IWarehouseService warehouseService,
            ICustomerService customerService,
            IMapper mapper,
            INotificationService notificationService)
        {
            _unitOfWork = unitOfWork;
            _methodHelper = methodHelper;
            _warehouseService = warehouseService;
            _customerService = customerService;
            _mapper = mapper;
        }
        public async Task CheckRentalOrdersForExpiration()
        {
            var expiringOrders = await _unitOfWork.RentalOrderRepository.GetAsync(o => o.RentalEndDate >= DateTime.Now.Date.AddDays(1));

            foreach (var order in expiringOrders)
            {
                await _notificationService.SendRentalOrderExpirationNotificationAsync(order.UserId.ToString(), order.RentalOrderCode, (DateTime)order.RentalEndDate);
                //await _emailService.SendEmailAsync(order.CustomerEmail, "Rental Order Expiring Soon", $"Your rental order {order.OrderCode} will expire soon. Please return it on time or contact us if you need to extend.");
            }
        }
        public async Task<ResponseDTO<RentalOrderVM>> CreateRentalOrderAsync(RentalOrderCM rentalOrderCM)
        {
            var response = new ResponseDTO<RentalOrderVM>();
            using var transaction = await _unitOfWork.BeginTransactionAsync();

            try
            {
                if (rentalOrderCM.CustomerInfo.UserID.HasValue && rentalOrderCM.CustomerInfo.ShipmentDetailID.HasValue)
                {
                    var shipmentDetail = await _unitOfWork.ShipmentDetailRepository
                                    .GetObjectAsync(s => s.Id == rentalOrderCM.CustomerInfo.ShipmentDetailID 
                                                                && s.UserId == rentalOrderCM.CustomerInfo.UserID);
                    if(shipmentDetail is null)
                        return GenerateErrorResponse($"ShipmentDetail with ID = {rentalOrderCM.CustomerInfo.ShipmentDetailID} is not found!");
                }
                if (rentalOrderCM.rentalOrderItems.Count== 0)
                        return GenerateErrorResponse($"List of items are empty");

                foreach (var item in rentalOrderCM.rentalOrderItems)
                {
                    var productInWarehouse = await _unitOfWork.WarehouseRepository
                        .GetObjectAsync(p => p.Id == item.WarehouseId, new string[] { "Branch", "Product" });

                    if (!_warehouseService.IsStockAvailable(productInWarehouse, item.Quantity))
                    {
                        response.IsSuccess = false;
                        response.Message = $"Not enough stock in warehouse {item.WarehouseId}";
                        return response;
                    }

                    // Reduce stock
                    await _warehouseService.UpdateWarehouseStock(productInWarehouse, item.Quantity);

                    // Create the rental order
                    var order = CreateRentalOrder(rentalOrderCM, item, productInWarehouse);

                    if (!_methodHelper.CheckValidOfRentalDate(order.RentalStartDate, order.RentalEndDate))
                    {
                        response.IsSuccess = false;
                        response.Message = "Rental dates are invalid!";
                        return response;
                    }

                    await _unitOfWork.RentalOrderRepository.InsertAsync(order);
                    await transaction.CommitAsync();
                    //Send notifications to Admmin
                    await _notificationService.NotifyForCreatingNewOrderAsync(order.RentalOrderCode);

                    // Return success response
                    response = GenerateSuccessResponse(order, "Rental order inserted successfully");
                }
                return response;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                response.IsSuccess = false;
                response.Message = ex.Message;
                return response;
            }
        }
        public async Task<ResponseDTO<RentalOrderVM>> UpdateRentalOrderAsync(int orderId, RentalOrderUM rentalOrderUM)
        {
            var response = new ResponseDTO<RentalOrderVM>();
            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    var toUpdate = await _unitOfWork.RentalOrderRepository.GetObjectAsync(o => o.Id == orderId);
                    if (toUpdate == null)
                        return GenerateErrorResponse($"Order with id {orderId} not found!");

                    var warehouseToRestock = await _unitOfWork.WarehouseRepository.GetObjectAsync(w => w.ProductId == toUpdate.ProductId && w.BranchId == toUpdate.BranchId);
                    if (warehouseToRestock != null)
                    {
                        await _warehouseService.AdjustStockLevel(warehouseToRestock, toUpdate.Quantity, true);
                    }
                    var warehouseToUpdate = await _unitOfWork.WarehouseRepository
                        .GetObjectAsync(w => w.Id == rentalOrderUM.rentalOrderItem.WarehouseId, new string[] { "Product", "Branch" });
                    if (warehouseToUpdate == null)
                        return GenerateErrorResponse("Warehouse or product not found.");

                    if (warehouseToUpdate.AvailableQuantity < rentalOrderUM.rentalOrderItem.Quantity)
                        return GenerateErrorResponse($"Not enough stock for product {rentalOrderUM.rentalOrderItem.WarehouseId} at branch {warehouseToUpdate.Branch.BranchName}");

                    await _warehouseService.AdjustStockLevel(warehouseToUpdate, rentalOrderUM.rentalOrderItem.Quantity, false);

                    UpdateCustomerInfo(toUpdate, rentalOrderUM.CustomerInfo);
                    UpdateRentalInfo(toUpdate, rentalOrderUM.rentalInfor);
                    UpdateOrderDetails(toUpdate, rentalOrderUM, warehouseToUpdate);
                    await _unitOfWork.RentalOrderRepository.UpdateAsync(toUpdate);

                    await transaction.CommitAsync();


                    response = GenerateSuccessResponse(toUpdate, "Rental order updated successfully");
                    return response;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    response.IsSuccess = false;
                    response.Message = ex.Message;
                    return response;
                }
            }
        }
        public async Task<ResponseDTO<int>> DeleteRentalOrderAsync(int rentalOrderId)
        {
            var response = new ResponseDTO<int>();
            try
            {
                var toDelete = await _unitOfWork.RentalOrderRepository.GetObjectAsync(_ => _.Id ==  rentalOrderId);
                if (toDelete != null)
                {
                     await _unitOfWork.RentalOrderRepository.DeleteAsync(toDelete);
                    response.IsSuccess = true;
                    response.Message = "Deleted Successfully";
                    response.Data = 1;
                }
                else
                {
                    response.IsSuccess = true;
                    response.Message = "Deleted Error";
                    response.Data = 0;
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = true;
                response.Message = ex.Message;
                response.Data = 0;
            }
            return response;
        }
        private RentalOrder CreateRentalOrder(RentalOrderCM rentalOrderCM, RentalOrderItems item, Warehouse productInWarehouse)
        {
            int totaOfDays = CalculateTotalOfDay(item.RentalStartDate, item.RentalEndDate);
            return new RentalOrder
            {
                RentalOrderCode = _methodHelper.GenerateOrderCode(),
                Address = rentalOrderCM.CustomerInfo.Address,
                FullName = rentalOrderCM.CustomerInfo.FullName,
                ContactPhone = rentalOrderCM.CustomerInfo.ContactPhone,
                Email = rentalOrderCM.CustomerInfo.Email,
                PaymentMethodId = rentalOrderCM.PaymentMethodID,
                UserId = rentalOrderCM.CustomerInfo.UserID,
                BranchName = productInWarehouse.Branch.BranchName,
                ImgAvatarPath = item.ImgAvatarPath ?? productInWarehouse.Product.ImgAvatarPath,
                OrderStatus = (int)OrderStatus.PENDING,
                PaymentStatus = (int)PaymentStatus.IsWating,
                ProductId = productInWarehouse.ProductId,
                ProductName = productInWarehouse.Product.ProductName,
                RentPrice = productInWarehouse.Product.RentPrice,
                Quantity = item.Quantity,
                RentalStartDate = item.RentalStartDate,
                RentalEndDate = item.RentalEndDate,
                SubTotal = CalculateSubTotalForRentalOrder((decimal)productInWarehouse.Product.RentPrice, item.Quantity, totaOfDays),
                Note = rentalOrderCM.Note,
                CreatedAt = DateTime.Now,
            };
        }
        private void UpdateCustomerInfo(RentalOrder toUpdate, CustomerInfo customerInfo)
        {
            //5
            toUpdate.Address = customerInfo.Address;
            toUpdate.FullName = customerInfo.FullName;
            toUpdate.ContactPhone = customerInfo.ContactPhone;
            toUpdate.Email = customerInfo.Email;
            toUpdate.UserId = customerInfo.UserID;
        }
        private void UpdateRentalInfo(RentalOrder toUpdate, RentalInfor rentalInfo)
        {
            //5
            toUpdate.IsInspected = rentalInfo.IsInspected;
            toUpdate.IsRestocked = rentalInfo.IsRestocked;
            toUpdate.LateFee = rentalInfo.LateFee ?? 0;
            toUpdate.DamageFee = rentalInfo.DamageFee ?? 0;
            toUpdate.ReturnDate = rentalInfo.ReturnDate;
        }
        private void UpdateOrderDetails(RentalOrder toUpdate, RentalOrderUM rentalOrderUM, Warehouse warehouseToUpdate)
        {
            int totalOfDays = CalculateTotalOfDay(rentalOrderUM.rentalOrderItem.RentalStartDate, rentalOrderUM.rentalOrderItem.RentalEndDate);
            //16
            toUpdate.BranchId = warehouseToUpdate.BranchId;
            toUpdate.BranchName = warehouseToUpdate.Branch.BranchName;
            toUpdate.ProductId = warehouseToUpdate.ProductId;
            toUpdate.ProductName = warehouseToUpdate.Product.ProductName;
            toUpdate.RentPrice = warehouseToUpdate.Product.RentPrice;
            toUpdate.Quantity = rentalOrderUM.rentalOrderItem.Quantity;
            toUpdate.SubTotal = rentalOrderUM.SubTotal ?? CalculateSubTotalForRentalOrder((decimal)toUpdate.RentPrice,(int)toUpdate.Quantity, totalOfDays);
            toUpdate.TranSportFee = rentalOrderUM.TranSportFee ?? 0;
            toUpdate.TotalAmount = rentalOrderUM.TotalAmount;
            toUpdate.OrderStatus = rentalOrderUM.OrderStatus;
            toUpdate.PaymentStatus = rentalOrderUM.PaymentStatus;
            toUpdate.UpdatedAt = DateTime.Now;
            toUpdate.ImgAvatarPath = rentalOrderUM.rentalOrderItem.ImgAvatarPath;
            toUpdate.PaymentMethodId = rentalOrderUM.PaymentMethodID;
            toUpdate.RentalStartDate = rentalOrderUM.rentalOrderItem.RentalStartDate;
            toUpdate.RentalEndDate = rentalOrderUM.rentalOrderItem.RentalEndDate;
        }
        private ResponseDTO<RentalOrderVM> GenerateSuccessResponse(RentalOrder order, string messagge)
        {
            var result = _mapper.Map<RentalOrderVM>(order);
            result.OrderStatus = Enum.GetName((OrderStatus)order.OrderStatus);
            result.PaymentStatus = Enum.GetName<PaymentStatus>((PaymentStatus)order.PaymentStatus);
            return new ResponseDTO<RentalOrderVM>
            {
                IsSuccess = true,
                Message = messagge,
                Data = result
            };
        }
        private ResponseDTO<RentalOrderVM> GenerateErrorResponse(string message)
        {
            return new ResponseDTO<RentalOrderVM>()
            {
                IsSuccess = false,
                Message = message,
                Data = null
            };
        }
        private decimal CalculateTotalAmount(decimal? tranSportFee, decimal? subTotal)
        {
            return (decimal)(tranSportFee + subTotal);
        }
        private decimal CalculateSubTotalForRentalOrder(decimal rentPrice, int  quantity, int totalOfDays)
        {
            return (rentPrice * quantity) * totalOfDays;
        }
        private int CalculateTotalOfDay(DateTime startDate, DateTime endDate)
        {
            if (endDate < startDate)
            {
                throw new ArgumentException("End date should be after start date.");
            }
            TimeSpan rentalDuration = endDate - startDate;

            return rentalDuration.Days;
        }
        public async Task<ResponseDTO<List<RentalOrderVM>>> GetAllRentalOrderAsync()
        {
            var response = new ResponseDTO<List<RentalOrderVM>>();

            try
            {
                var rentalOrders = await _unitOfWork.RentalOrderRepository.GetAllAsync();

                if (rentalOrders != null && rentalOrders.Any())
                {
                    var result = _mapper.Map<List<RentalOrderVM>>(rentalOrders);
                    response.IsSuccess = true;
                    response.Message = "Query successful";
                    response.Data = result;
                }
                else
                {
                    response.IsSuccess = false;
                    response.Message = "No rental orders found";
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = $"Error: {ex.Message}";
            }

            return response;
        }

        public async Task<ResponseDTO<RentalOrderVM>> GetRentalOrderByIdAsync(int rentalOrderId)
        {
            var response = new ResponseDTO<RentalOrderVM>();

            try
            {
                var rentalOrder = await _unitOfWork.RentalOrderRepository.GetObjectAsync(
                    r => r.Id == rentalOrderId
                );

                if (rentalOrder != null)
                {
                    var result = _mapper.Map<RentalOrderVM>(rentalOrder);
                    response.IsSuccess = true;
                    response.Message = "Rental order found";
                    response.Data = result;
                }
                else
                {
                    response.IsSuccess = false;
                    response.Message = $"Rental order with ID = {rentalOrderId} not found";
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = $"Error: {ex.Message}";
            }

            return response;
        }

        public async Task<ResponseDTO<RentalOrderVM>> GetRentalOrderByOrderCodeAsync(string orderCode)
        {
            var response = new ResponseDTO<RentalOrderVM>();
            try
            {
                var rentalOrder = await _unitOfWork.RentalOrderRepository.GetObjectAsync(
                    r => r.RentalOrderCode == orderCode
                );

                if (rentalOrder != null)
                {
                    var result = _mapper.Map<RentalOrderVM>(rentalOrder);
                    response.IsSuccess = true;
                    response.Message = "Rental order found";
                    response.Data = result;
                }
                else
                {
                    response.IsSuccess = false;
                    response.Message = $"Rental order with Order Code = {orderCode} not found";
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = $"Error: {ex.Message}";
            }
            return response;
        }

        public async Task<ResponseDTO<List<RentalOrderVM>>> GetOrdersByUserIdAsync(int userId)
        {
            var response = new ResponseDTO<List<RentalOrderVM>>();

            try
            {
                var orders = await _unitOfWork.RentalOrderRepository
                    .GetAsync(o => o.UserId == userId);

                if (orders != null && orders.Any())
                {
                    var result = _mapper.Map<List<RentalOrderVM>>(orders);
                    response.IsSuccess = true;
                    response.Message = "Orders retrieved successfully";
                    response.Data = result;
                }
                else
                {
                    response.IsSuccess = false;
                    response.Message = $"No orders found for user with ID = {userId}";
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = $"Error: {ex.Message}";
            }

            return response;
        }

        public async Task<ResponseDTO<List<RentalOrderVM>>> GetRentalOrdersByStatusAsync(int? orderStatus, int? paymentStatus)
        {
            var response = new ResponseDTO<List<RentalOrderVM>>();

            try
            {
                var orders = await _unitOfWork.RentalOrderRepository
                    .GetAllAsync();
                if (orderStatus.HasValue)
                {
                    orders = orders.Where(o => o.OrderStatus == orderStatus.Value);
                }
                if (paymentStatus.HasValue)
                {
                    orders = orders.Where(o => o.PaymentStatus == paymentStatus.Value);
                }
                if (orders != null && orders.Any())
                {
                    var result = _mapper.Map<List<RentalOrderVM>>(orders);
                    response.IsSuccess = true;
                    response.Message = "Rental orders retrieved successfully";
                    response.Data = result;
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = $"Error: {ex.Message}";
            }

            return response;
        }

        public async Task<ResponseDTO<int>> ChangeStatusRentalOrderAsync(int orderId, int status)
        {
            var response = new ResponseDTO<int>();
            try
            {
                var order =  await _unitOfWork.RentalOrderRepository.GetObjectAsync();
                if(order is null)
                {
                    response.IsSuccess = false;
                    response.Message = "Updated Error";
                    response.Data = 0;
                }
                else
                {
                    order.OrderStatus = status;
                    response.IsSuccess = true;
                    response.Message = "Update Successfully";
                    response.Data = 1;
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
                response.Data = 0;
            }
            return response;
        }

        public async Task<ResponseDTO<int>> CancelRentalOrderAsync(int orderId)
        {
            var response = new ResponseDTO<int>();
            try
            {
                var order = await _unitOfWork.RentalOrderRepository.GetObjectAsync(o => o.Id ==  orderId);
                if(order is null)
                {

                }
                else
                {
                    order.OrderStatus = (int)OrderStatus.CANCELLED;
                    await _unitOfWork.RentalOrderRepository.UpdateAsync(order);
                    //Thong bao den truc page

                }
            }
            catch (Exception)
            {

                throw;
            }
            throw new NotImplementedException();
        }

        public async Task<ResponseDTO<int>> RequestExtendRentalPeriod(string orderCode, int? quantity, int period)
        {
            var response = new ResponseDTO<int>();
            try
            {
                var order = await _unitOfWork.RentalOrderRepository.GetObjectAsync(o => o.RentalOrderCode == orderCode);
                if(order is null)
                {
                    response.IsSuccess = false;
                    response.Message = $"Order with code {orderCode} not found!";
                }
                else
                {
                    var extendOrder = order;
                    extendOrder.ParentOrderCode = order.RentalOrderCode;
                    extendOrder.RentalOrderCode = _methodHelper.GenerateOrderCode();

                    extendOrder.RentalStartDate = order.RentalEndDate;
                    extendOrder.RentalEndDate = (order.RentalEndDate).Value.AddDays(period);

                    extendOrder.Quantity = quantity ?? order.Quantity;
                    extendOrder.SubTotal = order.RentPrice * extendOrder.Quantity * period;
                    extendOrder.TranSportFee = 0;
                    extendOrder.TotalAmount = (decimal)extendOrder.SubTotal;
                    extendOrder.OrderStatus = (int)RentalOrderStatus.EXTEND;
                    //
                   await _unitOfWork.RentalOrderRepository.InsertAsync(extendOrder);
                    response.IsSuccess = true;
                    response.Message = "Created Successfully";
                    response.Data = 1;
                }
            }
            catch (Exception ex)
            {

                throw;
            }
            return response;
        }
    }
}
