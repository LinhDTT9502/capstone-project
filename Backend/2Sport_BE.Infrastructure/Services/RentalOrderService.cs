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
using _2Sport_BE.Services;
using Microsoft.IdentityModel.Tokens;


namespace _2Sport_BE.Infrastructure.Services
{
    public interface IRentalOrderService
    {
        Task<ResponseDTO<List<RentalOrderVM>>> GetAllRentalOrderAsync();
        Task<ResponseDTO<RentalOrderVM>> GetRentalOrderByIdAsync(int orderId);
        Task<ResponseDTO<List<RentalOrderVM>>> GetRentalOrderByParentCodeAsync(string parentCode);

        Task<ResponseDTO<List<RentalOrderVM>>> GetOrdersByUserIdAsync(int userId);
        Task<ResponseDTO<RentalOrderVM>> GetRentalOrderByOrderCodeAsync(string orderCode);
        Task<ResponseDTO<List<RentalOrderVM>>> GetRentalOrdersByStatusAsync(int? orderStatus, int? paymentStatus);
        Task<ResponseDTO<RentalOrderVM>> CreateRentalOrderAsync(RentalOrderCM rentalOrderCM);
        Task<ResponseDTO<RentalOrderVM>> UpdateRentalOrderAsync(int orderId, RentalOrderUM rentalOrderUM);
        Task<ResponseDTO<int>> ChangeStatusRentalOrderAsync(int orderId, int status);
        Task<ResponseDTO<int>> DeleteRentalOrderAsync(int rentalOrderId);
        Task<ResponseDTO<int>> CancelRentalOrderAsync(int orderId);
        Task<ResponseDTO<RentalOrderVM>> ProcessExtendRentalPeriod(ExtendRentalModel extendRentalModel);
        Task<ResponseDTO<RentalOrderVM>> ReturnOrder(ParentOrderReturnModel rentalInfor);

        Task<RentalOrder> FindRentalOrderByOrderCode(string orderCode);
        Task<RentalOrder> FindRentalOrderById(int orderId);
        Task<int> UpdaterRentalOrder(RentalOrder rentalOrder);
        Task<bool> UpdatePaymentStatus(string orderCode, int paymentStatus);
        Task CheckRentalOrdersForExpiration();
        //Task ProcessLateFees();
    }

    public class RentalOrderService : IRentalOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMethodHelper _methodHelper;
        private readonly IWarehouseService _warehouseService;
        private readonly ICustomerService _customerService;
        private readonly IMapper _mapper;
        private readonly INotificationService _notificationService;
        private readonly IDeliveryMethodService _deliveryMethodService;
        private readonly IMailService _mailService;
        public RentalOrderService(
            IUnitOfWork unitOfWork,
            IMethodHelper methodHelper,
            IWarehouseService warehouseService,
            ICustomerService customerService,
            IMapper mapper,
            INotificationService notificationService,
            IDeliveryMethodService deliveryMethodService,
            IMailService mailService)
        {
            _unitOfWork = unitOfWork;
            _methodHelper = methodHelper;
            _warehouseService = warehouseService;
            _customerService = customerService;
            _mapper = mapper;
            _deliveryMethodService = deliveryMethodService;
            _mailService = mailService;
            _notificationService = notificationService;
        }

        private int CalculateTotalRentalDays(RentalOrder order)
        {
            if (order.RentalEndDate.HasValue)
            {
                return (order.RentalEndDate.Value - order.RentalStartDate.Value).Days;
            }
            return order.RentalDays;
        }
        private decimal CalculateTotalAmount(decimal? subTotal = null,
                                            decimal? transportFee = null,
                                            decimal? lateFee = null,
                                            decimal? damageFee = null,
                                            decimal? depositAmount = null)
        {
            return (decimal)(subTotal + transportFee + lateFee + damageFee - depositAmount);
        }
        private void AssignRentalDates(DateTime startDate, DateTime endDate, RentalOrder rentalOrder)
        {
            rentalOrder.RentalStartDate = startDate;
            rentalOrder.RentalEndDate = endDate;
            rentalOrder.RentalDays = CalculateTotalRentalDays(rentalOrder);
        }
        private void AssignRentalProduct(Product product, RentalOrder rentalOrder, int quantity)
        {
            rentalOrder.ProductId = product.Id;
            rentalOrder.ProductName = product.ProductName;
            rentalOrder.RentPrice = product.RentPrice;
            rentalOrder.ImgAvatarPath = product.ImgAvatarPath;
            rentalOrder.Quantity = quantity;
        }
        private void AssignRentalCost(RentalOrder rentalOrder, decimal subTotal)
        {
            rentalOrder.SubTotal = subTotal;
            rentalOrder.TranSportFee = 0;
            rentalOrder.TotalAmount = CalculateTotalAmount(rentalOrder.SubTotal, rentalOrder.TranSportFee);
            rentalOrder.TotalAmount = (decimal)(rentalOrder.SubTotal + rentalOrder.TranSportFee);
        }
        public async Task CheckRentalOrdersForExpiration()
        {
            var expiringOrders = await _unitOfWork.RentalOrderRepository.GetAsync(o => o.RentalEndDate >= DateTime.Now.Date.AddDays(1) && o.ParentOrderCode == null);

            foreach (var order in expiringOrders)
            {
                await _notificationService.SendRentalOrderExpirationNotificationAsync(order.UserId.ToString(), order.RentalOrderCode, (DateTime)order.RentalEndDate);
                await _mailService.SendRentalOrderReminder(order ,order.Email);
            }
        }

        /*public async Task ProcessLateFees()
        {
            var lateOrders = await _unitOfWork.RentalOrderRepository.GetAsync(o =>  DateTime.Now.Date > o.RentalEndDate && o.ParentOrderCode == null);
            foreach (var order in lateOrders)
            {
                var daysLate = (DateTime.Now - order.ExpectedReturnDate).Days;
                if (daysLate > 0)
                {
                    order.LateFee = daysLate * _policy.DailyLateFee;
                    order.Status = "Trễ hạn";
                }

                _rentalOrderRepository.Update(order);
            }

            await _unitOfWork.SaveChangesAsync();
        }*/

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
                    if (rentalOrderCM.rentalOrderItemCMs.Count == 0)
                        return GenerateErrorResponse($"List of items are empty");

                    string DeliveryMethod = rentalOrderCM.DeliveryMethod;
                    if (string.IsNullOrEmpty(DeliveryMethod) || !_deliveryMethodService.IsValidMethod(DeliveryMethod))
                        return GenerateErrorResponse("Delivery Method is invalid");

                    if (DeliveryMethod.Equals("STORE_PICKUP") && !rentalOrderCM.BranchId.HasValue)
                        return GenerateErrorResponse("Delivery Method is invalid");

                    if(rentalOrderCM.rentalOrderItemCMs.Count == 1) // Create only 1 order
                    {
                        var rentalOrder = MapRentalOrderCmToRentalOrder(rentalOrderCM);

                        if (user != null && shipmentDetail != null)
                        {
                            SetOrderUserDetails(rentalOrder, user, shipmentDetail);
                        }

                        decimal subTotal = 0;
                        foreach (var item in rentalOrderCM.rentalOrderItemCMs)
                        {
                            if (!_methodHelper.CheckValidOfRentalDate(item.RentalStartDate, item.RentalEndDate, rentalOrderCM.DateOfReceipt.Value))
                                return GenerateErrorResponse("Rental period or Date of receipt are invalid");

                            AssignRentalDates(item.RentalStartDate, item.RentalEndDate, rentalOrder);

                            var product = await _unitOfWork.ProductRepository.GetObjectAsync(p => p.Id == item.ProductId);
                            if (product == null) return GenerateErrorResponse($"The product with id {item.ProductId} are not found");

                            
                            AssignRentalProduct(product, rentalOrder, item.Quantity.Value);

                            var branch = await _unitOfWork.BranchRepository.GetObjectAsync(b => b.Id == rentalOrderCM.BranchId);
                            if (branch != null)
                            {
                                rentalOrder.BranchId = branch.Id;
                                rentalOrder.BranchName = branch.BranchName;
                                
                                var reduceInWarehouse = await FindAndReduceQuantityInWarehouse(item, branch.Id);
                                if (!reduceInWarehouse)
                                {
                                    await transaction.RollbackAsync();
                                    return GenerateErrorResponse($"Failed to update stock for product {item.ProductName}");
                                }
                            }

                            subTotal += (decimal)(item.RentPrice * item.Quantity * rentalOrder.RentalDays);
                        }
   
                        AssignRentalCost(rentalOrder, subTotal);
                        await _unitOfWork.RentalOrderRepository.InsertAsync(rentalOrder);
                        await _unitOfWork.SaveChanges();

                        //Send notifications to Coordinator
                        await _notificationService.NotifyForCreatingNewOrderAsync(rentalOrder.RentalOrderCode);
                        await _mailService.SendRentalOrderInformationToCustomer(rentalOrder, null, rentalOrder.Email);


                        response = GenerateSuccessResponse(rentalOrder, null, "Rental order created successfully");
                        await transaction.CommitAsync();
                    }
                    else if( rentalOrderCM.rentalOrderItemCMs.Count > 1 )//Parent order and child order
                    {
                        var parentRentalOrder = MapRentalOrderCmToRentalOrder(rentalOrderCM);
                        if (user != null && shipmentDetail != null) SetOrderUserDetails(parentRentalOrder, user, shipmentDetail);

                        parentRentalOrder.BranchId = rentalOrderCM.BranchId.HasValue ? rentalOrderCM.BranchId : null;
                        await _unitOfWork.RentalOrderRepository.InsertAsync(parentRentalOrder);

                        decimal childSubTotal = 0;
                        decimal parentSubtotal = 0;
                        decimal parentTransportFee = 0;
                        foreach (var item in rentalOrderCM.rentalOrderItemCMs)
                        {
                            var childRentalOrder = MapRentalOrderCmToRentalOrder(rentalOrderCM);
                            childRentalOrder.ParentOrderCode = parentRentalOrder.RentalOrderCode;

                            if (user != null && shipmentDetail != null) SetOrderUserDetails(childRentalOrder, user, shipmentDetail);

                            if (!_methodHelper.CheckValidOfRentalDate(item.RentalStartDate, item.RentalEndDate, rentalOrderCM.DateOfReceipt.Value))
                                return GenerateErrorResponse("Rental period or Date of receipt are invalid");

                            AssignRentalDates(item.RentalStartDate, item.RentalEndDate, childRentalOrder);

                            var branch = await _unitOfWork.BranchRepository.GetObjectAsync(b => b.Id == rentalOrderCM.BranchId);
                            if (branch != null)
                            {
                                childRentalOrder.BranchId = branch.Id;
                                childRentalOrder.BranchName = branch.BranchName;

                                var reduceInWarehouse = await FindAndReduceQuantityInWarehouse(item, branch.Id);
                                if (!reduceInWarehouse)
                                {
                                    await transaction.RollbackAsync();
                                    return GenerateErrorResponse($"Failed to update stock for product {item.ProductName}");
                                }
                            }

                            var product = await _unitOfWork.ProductRepository.GetObjectAsync(p => p.Id == item.ProductId);
                            if (product == null) return GenerateErrorResponse($"The product with id {item.ProductId} are not found");

                            AssignRentalProduct(product, childRentalOrder, item.Quantity.Value);

                            AssignRentalCost(childRentalOrder, childSubTotal);
                            await _unitOfWork.RentalOrderRepository.InsertAsync(childRentalOrder);

                            parentTransportFee += (decimal)childRentalOrder.TranSportFee;
                            parentSubtotal += childRentalOrder.TotalAmount;
                        }

                        parentRentalOrder.SubTotal = parentSubtotal;
                        parentRentalOrder.TranSportFee = parentTransportFee;
                        parentRentalOrder.TotalAmount = (decimal)(parentRentalOrder.SubTotal + parentRentalOrder.TranSportFee);
                        await _unitOfWork.RentalOrderRepository.UpdateAsync(parentRentalOrder);

                        //Send notifications to Coordinator
                        await _notificationService.NotifyForCreatingNewOrderAsync(parentRentalOrder.RentalOrderCode);

                        var listChild = await _unitOfWork.RentalOrderRepository.GetAsync(o => o.ParentOrderCode.Equals(parentRentalOrder.RentalOrderCode));
                        await _mailService.SendRentalOrderInformationToCustomer(parentRentalOrder, listChild.ToList(), parentRentalOrder.Email);
                        
                        response = GenerateSuccessResponse(parentRentalOrder, listChild.ToList(), "Rental order inserted successfully");
                        await transaction.CommitAsync();
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
        private void MapProductToRentalOrder(RentalOrder rentalOrder,Product product)
        {
            rentalOrder.ProductId = product.Id;
            rentalOrder.ProductName = product.ProductName;
            rentalOrder.ImgAvatarPath = product.ImgAvatarPath;
            rentalOrder.RentPrice = product.RentPrice;
            rentalOrder.ProductCode = product.ProductCode;
        }
        private void MapRentalOrderUmToRentalOrder(RentalOrder parentOrder, RentalOrderUM rentalOrderUM)
        {
            parentOrder.Email = rentalOrderUM.Email;
            parentOrder.FullName = rentalOrderUM.FullName;
            parentOrder.Address = rentalOrderUM.Address;
            parentOrder.ContactPhone = rentalOrderUM.ContactPhone;
            parentOrder.Gender = rentalOrderUM.Gender;

            parentOrder.UpdatedAt = DateTime.UtcNow;
            parentOrder.OrderStatus = rentalOrderUM.OrderStatus;
            parentOrder.PaymentStatus = rentalOrderUM.PaymentStatus;
            parentOrder.Note = rentalOrderUM.Note;

            parentOrder.DeliveryMethod = rentalOrderUM.DeliveryMethod;

        }
        private void MapRentalOrderItemsUmToRentalOrderItems(RentalOrderItemUM item, RentalOrder rentalOrder, Product product)
        {
            rentalOrder.ProductId = item.ProductId;
            rentalOrder.ProductCode = item.ProductCode;
            rentalOrder.ProductName = item.ProductName;
            rentalOrder.RentPrice = item.RentPrice;
            rentalOrder.Quantity = item.Quantity;
            rentalOrder.RentalStartDate = item.RentalStartDate;
            rentalOrder.RentalEndDate = item.RentalEndDate;
            rentalOrder.ImgAvatarPath = !string.IsNullOrEmpty(item.ImgAvatarPath) ? item.ImgAvatarPath : product.ImgAvatarPath;
            int rentalDays = Math.Max((item.RentalEndDate - item.RentalStartDate).Days, 1);
            rentalOrder.SubTotal += item.RentPrice * item.Quantity * rentalDays;
        }
        private void MapTransportFeeAndCalculateTotalAmount(RentalOrder rentalOrder, RentalOrderUM rentalOrderUM)
        {
            rentalOrder.TranSportFee = rentalOrderUM.TranSportFee;
            rentalOrder.TotalAmount = (decimal)(rentalOrder.TranSportFee + rentalOrder.SubTotal);
        }
        public async Task<ResponseDTO<RentalOrderVM>> UpdateRentalOrderAsync(int orderId, RentalOrderUM rentalOrderUM)
        {
            var response = new ResponseDTO<RentalOrderVM>();
            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    var parentOrder = await _unitOfWork.RentalOrderRepository.GetObjectAsync(o => o.Id == orderId);
                    if (parentOrder == null) return GenerateErrorResponse($"Rental Order with id {orderId} not found!");

                    var branch = await _unitOfWork.BranchRepository.GetObjectAsync(o => o.Id == rentalOrderUM.BranchId);
                    if (branch is null) return GenerateErrorResponse($"The branch with id {orderId} not found!");

                    MapRentalOrderUmToRentalOrder(parentOrder, rentalOrderUM);

                    parentOrder.BranchId = branch.Id;
                    parentOrder.BranchName = branch.BranchName;

                    if (rentalOrderUM.rentalOrderItemUMs.Count == 1)
                    {
                        foreach (var item in rentalOrderUM.rentalOrderItemUMs)
                        {
                            if (!parentOrder.ProductCode.Equals(item.ProductCode)) 
                                return GenerateErrorResponse($"The product {item.ProductId} - {item.ProductName} are not match with the product of items in Child Orders!");
                            var product = await _unitOfWork.ProductRepository
                                .GetObjectAsync(p => p.Id == item.ProductId && p.ProductCode.Equals(item.ProductCode));
                            if (product is null)
                                return GenerateErrorResponse($"The product {item.ProductId} - {item.ProductName} are not found");

                            MapRentalOrderItemsUmToRentalOrderItems(item, parentOrder, product);
                        }
                        MapTransportFeeAndCalculateTotalAmount(parentOrder, rentalOrderUM);
                        await _unitOfWork.RentalOrderRepository.UpdateAsync(parentOrder);
                    }
                    else //count >= 2 => update child orders
                    {
                        var childOrders = await _unitOfWork.RentalOrderRepository.GetAndIncludeAsync(o => o.ParentOrderCode.Equals(parentOrder));
                        if (rentalOrderUM.rentalOrderItemUMs.Count != childOrders.ToList().Count || rentalOrderUM.rentalOrderItemUMs.Count == 0)
                            return GenerateErrorResponse($"The number of items are not match with the number of items in Child Orders!");
                        
                        foreach (var item in rentalOrderUM.rentalOrderItemUMs)
                        {
                            var childOrder = childOrders.FirstOrDefault(p => p.ProductCode == item.ProductCode);
                            if (childOrder == null) return GenerateErrorResponse($"The product {item.ProductCode} - {item.ProductName} are not match with the product of items in Child Orders!");
                            else
                            {
                                var product = await _unitOfWork.ProductRepository
                                    .GetObjectAsync(p => p.Id == item.ProductId && p.ProductCode.Equals(item.ProductCode));

                                if (product is null) return GenerateErrorResponse($"The product {item.ProductId} - {item.ProductName} are not found");
                                
                                MapRentalOrderItemsUmToRentalOrderItems(item, childOrder, product);
                                parentOrder.SubTotal += childOrder.SubTotal;

                                await _unitOfWork.RentalOrderRepository.UpdateAsync(childOrder);
                            }
                            MapTransportFeeAndCalculateTotalAmount(parentOrder, rentalOrderUM);
                            await _unitOfWork.RentalOrderRepository.UpdateAsync(parentOrder);
                        }
                    }

                    await _unitOfWork.RentalOrderRepository.UpdateAsync(parentOrder);
                    await transaction.CommitAsync();
                    response = GenerateSuccessResponse(parentOrder,null, "Rental order updated successfully");
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
        private RentalOrder MapRentalOrderCmToRentalOrder(RentalOrderCM rentalOrderCM)
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
                DepositStatus = (int)DepositStatus.Not_Paid,
                DepositAmount = 0
            };
        }
        private ResponseDTO<RentalOrderVM> GenerateSuccessResponse(RentalOrder order, List<RentalOrder>? listChild, string messagge)
        {
            var result = _mapper.Map<RentalOrderVM>(order);
            result.OrderStatus = Enum.GetName((OrderStatus)order.OrderStatus);
            result.PaymentStatus = Enum.GetName((PaymentStatus)order.PaymentStatus);
            result.PaymentMethod = order.PaymentMethodId.HasValue
                               ? Enum.GetName(typeof(OrderMethods), order.PaymentMethodId.Value)
                               : "Unknown PaymentMethod";
            
            result.Id = order.Id;
            if (listChild == null || !listChild.Any())
            {
                listChild = new List<RentalOrder>();
            }
            else
            {
                result.listChild = _mapper.Map<List<RentalOrderVM>>(listChild);

            }
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
                    response = GenerateSuccessResponse(rentalOrder, null, "Rental order found");
                }
                else
                {
                    response = GenerateErrorResponse($"Rental order with Order Code = {orderCode} not found");
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

        private decimal CalculateExtensionCost(int rentalDates, RentalOrder rentalOrder)
        {
            return (decimal)(rentalOrder.RentPrice * rentalOrder.Quantity * rentalDates);
        }
        public async Task<ResponseDTO<RentalOrderVM>> ProcessExtendRentalPeriod(ExtendRentalModel extendRental)
        {
            var response = new ResponseDTO<RentalOrderVM>();
            try
            {
                var rentalOrder = _unitOfWork.RentalOrderRepository.FindObject(o => o.Id == extendRental.ChildOrderId);
                if(rentalOrder is null) return GenerateErrorResponse("The order are not found");

                    rentalOrder.IsExtended = true;
                    rentalOrder.ExtensionDays = extendRental.ExtensionDays;
                    rentalOrder.RentalEndDate = rentalOrder.RentalEndDate.Value.AddDays(extendRental.ExtensionDays);
                    rentalOrder.ExtensionCost = CalculateExtensionCost(extendRental.ExtensionDays, rentalOrder);
                    rentalOrder.TotalAmount = rentalOrder.TotalAmount + rentalOrder.ExtensionCost.Value;

                    await _unitOfWork.RentalOrderRepository.UpdateAsync(rentalOrder);

                    response = GenerateSuccessResponse(rentalOrder, null, "Created Application for extension");
            }
            catch (Exception ex)
            {
                response = GenerateErrorResponse(ex.Message);
            }
            return response;
        }

        private async Task<bool> UpdateStock(RentalOrderItemUM item, int branchId, bool isReturningStock)
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
        private async Task<bool> FindAndReduceQuantityInWarehouse(RentalOrderItemCM item, int branchId)
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
        private void SetOrderUserDetails(RentalOrder order, User user, ShipmentDetail shipmentDetail)
        {
            order.Address = shipmentDetail.Address;
            order.FullName = shipmentDetail.FullName;
            order.Email = shipmentDetail.Email;
            order.ContactPhone = shipmentDetail.PhoneNumber;
            order.UserId = user.Id;
        }

        public async Task<ResponseDTO<RentalOrderVM>> ReturnOrder(ParentOrderReturnModel rentalInfor)
        {
            var parentOrder = await _unitOfWork.RentalOrderRepository.GetObjectAsync(o => o.Id.Equals(rentalInfor.ParentOrderId));
            if(parentOrder is null) return GenerateErrorResponse($"The Rental Order - {rentalInfor.ParentOrderId} are not found!");
            
            decimal? totalLateFee = 0, totalDamageFee = 0;
            if(rentalInfor.ChildOrders.Count() > 0)
            {
                foreach (var item in rentalInfor.ChildOrders)
                {
                    var childOrder = _unitOfWork.RentalOrderRepository
                            .FindObject(o => o.ParentOrderCode.Equals(parentOrder.RentalOrderCode) && o.Id == item.OrderId);

                    if (childOrder != null)
                    {
                        childOrder.ReturnDate = item.ReturnDate;
                        childOrder.LateFee = item.LateFee;
                        childOrder.DamageFee = item.DamageFee;
                        childOrder.IsRestocked = item.IsRestocked;
                        childOrder.IsInspected = item.IsInspected;
                        await _unitOfWork.RentalOrderRepository.UpdateAsync(childOrder);

                        totalLateFee += childOrder.LateFee;
                        totalDamageFee += childOrder.DamageFee;
                    }

                }
            }

            parentOrder.LateFee = rentalInfor.TotalLateFee != null ? rentalInfor.TotalLateFee : totalLateFee;
            parentOrder.DamageFee = rentalInfor.TotalDamageFee != null ? rentalInfor.TotalDamageFee : totalDamageFee;
            parentOrder.UpdatedAt = DateTime.Now;
            parentOrder.IsInspected = rentalInfor.IsInspected;
            parentOrder.IsRestocked = rentalInfor.IsRestocked;

            /*if (order.IsExtendRentalOrder.Value)
            {
                var extendOrder = await _unitOfWork.RentalOrderRepository.GetObjectAsync(o => o.ParentOrderCode.Equals(order.RentalOrderCode));
                if(extendOrder != null)
                {
                    order.
                }
            }*/
            var childOrders = await _unitOfWork.RentalOrderRepository.GetAsync(co => co.ParentOrderCode.Equals(parentOrder.ParentOrderCode));
            var result = _mapper.Map<RentalOrderVM>(parentOrder);
            var response = GenerateSuccessResponse(parentOrder, childOrders.ToList(), "Updated Succesfully");
            return response;
        }

        public async Task<ResponseDTO<List<RentalOrderVM>>> GetRentalOrderByParentCodeAsync(string  parentCode)
        {
            var response = new ResponseDTO<List<RentalOrderVM>>();

            try
            {
                var rentalOrder = await _unitOfWork.RentalOrderRepository.GetAsync(
                    r => r.ParentOrderCode.Equals(parentCode)
                );

                if (rentalOrder != null)
                {
                    var result = _mapper.Map<List<RentalOrderVM>>(rentalOrder);

                    response.IsSuccess = true;
                    response.Message = "Rental order found";
                    response.Data = result;
                }
                else
                {
                    response.IsSuccess = false;
                    response.Message = $"Rental order with ParentCode = {parentCode} not found";
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = $"Error: {ex.Message}";
            }

            return response;
        }

        public async Task<RentalOrder> FindRentalOrderByOrderCode(string orderCode)
        {
                var rentalOrder = await _unitOfWork.RentalOrderRepository.GetObjectAsync(
                    r => r.RentalOrderCode == orderCode
                );
            return rentalOrder;
        }
        public async Task<RentalOrder> FindRentalOrderById(int orderId)
        {
            var rentalOrder = await _unitOfWork.RentalOrderRepository.GetObjectAsync(
                   r => r.Id == orderId
               );
            return rentalOrder;
        }
        public async Task<int> UpdaterRentalOrder(RentalOrder rentalOrder)
        {
            await _unitOfWork.RentalOrderRepository.UpdateAsync(rentalOrder);
            return 1;
        }

        public async Task<bool> UpdatePaymentStatus(string orderCode, int paymentStatus)
        {
            bool response = false;

            try
            {
                var rentalOrder = await _unitOfWork.RentalOrderRepository.GetObjectAsync(o => o.RentalOrderCode.Equals(orderCode));
                if (rentalOrder == null)
                    response = false;
                else
                {
                    rentalOrder.PaymentStatus = paymentStatus;

                    await _unitOfWork.RentalOrderRepository.UpdateAsync(rentalOrder);

                    response = true;
                }
            }
            catch (Exception ex)
            {
                response = false;
            }
            return response;
        }
    }
}
