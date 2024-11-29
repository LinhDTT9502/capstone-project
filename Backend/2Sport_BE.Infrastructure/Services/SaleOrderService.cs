using _2Sport_BE.Infrastructure.Helpers;
using _2Sport_BE.Repository.Interfaces;
using _2Sport_BE.Repository.Models;
using _2Sport_BE.Infrastructure.DTOs;
using _2Sport_BE.Infrastructure.Enums;
using AutoMapper;
using System.Net;
using Microsoft.CodeAnalysis.Semantics;
using _2Sport_BE.Services;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Microsoft.EntityFrameworkCore;
using MailKit.Search;
using _2Sport_BE.Service.Enums;
using _2Sport_BE.Service.Helpers;

namespace _2Sport_BE.Infrastructure.Services
{
    public interface ISaleOrderService
    {
        #region IRead
        Task<ResponseDTO<List<SaleOrderVM>>> GetAllSaleOrdersAsync();
        Task<ResponseDTO<SaleOrderVM>> GetSaleOrderDetailByIdAsync(int saleOrderId);
        Task<ResponseDTO<List<SaleOrderVM>>> GetSaleOrdersByStatus(int? orderStatus, int? paymentStatus);
        Task<ResponseDTO<List<SaleOrderVM>>> GetSaleOrdersOfUserAsync(int userId);
        Task<ResponseDTO<List<SaleOrderVM>>> GetSaleOrdersByBranchAsync(int branchId);
        Task<ResponseDTO<List<SaleOrderVM>>> GetSaleOrdersByDateRangeAndStatus(DateTime? fromDate, DateTime? toDate, int? orderStatus);
        //=============================================
        Task<ResponseDTO<RevenueVM>> GetSaleOrdersRevenue(int? branchId, int? SaleOrderType, DateTime? from, DateTime? to, int? status);
        Task<ResponseDTO<SaleOrderVM>> GetSaleOrderBySaleOrderCode(string SaleOrderCode);
        //Task<ResponseDTO<List<SaleOrderVM>>> GetSaleOrdersByMonthAndStatus(DateTime startDate, DateTime endDate, int status);
        #endregion

        #region ICreate_IUpdate_IDelete
        Task<ResponseDTO<SaleOrderVM>> CreateSaleOrderAsync(SaleOrderCM SaleOrderCM);
        Task<ResponseDTO<SaleOrderVM>> UpdateSaleOrderAsync(int SaleOrderId, SaleOrderUM SaleOrderUM);
        Task<ResponseDTO<SaleOrderVM>> UpdateSaleOrderStatusAsync(int id, int status);
        Task<ResponseDTO<int>> UpdateBranchForSaleOrder(int orderId, int branchId);
        Task<ResponseDTO<int>> DeleteSaleOrderAsync(int id);
        Task<ResponseDTO<SaleOrderVM>> ApproveSaleOrderAsync(int orderId);
        Task<ResponseDTO<SaleOrderVM>> RejectSaleOrderAsync(int orderId);
        #endregion

        #region CRUD_Order
        Task<IQueryable<SaleOrder>> FindAllSaleOrderQueryableAsync();
        Task<SaleOrder> FindSaleOrderByIdFromUserAsync(int SaleOrderId, int userId);
        Task<SaleOrder> FindSaleOrderById(int orderId);
        Task<SaleOrder> FindSaleOrderByCode(string orderCode);
        Task<int> UpdateSaleOrder(SaleOrder saleOrder);
        Task<bool> UpdatePaymentStatusOfSaleOrder(string orderCode, int paymentStatus);

        #endregion

    }
    public class SaleOrderService : ISaleOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICustomerService _customerDetailService;
        private readonly IWarehouseService _warehouseService;
        private readonly IMethodHelper _methodHelper;
        private readonly IDeliveryMethodService _deliveryMethodService;
        private readonly IMapper _mapper;
        private readonly INotificationService _notificationService;
        private readonly IMailService _mailService;
        public SaleOrderService(IUnitOfWork unitOfWork,
            ICustomerService customerDetailService,
            IWarehouseService warehouseService,
            IMethodHelper methodHelper,
            IDeliveryMethodService deliveryMethodService,
            IMapper mapper,
            INotificationService notificationService,
            IMailService mailService)
        {
            _unitOfWork = unitOfWork;
            _customerDetailService = customerDetailService;
            _warehouseService = warehouseService;
            _methodHelper = methodHelper;
            _deliveryMethodService = deliveryMethodService;
            _mapper = mapper;
            _notificationService = notificationService;
            _mailService = mailService;
        }
        private void AssignSaleProduct(ProductInfor productInformation, OrderDetail orderDetail)
        {
            orderDetail.ProductId = productInformation.ProductId;
            orderDetail.ProductName = productInformation.ProductName;
            orderDetail.ProductCode = productInformation.ProductCode;
            orderDetail.Size = productInformation.Size;
            orderDetail.Color = productInformation.Color;
            orderDetail.Condition = productInformation.Condition;
            orderDetail.UnitPrice = productInformation.UnitPrice;
            orderDetail.ImgAvatarPath = productInformation.ImgAvatarPath;
            orderDetail.Quantity = productInformation.Quantity;

        }
        private void AssignSaleCost(SaleOrder saleOrder,
                                      SaleCosts saleCosts,
                                      ProductInfor productInformation)
        {
            saleOrder.SubTotal = saleCosts.SubTotal ?? (decimal)(productInformation.UnitPrice * productInformation.Quantity);
            saleOrder.TranSportFee = saleCosts.TranSportFee ?? 0;
            saleOrder.TotalAmount = saleCosts.TotalAmount.Value != null ? saleCosts.TotalAmount.Value : (decimal)(saleOrder.SubTotal + saleOrder.TranSportFee);

        }
        private void AssignCustomerInformation(SaleOrder saleOrder, CustomerInformation customerInformation)
        {
            if (customerInformation == null) return;
            saleOrder.UserId = customerInformation.UserId;
            saleOrder.Email = customerInformation.Email;
            saleOrder.Gender = customerInformation.Gender;
            saleOrder.FullName = customerInformation.FullName;
            saleOrder.ContactPhone = customerInformation.ContactPhone;
            saleOrder.Address = customerInformation.Address;
        }
        private void AssignBranchInformation(SaleOrder saleOrder, Branch branch)
        {
            saleOrder.BranchId = branch.Id;
            saleOrder.BranchName = branch.BranchName;
        }
        private void AssignBranchInformation(OrderDetail orderDetail, Branch branch)
        {
            orderDetail.BranchId = branch.Id;
            orderDetail.BranchName = branch.BranchName;
        }

        private void AssignSaleOrderInfomation(SaleOrder saleOrder, SaleOrderCM saleOrderCM)
        {
            saleOrder.SaleOrderCode = _methodHelper.GenerateOrderCode();
            saleOrder.DeliveryMethod = saleOrderCM.DeliveryMethod;
            saleOrder.Note = saleOrderCM.Note;
            saleOrder.DateOfReceipt = saleOrderCM.DateOfReceipt != null ? saleOrderCM.DateOfReceipt : DateTime.Now.AddDays(3);
            saleOrder.CreatedAt = DateTime.Now;

        }
        private void AssignSaleOrderInfomation(SaleOrder saleOrder, SaleOrderUM saleOrderUM)
        { 
            saleOrder.DeliveryMethod = saleOrderUM.DeliveryMethod;
            saleOrder.Note = saleOrderUM.Note;
            saleOrder.DateOfReceipt = saleOrderUM.DateOfReceipt != null ? saleOrderUM.DateOfReceipt : DateTime.Now.AddDays(3);
            saleOrder.CreatedAt = DateTime.Now;
        }
        #region Read_SaleOrder
        public async Task<ResponseDTO<List<SaleOrderVM>>> GetAllSaleOrdersAsync()
        {
            var response = new ResponseDTO<List<SaleOrderVM>>();
            try
            {
                var query = await _unitOfWork.SaleOrderRepository.GetAllAsync(new string[] { "User", "OrderDetails" });
                if (query == null || !query.Any())
                {
                    response.IsSuccess = false;
                    response.Message = "SaleOrders are not found";
                    return response;
                }
                var resultList = query.Select(saleOrder =>
                {
                    var result = _mapper.Map<SaleOrderVM>(saleOrder);
                    MapSaleOrderToSaleOrderVM(saleOrder, result);
                    return result;
                }).ToList();

                response.IsSuccess = true;
                response.Message = "Query successfully";
                response.Data = resultList;
                return response;

            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
                return response;
            }
        }
        public async Task<ResponseDTO<SaleOrderVM>> GetSaleOrderDetailByIdAsync(int id)
        {
            var response = new ResponseDTO<SaleOrderVM>();
            try
            {
                var saleOrder = (await _unitOfWork.SaleOrderRepository.GetObjectAsync(_ => _.Id == id, new string[] { "User", "OrderDetails" }));
                if (saleOrder == null)
                {
                    response.IsSuccess = false;
                    response.Message = $"SaleOrder with id = {id} is not found";
                    return response;
                }

                response = GenerateSuccessResponse(saleOrder, "Query Successfully");
                return response;

            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
                return response;
            }
        }
        public async Task<ResponseDTO<List<SaleOrderVM>>> GetSaleOrdersOfUserAsync(int userId)
        {
            var response = new ResponseDTO<List<SaleOrderVM>>();
            try
            {
                var query = await _unitOfWork.SaleOrderRepository.GetAndIncludeAsync(o => o.UserId == userId, new string[] { "User", "OrderDetails" });
                if (query == null || !query.Any())
                {
                    response.IsSuccess = false;
                    response.Message = "SaleOrders are not found";
                    return response;
                }
                var resultList = query.Select(saleOrder =>
                {
                    var result = _mapper.Map<SaleOrderVM>(saleOrder);
                    result.SaleOrderDetailVMs = _mapper.Map<List<SaleOrderDetailVM>> (saleOrder.OrderDetails.ToList());
                    MapSaleOrderToSaleOrderVM(saleOrder, result);
                    return result;
                }).ToList();

                response.IsSuccess = true;
                response.Message = "Query successfully";
                response.Data = resultList;
                return response;

            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
                return response;
            }
        }
        public async Task<ResponseDTO<List<SaleOrderVM>>> GetSaleOrdersByStatus(int? orderStatus, int? paymentStatus)
        {
            var response = new ResponseDTO<List<SaleOrderVM>>();
            try
            {
                var query = await _unitOfWork.SaleOrderRepository.GetAllAsync(new string[] { "User", "OrderDetails" });
                if (query == null || !query.Any())
                {
                    response.IsSuccess = false;
                    response.Message = "SaleOrders are not found";
                    return response;
                }
                if (orderStatus > 0 && orderStatus.ToString() != string.Empty)
                {
                    query = query.Where(o => o.OrderStatus == orderStatus);
                }
                if (paymentStatus > 0 && paymentStatus.ToString() != string.Empty)
                {
                    query = query.Where(o => o.PaymentStatus == paymentStatus);
                }

                var resultList = query.Select(saleOrder =>
                {
                    var result = _mapper.Map<SaleOrderVM>(saleOrder);
                    MapSaleOrderToSaleOrderVM(saleOrder, result);
                    return result;
                }).ToList();

                response.IsSuccess = true;
                response.Message = "Query successfully";
                response.Data = resultList;
                return response;

            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
                return response;
            }
        }
        public async Task<ResponseDTO<List<SaleOrderVM>>> GetSaleOrdersByDateRangeAndStatus(DateTime? fromDate, DateTime? toDate, int? orderStatus)
        {

            throw new NotImplementedException();
        }
        private async Task<bool> UpdateStock(ProductInfor productInformation, int branchId, bool isReturningStock)
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
        public async Task<ResponseDTO<RevenueVM>> GetSaleOrdersRevenue(int? branchId, int? SaleOrderType, DateTime? from, DateTime? to, int? status)
        {
            var response = new ResponseDTO<RevenueVM>();
            try
            {
                /*var SaleOrdersQuery = (await _unitOfWork.SaleOrderRepository.GetAllAsync()).AsQueryable();
                if (branchId.HasValue)
                {
                    SaleOrdersQuery = SaleOrdersQuery.Where(o => o.BranchId == branchId.Value);
                }
                if (branchId.HasValue)
                {
                    SaleOrdersQuery = SaleOrdersQuery.Where(o => o.BranchId == branchId.Value);
                }
                if (from.HasValue)
                {
                    SaleOrdersQuery = SaleOrdersQuery.Where(o => o.CreateAt >= from.Value);
                }
                if (to.HasValue)
                {
                    SaleOrdersQuery = SaleOrdersQuery.Where(o => o.CreateAt <= to.Value);
                }

                if (status.HasValue)
                {
                    SaleOrdersQuery = SaleOrdersQuery.Where(o => o.Status == status.Value);
                }


                var SaleOrders = await SaleOrdersQuery.ToListAsync();


                var totalRevenue = SaleOrders.Sum(o => o.TotalPrice);

                response.IsSuccess = true;
                response.Message = "Query successfully!";
                response.Data = new RevenueVM()
                {
                    TotalSaleOrders = SaleOrders.Count,
                    TotalPrice = totalRevenue.ToString()
                };*/

                return null;
            }
            catch (Exception ex)
            {

                response.IsSuccess = false;
                response.Message = ex.Message;
                return response;
            }
        }
        public async Task<ResponseDTO<SaleOrderVM>> GetSaleOrderBySaleOrderCode(string SaleOrderCode)
        {
            try
            {
                var saleOder = await _unitOfWork.SaleOrderRepository.GetObjectAsync(o => o.SaleOrderCode.Equals(SaleOrderCode), new string[] { "User", "OrderDetails" });
                if (saleOder == null)
                {
                    return GenerateErrorResponse($"SaleOrder with code {SaleOrderCode} are not found");
                } 
                return GenerateSuccessResponse(saleOder, "Query Successfully");
            }
            catch (Exception ex)
            {
                return GenerateErrorResponse(ex.Message);
            }
        }
        #endregion
        #region Create_Update_Delete_SaleOrder
        public async Task<ResponseDTO<SaleOrderVM>> CreateSaleOrderAsync(SaleOrderCM saleOrderCM)
        {
            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    User user = null;
                    if (saleOrderCM.CustomerInformation.UserId.HasValue && saleOrderCM.CustomerInformation.UserId != 0)
                    {
                        user = await _unitOfWork.UserRepository
                                        .GetObjectAsync(u => u.Id == saleOrderCM.CustomerInformation.UserId);
                        if (user == null) return GenerateErrorResponse($"User with Id = {saleOrderCM.CustomerInformation.UserId} is not found!");
                    }

                    if (saleOrderCM.ProductInformations.Count == 0)
                        return GenerateErrorResponse($"List of items are empty");

                    string DeliveryMethod = saleOrderCM.DeliveryMethod;
                    if (string.IsNullOrEmpty(DeliveryMethod) || !_deliveryMethodService.IsValidMethod(DeliveryMethod))
                        return GenerateErrorResponse("Delivery Method is invalid");

                    if (DeliveryMethod.Equals("STORE_PICKUP") && !saleOrderCM.BranchId.HasValue) //để store lấy thì phải có branchId
                        return GenerateErrorResponse("Delivery Method is invalid");

                    var saleOrder = new SaleOrder();
                    AssignCustomerInformation(saleOrder, saleOrderCM.CustomerInformation);

                    var branch = saleOrderCM.BranchId != null ? await _unitOfWork.BranchRepository.GetObjectAsync(b => b.Id == saleOrderCM.BranchId) : null;
                    if (branch != null)
                    {
                        AssignBranchInformation(saleOrder, branch);
                    }
                    AssignSaleOrderInfomation(saleOrder, saleOrderCM);

                    saleOrder.OrderStatus = (int?)OrderStatus.PENDING;
                    saleOrder.PaymentStatus = (int)PaymentStatus.IsWating;
                   
                    await _unitOfWork.SaleOrderRepository.InsertAsync(saleOrder);

                    string saleOrderCode = saleOrder.SaleOrderCode;

                    decimal subTotal = 0;
                    foreach (var item in saleOrderCM.ProductInformations)
                    {
                        var detail = new OrderDetail
                        {
                            SaleOrderCode = saleOrderCode,
                            SaleOrderId = saleOrder.Id,
                            Quantity = item.Quantity,
                            CreatedAt = DateTime.Now,
                        };
                        AssignSaleProduct(item, detail);
                        if (branch != null)
                        {
                            AssignBranchInformation(detail, branch);
                            var reduceInWarehouse = await UpdateStock(item, branch.Id, false);
                            if (!reduceInWarehouse)
                            {
                                await transaction.RollbackAsync();
                                return GenerateErrorResponse($"Failed to update stock for product {item.ProductName}");
                            }
                        }

                        await _unitOfWork.OrderDetailRepository.InsertAsync(detail);
                        subTotal += (decimal)(detail.UnitPrice * item.Quantity);
                    }

                    saleOrder.SubTotal = saleOrderCM.SaleCosts.SubTotal ?? subTotal;
                    saleOrder.TranSportFee = saleOrderCM.SaleCosts.TranSportFee ?? 0;
                    saleOrder.TotalAmount = saleOrderCM.SaleCosts.TotalAmount ?? (decimal)(saleOrder.SubTotal + saleOrder.TranSportFee);

                    await _unitOfWork.SaleOrderRepository.UpdateAsync(saleOrder);

                    //Send notifications to Admmin
                    await _notificationService.NotifyForCreatingNewOrderAsync(saleOrder.SaleOrderCode);
                    await _mailService.SendSaleOrderInformationToCustomer(saleOrder, saleOrder.OrderDetails.ToList(), saleOrder.Email);
                    await transaction.CommitAsync();

                    return GenerateSuccessResponse(saleOrder, $"SaleOrder processed successfully");
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return GenerateErrorResponse(ex.Message);
                }
            }
        }
        public async Task<ResponseDTO<SaleOrderVM>> UpdateSaleOrderAsync(int saleOrderId, SaleOrderUM saleOrderUM)
        {
            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    var toUpdate = _unitOfWork.SaleOrderRepository.FindObject(o => o.Id == saleOrderId);
                    if (toUpdate == null)
                        return GenerateErrorResponse($"SaleOrder with id {saleOrderId} not found!");

                    var branch = await _unitOfWork.BranchRepository.GetObjectAsync(o => o.Id == saleOrderUM.BranchId);
                    if (branch is null) return GenerateErrorResponse($"The branch with id {saleOrderUM.BranchId} not found!");

                    AssignCustomerInformation(toUpdate, saleOrderUM.CustomerInformation);
                    AssignSaleOrderInfomation(toUpdate, saleOrderUM);

                    await _unitOfWork.SaleOrderRepository.UpdateAsync(toUpdate);

                    var orderDetails = await _unitOfWork.OrderDetailRepository
                        .GetAsync(od => od.SaleOrderId == saleOrderId);

                    if (saleOrderUM.ProductInformations.Count != orderDetails.ToList().Count || saleOrderUM.ProductInformations.Count == 0)
                        return GenerateErrorResponse($"The number of items are not match with the number of items in Child Orders!");


                    decimal supTotal = 0;
                    for (int i = 0; i < saleOrderUM.ProductInformations.Count; i++)
                    {
                        var itemUm = saleOrderUM.ProductInformations[i];
                        var detail = orderDetails.ToList()[i];

                        if (detail == null)
                            return GenerateErrorResponse($"The product with code: {itemUm.ProductCode} - {itemUm.ProductName} is not found in the list of order details");

                        if (detail.ProductId == itemUm.ProductId)
                        {
                            var productInWarehouse = _unitOfWork.WarehouseRepository.FindObject(w => w.ProductId == detail.ProductId.Value && w.BranchId == branch.Id);
                            if (productInWarehouse == null)
                                return GenerateErrorResponse($"Product with Id = {itemUm.ProductId} not found");

                            var isUpdatedStock = await UpdateStock(productInWarehouse, detail.Quantity.Value, itemUm.Quantity.Value);
                            if (!isUpdatedStock)
                            {
                                await transaction.RollbackAsync();
                                return GenerateErrorResponse($"Product with Id = {itemUm.ProductId} failed to update stock");
                            }
                        }
                        else // Nếu thay đổi sản phẩm trong order
                        {
                            if (toUpdate.BranchId != null)
                            {
                                // Cập nhật lại kho cho sản phẩm cũ (nếu cần)
                                var oldProductInWarehouse = _unitOfWork.WarehouseRepository
                                    .FindObject(_ => _.ProductId == detail.ProductId && _.BranchId == toUpdate.BranchId);

                                if (oldProductInWarehouse != null)
                                {
                                    oldProductInWarehouse.AvailableQuantity += detail.Quantity;
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
                        AssignSaleProduct(itemUm, detail);
                        AssignBranchInformation(detail, branch);
                        detail.UpdatedAt = DateTime.Now;

                        await _unitOfWork.OrderDetailRepository.UpdateAsync(detail);
                    }
                   
                    toUpdate.SubTotal = saleOrderUM.SaleCosts.SubTotal != 0 ? saleOrderUM.SaleCosts.SubTotal : supTotal;
                    toUpdate.TranSportFee = saleOrderUM.SaleCosts.TranSportFee != 0 ? saleOrderUM.SaleCosts.TranSportFee : 0;
                    toUpdate.TotalAmount = saleOrderUM.SaleCosts.TotalAmount.Value != 0 ? saleOrderUM.SaleCosts.TotalAmount.Value : (decimal)(toUpdate.SubTotal + toUpdate.TranSportFee);
                    await _unitOfWork.SaleOrderRepository.UpdateAsync(toUpdate);

                    await transaction.CommitAsync();

                    return GenerateSuccessResponse(toUpdate, $"Update SaleOrder processed successfully");
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return GenerateErrorResponse(ex.Message);
                }
            }
        }
        private void MapSaleOrderToSaleOrderVM(SaleOrder saleOrder, SaleOrderVM saleOrderVM)
        {
            saleOrderVM.PaymentStatus = saleOrder.PaymentStatus != null
                ? EnumDisplayHelper.GetEnumDescription<PaymentStatus>(saleOrder.PaymentStatus.Value)
                : "N/A";
            saleOrderVM.OrderStatus = saleOrder.OrderStatus != null
              ? EnumDisplayHelper.GetEnumDescription<RentalOrderStatus>(saleOrder.OrderStatus.Value)
              : "N/A";
            saleOrderVM.PaymentMethod = saleOrder.PaymentMethodId.HasValue
                                ? Enum.GetName(typeof(OrderMethods), saleOrder.PaymentMethodId.Value)
                                : "Unknown PaymentMethod";
            saleOrderVM.DeliveryMethod = _deliveryMethodService.GetDescription(saleOrder.DeliveryMethod);
        }
        #endregion
        public async Task<ResponseDTO<int>> DeleteSaleOrderAsync(int id)
        {
            var response = new ResponseDTO<int>();
            try
            {
                var SaleOrder = _unitOfWork.SaleOrderRepository.FindObject(o => o.Id == id);
                if (SaleOrder == null)
                {
                    response.IsSuccess = false;
                    response.Message = $"SaleOrder with id = {id} is not found!";
                    response.Data = 0;
                    return response;
                }
                else
                {
                    await _unitOfWork.SaleOrderRepository.DeleteAsync(id);

                    response.IsSuccess = true;
                    response.Message = $"Remove SaleOrder with id = {id} successfully";
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
        #region CRUD_SALE_ORDER_BASIC
        public async Task<SaleOrder> FindSaleOrderById(int orderId)
        {
            return await _unitOfWork.SaleOrderRepository.GetObjectAsync(o => o.Id == orderId);
        }
        public async Task<SaleOrder> FindSaleOrderByCode(string orderCode)
        {
            return await _unitOfWork.SaleOrderRepository.GetObjectAsync(o => o.SaleOrderCode == orderCode);
        }
        public async Task<SaleOrder> FindSaleOrderByIdFromUserAsync(int SaleOrderId, int userId)
        {
            return await _unitOfWork.SaleOrderRepository.GetObjectAsync(_ => _.Id == SaleOrderId && _.UserId == userId);
        }
        public async Task<IQueryable<SaleOrder>> FindAllSaleOrderQueryableAsync()
        {
            var query = await _unitOfWork.SaleOrderRepository.GetAllAsync();

            return query.AsQueryable();
        }
        public async Task<int> UpdateSaleOrder(SaleOrder saleOrder)
        {
            await _unitOfWork.SaleOrderRepository.UpdateAsync(saleOrder);
            return 1;
        }
        public async Task<ResponseDTO<SaleOrderVM>> UpdateSaleOrderStatusAsync(int id, int orderStatus)
        {
            try
            {
                var saleOrder = await _unitOfWork.SaleOrderRepository.GetObjectAsync(o => o.Id == id, new string[] {"OrderDetails" } );
                if (saleOrder == null) return GenerateErrorResponse($"SaleOrder with id = {id} is not found!");

                saleOrder.OrderStatus = orderStatus;
                await _unitOfWork.SaleOrderRepository.UpdateAsync(saleOrder);

                string message = "";

                if (saleOrder.OrderStatus == (int)OrderStatus.COMPLETED)
                {
                    var loyaltyUpdateResponse = await _customerDetailService.UpdateLoyaltyPoints(saleOrder.Id);
                    if (loyaltyUpdateResponse.IsSuccess) message = "Order status updated and loyalty points awarded successfully.";
                    else message = "Order status updated, but there was an error updating loyalty points.";

                }
                else message = "Order status updated successfully.";

                return GenerateSuccessResponse(saleOrder, message);
            }
            catch (Exception ex)
            {
                return GenerateErrorResponse(ex.Message);
            }
        }
        public async Task<bool> UpdatePaymentStatusOfSaleOrder(string orderCode, int paymentStatus)
        {
            bool response = false;

            try
            {
                var saleOrder = await _unitOfWork.SaleOrderRepository.GetObjectAsync(o => o.SaleOrderCode.Equals(orderCode));
                if (saleOrder == null)
                    response = false;
                else
                {
                    saleOrder.PaymentStatus = paymentStatus;

                    await _unitOfWork.SaleOrderRepository.UpdateAsync(saleOrder);

                    response = true;
                }
            }
            catch (Exception ex)
            {
                response = false;
            }
            return response;
        }
        #endregion
        public ResponseDTO<SaleOrderVM> GenerateSuccessResponse(SaleOrder order, string messagge)
        {
            var result = _mapper.Map<SaleOrderVM>(order);

            result.OrderStatus = order.OrderStatus != null
                ? EnumDisplayHelper.GetEnumDescription<RentalOrderStatus>(order.OrderStatus.Value)
                : "N/A";

            result.PaymentStatus = order.PaymentStatus != null
                ? EnumDisplayHelper.GetEnumDescription<PaymentStatus>(order.PaymentStatus.Value)
                : "N/A";
            

            result.DeliveryMethod = _deliveryMethodService.GetDescription(order.DeliveryMethod);

            if (order.OrderDetails.Any())
            {
                result.SaleOrderDetailVMs = order.OrderDetails.Select(od => new SaleOrderDetailVM()
                {
                    ProductId = od.ProductId,
                    ProductName = od.ProductName,
                    UnitPrice = od.UnitPrice,
                    Quantity = od.Quantity,
                    TotalAmount = od.TotalAmount,
                    ImgAvatarPath = od.ImgAvatarPath,
                    ProductCode = od.ProductCode,
                    Color = od.Color,
                    Condition = od.Condition,
                    Size = od.Size,
                }).ToList();
            }

            return new ResponseDTO<SaleOrderVM>
            {
                IsSuccess = true,
                Message = messagge,
                Data = result
            };
        }
        public ResponseDTO<SaleOrderVM> GenerateErrorResponse(string message)
        {
            return new ResponseDTO<SaleOrderVM>()
            {
                IsSuccess = false,
                Message = message,
                Data = null
            };
        }

        public async Task<ResponseDTO<int>> UpdateBranchForSaleOrder(int orderId, int branchId)
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
                var SaleOrder = await _unitOfWork.SaleOrderRepository.GetObjectAsync(o => o.Id == orderId);
                if (SaleOrder == null)
                {
                    response.IsSuccess = false;
                    response.Message = $"SaleOrder with id = {orderId} is not found!";
                    response.Data = 0;
                }
                else
                {
                    SaleOrder.BranchId = branchId;

                    await _unitOfWork.SaleOrderRepository.UpdateAsync(SaleOrder);


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
        public async Task<ResponseDTO<List<SaleOrderVM>>> GetSaleOrdersByBranchAsync(int branchId)
        {
            var response = new ResponseDTO<List<SaleOrderVM>>();
            try
            {
                var query = await _unitOfWork.SaleOrderRepository.GetAndIncludeAsync(o => o.BranchId == branchId, new string[] { "User", "OrderDetails" });
                if (query == null || !query.Any())
                {
                    response.IsSuccess = false;
                    response.Message = "SaleOrders are not found";
                    return response;
                }

                var resultList = query.Select(saleOrder =>
                {
                    var result = _mapper.Map<SaleOrderVM>(saleOrder);
                    MapSaleOrderToSaleOrderVM(saleOrder, result);
                    return result;
                }).ToList();

                response.IsSuccess = true;
                response.Message = "Query successfully";
                response.Data = resultList;
                return response;

            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
                return response;
            }
        }
        public async Task<ResponseDTO<SaleOrderVM>> ApproveSaleOrderAsync(int orderId)
        {
            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    var SaleOrder = await _unitOfWork.SaleOrderRepository.GetObjectAsync(o => o.Id == orderId, new string[] { "OrderDetails" });
                    if (SaleOrder == null) return GenerateErrorResponse($"SaleOrder with id = {orderId} is not found!");
                    else
                    {
                        if (SaleOrder.OrderStatus == (int)OrderStatus.CONFIRMED)
                            return GenerateErrorResponse($"Sales order status with id = {orderId} has been previously confirmed!");

                        SaleOrder.OrderStatus = (int)OrderStatus.CONFIRMED;

                        foreach (var item in SaleOrder.OrderDetails)
                        {
                            var productInWarehouse = (await _warehouseService.GetWarehouseByProductIdAndBranchId(item.ProductId.Value, SaleOrder.BranchId)).FirstOrDefault();
                            if (productInWarehouse == null)
                            {
                                await transaction.RollbackAsync();
                                return GenerateErrorResponse($"Failed to update stock for product {item.ProductName}");
                            }
                            productInWarehouse.AvailableQuantity -= item.Quantity;
                            await _warehouseService.UpdateWarehouseAsync(productInWarehouse);
                        }

                        await _unitOfWork.SaleOrderRepository.UpdateAsync(SaleOrder);
                        await transaction.CommitAsync(); 

                        return GenerateSuccessResponse(SaleOrder, $"Approving SaleOrder with id = {orderId} successfully");
                    }
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return GenerateErrorResponse(ex.Message);
                }
            }
        }
        public async Task<ResponseDTO<SaleOrderVM>> RejectSaleOrderAsync(int orderId)
        {
            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    var SaleOrder = await _unitOfWork.SaleOrderRepository.GetObjectAsync(o => o.Id == orderId, new string[] { "OrderDetails" });
                    if (SaleOrder == null)
                    {
                        return GenerateErrorResponse($"SaleOrder with id = {orderId} is not found!");
                    }
                    else
                    {
                        await _notificationService.NotifyForRejectOrderOrderAsync(SaleOrder.SaleOrderCode, SaleOrder.BranchId.Value);

                        SaleOrder.OrderStatus = (int)OrderStatus.PENDING;
                        SaleOrder.BranchId = null;

                        await _unitOfWork.SaleOrderRepository.UpdateAsync(SaleOrder);
                        await transaction.CommitAsync();

                        return GenerateSuccessResponse(SaleOrder, $"Rejecting SaleOrder with id = {orderId} successfully");
                    }
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return GenerateErrorResponse(ex.Message);
                }  
            }

        }
    }
}