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
using Hangfire.Server;


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
        private readonly IDeliveryMethodService _deliveryMethodService;
        public RentalOrderService(
            IUnitOfWork unitOfWork,
            IMethodHelper methodHelper,
            IWarehouseService warehouseService,
            ICustomerService customerService,
            IMapper mapper,
            INotificationService notificationService,
            IDeliveryMethodService deliveryMethodService)
        {
            _unitOfWork = unitOfWork;
            _methodHelper = methodHelper;
            _warehouseService = warehouseService;
            _customerService = customerService;
            _mapper = mapper;
            _deliveryMethodService = deliveryMethodService;
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
            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    User user = null;
                    ShipmentDetail shipmentDetail = null;
                    if (rentalOrderCM.UserID.HasValue && rentalOrderCM.UserID != 0)
                    {
                        user = await _unitOfWork.UserRepository
                                        .GetObjectAsync(u => u.Id == rentalOrderCM.UserID);
                        if (user == null)
                        {
                            return GenerateErrorResponse($"User with Id = {rentalOrderCM.UserID} is not found!");
                        }
                        else
                        {
                            shipmentDetail = await _unitOfWork.ShipmentDetailRepository
                                    .GetObjectAsync(s => s.Id == rentalOrderCM.ShipmentDetailID && s.UserId == user.Id);
                            if (shipmentDetail == null)
                            {
                                return GenerateErrorResponse($"ShipmenDetail with Id = {rentalOrderCM.ShipmentDetailID} is not found!");
                            }
                        }
                    }
                    if (rentalOrderCM.rentalOrderItems.Count == 0)
                        return GenerateErrorResponse($"List of items are empty");

                    string DeliveryMethod = rentalOrderCM.DeliveryMethod;
                    if (string.IsNullOrEmpty(DeliveryMethod) || !_deliveryMethodService.IsValidMethod(DeliveryMethod))
                    {
                        return GenerateErrorResponse("Delivery Method is invalid");
                    }
                    //Den store lay thi phai co branchId
                    if (DeliveryMethod.Equals("STORE_PICKUP") && !rentalOrderCM.BranchId.HasValue)
                    {
                        return GenerateErrorResponse("Delivery Method is invalid");
                    }
                    if(rentalOrderCM.rentalOrderItems.Count == 1) // Create only 1 order
                    {

                        var rentalOrder = CreateParentRentalOrder(rentalOrderCM);
                        if (user != null && shipmentDetail != null)
                        {
                            if (user != null && shipmentDetail != null)
                            {
                                SetOrderUserDetails(rentalOrder, user, shipmentDetail);
                            }
                        }
                        if (DeliveryMethod.Equals("HOME_DELIVERY"))
                        {
                            rentalOrder.PaymentMethodId = (int)OrderMethods.COD;
                        }
                        foreach (var item in rentalOrderCM.rentalOrderItems)
                        {
                            //Set rentalPeriod
                            if (!_methodHelper.CheckValidOfRentalDate(item.RentalStartDate, item.RentalEndDate))
                            {
                                response.IsSuccess = false;
                                response.Message = "Rental dates are invalid!";
                                return response;
                            }
                            rentalOrder.RentalEndDate = item.RentalEndDate;
                            rentalOrder.RentalStartDate = item.RentalStartDate;

                            var branch = await _unitOfWork.BranchRepository.GetObjectAsync(b => b.Id == rentalOrder.BranchId);
                            if (branch != null)
                            {
                                rentalOrder.BranchId = branch.Id;
                                rentalOrder.BranchName = branch.BranchName;
                                //Neu chon den cua hang, thi tru available quantity trong cua hang truoc
                                var reduceInWarehouse = await FindAndReduceQuantityInWarehouse(item, branch.Id);
                                if (!reduceInWarehouse)
                                {
                                    await transaction.RollbackAsync();
                                    return GenerateErrorResponse($"Failed to update stock for product {item.ProductName}");
                                }
                            }
                            var product = await _unitOfWork.ProductRepository.GetObjectAsync(p => p.Id == item.ProductId);
                            if (product != null)
                            {
                                rentalOrder.ProductId = product.Id;
                                rentalOrder.ProductName = product.ProductName;
                                rentalOrder.RentPrice = product.RentPrice;
                            }
                            int rentalDays = Math.Max((item.RentalEndDate - item.RentalStartDate).Days, 1);
                            rentalOrder.SubTotal += item.UnitPrice * item.Quantity * rentalDays;
                        }
                        rentalOrder.TranSportFee = 0;
                        rentalOrder.TotalAmount = (decimal)(rentalOrder.SubTotal + rentalOrder.TranSportFee);

                        await transaction.CommitAsync();
                        //Send notifications to Admmin
                        await _notificationService.NotifyForCreatingNewOrderAsync(rentalOrder.RentalOrderCode);

                        // Return success response
                        response = GenerateSuccessResponse(rentalOrder, "Rental order inserted successfully");

                    }
                    else if( rentalOrderCM.rentalOrderItems.Count > 1 )//Parent order and child order
                    {
                        var parentRentalOrder = CreateParentRentalOrder(rentalOrderCM);
                        if (user != null && shipmentDetail != null)
                        {
                            if (user != null && shipmentDetail != null)
                            {
                                SetOrderUserDetails(parentRentalOrder, user, shipmentDetail);
                            }
                        }
                        if (DeliveryMethod.Equals("HOME_DELIVERY"))
                        {
                            parentRentalOrder.PaymentMethodId = (int)OrderMethods.COD;
                        }
                        await _unitOfWork.RentalOrderRepository.InsertAsync(parentRentalOrder);

                        foreach (var item in rentalOrderCM.rentalOrderItems)
                        {
                            var childRentalOrder = CreateChildRentalOrder(rentalOrderCM, parentRentalOrder.ParentOrderCode);
                            if (user != null && shipmentDetail != null)
                            {
                                SetOrderUserDetails(childRentalOrder,user, shipmentDetail);
                            }
                            if (DeliveryMethod.Equals("HOME_DELIVERY"))
                            {
                                childRentalOrder.PaymentMethodId = (int)OrderMethods.COD;
                            }
                            //Set rentalPeriod
                            if (!_methodHelper.CheckValidOfRentalDate(item.RentalStartDate, item.RentalEndDate))
                            {
                                response.IsSuccess = false;
                                response.Message = "Rental dates are invalid!";
                                return response;
                            }
                            childRentalOrder.RentalEndDate = item.RentalEndDate;
                            childRentalOrder.RentalStartDate = item.RentalStartDate;

                            var branch = await _unitOfWork.BranchRepository.GetObjectAsync(b => b.Id == rentalOrderCM.BranchId);
                            if (branch != null)
                            {
                                childRentalOrder.BranchId = branch.Id;
                                childRentalOrder.BranchName = branch.BranchName;
                                //Neu chon den cua hang, thi tru available quantity trong cua hang truoc
                                var reduceInWarehouse = await FindAndReduceQuantityInWarehouse(item, branch.Id);
                                if (!reduceInWarehouse)
                                {
                                    response.IsSuccess = false;
                                    response.Message = $"Failed to update stock for product {item.ProductName}";
                                    await transaction.RollbackAsync();
                                    return response;
                                }
                            }
                            var product = await _unitOfWork.ProductRepository.GetObjectAsync(p => p.Id == item.ProductId);
                            if (product != null)
                            {
                                childRentalOrder.ProductId = product.Id;
                                childRentalOrder.ProductName = product.ProductName;
                                childRentalOrder.RentPrice = product.RentPrice;
                            }
                            int rentalDays = Math.Max((item.RentalEndDate - item.RentalStartDate).Days, 1);
                            childRentalOrder.SubTotal += item.UnitPrice * item.Quantity * rentalDays;
                            childRentalOrder.TranSportFee = 0;
                            childRentalOrder.TotalAmount = (decimal)(childRentalOrder.SubTotal + childRentalOrder.TranSportFee);
                            await _unitOfWork.RentalOrderRepository.InsertAsync(childRentalOrder);
                            parentRentalOrder.SubTotal += childRentalOrder.TotalAmount;
                        }
                        parentRentalOrder.TotalAmount += (decimal)parentRentalOrder.SubTotal;
                        await _unitOfWork.RentalOrderRepository.UpdateAsync(parentRentalOrder);

                        await transaction.CommitAsync();
                        //Send notifications to Admmin
                        await _notificationService.NotifyForCreatingNewOrderAsync(parentRentalOrder.RentalOrderCode);

                        // Return success response
                        response = GenerateSuccessResponse(parentRentalOrder, "Rental order inserted successfully");
                    }
                    else
                    {
                        response.IsSuccess = false;
                        response.Message = "Error in created rentalOrder Process!";
                        return response;
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
        }
        private decimal CalculateItemSubTotal(RentalOrderItems item)
        {
            int rentalDays = Math.Max((item.RentalEndDate - item.RentalStartDate).Days, 1);
            return (decimal)(item.UnitPrice * item.Quantity * rentalDays);
        }
        private void SetOrderUserDetails(RentalOrder order, User user, ShipmentDetail shipmentDetail)
        {
            order.Address = shipmentDetail.Address;
            order.FullName = shipmentDetail.FullName;
            order.Email = shipmentDetail.Email;
            order.ContactPhone = shipmentDetail.PhoneNumber;
            order.UserId = user.Id;
        }
        public async Task<ResponseDTO<RentalOrderVM>> UpdateRentalOrderAsync(int orderId, RentalOrderUM rentalOrderUM)
        {
            var response = new ResponseDTO<RentalOrderVM>();
            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    var toUpdate = await _unitOfWork.RentalOrderRepository.GetObjectAsync(o => o.Id == orderId);

                    /*if (toUpdate == null)
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

                    await transaction.CommitAsync();*/


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
        private RentalOrder CreateParentRentalOrder(RentalOrderCM rentalOrderCM)
        {
            return new RentalOrder()
            {
                RentalOrderCode = _methodHelper.GenerateOrderCode(),

                Address = rentalOrderCM.Address,
                FullName = rentalOrderCM.FullName,
                Gender = rentalOrderCM.Gender,
                Email = rentalOrderCM.Email,
                ContactPhone = rentalOrderCM.ContactPhone,

                DeliveryMethod = rentalOrderCM.DeliveryMethod,
                DateOfReceipt = rentalOrderCM.DateOfReceipt ?? DateTime.Now.AddDays(3),

                CreatedAt = DateTime.Now,
                Note = rentalOrderCM.Note,
                OrderStatus = (int?)OrderStatus.PENDING,
                PaymentStatus = (int)PaymentStatus.IsWating,
            };
        }
        private RentalOrder CreateChildRentalOrder(RentalOrderCM rentalOrderCM, string parentCode)
        {
            return new RentalOrder()
            {
                RentalOrderCode = _methodHelper.GenerateOrderCode(),
                ParentOrderCode = parentCode,

                Address = rentalOrderCM.Address,
                FullName = rentalOrderCM.FullName,
                Gender = rentalOrderCM.Gender,
                Email = rentalOrderCM.Email,
                ContactPhone = rentalOrderCM.ContactPhone,

                DeliveryMethod = rentalOrderCM.DeliveryMethod,
                DateOfReceipt = rentalOrderCM.DateOfReceipt ?? DateTime.Now.AddDays(3),
                
                CreatedAt = DateTime.Now,
                Note = rentalOrderCM.Note,
                OrderStatus = (int?)OrderStatus.PENDING,
                PaymentStatus = (int)PaymentStatus.IsWating,
            };
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

        private async Task<bool> UpdateStock(RentalOrderItems item, int branchId, bool isReturningStock)
        {
            var warehouse = await _unitOfWork.WarehouseRepository
                .GetObjectAsync(w => w.ProductId == item.ProductId && w.BranchId == branchId);

            if (warehouse == null || warehouse.TotalQuantity < item.Quantity || warehouse.AvailableQuantity < item.Quantity)
            {
                return false;
            }

            // Cập nhật kho dựa trên loại yêu cầu
            int quantityAdjustment = isReturningStock ? item.Quantity.Value : -item.Quantity.Value;
            warehouse.AvailableQuantity += quantityAdjustment;
            warehouse.TotalQuantity += quantityAdjustment;

            await _unitOfWork.WarehouseRepository.UpdateAsync(warehouse);
            return true;
        }
        private async Task<bool> FindAndReduceQuantityInWarehouse(RentalOrderItems item, int branchId)
        {
            var warehouse = await _unitOfWork.WarehouseRepository
                .GetObjectAsync(w => w.ProductId == item.ProductId && w.BranchId == branchId);

            if (warehouse == null || warehouse.TotalQuantity < item.Quantity || warehouse.AvailableQuantity < item.Quantity)
            {
                return false;
            }

            // Cập nhật kho dựa trên loại yêu cầu
            warehouse.AvailableQuantity -= item.Quantity;

            await _unitOfWork.WarehouseRepository.UpdateAsync(warehouse);
            return true;
        }
    }
}
