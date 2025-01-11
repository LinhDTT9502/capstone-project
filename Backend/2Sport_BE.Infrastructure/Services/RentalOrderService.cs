using _2Sport_BE.Repository.Interfaces;
using _2Sport_BE.Repository.Models;
using _2Sport_BE.Infrastructure.DTOs;
using _2Sport_BE.Infrastructure.Enums;
using _2Sport_BE.Infrastructure.Helpers;
using AutoMapper;
using _2Sport_BE.Service.Enums;
using _2Sport_BE.Services;
using _2Sport_BE.Service.Helpers;
using Pipelines.Sockets.Unofficial.Arenas;
using System.ComponentModel.DataAnnotations;
using ValidationResult = _2Sport_BE.Infrastructure.Helpers.ValidationResult;
using StackExchange.Redis;
using System.Runtime.Intrinsics.X86;
using _2Sport_BE.Service.Services;


namespace _2Sport_BE.Infrastructure.Services
{
    public interface IRentalOrderService
    {
        Task RefundRentalOrder();
        Task CheckRentalOrdersForExpiration();//Background Jobs
        Task CheckPendingOrderForget();

        Task<ResponseDTO<List<RentalOrderVM>>> GetAllRentalOrderAsync();
        Task<ResponseDTO<List<RentalOrderVM>>> GetOrdersByUserIdAsync(int userId);
        Task<ResponseDTO<List<RentalOrderVM>>> GetRentalOrdersByBranchAsync(int branchId);
        Task<ResponseDTO<List<RentalOrderVM>>> GetRentalOrderByParentCodeAsync(string parentCode);
        Task<ResponseDTO<List<RentalOrderVM>>> GetRentalOrdersByStatusAsync(int? orderStatus, int? paymentStatus);
        Task<ResponseDTO<List<RentalOrderVM>>> GetRentalOrdersByBranchAndExtensionStatus(int branchId, int extensionStatus);

        Task<ResponseDTO<RentalOrderVM>> GetRentalOrderDetailsByIdAsync(int orderId);
        Task<ResponseDTO<RentalOrderVM>> GetRentalOrderByOrderCodeAsync(string orderCode);

        Task<ResponseDTO<RentalOrderVM>> CreateRentalOrderAsync(RentalOrderCM rentalOrderCM);
        Task<ResponseDTO<RentalOrderVM>> UpdateRentalOrderAsync(int orderId, RentalOrderUM rentalOrderUM);
        Task<ResponseDTO<int>> UpdateRentalOrderStatusAsync(int orderId, int status);
        Task<ResponseDTO<int>> UpdateBranchForRentalOrder(int orderId, int branchId);
        Task<int> UpdaterRentalOrder(RentalOrder rentalOrder);
        Task<ResponseDTO<int>> UpdateRentalPaymentStatus(int orderId, int paymentStatus);
        Task<ResponseDTO<int>> UpdateRentalDepositStatus(int orderId, int depositStatus);
        Task<ResponseDTO<int>> DeleteRentalOrderAsync(int rentalOrderId);

        Task<ResponseDTO<int>> CancelRentalOrderAsync(int orderId, string reason);
        Task<RentalOrder> FindRentalOrderByOrderCode(string orderCode);
        Task<RentalOrder> FindRentalOrderById(int orderId);

        Task<ResponseDTO<RentalOrderVM>> ApproveRentalOrderAsync(int orderId);
        Task<ResponseDTO<RentalOrderVM>> RejectRentalOrderAsync(int orderId);

        Task<ResponseDTO<int>> RequestExtensionAsync(ExtensionRequestModel extensionRequest);
        Task<ResponseDTO<int>> ApproveExtensionAsync(string rentalOrderCode);
        Task<ResponseDTO<int>> RejectExtensionAsync(string rentalOrderCode, string rejectionReason);

        Task<ResponseDTO<int>> RequestReturnAsync(ReturnRequestModel returnRequestModel);
        Task<ResponseDTO<int>> ProcessReturnOrder(ReturnRequestModelUM rentalInfor);
    }

    public class RentalOrderService : IRentalOrderService
    {
        #region ServiceInjection
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMethodHelper _methodHelper;
        private readonly IWarehouseService _warehouseService;
        private readonly ICustomerService _customerService;
        private readonly IMapper _mapper;
        private readonly INotificationService _notificationService;
        private readonly IDeliveryMethodService _deliveryMethodService;
        private readonly IMailService _mailService;
        private readonly IBranchService _branchService;

        #endregion
        public RentalOrderService(
            IUnitOfWork unitOfWork,
            IMethodHelper methodHelper,
            IWarehouseService warehouseService,
            ICustomerService customerService,
            IMapper mapper,
            INotificationService notificationService,
            IDeliveryMethodService deliveryMethodService,
            IBranchService  branchService,
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
            _branchService = branchService;
        }
        #region BackgroundJobServices
        public async Task CheckRentalOrdersForExpiration()
        {
            var expiringOrders = await _unitOfWork.RentalOrderRepository.GetAsync(o => o.RentalEndDate >= DateTime.Now.Date.AddDays(1) && o.ParentOrderCode == null);

            foreach (var order in expiringOrders)
            {
                await _notificationService.SendRentalOrderExpirationNotificationAsync(order.UserId.ToString(), order.RentalOrderCode, (DateTime)order.RentalEndDate);
                await _mailService.SendRentalOrderReminder(order, order.Email);
            }
        }
        public async Task CheckPendingOrderForget()
        {
            var pendingOrders = await _unitOfWork.RentalOrderRepository.GetAsync(o => o.CreatedAt.Value.AddHours(1) == DateTime.Now.Date && o.ParentOrderCode == null);
            foreach (var order in pendingOrders)
            {
                await _notificationService.NotifyForPendingOrderAsync(order.RentalOrderCode, true, order.CreatedAt.Value, order.BranchId);
            }
        }
        public async Task RefundRentalOrder()
        {
            var orders = await _unitOfWork.RentalOrderRepository
                .GetAndIncludeAsync(o => o.OrderStatus == (int)OrderStatus.CANCELLED && (o.PaymentStatus == (int)PaymentStatus.PAID || o.DepositAmount > 0),
                                        new string[] { "RefundRequests" });
            if (orders.Any())
            {
                foreach (var order in orders)
                {
                    var existedRefund = order.RefundRequests;
                    if (existedRefund == null || !existedRefund.Any())
                    {
                        RefundRequest newRefund = new RefundRequest
                        {
                            RentalOrderCode = order.RentalOrderCode,
                            RentalOrderID = order.Id,
                            BranchId = order.BranchId != null ? order.BranchId.Value : 0,
                            IsAgreementAccepted = true,
                            Reason = "Đơn đã hủy, chưa hoàn lại tiền cọc",
                            Status = RefundStatus.Pending.ToString(),
                            Notes = string.Empty,
                            CreatedAt = _methodHelper.GetTimeInUtcPlus7()
                        };
                        await _unitOfWork.RefundRequestRepository.InsertAsync(newRefund);

                    }
                }
            }
        }
        #endregion
        //Get list
        public async Task<ResponseDTO<List<RentalOrderVM>>> GetAllRentalOrderAsync()
        {
            var response = new ResponseDTO<List<RentalOrderVM>>();

            try
            {
                var rentalOrders = await _unitOfWork.RentalOrderRepository.GetAllAsync();

                if (rentalOrders != null && rentalOrders.Any())
                {
                    var resultList = rentalOrders.Select(rentalOrder =>
                    {
                        var result = _mapper.Map<RentalOrderVM>(rentalOrder);
                        MapToRentalOrderVM(rentalOrder, result);
                        return result;
                    }).ToList();

                    response.IsSuccess = true;
                    response.Message = "Query successful";
                    response.Data = resultList;
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

        public async Task<ResponseDTO<List<RentalOrderVM>>> GetOrdersByUserIdAsync(int userId)
        {
            var response = new ResponseDTO<List<RentalOrderVM>>();

            try
            {
                var orders = await _unitOfWork.RentalOrderRepository
                    .GetAsync(o => o.UserId == userId);

                if (orders != null && orders.Any())
                {
                    var resultList = new List<RentalOrderVM>();

                    foreach (var rentalOrder in orders)
                    {
                        var result = _mapper.Map<RentalOrderVM>(rentalOrder);
                        MapToRentalOrderVM(rentalOrder, result);

                        // Truy vấn child orders
                        var listChild = await _unitOfWork.RentalOrderRepository.GetAsync(r => r.ParentOrderCode == rentalOrder.RentalOrderCode);

                        result.childOrders = _mapper.Map<List<RentalOrderVM>>(listChild);

                        resultList.Add(result);
                    }

                    response.IsSuccess = true;
                    response.Message = "Orders retrieved successfully";
                    response.Data = resultList.ToList();
                }
                else
                {
                    response.IsSuccess = true;
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

        public async Task<ResponseDTO<List<RentalOrderVM>>> GetRentalOrdersByBranchAsync(int branchId)
        {
            var response = new ResponseDTO<List<RentalOrderVM>>();

            try
            {
                var rentalOrders = await _unitOfWork.RentalOrderRepository.GetAsync(
                    r => r.BranchId == branchId && r.ParentOrderCode == null
                );

                if (rentalOrders != null && rentalOrders.Any())
                {
                    var resultList = rentalOrders.Select(rentalOrder =>
                    {
                        var result = _mapper.Map<RentalOrderVM>(rentalOrder);
                        MapToRentalOrderVM(rentalOrder, result);
                        return result;
                    }).ToList();

                    response.IsSuccess = true;
                    response.Message = "Rental orders retrieved successfully";
                    response.Data = resultList;
                }
                else
                {
                    response.IsSuccess = true;
                    response.Message = "Rental orders are not found";
                    response.Data = new List<RentalOrderVM>();
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = $"Error: {ex.Message}";

            }
            return response;
        }

        public async Task<ResponseDTO<List<RentalOrderVM>>> GetRentalOrderByParentCodeAsync(string parentCode)
        {
            var response = new ResponseDTO<List<RentalOrderVM>>();

            try
            {
                var rentalOrders = await _unitOfWork.RentalOrderRepository.GetAsync(
                    r => r.ParentOrderCode.Equals(parentCode)
                );

                if (rentalOrders != null)
                {

                    var resultList = rentalOrders.Select(rentalOrder =>
                    {
                        var result = _mapper.Map<RentalOrderVM>(rentalOrder);
                        MapToRentalOrderVM(rentalOrder, result);
                        return result;
                    }).ToList();

                    response.IsSuccess = true;
                    response.Message = "Rental orders retrieved successfully";
                    response.Data = resultList;
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

        public async Task<ResponseDTO<List<RentalOrderVM>>> GetRentalOrdersByStatusAsync(int? orderStatus, int? paymentStatus)
        {
            var response = new ResponseDTO<List<RentalOrderVM>>();

            try
            {
                var orders = await _unitOfWork.RentalOrderRepository.GetAsync(o => o.ParentOrderCode == null);
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
                    var resultList = orders.Select(rentalOrder =>
                    {
                        var result = _mapper.Map<RentalOrderVM>(rentalOrder);
                        MapToRentalOrderVM(rentalOrder, result);
                        return result;
                    }).ToList();

                    response.IsSuccess = true;
                    response.Message = "Rental orders retrieved successfully";
                    response.Data = resultList;
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = $"Error: {ex.Message}";
            }

            return response;
        }

        public async Task<ResponseDTO<List<RentalOrderVM>>> GetRentalOrdersByBranchAndExtensionStatus(int branchId, int extensionStatus)
        {
            var response = new ResponseDTO<List<RentalOrderVM>>();

            try
            {
                var orders = await _unitOfWork.RentalOrderRepository
                    .GetAsync(o => o.BranchId == branchId && o.ExtensionStatus.Value == extensionStatus);

                if (orders != null && orders.Any())
                {
                    var resultList = orders.Select(rentalOrder =>
                    {
                        var result = _mapper.Map<RentalOrderVM>(rentalOrder);
                        MapToRentalOrderVM(rentalOrder, result);
                        return result;
                    }).ToList();

                    response.IsSuccess = true;
                    response.Message = "Orders retrieved successfully";
                    response.Data = resultList;
                }
                else
                {
                    response.IsSuccess = true;
                    response.Message = $"No orders found";
                    response.Data = new List<RentalOrderVM>();
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = $"Error: {ex.Message}";
            }

            return response;
        }


        //Get an object
        public async Task<ResponseDTO<RentalOrderVM>> GetRentalOrderDetailsByIdAsync(int rentalOrderId)
        {
            var response = new ResponseDTO<RentalOrderVM>();

            try
            {
                var rentalOrder = await _unitOfWork.RentalOrderRepository.GetObjectAsync(
                    r => r.Id == rentalOrderId
                );
                var listChild = await _unitOfWork.RentalOrderRepository.GetAsync(r => r.ParentOrderCode == rentalOrder.RentalOrderCode);
                if (rentalOrder != null)
                {
                    if (listChild.Count() > 0) response = GenerateSuccessResponse(rentalOrder, listChild.ToList(), "Query Successfully");
                    else response = GenerateSuccessResponse(rentalOrder, null, "Query Successfully");
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

                var listChild = await _unitOfWork.RentalOrderRepository.GetAsync(r => r.ParentOrderCode == rentalOrder.RentalOrderCode);
                if (rentalOrder != null)
                {
                    if (listChild.Count() > 0) response = GenerateSuccessResponse(rentalOrder, listChild.ToList(), "Query Successfully");
                    else response = GenerateSuccessResponse(rentalOrder, null, "Query Successfully");
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


        public async Task<ResponseDTO<RentalOrderVM>> CreateRentalOrderAsync(RentalOrderCM rentalOrderCM)
        {
            var response = new ResponseDTO<RentalOrderVM>();
            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    User user = null;
                    if (rentalOrderCM.CustomerInformation.UserId.HasValue && rentalOrderCM.CustomerInformation.UserId != 0)
                    {
                        user = await _unitOfWork.UserRepository
                                        .GetObjectAsync(u => u.Id == rentalOrderCM.CustomerInformation.UserId);
                        if (user == null) return GenerateErrorResponse($"User with Id = {rentalOrderCM.CustomerInformation.UserId} is not found!");
                    }

                    if (rentalOrderCM.ProductInformations.Count == 0)
                        return GenerateErrorResponse($"List of items are empty");

                    string DeliveryMethod = rentalOrderCM.DeliveryMethod;
                    if (string.IsNullOrEmpty(DeliveryMethod) || !_deliveryMethodService.IsValidMethod(DeliveryMethod))
                        return GenerateErrorResponse("Delivery Method is invalid");

                    if (DeliveryMethod.Equals("STORE_PICKUP") && !rentalOrderCM.BranchId.HasValue) //để store lấy thì phải có branchId
                        return GenerateErrorResponse("Delivery Method is invalid");

                    var rentalOrder = new RentalOrder();
                    AssignCustomerInformation(rentalOrder, rentalOrderCM.CustomerInformation);
                    rentalOrder.RentalOrderCode = _methodHelper.GenerateOrderCode();
                    rentalOrder.Note = rentalOrderCM.Note;
                    rentalOrder.CreatedAt = DateTime.Now;
                    rentalOrder.DeliveryMethod = DeliveryMethod;
                    rentalOrder.OrderStatus = (int)RentalOrderStatus.PENDING;
                    //ParentCode For Child Orders
                    string parentOrderCode = rentalOrder.RentalOrderCode;

                    if (rentalOrderCM.ProductInformations.Count == 1) // Create only 1 order
                    {
                        foreach (var item in rentalOrderCM.ProductInformations)
                        {

                            var product = await _unitOfWork.ProductRepository.GetObjectAsync(p => p.Id == item.ProductId && p.ProductCode == item.ProductCode);
                            if (product == null) return GenerateErrorResponse($"The product with id {item.ProductId} are not found");

                            if (!_methodHelper.CheckValidOfRentalDate(item.RentalDates.RentalStartDate, item.RentalDates.RentalEndDate, item.RentalDates.DateOfReceipt))
                                return GenerateErrorResponse("Rental period or Date of receipt are invalid");

                            AssignRentalProduct(item, rentalOrder);
                            AssignRentalDates(rentalOrder, item.RentalDates);

                            var branch = rentalOrderCM.BranchId != null ? await _unitOfWork.BranchRepository.GetObjectAsync(b => b.Id == rentalOrderCM.BranchId) : null;
                            if (branch != null)
                            {
                                AssignBranchInformation(rentalOrder, branch);

                                var reduceInWarehouse = await UpdateStock(item, branch.Id, false);
                                if (!reduceInWarehouse)
                                {
                                    await transaction.RollbackAsync();
                                    return GenerateErrorResponse($"Failed to update stock for product {item.ProductName}");
                                }
                            }

                            AssignRentalCost(rentalOrder, item);
                        }
                        await _unitOfWork.RentalOrderRepository.InsertAsync(rentalOrder);

                        await _unitOfWork.SaveChanges();

                        await _notificationService.NotifyForCreatingNewOrderAsync(rentalOrder.RentalOrderCode, true, rentalOrder.BranchId);
                        await _mailService.SendRentalOrderInformationToCustomer(rentalOrder, null, rentalOrder.Email);
                        response = GenerateSuccessResponse(rentalOrder, null, "Rental order created successfully");
                        await transaction.CommitAsync();
                    }
                    else if (rentalOrderCM.ProductInformations.Count > 1)//Parent order and child order
                    {
                        var branch = rentalOrderCM.BranchId != null ? await _unitOfWork.BranchRepository.GetObjectAsync(b => b.Id == rentalOrderCM.BranchId) : null;
                        if (branch != null)
                        {
                            AssignBranchInformation(rentalOrder, branch);
                        }

                        decimal? parentSubtotal = 0;
                        decimal? parentTransportFee = 0;
                        var childOrders = new List<RentalOrder>();
                        for (var i = 0; i < rentalOrderCM.ProductInformations.Count; i++)
                        {
                            var itemCM = rentalOrderCM.ProductInformations[i];

                            var childRentalOrder = new RentalOrder();
                            AssignCustomerInformation(childRentalOrder, rentalOrderCM.CustomerInformation);
                            childRentalOrder.ParentOrderCode = parentOrderCode;
                            childRentalOrder.RentalOrderCode = _methodHelper.GenerateOrderCode();
                            childRentalOrder.CreatedAt = DateTime.Now;
                            childRentalOrder.OrderStatus = (int)RentalOrderStatus.PENDING;
                            childRentalOrder.DeliveryMethod = rentalOrderCM.DeliveryMethod;
                            var product = await _unitOfWork.ProductRepository.GetObjectAsync(p => p.Id == itemCM.ProductId);
                            if (product == null) return GenerateErrorResponse($"The product with id {itemCM.ProductId} are not found");

                            if (!_methodHelper.CheckValidOfRentalDate(itemCM.RentalDates.RentalStartDate, itemCM.RentalDates.RentalEndDate, itemCM.RentalDates.DateOfReceipt))
                                return GenerateErrorResponse("Rental period or Date of receipt are invalid");

                            AssignRentalProduct(itemCM, childRentalOrder);
                            AssignRentalDates(childRentalOrder, itemCM.RentalDates);

                            if (branch != null)
                            {
                                AssignBranchInformation(childRentalOrder, branch);

                                var reduceInWarehouse = await UpdateStock(itemCM, rentalOrderCM.BranchId.Value, false);
                                if (!reduceInWarehouse)
                                {
                                    await transaction.RollbackAsync();
                                    return GenerateErrorResponse($"Failed to update stock for product {itemCM.ProductName}");
                                }
                            }
                            AssignRentalCost(childRentalOrder, itemCM);
                            parentSubtotal += childRentalOrder.SubTotal.Value;
                            parentTransportFee += childRentalOrder.TranSportFee.Value;

                            childOrders.Add(childRentalOrder);
                            await _unitOfWork.RentalOrderRepository.InsertAsync(childRentalOrder);
                        }

                        rentalOrder.SubTotal = parentSubtotal ?? childOrders.Sum(o => o.SubTotal);
                        rentalOrder.TranSportFee = parentTransportFee ?? childOrders.Sum(o => o.TranSportFee);
                        rentalOrder.TotalAmount = CalculateTotalAmount(rentalOrder);

                        await _unitOfWork.RentalOrderRepository.InsertAsync(rentalOrder);

                        //Send notifications to Coordinator
                        var listChild = await _unitOfWork.RentalOrderRepository.GetAsync(o => o.ParentOrderCode.Equals(rentalOrder.RentalOrderCode));
                        await _notificationService.NotifyForCreatingNewOrderAsync(rentalOrder.RentalOrderCode, true, rentalOrder.BranchId);
                        await _mailService.SendRentalOrderInformationToCustomer(rentalOrder, listChild.ToList(), rentalOrder.Email);

                        response = GenerateSuccessResponse(rentalOrder, listChild.ToList(), "Rental order inserted successfully");
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

        private async Task<ResponseDTO<RentalOrderVM>> UpdateSingleProductAsync(RentalOrder parentOrder, ProductInformation product, Branch branch)
        {
            // Check product matching
            if (parentOrder.ProductCode != product.ProductCode)
                return GenerateErrorResponse($"Product code mismatch: {product.ProductCode}");

            // Update stock and product details
            var productInWarehouse = await _warehouseService.GetWarehouseByProductIdAndBranchId(parentOrder.ProductId.Value, branch.Id);

            if (productInWarehouse == null)
                return GenerateErrorResponse($"Product with Id = {product.ProductId} not found in warehouse");

            var isUpdatedStock = await UpdateStock(productInWarehouse.First(), parentOrder.Quantity.Value, product.Quantity.Value);
            if (!isUpdatedStock)
                return GenerateErrorResponse($"Failed to update stock for product Id = {product.ProductId}");

            AssignRentalProduct(product, parentOrder);
            AssignRentalDates(parentOrder, product.RentalDates);
            AssignRentalCost(parentOrder, product);

            return GenerateSuccessResponse(null, null, "Single product updated successfully");
        }

        public async Task<ResponseDTO<RentalOrderVM>> UpdateRentalOrderAsync(int orderId, RentalOrderUM rentalOrderUM)
        {
            var response = new ResponseDTO<RentalOrderVM>();
            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    // 1. Validate parent order
                    var parentOrder = await ValidateRentalOrderAsync(orderId);
                    if (parentOrder == null) return GenerateErrorResponse($"Rental Order with id {orderId} not found!");

                    // 2. Validate branch
                    var branch = await _branchService.GetBranchById(rentalOrderUM.BranchId);
                    if (branch == null) return GenerateErrorResponse($"Branch with id {rentalOrderUM.BranchId} not found!");

                    // 3. Update parent order details
                    UpdateParentOrderDetails(parentOrder, rentalOrderUM, branch);

                    // 4. Process product information (parent or child orders)
                    var childList = new List<RentalOrder>();
                    if (rentalOrderUM.ProductInformations.Count == 1) 
                    {
                        // Update single product
                        var result = await UpdateSingleProductAsync(parentOrder, rentalOrderUM.ProductInformations.First(), branch);
                        if (!result.IsSuccess) return result;
                    }
                    else //Cập nhật đơn thuê nhiều hơn 1 sản phẩm => cập nhật các hóa đơn con
                    {
                        var childOrders = await _unitOfWork.RentalOrderRepository.GetAndIncludeAsync(o => o.ParentOrderCode.Equals(parentOrder.RentalOrderCode));
                        if (rentalOrderUM.ProductInformations.Count != childOrders.ToList().Count || rentalOrderUM.ProductInformations.Count == 0)
                            return GenerateErrorResponse($"The number of items are not match with the number of items in Child Orders!");

                        for (var i = 0; i < rentalOrderUM.ProductInformations.Count; i++)
                        {
                            var itemUm = rentalOrderUM.ProductInformations[i];
                            // Lấy lại existingChild trong mỗi vòng lặp để làm việc với đối tượng mới nhất
                            var existingChild = childOrders.ToList()[i];

                            if (existingChild == null)
                                return GenerateErrorResponse($"The product with code: {itemUm.ProductCode} - {itemUm.ProductName} is not found in the list of child orders");

                            // Kiểm tra nếu sản phẩm cũ không đổi (cập nhật quantity)
                            if (existingChild.ProductId == itemUm.ProductId)
                            {
                                var productInWarehouse = _unitOfWork.WarehouseRepository.FindObject(w => w.ProductId == existingChild.ProductId.Value && w.BranchId == branch.Id);
                                if (productInWarehouse == null)
                                    return GenerateErrorResponse($"Product with Id = {itemUm.ProductId} not found");

                                var isUpdatedStock = await UpdateStock(productInWarehouse, existingChild.Quantity.Value, itemUm.Quantity.Value);
                                if (!isUpdatedStock)
                                {
                                    await transaction.RollbackAsync();
                                    return GenerateErrorResponse($"Product with Id = {itemUm.ProductId} failed to update stock");
                                }
                            }
                            else // Nếu thay đổi sản phẩm trong order
                            {
                                if (parentOrder.BranchId != null)
                                {
                                    // Cập nhật lại kho cho sản phẩm cũ (nếu cần)
                                    var oldProductInWarehouse = _unitOfWork.WarehouseRepository
                                        .FindObject(_ => _.ProductId == existingChild.ProductId && _.BranchId == parentOrder.BranchId);

                                    if (oldProductInWarehouse != null)
                                    {
                                        oldProductInWarehouse.AvailableQuantity += existingChild.Quantity;
                                        await _unitOfWork.WarehouseRepository.UpdateAsync(oldProductInWarehouse);
                                    }
                                }

                                // Kiểm tra sản phẩm mới trong kho
                                var newProductInWarehouse = _unitOfWork.WarehouseRepository
                                    .FindObject(_ => _.ProductId == itemUm.ProductId && _.BranchId == branch.Id);

                                if (newProductInWarehouse == null)
                                    return GenerateErrorResponse($"Product with Id = {itemUm.ProductId} not found in warehouse");

                                if (newProductInWarehouse.AvailableQuantity < itemUm.Quantity || newProductInWarehouse.TotalQuantity < itemUm.Quantity)
                                    return GenerateErrorResponse($"Product with Id = {itemUm.ProductId} not enough to update stock");

                                newProductInWarehouse.AvailableQuantity -= itemUm.Quantity;
                                await _unitOfWork.WarehouseRepository.UpdateAsync(newProductInWarehouse);
                            }
                            AssignRentalProduct(itemUm, existingChild);
                            AssignRentalDates(existingChild, itemUm.RentalDates);
                            AssignRentalCost(existingChild, itemUm);

                            childList.Add(existingChild);
                            // Lưu lại thay đổi cho existingChild
                            existingChild.UpdatedAt = DateTime.Now;
                            await _unitOfWork.RentalOrderRepository.UpdateAsync(existingChild);
                        }

                    }

                    if (childList.Any())
                    {
                        decimal? subTotalOfParent = rentalOrderUM.ParentSubTotal ?? childList.Sum(_ => _.SubTotal);
                        decimal? transportFee = rentalOrderUM.ParentTranSportFee ?? childList.Sum(_ => _.TranSportFee);
                        parentOrder.SubTotal = subTotalOfParent;
                        parentOrder.TranSportFee = transportFee;
                        parentOrder.TotalAmount = (decimal)(subTotalOfParent + transportFee);
                    }

                    await _unitOfWork.RentalOrderRepository.UpdateAsync(parentOrder);
                    await transaction.CommitAsync();

                    var listChilds = await _unitOfWork.RentalOrderRepository.GetAsync(o => o.ParentOrderCode.Equals(parentOrder.RentalOrderCode));

                    response = GenerateSuccessResponse(parentOrder, null, "Rental order updated successfully");
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
                var toDelete = await _unitOfWork.RentalOrderRepository.GetObjectAsync(_ => _.Id == rentalOrderId, "RefundRequests");
                if (toDelete == null)
                {
                    response.IsSuccess = false;
                    response.Message = $"SaleOrder with id = {rentalOrderId} is not found!";
                    response.Data = 0;
                    return response;
                }
                if (toDelete.RefundRequests.Count > 0)
                {
                    response.IsSuccess = false;
                    response.Message = $"SaleOrder with id = {rentalOrderId} has refund requests. Deleted fail!";
                    response.Data = 0;
                    return response;
                }
                if (toDelete != null)
                {
                    var childs = await _unitOfWork.RentalOrderRepository.GetAsync(_ => _.ParentOrderCode == toDelete.RentalOrderCode);
                    if (childs != null || !childs.Any())
                    {
                        foreach (var item in childs)
                        {
                            await _unitOfWork.RentalOrderRepository.DeleteAsync(item);
                        }
                    }
                    await _unitOfWork.RentalOrderRepository.DeleteAsync(toDelete);

                    response.IsSuccess = true;
                    response.Message = "Deleted Rental Order Successfully";
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

        public async Task<ResponseDTO<int>> UpdateRentalOrderStatusAsync(int orderId, int newStatus)
        {
            try
            {
                var rentalOrder = await _unitOfWork.RentalOrderRepository.GetObjectAsync(o => o.Id == orderId);
                if (rentalOrder is null)
                {
                    return new ResponseDTO<int>
                    {
                        IsSuccess = false,
                        Message = "Đơn hàng không tồn tại.",
                        Data = 0
                    };
                }

                var validationResult = ValidateStatusTransition(rentalOrder, newStatus);
                if (!validationResult.IsValid)
                {
                    return new ResponseDTO<int>
                    {
                        IsSuccess = false,
                        Message = validationResult.ErrorMessage,
                        Data = 0
                    };
                }

                rentalOrder.OrderStatus = newStatus;
                rentalOrder.UpdatedAt = _methodHelper.GetTimeInUtcPlus7();
                var childs = await _unitOfWork.RentalOrderRepository.GetAsync(_ => _.ParentOrderCode == rentalOrder.RentalOrderCode);

                if (childs != null && childs.Any())
                {
                    foreach (var child in childs)
                    {
                        child.OrderStatus = newStatus;
                        child.UpdatedAt = _methodHelper.GetTimeInUtcPlus7();
                    }
                    await _unitOfWork.RentalOrderRepository.UpdateRangeAsync(childs.ToList());
                }

                await _unitOfWork.RentalOrderRepository.UpdateAsync(rentalOrder);

                if (rentalOrder.OrderStatus == (int)OrderStatus.COMPLETED)
                {
                    var loyaltyUpdateResponse = await _customerService.UpdateLoyaltyPointsRental(rentalOrder.Id);
                }
                return new ResponseDTO<int>
                {
                    IsSuccess = true,
                    Message = "Cập nhật trạng thái đơn hàng thành công.",
                    Data = 1
                };
            }
            catch (Exception ex)
            {
                return new ResponseDTO<int>
                {
                    IsSuccess = false,
                    Message = $"Lỗi hệ thống: {ex.Message}",
                    Data = 0
                };
            }
        }

        public async Task<ResponseDTO<int>> CancelRentalOrderAsync(int orderId, string reason)
        {
            var response = new ResponseDTO<int>();
            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    var order = await _unitOfWork.RentalOrderRepository.GetObjectAsync(o => o.Id == orderId);

                    if (order is null)
                    {
                        response.IsSuccess = false;
                        response.Message = "Không tìm thấy đơn hàng!";
                        response.Data = 0;
                        return response;
                    }

                    if (order.OrderStatus != (int)RentalOrderStatus.PENDING)
                    {
                        response.IsSuccess = false;
                        response.Message = "Đơn hàng không thể hủy ở trạng thái hiện tại!";
                        response.Data = 0;
                        return response;
                    }

                    order.OrderStatus = (int)RentalOrderStatus.CANCELED;
                    order.Reason = reason;
                    order.UpdatedAt = _methodHelper.GetTimeInUtcPlus7();

                    await _unitOfWork.RentalOrderRepository.UpdateAsync(order);

                    var childs = await _unitOfWork.RentalOrderRepository.GetAsync(_ => _.ParentOrderCode == order.RentalOrderCode);

                    if (childs != null && childs.Any())
                    {
                        foreach (var child in childs)
                        {
                            child.OrderStatus = (int)RentalOrderStatus.CANCELED;
                            child.Reason = reason;
                            order.UpdatedAt = _methodHelper.GetTimeInUtcPlus7();

                            await _unitOfWork.RentalOrderRepository.UpdateAsync(child);
                        }
                    }

                    if (order.BranchId != null)
                    {
                        var isRestocked = await _warehouseService.UpdateRentalStock(order, isDelivered: false, isCanceled: true);
                        if (!isRestocked)
                        {
                            response.IsSuccess = false;
                            response.Message = "Có lỗi trong quá trình cập nhật số lượng!";
                            response.Data = 0;
                            return response;
                        }
                    }

                    await _notificationService.NotifyToGroupAsync(
                        $"Đơn hàng T-{order.RentalOrderCode} đã bị hủy bởi khách hàng",
                        order.BranchId
                    );

                    await transaction.CommitAsync();

                    response.IsSuccess = true;
                    response.Message = "Đơn hàng đã được hủy thành công!";
                    response.Data = 1;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();

                    response.IsSuccess = false;
                    response.Message = $"Đã xảy ra lỗi: {ex.Message}";
                    response.Data = 0;
                }
            }
            return response;
        }

        public async Task<ResponseDTO<int>> UpdateBranchForRentalOrder(int orderId, int branchId)
        {
            var response = new ResponseDTO<int>();
            try
            {
                var branch = await _unitOfWork.BranchRepository.GetObjectAsync(b => b.Id == branchId);
                if (branch is null)
                {
                    response.IsSuccess = false;
                    response.Message = $"Branch with id = {branchId} is not found!";
                    response.Data = 0;
                }
                var rentalOrder = await _unitOfWork.RentalOrderRepository.GetObjectAsync(o => o.Id == orderId);
                if (rentalOrder == null)
                {
                    response.IsSuccess = false;
                    response.Message = $"SaleOrder with id = {orderId} is not found!";
                    response.Data = 0;
                }
                else
                {
                    AssignBranchInformation(rentalOrder, branch);
                    var childs = await _unitOfWork.RentalOrderRepository.GetAndIncludeAsync(_ => _.ParentOrderCode == rentalOrder.RentalOrderCode);
                    if(childs != null)
                    {
                        foreach (var child in childs)
                        {
                            AssignBranchInformation(child, branch);
                            child.UpdatedAt = null;
                        }
                        await _unitOfWork.RentalOrderRepository.UpdateRangeAsync(childs.ToList());
                    }
                    rentalOrder.UpdatedAt = null;
                    await _unitOfWork.RentalOrderRepository.UpdateAsync(rentalOrder);

                    response.IsSuccess = true;
                    response.Message = $"BranchId of SaleOrder with id = {orderId} updated successfully";
                    response.Data = 1;
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<ResponseDTO<int>> UpdateRentalPaymentStatus(int orderId, int paymentStatus)
        {
            try
            {
                var rentalOrder = await _unitOfWork.RentalOrderRepository.GetObjectAsync(o => o.Id == orderId);
                if (rentalOrder is null)
                {
                    return new ResponseDTO<int>
                    {
                        IsSuccess = false,
                        Message = "Đơn hàng không tồn tại.",
                        Data = 0
                    };
                }

                rentalOrder.PaymentStatus = paymentStatus;
                await _unitOfWork.RentalOrderRepository.UpdateAsync(rentalOrder);

                return new ResponseDTO<int>
                {
                    IsSuccess = true,
                    Message = "Cập nhật trạng thái thanh toán thành công.",
                    Data = 1
                };
            }
            catch (Exception ex)
            {
                return new ResponseDTO<int>
                {
                    IsSuccess = false,
                    Message = $"Lỗi hệ thống: {ex.Message}",
                    Data = 0
                };
            }
        }

        public async Task<ResponseDTO<int>> UpdateRentalDepositStatus(int orderId, int depositStatus)
        {
            try
            {
                var rentalOrder = await _unitOfWork.RentalOrderRepository.GetObjectAsync(o => o.Id == orderId);
                if (rentalOrder is null)
                {
                    return new ResponseDTO<int>
                    {
                        IsSuccess = false,
                        Message = "Đơn hàng không tồn tại.",
                        Data = 0
                    };
                }

                rentalOrder.DepositStatus = depositStatus;
                await _unitOfWork.RentalOrderRepository.UpdateAsync(rentalOrder);

                return new ResponseDTO<int>
                {
                    IsSuccess = true,
                    Message = "Cập nhật trạng thái đặt cọc thành công.",
                    Data = 1
                };
            }
            catch (Exception ex)
            {
                return new ResponseDTO<int>
                {
                    IsSuccess = false,
                    Message = $"Lỗi hệ thống: {ex.Message}",
                    Data = 0
                };
            }
        }

        public async Task<int> UpdaterRentalOrder(RentalOrder rentalOrder)
        {
            await _unitOfWork.RentalOrderRepository.UpdateAsync(rentalOrder);
            return 1;
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


        public async Task<ResponseDTO<RentalOrderVM>> ApproveRentalOrderAsync(int orderId)
        {
            var response = new ResponseDTO<RentalOrderVM>();
            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    var rentalOrder = await _unitOfWork.RentalOrderRepository.GetObjectAsync(o => o.Id == orderId);
                    var childOrder = await _unitOfWork.RentalOrderRepository.GetAsync(_ => _.ParentOrderCode.Equals(rentalOrder.RentalOrderCode));
                    if (rentalOrder == null)
                    {
                        return GenerateErrorResponse($"SaleOrder with id = {orderId} is not found!");
                    }
                    else
                    {
                        if (rentalOrder.OrderStatus >= (int)RentalOrderStatus.CONFIRMED)
                            return GenerateErrorResponse($"Sales order status with id = {orderId} has been previously confirmed!");

                        rentalOrder.OrderStatus = (int)RentalOrderStatus.PENDING;
                        rentalOrder.UpdatedAt = DateTime.Now;
                        await _unitOfWork.RentalOrderRepository.UpdateAsync(rentalOrder);

                        await transaction.CommitAsync();
                        var result = _mapper.Map<RentalOrderVM>(rentalOrder);
                        response = GenerateSuccessResponse(rentalOrder, null, "Approved Succesfully");
                    }
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return GenerateErrorResponse(ex.Message);
                }
            }
            return response;
        }

        public async Task<ResponseDTO<RentalOrderVM>> RejectRentalOrderAsync(int orderId)
        {
            var response = new ResponseDTO<RentalOrderVM>();
            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    var rentalOrder = await _unitOfWork.RentalOrderRepository.GetObjectAsync(o => o.Id == orderId);
                    if (rentalOrder == null)
                    {
                        return GenerateErrorResponse($"SaleOrder with id = {orderId} is not found!");
                    }
                    else
                    {
                        var oldBranch = rentalOrder.BranchId.Value;
                        if (rentalOrder.OrderStatus >= (int)RentalOrderStatus.CONFIRMED)
                            return GenerateErrorResponse($"Sales order status with id = {orderId} has been previously confirmed!");

                        var childOrder = await _unitOfWork.RentalOrderRepository.GetAsync(_ => _.ParentOrderCode.Equals(rentalOrder.RentalOrderCode));

                        rentalOrder.OrderStatus = (int)RentalOrderStatus.PENDING;
                        rentalOrder.BranchId = null;
                        rentalOrder.BranchName = null;
                        rentalOrder.UpdatedAt = null;

                        if (childOrder.Any())
                        {
                            foreach (var item in childOrder.ToList())
                            {
                                item.OrderStatus = (int)RentalOrderStatus.PENDING;
                                item.BranchId = null;
                                item.BranchName = null;
                                item.UpdatedAt = null;
                            }
                            await _unitOfWork.RentalOrderRepository.UpdateRangeAsync(childOrder.ToList());
                        }

                        await _notificationService.NotifyForRejectOrderAsync(rentalOrder.RentalOrderCode, oldBranch);
                        await _unitOfWork.RentalOrderRepository.UpdateAsync(rentalOrder);

                        await transaction.CommitAsync();
                        var result = _mapper.Map<RentalOrderVM>(rentalOrder);
                        response = GenerateSuccessResponse(rentalOrder, null, "Rejected Succesfully");
                    }
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return GenerateErrorResponse(ex.Message);
                }
            }
            return response;
        }


        public async Task<ResponseDTO<int>> RequestExtensionAsync(ExtensionRequestModel extensionRequest)
        {
            var parent = await _unitOfWork.RentalOrderRepository.GetObjectAsync(r => r.Id == extensionRequest.ParentOrderId);
            if (parent == null)
                return new ResponseDTO<int>
                {
                    IsSuccess = true,
                    Message = "Order not found."
                };

            if (parent.OrderStatus < (int)RentalOrderStatus.DELIVERED)
                return new ResponseDTO<int>
                {
                    IsSuccess = false,
                    Message = "Đơn hàng chưa đủ điều kiện để tiến hành gia hạn."
                };

            var extensionDays = extensionRequest.ExtensionDays;
            if (extensionRequest.ChildOrderId != null)
            {
                var child = await _unitOfWork.RentalOrderRepository.GetObjectAsync(r => r.Id == extensionRequest.ChildOrderId);
                if (child.ExtensionStatus == (int)ExtensionRequestStatus.PENDING)
                    return new ResponseDTO<int>
                    {
                        IsSuccess = false,
                        Message = "Đơn hàng đã được yêu cầu gia hạn trước đó. Vui lòng đợi nhân viên hỗ trợ sau"
                    };

                if (child.OrderStatus < (int)RentalOrderStatus.DELIVERED)
                    return new ResponseDTO<int>
                    {
                        IsSuccess = false,
                        Message = "Đơn hàng chưa đủ điều kiện để tiến hành gia hạn."
                    };

                child.ExtensionStatus = (int)ExtensionRequestStatus.PENDING;
                child.ExtensionDays = extensionDays;

                await _unitOfWork.RentalOrderRepository.UpdateAsync(child);
                await _unitOfWork.SaveChanges();

                await _notificationService.NotifyForExtensionRequestAsync(parent.RentalOrderCode, child.ParentOrderCode, child.BranchId ?? parent.BranchId);

                return new ResponseDTO<int>
                {
                    IsSuccess = true,
                    Message = "Extension request submitted successfully.",
                    Data = 1
                };
            }
            else
            {
                if (parent.ExtensionStatus == (int)ExtensionRequestStatus.PENDING)
                    return new ResponseDTO<int>
                    {
                        IsSuccess = false,
                        Message = "Đơn hàng đã được yêu cầu gia hạn trước đó. Vui lòng đợi nhân viên hỗ trợ sau"
                    };

                parent.ExtensionStatus = (int)ExtensionRequestStatus.PENDING;
                parent.ExtensionDays = extensionRequest.ExtensionDays;

                await _unitOfWork.RentalOrderRepository.UpdateAsync(parent);
                await _unitOfWork.SaveChanges();

                await _notificationService.NotifyForExtensionRequestAsync(parent.RentalOrderCode, null, parent.BranchId);

                return new ResponseDTO<int>
                {
                    IsSuccess = true,
                    Message = "Extension request submitted successfully.",
                    Data = 1
                };
            }
        }

        public async Task<ResponseDTO<int>> ApproveExtensionAsync(string rentalOrderCode)
        {
            try
            {
                var rentalOrder = await _unitOfWork.RentalOrderRepository
                    .GetObjectAsync(r => r.RentalOrderCode == rentalOrderCode);

                if (rentalOrder == null)
                    return new ResponseDTO<int>
                    {
                        IsSuccess = true,
                        Message = "Order not found."
                    };

                if (rentalOrder.ExtensionStatus != (int)ExtensionRequestStatus.PENDING)
                    return new ResponseDTO<int>
                    {
                        IsSuccess = true,
                        Message = "No pending extension request found."
                    };

                var orderExtensionCost = rentalOrder.RentPrice * rentalOrder.ExtensionDays * rentalOrder.Quantity;

                var parentOrder = await _unitOfWork.RentalOrderRepository.GetObjectAsync(r => r.RentalOrderCode == rentalOrder.ParentOrderCode);
                if(parentOrder != null)
                {
                    parentOrder.OrderStatus = (int)RentalOrderStatus.EXTENSION_REQUESTED;
                    parentOrder.ExtensionCost += orderExtensionCost;
                    parentOrder.TotalAmount += parentOrder.ExtensionCost.Value;
                    parentOrder.UpdatedAt = _methodHelper.GetTimeInUtcPlus7();

                    await _unitOfWork.RentalOrderRepository.UpdateAsync(parentOrder);
                }

                rentalOrder.ExtensionStatus = (int)ExtensionRequestStatus.APPROVED;
                rentalOrder.OrderStatus = (int)RentalOrderStatus.EXTENSION_REQUESTED;
                rentalOrder.IsExtended = true;
                rentalOrder.ExtendedDueDate = rentalOrder.RentalEndDate.Value.AddDays(rentalOrder.ExtensionDays.Value);
                rentalOrder.ExtensionCost = orderExtensionCost;
                rentalOrder.UpdatedAt =_methodHelper.GetTimeInUtcPlus7();
                await _unitOfWork.RentalOrderRepository.UpdateAsync(rentalOrder);
                
                return new ResponseDTO<int>
                {
                    IsSuccess = true,
                    Message = "Extension request approved successfully.",
                    Data = 1
                };
            }
            catch (Exception ex)
            {
                return new ResponseDTO<int>
                {
                    IsSuccess = false,
                    Message = $"An error occurred while processing the extension request: {ex.Message}"
                };
            }
        }

        public async Task<ResponseDTO<int>> RejectExtensionAsync(string rentalOrderCode, string rejectionReason)
        {
            try
            {
                var rentalOrder = await _unitOfWork.RentalOrderRepository
                    .GetObjectAsync(r => r.RentalOrderCode == rentalOrderCode);

                if (rentalOrder == null)
                    return new ResponseDTO<int>
                    {
                        IsSuccess = false,
                        Message = "Order not found."
                    };

                if (rentalOrder.ExtensionStatus != (int)ExtensionRequestStatus.PENDING)
                    return new ResponseDTO<int>
                    {
                        IsSuccess = false,
                        Message = "No pending extension request found."
                    };

                rentalOrder.ExtensionStatus = (int)ExtensionRequestStatus.REJECTED;
                rentalOrder.IsExtended = false;

                rentalOrder.ExtendedDueDate = null;

                await _unitOfWork.RentalOrderRepository.UpdateAsync(rentalOrder);
                if (rentalOrder.UserId != null || rentalOrder.UserId.Value > 0)
                {
                    await _notificationService
                        .NotifyForRejectExtensionRequestAsync(rentalOrder.RentalOrderCode, rentalOrder.UserId.Value, rejectionReason);
                }

                return new ResponseDTO<int>
                {
                    IsSuccess = true,
                    Message = "Extension request rejected successfully.",
                    Data = 1
                };
            }
            catch (Exception ex)
            {
                return new ResponseDTO<int>
                {
                    IsSuccess = false,
                    Message = $"An error occurred while processing the extension rejection: {ex.Message}"
                };
            }
        }

        public ResponseDTO<RentalOrderVM> GenerateSuccessResponse(RentalOrder order, List<RentalOrder>? listChild, string messagge)
        {
            var result = _mapper.Map<RentalOrderVM>(order);
            result.DepositStatus = order.DepositStatus != null
               ? EnumDisplayHelper.GetEnumDescription<DepositStatus>(order.DepositStatus.Value)
               : "N/A";  // Giá trị mặc định nếu null

            result.OrderStatus = order.OrderStatus != null
                ? EnumDisplayHelper.GetEnumDescription<RentalOrderStatus>(order.OrderStatus.Value)
                : "N/A";

            result.PaymentStatus = order.PaymentStatus != null
                ? EnumDisplayHelper.GetEnumDescription<PaymentStatus>(order.PaymentStatus.Value)
                : "N/A";
            result.PaymentMethod = order.PaymentMethodId != null
                ? EnumDisplayHelper.GetEnumDescription<OrderMethods>(order.PaymentMethodId.Value)
                : "N/A";
            result.DeliveryMethod = _deliveryMethodService.GetDescription(order.DeliveryMethod);
            result.Id = order.Id;
            result.OrderStatusId = order.OrderStatus;
            if (listChild == null || !listChild.Any())
            {
                listChild = new List<RentalOrder>();
            }
            else
            {
                result.childOrders = _mapper.Map<List<RentalOrderVM>>(listChild);

            }
            return new ResponseDTO<RentalOrderVM>
            {
                IsSuccess = true,
                Message = messagge,
                Data = result
            };
        }

        public ResponseDTO<RentalOrderVM> GenerateErrorResponse(string message)
        {
            return new ResponseDTO<RentalOrderVM>()
            {
                IsSuccess = false,
                Message = message,
                Data = null
            };
        }

        private int CalculateTotalRentalDays(RentalOrder order)
        {
            if (order.RentalEndDate.HasValue)
            {
                return (order.RentalEndDate.Value - order.RentalStartDate.Value).Days;
            }
            return order.RentalDays;
        }

        private decimal CalculateTotalAmount(RentalOrder rentalOrder)
        {
            return (rentalOrder.SubTotal ?? 0)
                 + (rentalOrder.TranSportFee ?? 0)
                 + (rentalOrder.LateFee ?? 0)
                 + (rentalOrder.DamageFee ?? 0);
        }

        private void AssignRentalDates(RentalOrder rentalOrder, RentalDates rentalDates)
        {
            rentalOrder.DateOfReceipt = rentalDates.DateOfReceipt;
            rentalOrder.RentalStartDate = rentalDates.RentalStartDate;
            rentalOrder.RentalEndDate = rentalDates.RentalEndDate;
            rentalOrder.RentalDays = rentalDates.RentalDays ?? CalculateTotalRentalDays(rentalOrder);
        }
        private void AssignRentalProduct(ProductInformation productInformation, RentalOrder rentalOrder)
        {
            rentalOrder.ProductId = productInformation.ProductId;
            rentalOrder.ProductName = productInformation.ProductName;
            rentalOrder.ProductCode = productInformation.ProductCode;
            rentalOrder.Size = productInformation.Size;
            rentalOrder.Color = productInformation.Color;
            rentalOrder.Condition = productInformation.Condition;
            rentalOrder.RentPrice = productInformation.RentPrice;
            rentalOrder.ImgAvatarPath = productInformation.ImgAvatarPath;
            rentalOrder.Quantity = productInformation.Quantity;
        }
        private void AssignRentalCost(RentalOrder rentalOrder, ProductInformation productInformation)
        {
            rentalOrder.SubTotal = productInformation.RentalCosts.SubTotal ?? (decimal)(productInformation.RentPrice * productInformation.Quantity * productInformation.RentalDates.RentalDays);
            rentalOrder.TranSportFee = productInformation.RentalCosts.TranSportFee ?? 0;
            rentalOrder.TotalAmount = productInformation.RentalCosts.TotalAmount.Value != null ? productInformation.RentalCosts.TotalAmount.Value : CalculateTotalAmount(rentalOrder);
        }
        private void AssignCustomerInformation(RentalOrder rentalOrder, CustomerInformation customerInformation)
        {
            if (customerInformation == null) return;
            rentalOrder.UserId = customerInformation.UserId;
            rentalOrder.Email = customerInformation.Email;
            rentalOrder.Gender = customerInformation.Gender;
            rentalOrder.FullName = customerInformation.FullName;
            rentalOrder.ContactPhone = customerInformation.ContactPhone;
            rentalOrder.Address = customerInformation.Address;
        }
        private void AssignBranchInformation(RentalOrder rentalOrder, Branch branch)
        {
            rentalOrder.BranchId = branch.Id;
            rentalOrder.BranchName = branch.BranchName;
        }

        private async Task<bool> UpdateStock(ProductInformation productInformation, int branchId, bool isReturningStock)
        {
            var warehouse = await _unitOfWork.WarehouseRepository
                .GetObjectAsync(w => w.ProductId == productInformation.ProductId && w.BranchId == branchId);

            if (warehouse == null || warehouse.TotalQuantity < productInformation.Quantity || warehouse.AvailableQuantity < productInformation.Quantity)
            {
                return false;
            }

            // Cập nhật kho dựa trên loại yêu cầu
            int quantityAdjustment = isReturningStock ? productInformation.Quantity.Value : -productInformation.Quantity.Value;
            warehouse.AvailableQuantity += quantityAdjustment;
            //warehouse.TotalQuantity += quantityAdjustment;

            await _unitOfWork.WarehouseRepository.UpdateAsync(warehouse);
            return true;
        }
        private async Task<bool> UpdateStock(Warehouse warehouse, int oldQuantity, int newQuantity)
        {

            int quantityDifference = newQuantity - oldQuantity;
            int availableQuantity = warehouse.AvailableQuantity.Value;

            if (quantityDifference > 0)//newQuantity 2 > oldQuantity 1 = 1
            {
                // Kiểm tra xem kho có đủ hàng để trừ không
                if (quantityDifference <= availableQuantity)
                {
                    warehouse.AvailableQuantity -= quantityDifference;
                    await _warehouseService.UpdateWarehouseAsync(warehouse);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (quantityDifference < 0)//newQuantity 1 < oldQuantity 2 = -1
            {
                warehouse.AvailableQuantity -= quantityDifference;
                await _warehouseService.UpdateWarehouseAsync(warehouse);
                return true;
            }
            return true;
        }

        private void MapToRentalOrderVM(RentalOrder rentalOrder, RentalOrderVM rentalOrderVM)
        {
            rentalOrderVM.DepositStatus = rentalOrder.DepositStatus != null
                ? EnumDisplayHelper.GetEnumDescription<DepositStatus>(rentalOrder.DepositStatus.Value)
                : "N/A";  // Giá trị mặc định nếu null

            rentalOrderVM.OrderStatus = rentalOrder.OrderStatus != null
                ? EnumDisplayHelper.GetEnumDescription<RentalOrderStatus>(rentalOrder.OrderStatus.Value)
                : "N/A";

            rentalOrderVM.PaymentStatus = rentalOrder.PaymentStatus != null
                ? EnumDisplayHelper.GetEnumDescription<PaymentStatus>(rentalOrder.PaymentStatus.Value)
                : "N/A";
            rentalOrderVM.ExtensionStatus = rentalOrder.ExtensionStatus != null
               ? EnumDisplayHelper.GetEnumDescription<ExtensionRequestStatus>(rentalOrder.ExtensionStatus.Value)
               : "N/A";
            rentalOrderVM.PaymentMethod = rentalOrder.PaymentMethodId != null
               ? EnumDisplayHelper.GetEnumDescription<OrderMethods>(rentalOrder.PaymentMethodId.Value)
                : "N/A";
            rentalOrderVM.DeliveryMethod = _deliveryMethodService.GetDescription(rentalOrder.DeliveryMethod);
            rentalOrderVM.OrderStatusId = rentalOrder.OrderStatus;
        }

        private ValidationResult ValidateStatusTransition(RentalOrder rentalOrder, int newStatus)
        {
            var currentOrderStatus = rentalOrder.OrderStatus;
            var paymentStatus = rentalOrder.PaymentStatus;
            var depositStatus = rentalOrder.DepositStatus;
            var paymentMethod = rentalOrder.PaymentMethodId;
            switch (newStatus)
            {
                case (int)RentalOrderStatus.PENDING:
                    return ValidationResult.Valid();

                case (int)RentalOrderStatus.CANCELED:
                    /*if (paymentStatus != (int)PaymentStatus.CANCELED && depositStatus != (int)DepositStatus.CANCELED)
                    {
                        return ValidationResult.Invalid("Đơn hàng chỉ có thể hủy khi thanh toán hoặc đặt cọc đã bị hủy.");
                    }*/
                    return ValidationResult.Valid();

                case (int)RentalOrderStatus.CONFIRMED:
                    if (currentOrderStatus != (int)RentalOrderStatus.PENDING)
                    {
                        return ValidationResult.Invalid("Đơn hàng chỉ có thể xác nhận khi đang ở trạng thái chờ xử lý.");
                    }
                    break;

                case (int)RentalOrderStatus.PROCESSING:
                    if (currentOrderStatus != (int)RentalOrderStatus.CONFIRMED)
                    {
                        return ValidationResult.Invalid("Đơn hàng chỉ có thể xử lý khi đã được xác nhận và có đặt cọc (một phần hoặc đầy đủ).");
                    }

                    break;

                case (int)RentalOrderStatus.SHIPPED:
                    if (currentOrderStatus != (int)RentalOrderStatus.PROCESSING)
                    {
                        return ValidationResult.Invalid("Đơn hàng chỉ có thể giao khi đang được xử lý.");
                    }
                    break;

                case (int)RentalOrderStatus.DELIVERED:
                    if (currentOrderStatus != (int)RentalOrderStatus.SHIPPED)
                    {
                        return ValidationResult.Invalid("Đơn hàng chỉ có thể đánh dấu là đã giao khi đang ở trạng thái đã vận chuyển.");
                    }
                    break;

                case (int)RentalOrderStatus.DECLINED:
                    if (currentOrderStatus != (int)OrderStatus.SHIPPED)
                    {
                        return ValidationResult.Invalid("Đơn hàng chỉ có thể đánh dấu là đã giao khi đang ở trạng thái đã vận chuyển.");
                    }
                    break;

                case (int)RentalOrderStatus.AWAITING_PICKUP:
                    if (currentOrderStatus != (int)OrderStatus.PROCESSING || rentalOrder.DeliveryMethod != "STORE_PICKUP")
                    {
                        return ValidationResult.Invalid("Đơn hàng chỉ có thể đánh dấu là chờ nhận hàng khi phương thức vận chuyển là Đến cửa hàng nhận.");
                    }
                    break;

                case (int)RentalOrderStatus.FAILED:
                    return ValidationResult.Valid();

                case (int)RentalOrderStatus.EXTENSION_REQUESTED:
                    if (currentOrderStatus != (int)RentalOrderStatus.RENTED)
                    {
                        return ValidationResult.Invalid("Gia hạn chỉ có thể yêu cầu khi đơn hàng đang trong trạng thái thuê.");
                    }
                    break;

                case (int)RentalOrderStatus.RETURN_REQUESTED:
                    if (currentOrderStatus != (int)RentalOrderStatus.RENTED)
                    {
                        return ValidationResult.Invalid("Chỉ có thể yêu cầu trả khi đơn hàng đang trong trạng thái thuê.");
                    }
                    break;

                case (int)RentalOrderStatus.RETURNED:
                    if (currentOrderStatus != (int)RentalOrderStatus.RETURN_REQUESTED)
                    {
                        return ValidationResult.Invalid("Đơn hàng chỉ có thể trả sau khi đã yêu cầu trả.");
                    }
                    break;

                case (int)RentalOrderStatus.COMPLETED:
                    if (currentOrderStatus != (int)RentalOrderStatus.RETURNED)
                    {
                        return ValidationResult.Invalid("Đơn hàng chỉ có thể hoàn tất sau khi đã trả hoặc kiểm tra sản phẩm.");
                    }
                    break;

                default:
                    return ValidationResult.Valid();
            }

            return ValidationResult.Valid();
        }

        public async Task<ResponseDTO<int>> RequestReturnAsync(ReturnRequestModel returnRequestModel)
        {
            var selectedOrder = await _unitOfWork.RentalOrderRepository.GetObjectAsync(r => r.Id == returnRequestModel.SelectedReturnOrderId);
            if (selectedOrder == null)
            {
                return new ResponseDTO<int>
                {
                    IsSuccess = true,
                    Message = "Order not found."
                };
            }
            if (selectedOrder.OrderStatus < (int)RentalOrderStatus.DELIVERED)
            {
                return new ResponseDTO<int>
                {
                    IsSuccess = false,
                    Message = "Đơn hàng chưa đủ điều kiện để tiến hành hoàn trả."
                };
            }    
            if(selectedOrder.BranchId is null)
            {
                return new ResponseDTO<int>
                {
                    IsSuccess = false,
                    Message = "Dơn hàng chưa thuộc về chi nhánh"
                };
            }

            selectedOrder.OrderStatus = (int)RentalOrderStatus.RETURN_REQUESTED;
            selectedOrder.ReturnDate = returnRequestModel.RequestTimestamp;
            selectedOrder.UpdatedAt = _methodHelper.GetTimeInUtcPlus7();
            await _unitOfWork.RentalOrderRepository.UpdateAsync(selectedOrder);

            bool isSend = await NotifyReturnRequest(selectedOrder);

            if (isSend)
            {
                return new ResponseDTO<int>
                {
                    IsSuccess = true,
                    Message = "Đã gửi yêu cầu hoàn trả.",
                    Data = 1
                };

            }
            return new ResponseDTO<int>
            {
                IsSuccess = false,
                Message = "Yêu cầu hoàn trả thất bại.",
                Data = 0
            };
        }

        public async Task<ResponseDTO<int>> ProcessReturnOrder(ReturnRequestModelUM rentalInfor)
        {
            using var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                var seletedOrder = await _unitOfWork.RentalOrderRepository.GetObjectAsync(o => o.Id == rentalInfor.SelectedReturnOrderId);
                if (seletedOrder is null)
                    return new ResponseDTO<int> { IsSuccess = false, Message = "Không tìm thấy đơn hàng." };

                var oldTotalAmount = seletedOrder.TotalAmount;

                var result = await HandleSingleProductReturnAsync(seletedOrder, rentalInfor);
                if (!result.IsSuccess) return result;

                await _unitOfWork.RentalOrderRepository.UpdateAsync(seletedOrder);

                if (seletedOrder.ParentOrderCode != null)
                {
                    var parentOrder = await _unitOfWork.RentalOrderRepository.GetObjectAsync(r => r.RentalOrderCode == seletedOrder.ParentOrderCode);

                    if (parentOrder is null)
                        return new ResponseDTO<int> { IsSuccess = false, Message = "Parent order không tồn tại." };

                    parentOrder.TotalAmount += (seletedOrder.TotalAmount - oldTotalAmount);

                    var remainingOrders = await _unitOfWork.RentalOrderRepository
                        .GetAndIncludeAsync(r => r.ParentOrderCode == parentOrder.RentalOrderCode && r.OrderStatus != (int)RentalOrderStatus.RETURNED);

                    if (!remainingOrders.Any())
                    {
                        parentOrder.OrderStatus = (int)RentalOrderStatus.RETURNED;
                    }

                    await _unitOfWork.RentalOrderRepository.UpdateAsync(parentOrder);
                }

                await transaction.CommitAsync();

                return new ResponseDTO<int>
                {
                    IsSuccess = true,
                    Message = "Return processed successfully.",
                    Data = 1
                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return new ResponseDTO<int>
                {
                    IsSuccess = false,
                    Message = "Error processing return: " + ex.Message
                };
            }
        }

        private async Task<bool> NotifyReturnRequest(RentalOrder order)
        {
            var orderCode = order.ParentOrderCode ?? order.RentalOrderCode;
            return await _notificationService.NotifyForRentalReturnRequest(order.BranchId.Value, orderCode, order.ProductName);
        }
        private async Task<ResponseDTO<int>> HandleSingleProductReturnAsync(RentalOrder rentalOrder, ReturnRequestModelUM returnRequestModel)
        {
            if (rentalOrder.OrderStatus < (int)RentalOrderStatus.DELIVERED)
                return new ResponseDTO<int> { IsSuccess = false, Message = "Đơn hàng không thỏa điều kiện!" };

            var lateDays = _methodHelper.GetTimeSpan(rentalOrder.ReturnDate.Value, rentalOrder.RentalEndDate.Value);
            if (lateDays > 0) // trễ - cần tính 
            {
                rentalOrder.LateFee = returnRequestModel.LateFee != null ? returnRequestModel.LateFee : (lateDays * rentalOrder.RentPrice);
            }

            rentalOrder.IsRestocked = returnRequestModel.IsRestocked;
            rentalOrder.IsInspected = returnRequestModel.IsInspected;
            rentalOrder.DamageFee = returnRequestModel.DamageFee;
            rentalOrder.OrderStatus = (int)RentalOrderStatus.RETURNED;
            rentalOrder.TotalAmount = CalculateTotalAmount(rentalOrder);
            rentalOrder.UpdatedAt = _methodHelper.GetTimeInUtcPlus7();


            await _unitOfWork.RentalOrderRepository.UpdateAsync(rentalOrder);

            return new ResponseDTO<int> { IsSuccess = true, Message = "Hoàn trả thành công.", Data = 1 };
        }
        private async Task<RentalOrder> ValidateRentalOrderAsync(int orderId)
        {
            var order = await _unitOfWork.RentalOrderRepository.GetObjectAsync(o => o.Id == orderId);
            if (order == null) return null;

            // Perform additional validations if necessary
            return order;
        }
        private void UpdateParentOrderDetails(RentalOrder parentOrder, RentalOrderUM rentalOrderUM, Branch branch)
        {
            AssignCustomerInformation(parentOrder, rentalOrderUM.CustomerInformation);
            AssignBranchInformation(parentOrder, branch);

            parentOrder.PaymentMethodId = rentalOrderUM.PaymentMethodID ?? (int)OrderMethods.COD;
            parentOrder.PaymentStatus = rentalOrderUM.PaymentStatus;
            parentOrder.DeliveryMethod = rentalOrderUM.DeliveryMethod;

            parentOrder.Note = rentalOrderUM.Note;
            parentOrder.UpdatedAt = _methodHelper.GetTimeInUtcPlus7();
        }

    }
}
