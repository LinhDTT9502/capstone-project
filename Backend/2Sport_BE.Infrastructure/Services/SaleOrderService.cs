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

namespace _2Sport_BE.Infrastructure.Services
{
    public interface ISaleOrderService
    {
        #region IRead
        Task<ResponseDTO<List<SaleOrderVM>>> GetAllSaleOrdersAsync();
        Task<ResponseDTO<SaleOrderVM>> GetSaleOrderDetailByIdAsync(int saleOrderId);
        Task<ResponseDTO<List<SaleOrderVM>>> GetSaleOrdersByStatus(int? orderStatus, int? paymentStatus);
        Task<ResponseDTO<List<SaleOrderVM>>> GetSaleOrdersOfUserAsync(int userId);
        Task<ResponseDTO<List<SaleOrderVM>>> GetSaleOrdersByDateRangeAndStatus(DateTime? fromDate, DateTime? toDate, int? orderStatus);
        //=============================================

        Task<SaleOrder> GetSaleOrderByIdFromUserAsync(int SaleOrderId, int userId);
        Task<ResponseDTO<RevenueVM>> GetSaleOrdersRevenue(int? branchId, int? SaleOrderType, DateTime? from, DateTime? to, int? status);
        Task<ResponseDTO<SaleOrderVM>> GetSaleOrderBySaleOrderCode(string SaleOrderCode);
        //Task<ResponseDTO<List<SaleOrderVM>>> GetSaleOrdersByMonthAndStatus(DateTime startDate, DateTime endDate, int status);
        Task<IQueryable<SaleOrder>> GetAllSaleOrderQueryableAsync();
        #endregion
        #region ICreate_IUpdate_IDelete
        Task<ResponseDTO<SaleOrderVM>> CreatetSaleOrderAsync(SaleOrderCM SaleOrderCM);
        Task<ResponseDTO<SaleOrderVM>> UpdateSaleOrderAsync(int SaleOrderId, SaleOrderUM SaleOrderUM);
        Task<ResponseDTO<int>> UpdateSaleOrderStatusAsync(int id, int status);
        Task<ResponseDTO<int>> DeleteSaleOrderAsync(int id);
        #endregion
        #region CRUD_Order
        Task<SaleOrder> GetSaleOrderById(int orderId);
        Task<SaleOrder> GetSaleOrderByCode(string orderCode);
        Task<int> UpdateSaleOrder(SaleOrder saleOrder);

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
        private INotificationService _notificationService;
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
                var saleOrderVMs = new List<SaleOrderVM>();

                foreach (var item in query)
                {
                    var saleOrderVM = MapSaleOrderToSaleOrderVM(item);
                    saleOrderVMs.Add(saleOrderVM);
                }

                response.IsSuccess = true;
                response.Message = "Query successfully";
                response.Data = saleOrderVMs;
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

                var saleOrderVM = MapSaleOrderToSaleOrderVM(saleOrder);


                response.IsSuccess = true;
                response.Message = "Query successfully";
                response.Data = saleOrderVM;
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
                var saleOrderVMs = new List<SaleOrderVM>();

                foreach (var item in query)
                {
                    var saleOrderVM = MapSaleOrderToSaleOrderVM(item);
                    saleOrderVMs.Add(saleOrderVM);
                }
                response.IsSuccess = true;
                response.Message = "Query successfully";
                response.Data = saleOrderVMs;
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
                var query = await _unitOfWork.SaleOrderRepository.GetAllAsync(new string[] { "User, OrderDetails" });
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
                var saleOrderVMs = new List<SaleOrderVM>();

                foreach (var item in query)
                {
                    var saleOrderVM = MapSaleOrderToSaleOrderVM(item);
                    saleOrderVMs.Add(saleOrderVM);
                }

                response.IsSuccess = true;
                response.Message = "Query successfully";
                response.Data = saleOrderVMs;
                return response;

            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
                return response;
            }
        }
        public async Task<ResponseDTO<List<SaleOrderVM>>> GetSaleOrdersByMonth(DateTime month)
        {
            var response = new ResponseDTO<List<SaleOrderVM>>();
            int targetMonth = month.Month;
            int targetYear = month.Year;
            try
            {
                var query = await _unitOfWork.SaleOrderRepository
                    .GetAsync(o => o.CreatedAt.HasValue &&
                        o.CreatedAt.Value.Month == targetMonth &&
                        o.CreatedAt.Value.Year == targetYear,
                        "User,OrderDetails");
                if (query == null || !query.Any())
                {
                    response.IsSuccess = false;
                    response.Message = "SaleOrders are not found";
                    return response;
                }

                var saleOrderVMs = new List<SaleOrderVM>();

                foreach (var item in query)
                {
                    var saleOrderVM = MapSaleOrderToSaleOrderVM(item);
                    saleOrderVMs.Add(saleOrderVM);
                }

                response.IsSuccess = true;
                response.Message = "Query successfully";
                response.Data = saleOrderVMs;
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
        //============================
       
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
            var response = new ResponseDTO<SaleOrderVM>();
            try
            {
                var saleOder = await _unitOfWork.SaleOrderRepository.GetObjectAsync(o => o.OrderCode.Equals(SaleOrderCode), "User,OrderDetails");
                if (saleOder == null)
                {
                    response.IsSuccess = false;
                    response.Message = "SaleOrders are not found";
                    return response;
                }

                var result = MapSaleOrderToSaleOrderVM(saleOder);
                response.IsSuccess = true;
                response.Message = "Query successfully";
                response.Data = result;
                return response;

            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
                return response;
            }
        }
        #endregion
        #region Create_Update_Delete_SaleOrder

        public async Task<ResponseDTO<SaleOrderVM>> CreatetSaleOrderAsync(SaleOrderCM saleOrderCM)
        {
            var response = new ResponseDTO<SaleOrderVM>();
            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    User user = null;
                    ShipmentDetail shipmentDetail = null;
                    if (saleOrderCM.UserID.HasValue && saleOrderCM.UserID != 0)
                    {
                        user = await _unitOfWork.UserRepository
                                        .GetObjectAsync(u => u.Id == saleOrderCM.UserID);
                        if (user == null)
                        {
                            return GenerateErrorResponse($"User with Id = {saleOrderCM.UserID} is not found!");
                        }
                        else
                        {
                            shipmentDetail = await _unitOfWork.ShipmentDetailRepository
                                    .GetObjectAsync(s => s.Id == saleOrderCM.ShipmentDetailID && s.UserId == user.Id);
                            if (shipmentDetail == null)
                                return GenerateErrorResponse($"ShipmenDetail with Id = {saleOrderCM.ShipmentDetailID} is not found!");
                        }
                    }

                    string DeliveryMethod = saleOrderCM.DeliveryMethod;
                    if (string.IsNullOrEmpty(DeliveryMethod) || !_deliveryMethodService.IsValidMethod(DeliveryMethod))
                        return GenerateErrorResponse("Delivery Method is invalid");

                    if (DeliveryMethod.Equals("STORE_PICKUP") && !saleOrderCM.BranchId.HasValue)
                        return GenerateErrorResponse("Branch is required!");

                    var toCreate = new SaleOrder
                    {
                        OrderCode = _methodHelper.GenerateOrderCode(),
                        OrderStatus = (int?)OrderStatus.PENDING,
                        PaymentStatus = (int)PaymentStatus.IsWating,
                        Address = saleOrderCM.Address,
                        FullName = saleOrderCM.FullName,
                        Email = saleOrderCM.Email,
                        ContactPhone = saleOrderCM.ContactPhone,
                        CreatedAt = DateTime.Now,
                        Note = saleOrderCM.Note,
                        DeliveryMethod = saleOrderCM.DeliveryMethod,
                        DateOfReceipt = saleOrderCM.DateOfReceipt ?? DateTime.Now.AddDays(3),
                        BranchId = saleOrderCM.BranchId ?? null,
                        Gender = saleOrderCM.Gender,
                    };

                    if (user != null && shipmentDetail != null)
                        MapUserToSaleOrder(user, shipmentDetail, toCreate);

                    if (DeliveryMethod.Equals("HOME_DELIVERY"))
                    {
                        toCreate.PaymentMethodId = (int)OrderMethods.COD;
                    }
                    await _unitOfWork.SaleOrderRepository.InsertAsync(toCreate);

                    decimal subTotal = 0;
                    foreach (var item in saleOrderCM.SaleOrderDetailCMs)
                    {
                        var detail = new OrderDetail
                        {
                            SaleOrderId = toCreate.Id,
                            Quantity = item.Quantity,
                            CreatedAt = DateTime.Now,
                        };
                        var branch = await _unitOfWork.BranchRepository.GetObjectAsync(b => b.Id == saleOrderCM.BranchId);
                        if (branch != null)
                        {
                            detail.BranchId = branch.Id;
                            detail.BranchName = branch.BranchName;

                            var reduceInWarehouse = await FindAndReduceQuantityInWarehouse(item, branch.Id);
                            if (!reduceInWarehouse)
                            {
                                await transaction.RollbackAsync();
                                return GenerateErrorResponse($"Failed to update stock for product {item.ProductName}");
                            }
                        }
                        var product = await _unitOfWork.ProductRepository.GetObjectAsync(p => p.Id == item.ProductId);
                        if (product != null)
                            MapProductToSaleOrderDetail(detail, product);

                        await _unitOfWork.OrderDetailRepository.InsertAsync(detail);
                        subTotal += (decimal)(product.Price * item.Quantity);
                    }

                    toCreate.SubTotal = subTotal;
                    toCreate.TranSportFee = 0;
                    toCreate.TotalAmount = (decimal)toCreate.SubTotal;

                    await _unitOfWork.SaleOrderRepository.UpdateAsync(toCreate);

                    var result = MapSaleOrderToSaleOrderVM(toCreate);

                    
                    //Send notifications to Admmin
                    await _notificationService.NotifyForCreatingNewOrderAsync(toCreate.OrderCode);
                    await _mailService.SendSaleOrderInformationToCustomer(toCreate, toCreate.OrderDetails.ToList(), toCreate.Email);
                    await transaction.CommitAsync();

                    response.IsSuccess = true;
                    response.Message = $"SaleOrder processed successfully";
                    response.Data = result;
                    return response;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return GenerateErrorResponse(ex.Message);
                }
            }
        }
        private SaleOrderVM MapSaleOrderToSaleOrderVM(SaleOrder saleOrder)
        {
            var result = _mapper.Map<SaleOrderVM>(saleOrder);
            result.PaymentStatus = Enum.GetName((PaymentStatus)saleOrder.PaymentStatus);
            result.OrderStatus = Enum.GetName((OrderStatus)saleOrder.OrderStatus);
            result.PaymentMethod = saleOrder.PaymentMethodId.HasValue
                                ? Enum.GetName(typeof(OrderMethods), saleOrder.PaymentMethodId.Value)
                                : "Unknown PaymentMethod";
            result.SaleOrderId = saleOrder.Id;

            result.SaleOrderDetailVMs = saleOrder.OrderDetails.Select(od => new SaleOrderDetailVM()
            {
                ProductId = od.ProductId,
                ProductName = od.ProductName,
                UnitPrice = od.UnitPrice,
                Quantity = od.Quantity,
                TotalPrice = od.TotalAmount,
                ImgAvatarPath = od.ImgAvatarPath,
            }).ToList();
            return result;
        }
        private void MapUserToSaleOrder(User user, ShipmentDetail shipmentDetail, SaleOrder saleOrder)
        {
            saleOrder.Address = shipmentDetail.Address;
            saleOrder.FullName = shipmentDetail.FullName;
            saleOrder.Email = shipmentDetail.Email;
            saleOrder.ContactPhone = shipmentDetail.PhoneNumber;
            saleOrder.UserId = user.Id;
        }
        private void MapProductToSaleOrderDetail(OrderDetail detail, Product product)
        {
            detail.ProductId = product.Id;
            detail.ProductName = product.ProductName;
            detail.UnitPrice = product.Price;
            detail.ImgAvatarPath = product.ImgAvatarPath;
        }
        public async Task<ResponseDTO<SaleOrderVM>> UpdateSaleOrderAsync(int saleOrderId, SaleOrderUM saleOrderUM)
        {
            var response = new ResponseDTO<SaleOrderVM>();
            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    var toUpdate = await _unitOfWork.SaleOrderRepository.GetObjectAsync(o => o.Id == saleOrderId, new string[] { "OrderDetails" });
                    if (toUpdate == null)
                        return GenerateErrorResponse($"SaleOrder with id {saleOrderId} not found!");
                    
                    MapSaleOrderUmToSaleOrder(saleOrderUM, toUpdate);
                    await _unitOfWork.SaleOrderRepository.UpdateAsync(toUpdate);

                    var orderDetails = await _unitOfWork.OrderDetailRepository
                        .GetAndIncludeAsync(od => od.SaleOrderId == saleOrderId);
                    var orderDetailUMs = saleOrderUM.SaleOrderDetailUMs;

                    var mapToOrderDetails = _mapper.Map<List<OrderDetail>>(orderDetailUMs);
                    var differences = GetDifferencesOrderDetail(orderDetails.ToList(), mapToOrderDetails);
                    if (differences.Any())//Xoa cac san pham ko co trong update
                    {
                        foreach( var difference in differences)
                        {
                            await _unitOfWork.OrderDetailRepository.DeleteAsync(difference);
                        }
                    }
                    decimal supTotal = 0;
                    foreach (var updatedItem in orderDetailUMs)
                    {
                        OrderDetail orderDetail = orderDetails.FirstOrDefault(o => o.ProductId == updatedItem.ProductId);

                        if(orderDetail != null)//Co san pham do roi, chi update quantity
                        {
                            orderDetail.UnitPrice = updatedItem.UnitPrice;
                            orderDetail.Quantity = updatedItem.Quantity;
                            orderDetail.UpdatedAt = DateTime.Now;
                            supTotal += (decimal)orderDetail.TotalAmount;
                            await _unitOfWork.OrderDetailRepository.UpdateAsync(orderDetail);
                        }
                        else if(orderDetail == null)//Tao moi detail, cap nhat totalAmount
                        {
                            var product = await _unitOfWork.ProductRepository.GetObjectAsync(p => p.Id == updatedItem.ProductId);

                            var newOrderDetail = new OrderDetail()
                            {
                                ProductId = updatedItem.ProductId,
                                ProductName = updatedItem.ProductName,
                                ProductCode = updatedItem.ProductCode,
                                Quantity = updatedItem.Quantity,
                                UnitPrice = updatedItem.UnitPrice,
                                CreatedAt = DateTime.Now,
                                SaleOrderId = toUpdate.Id,
                                ImgAvatarPath = product != null ? product.ImgAvatarPath : null,
                            };

                            supTotal += (decimal)newOrderDetail.TotalAmount;
                            await _unitOfWork.OrderDetailRepository.InsertAsync(newOrderDetail);
                        }            
                    }
                    toUpdate.SubTotal = saleOrderUM.SubTotal != 0 ? saleOrderUM.SubTotal : supTotal;
                    toUpdate.TranSportFee = saleOrderUM.TransportFee != 0 ? saleOrderUM.TransportFee : 0;
                    toUpdate.TotalAmount = saleOrderUM.TotalAmount != 0 ? saleOrderUM.TotalAmount : (decimal)(toUpdate.SubTotal + toUpdate.TranSportFee);
                    await _unitOfWork.SaleOrderRepository.UpdateAsync(toUpdate);

                    await transaction.CommitAsync();
                    //Return
                    var result = MapSaleOrderToSaleOrderVM(toUpdate);

                    response.IsSuccess = true;
                    response.Message = $"Update SaleOrder processed successfully";
                    response.Data = result;
                    return response;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return GenerateErrorResponse(ex.Message);
                }
            }
        }
        public List<OrderDetail> GetDifferencesOrderDetail(List<OrderDetail> list1, List<OrderDetail> listUM)
        {


            var idsInList2 = listUM.Select(p => p.ProductId).ToHashSet();

            // Tìm các sản phẩm có trong list1 mà không có trong list2
            var uniqueInList1 = list1.Where(p => !idsInList2.Contains(p.ProductId)).ToList();
            return uniqueInList1;
        }
        private void MapSaleOrderUmToSaleOrder(SaleOrderUM saleOrderUM, SaleOrder saleOrder)
        {
            //Customer info
            saleOrder.Email = saleOrderUM.Email;
            saleOrder.FullName = saleOrderUM.FullName;
            saleOrder.ContactPhone = saleOrderUM.ContactPhone;
            saleOrder.Address = saleOrderUM.Address;

            //Status info
            saleOrder.OrderStatus = saleOrderUM.OrderStatus;
            saleOrder.PaymentStatus = saleOrderUM.PaymentStatus;
            //Date info
            saleOrder.DateOfReceipt = saleOrderUM.DateOfReceipt;
            saleOrder.UpdatedAt = DateTime.Now;
            //Others info
            saleOrder.Note = saleOrderUM.Note;
            saleOrder.DeliveryMethod = saleOrderUM.DeliveryMethod;
            saleOrder.BranchId = saleOrderUM.BranchId != 0 ? saleOrderUM.BranchId : null;
            saleOrder.PaymentMethodId = saleOrderUM.PaymentMethodId;
            saleOrder.DateOfReceipt = saleOrderUM.DateOfReceipt;
        }
        public async Task<ResponseDTO<int>> UpdateSaleOrderStatusAsync(int id, int orderStatus)
        {
            var response = new ResponseDTO<int>();
            try
            {
                var SaleOrder = await _unitOfWork.SaleOrderRepository.GetObjectAsync(o => o.Id == id);
                if (SaleOrder == null)
                {
                    response.IsSuccess = false;
                    response.Message = $"SaleOrder with id = {id} is not found!";
                    response.Data = 0;
                }
                else
                {
                    SaleOrder.OrderStatus = orderStatus;

                    if (orderStatus == (int)OrderStatus.COMPLETED)
                    {
                        await _customerDetailService.UpdateLoyaltyPoints(SaleOrder.Id);
                    }

                    await _unitOfWork.SaleOrderRepository.UpdateAsync(SaleOrder);


                    response.IsSuccess = true;
                    response.Message = $"Change status of SaleOrder with id = {id} successfully";
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
        public async Task<ResponseDTO<int>> DeleteSaleOrderAsync(int id)
        {
            var response = new ResponseDTO<int>();
            try
            {
                var SaleOrder = await _unitOfWork.SaleOrderRepository.GetObjectAsync(o => o.Id == id);
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
        #endregion
       
        private async Task<OrderDetail> UpdateSaleOrderDetailFromSaleOrderDetailUM(OrderDetail orderDetail,int saleOrderId, SaleOrderDetailUM detailUM, bool isExisted)
        {

            if (!isExisted)
            {
                return new OrderDetail()
                {
                    ProductId = detailUM.ProductId,
                    ProductName = detailUM.ProductName,
                    ProductCode = detailUM.ProductCode,
                    ImgAvatarPath = detailUM.ImgAvatarPath,
                    CreatedAt = DateTime.Now,
                    SaleOrderId = saleOrderId,
                    Quantity = detailUM.Quantity,
                };
            }
            else
            {
                int difference = (int)(detailUM.Quantity - orderDetail.Quantity);
                orderDetail.Quantity = detailUM.Quantity;
                return orderDetail;
            }
            // Cập nhật kho dựa trên sự chênh lệch
            /*bool isReturningStock = difference < 0;
            bool stockUpdated = await UpdateStock(new SaleOrderDetailUM { ProductId = detailUM.ProductId, Quantity = Math.Abs(difference) }, branchId, isReturningStock);*/
            /*if (!stockUpdated)
            {
                return false;
            }*/
        }
        private async Task<bool> UpdateStock(SaleOrderDetailUM detailUM, int branchId, bool isReturningStock)
        {
            var warehouse = await _unitOfWork.WarehouseRepository
                .GetObjectAsync(w => w.ProductId == detailUM.ProductId && w.BranchId == branchId);

            if (warehouse == null || warehouse.TotalQuantity < detailUM.Quantity || warehouse.AvailableQuantity < detailUM.Quantity)
            {
                return false;
            }

            // Cập nhật kho dựa trên loại yêu cầu
            int quantityAdjustment = isReturningStock ? detailUM.Quantity.Value : -detailUM.Quantity.Value;
            warehouse.AvailableQuantity += quantityAdjustment;
            warehouse.TotalQuantity += quantityAdjustment;

            await _unitOfWork.WarehouseRepository.UpdateAsync(warehouse);
            return true;
        }
        private async Task<bool> FindAndReduceQuantityInWarehouse(SaleOrderDetailCM saleOrderDetailCM, int branchId)
        {
            var warehouse = await _unitOfWork.WarehouseRepository
                .GetObjectAsync(w => w.ProductId == saleOrderDetailCM.ProductId && w.BranchId == branchId);

            if (warehouse == null || warehouse.TotalQuantity < saleOrderDetailCM.Quantity || warehouse.AvailableQuantity < saleOrderDetailCM.Quantity)
            {
                return false;
            }

            // Cập nhật kho dựa trên loại yêu cầu
            warehouse.AvailableQuantity -= saleOrderDetailCM.Quantity;

            await _unitOfWork.WarehouseRepository.UpdateAsync(warehouse);
            return true;
        }
        #region CRUD_SALE_ORDER_BASIC
        public async Task<SaleOrder> GetSaleOrderById(int orderId)
        {
            return await _unitOfWork.SaleOrderRepository.GetObjectAsync(o => o.Id == orderId);
        }
        public async Task<SaleOrder> GetSaleOrderByCode(string orderCode)
        {
            return await _unitOfWork.SaleOrderRepository.GetObjectAsync(o => o.OrderCode == orderCode);
        }
        public async Task<SaleOrder> GetSaleOrderByIdFromUserAsync(int SaleOrderId, int userId)
        {
            return await _unitOfWork.SaleOrderRepository.GetObjectAsync(_ => _.Id == SaleOrderId && _.UserId == userId);
        }
        public async Task<IQueryable<SaleOrder>> GetAllSaleOrderQueryableAsync()
        {
            var query = await _unitOfWork.SaleOrderRepository.GetAllAsync();

            return query.AsQueryable();
        }

        public async Task<int> UpdateSaleOrder(SaleOrder saleOrder)
        {
            await _unitOfWork.SaleOrderRepository.UpdateAsync(saleOrder);
            return 1;
        }
        #endregion
        private ResponseDTO<SaleOrderVM> GenerateErrorResponse(string message)
        {
            return new ResponseDTO<SaleOrderVM>()
            {
                IsSuccess = false,
                Message = message,
                Data = null
            };
        }
    }
}