using _2Sport_BE.Repository.Interfaces;
using _2Sport_BE.Repository.Models;
using _2Sport_BE.Infrastructure.DTOs;
using _2Sport_BE.Infrastructure.Enums;
using _2Sport_BE.Infrastructure.Helpers;
using AutoMapper;
using _2Sport_BE.Service.Enums;
using _2Sport_BE.Services;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Microsoft.AspNetCore.Identity.UI.Services;
using MailKit.Search;
using _2Sport_BE.Service.Helpers;
using StackExchange.Redis;


namespace _2Sport_BE.Infrastructure.Services
{
    public interface IRentalOrderService
    {
        Task<ResponseDTO<List<RentalOrderVM>>> GetAllRentalOrderAsync();
        Task<ResponseDTO<RentalOrderVM>> GetRentalOrderDetailsByIdAsync(int orderId);
        Task<ResponseDTO<List<RentalOrderVM>>> GetRentalOrderByParentCodeAsync(string parentCode);
        Task<ResponseDTO<List<RentalOrderVM>>> GetRentalOrdersByBranchAsync(int branchId);

        Task<ResponseDTO<List<RentalOrderVM>>> GetOrdersByUserIdAsync(int userId);
        Task<ResponseDTO<RentalOrderVM>> GetRentalOrderByOrderCodeAsync(string orderCode);
        Task<ResponseDTO<List<RentalOrderVM>>> GetRentalOrdersByStatusAsync(int? orderStatus, int? paymentStatus);
        Task<ResponseDTO<RentalOrderVM>> CreateRentalOrderAsync(RentalOrderCM rentalOrderCM);
        Task<ResponseDTO<RentalOrderVM>> UpdateRentalOrderAsync(int orderId, RentalOrderUM rentalOrderUM);
        Task<ResponseDTO<int>> UpdateRentalOrderStatusAsync(int orderId, int status);
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
        Task<bool> UpdateDepositStatusAndAmount(string orderCode, int depositStatus, decimal amount);

        Task<bool> UpdateRentalOrderAfterCheckout(string orderCode, int depositStatus, decimal amount, int paymentStatus);


        Task<ResponseDTO<int>> UpdateBranchForRentalOrder(int orderId, int branchId);
        Task<ResponseDTO<RentalOrderVM>> ApproveRentalOrderAsync(int orderId);
        Task<ResponseDTO<RentalOrderVM>> RejectRentalOrderAsync(int orderId);
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
        #endregion
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
        #endregion

        private decimal CalculateExtensionCost(int rentalDates, RentalOrder rentalOrder)
        {
            return (decimal)(rentalOrder.RentPrice * rentalOrder.Quantity * rentalDates);
        }
        private int CalculateTotalRentalDays(RentalOrder order)
        {
            if (order.RentalEndDate.HasValue)
            {
                return (order.RentalEndDate.Value - order.RentalStartDate.Value).Days;
            }
            return order.RentalDays;
        }
        private decimal CalculateTotalAmount(decimal? subTotal = 0,
                                            decimal? transportFee = 0,
                                            decimal? lateFee = 0,
                                            decimal? damageFee = 0,
                                            decimal? depositAmount = 0)
        {
            return (decimal)(subTotal + transportFee + lateFee + damageFee - depositAmount);
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
            rentalOrder.TotalAmount = productInformation.RentalCosts.TotalAmount.Value != null ? productInformation.RentalCosts.TotalAmount.Value : CalculateTotalAmount(rentalOrder.SubTotal, rentalOrder.TranSportFee);
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
        public async Task<bool> UpdateStock(Warehouse warehouse, int oldQuantity, int newQuantity)
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
            rentalOrderVM.DeliveryMethod = _deliveryMethodService.GetDescription(rentalOrder.DeliveryMethod);
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
                    rentalOrder.OrderStatus = (int)OrderStatus.PENDING;
                    rentalOrder.PaymentStatus = (int)PaymentStatus.IsWating;
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
                            childRentalOrder.OrderStatus = (int)OrderStatus.PENDING;
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
                        rentalOrder.TotalAmount = CalculateTotalAmount(rentalOrder.SubTotal, rentalOrder.TranSportFee);

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
                    if (branch is null) return GenerateErrorResponse($"The branch with id {rentalOrderUM.BranchId} not found!");

                    AssignCustomerInformation(parentOrder, rentalOrderUM.CustomerInformation);
                    AssignBranchInformation(parentOrder, branch);

                    parentOrder.PaymentMethodId = rentalOrderUM.PaymentMethodID ?? (int)OrderMethods.COD;
                    parentOrder.PaymentStatus = rentalOrderUM.PaymentStatus;
                    parentOrder.DeliveryMethod = rentalOrderUM.DeliveryMethod;

                    parentOrder.Note = rentalOrderUM.Note;
                    parentOrder.UpdatedAt = DateTime.UtcNow;

                    var childList = new List<RentalOrder>();
                    if (rentalOrderUM.ProductInformations.Count == 1) //cập nhật đơn thuê 1 sản phẩm
                    {
                        foreach (var item in rentalOrderUM.ProductInformations)
                        {
                            var isProductCodeMatching = parentOrder.ProductCode == item.ProductCode;
                            // Chỉ cập nhật nếu mã sản phẩm giống nhau
                            if (!isProductCodeMatching)
                                return GenerateErrorResponse($"The product {item.ProductCode} - {item.ProductName} are not match with the product code in order!");

                            if (parentOrder.ProductId.Value == item.ProductId) // (== productCode, == ProductId) => update quantity
                            {
                                var productInWarehouse = _unitOfWork.WarehouseRepository
                                    .FindObject(_ => _.ProductId == parentOrder.ProductId && _.BranchId == parentOrder.BranchId);
                                //so sánh quantity, sau đó update trong order và lưu lại
                                if (productInWarehouse == null)
                                    return GenerateErrorResponse($"Product with Id = {item.ProductId} not found");
                                var isUpdatedStock = await UpdateStock(productInWarehouse, parentOrder.Quantity.Value, item.Quantity.Value);
                                if (!isUpdatedStock)
                                {
                                    await transaction.RollbackAsync();
                                    return GenerateErrorResponse($"Product with Id = {item.ProductId} fail to update stock");
                                }
                            }
                            else // (== productCode, != ProductId)
                            {
                                var product = _unitOfWork.ProductRepository.FindObject(p => p.Id == item.ProductId && p.ProductCode == item.ProductCode);
                                if (parentOrder.BranchId != null)
                                {
                                    //cập nhật lại available quantity với sản phẩm của order ban đầu
                                    var oldProductInWarehouse = _unitOfWork.WarehouseRepository.FindObject(_ => _.ProductId == parentOrder.ProductId && _.BranchId == parentOrder.BranchId);
                                    if (oldProductInWarehouse != null)
                                    {
                                        oldProductInWarehouse.AvailableQuantity += parentOrder.Quantity;
                                        await _unitOfWork.WarehouseRepository.UpdateAsync(oldProductInWarehouse);
                                    }
                                }
                                //check new product in warhouse
                                var newProductInWarehouse = _unitOfWork.WarehouseRepository.FindObject(_ => _.ProductId == item.ProductId && _.BranchId == branch.Id);
                                if (newProductInWarehouse == null)
                                    return GenerateErrorResponse($"Product with Id = {item.ProductId} not found");
                                if (newProductInWarehouse.AvailableQuantity < item.Quantity)
                                    return GenerateErrorResponse($"Product with Id = {item.ProductId} not enough to update stock");

                                newProductInWarehouse.AvailableQuantity -= item.Quantity;
                                await _unitOfWork.WarehouseRepository.UpdateAsync(newProductInWarehouse);
                            }
                            AssignRentalProduct(item, parentOrder);
                            AssignRentalDates(parentOrder, item.RentalDates);
                            AssignRentalCost(parentOrder, item);
                        }
                        await _unitOfWork.RentalOrderRepository.UpdateAsync(parentOrder);
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
                var toDelete = await _unitOfWork.RentalOrderRepository.GetObjectAsync(_ => _.Id == rentalOrderId);
                
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
            result.DeliveryMethod = _deliveryMethodService.GetDescription(order.DeliveryMethod);
            result.Id = order.Id;
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

        public async Task<ResponseDTO<List<RentalOrderVM>>> GetOrdersByUserIdAsync(int userId)
        {
            var response = new ResponseDTO<List<RentalOrderVM>>();

            try
            {
                var orders = await _unitOfWork.RentalOrderRepository
                    .GetAsync(o => o.UserId == userId);

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

        public async Task<ResponseDTO<int>> UpdateRentalOrderStatusAsync(int orderId, int status)
        {
            var response = new ResponseDTO<int>();
            try
            {
                var isExisted = await _unitOfWork.RentalOrderRepository.GetObjectAsync(o => o.Id == orderId);
                if (isExisted is null)
                {
                    response.IsSuccess = false;
                    response.Message = "Updated Error";
                    response.Data = 0;
                }
                else
                {
                    isExisted.OrderStatus = status;
                    await _unitOfWork.RentalOrderRepository.UpdateAsync(isExisted);

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
                var order = await _unitOfWork.RentalOrderRepository.GetObjectAsync(o => o.Id == orderId);
                if (order is null)
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

        public async Task<ResponseDTO<RentalOrderVM>> ProcessExtendRentalPeriod(ExtendRentalModel extendRental)
        {
            var response = new ResponseDTO<RentalOrderVM>();
            try
            {
                var rentalOrder = _unitOfWork.RentalOrderRepository.FindObject(o => o.Id == extendRental.ChildOrderId);
                if (rentalOrder is null) return GenerateErrorResponse("The order are not found");

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

        public async Task<ResponseDTO<RentalOrderVM>> ReturnOrder(ParentOrderReturnModel rentalInfor)
        {
            var parentOrder = await _unitOfWork.RentalOrderRepository.GetObjectAsync(o => o.Id.Equals(rentalInfor.ParentOrderId));
            if (parentOrder is null) return GenerateErrorResponse($"The Rental Order - {rentalInfor.ParentOrderId} are not found!");

            decimal? totalLateFee = 0, totalDamageFee = 0;
            if (rentalInfor.ChildOrders.Count() > 0)
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
        public async Task<bool> UpdateDepositStatusAndAmount(string orderCode, int depositStatus, decimal depositAmount)
        {
            bool response = false;

            try
            {
                var rentalOrder = await _unitOfWork.RentalOrderRepository.GetObjectAsync(o => o.RentalOrderCode.Equals(orderCode));
                if (rentalOrder == null)
                    response = false;
                else
                {
                    rentalOrder.DepositAmount = depositAmount;
                    rentalOrder.DepositStatus = depositStatus;

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
        public async Task<bool> UpdateRentalOrderAfterCheckout(string orderCode, int depositStatus, decimal depositAmount, int paymentStatus)
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
                    rentalOrder.DepositAmount = depositAmount;
                    rentalOrder.DepositStatus = depositStatus;

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
                        if (rentalOrder.OrderStatus == (int)OrderStatus.CONFIRMED)
                            return GenerateErrorResponse($"Sales order status with id = {orderId} has been previously confirmed!");

                        rentalOrder.OrderStatus = (int)OrderStatus.CONFIRMED;
                        if (childOrder.Any())
                        {
                            foreach (var item in childOrder.ToList())
                            {
                                item.OrderStatus = (int)OrderStatus.CONFIRMED;
                                var productInWarehouse = (await _warehouseService.GetWarehouseByProductIdAndBranchId(item.ProductId.Value, item.BranchId)).FirstOrDefault();
                                if (productInWarehouse == null)
                                {
                                    await transaction.RollbackAsync();
                                    return GenerateErrorResponse($"Failed to update stock for product {item.ProductName}");
                                }
                                productInWarehouse.AvailableQuantity -= item.Quantity;
                                await _warehouseService.UpdateWarehouseAsync(productInWarehouse);
                                await _unitOfWork.RentalOrderRepository.UpdateAsync(item);
                            }
                        }
                        else
                        {
                            var productInWarehouse = (await _warehouseService.GetWarehouseByProductIdAndBranchId(rentalOrder.ProductId.Value, rentalOrder.BranchId)).FirstOrDefault();
                            if (productInWarehouse == null)
                            {
                                await transaction.RollbackAsync();
                                return GenerateErrorResponse($"Failed to update stock for product {rentalOrder.ProductName}");
                            }
                            productInWarehouse.AvailableQuantity -= rentalOrder.Quantity;
                            await _warehouseService.UpdateWarehouseAsync(productInWarehouse);
                        }


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
                        var childOrder = await _unitOfWork.RentalOrderRepository.GetAsync(_ => _.ParentOrderCode.Equals(rentalOrder.RentalOrderCode));

                        rentalOrder.OrderStatus = (int)OrderStatus.PENDING;
                        rentalOrder.BranchId = null;

                        if (childOrder.Any())
                        {
                            foreach (var item in childOrder.ToList())
                            {
                                item.OrderStatus = (int)OrderStatus.PENDING;
                                item.BranchId = null;
                                await _unitOfWork.RentalOrderRepository.UpdateAsync(item);
                            }
                        }

                        await _notificationService.NotifyForRejectOrderAsync(rentalOrder.RentalOrderCode, rentalOrder.BranchId.Value);
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
                    rentalOrder.BranchId = branchId;

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
                    response.IsSuccess = false;
                    response.Message = "Rental orders are not found";
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = $"Error: {ex.Message}";

            }
            return response;
        }
    }
}
