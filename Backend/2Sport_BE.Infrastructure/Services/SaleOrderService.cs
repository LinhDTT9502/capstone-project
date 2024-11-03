using _2Sport_BE.Infrastructure.Helpers;
using _2Sport_BE.Repository.Interfaces;
using _2Sport_BE.Repository.Models;
using _2Sport_BE.Infrastructure.DTOs;
using _2Sport_BE.Infrastructure.Enums;
using AutoMapper;
using System.Net;
using Microsoft.CodeAnalysis.Semantics;

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
        public SaleOrderService(IUnitOfWork unitOfWork,
            ICustomerService customerDetailService,
            IWarehouseService warehouseService,
            IMethodHelper methodHelper,
            IDeliveryMethodService deliveryMethodService,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _customerDetailService = customerDetailService;
            _warehouseService = warehouseService;
            _methodHelper = methodHelper;
            _deliveryMethodService = deliveryMethodService;
            _mapper = mapper;
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
                    var saleOrderVM = _mapper.Map<SaleOrderVM>(item);
                    saleOrderVM.PaymentStatus = Enum.GetName((PaymentStatus)item.PaymentStatus);
                    saleOrderVM.OrderStatus = Enum.GetName((OrderStatus)item.OrderStatus);
                    saleOrderVM.PaymentMethod = item.PaymentMethodId != null
                                    ? Enum.GetName(typeof(OrderMethods), item.PaymentMethodId) : null;
                    saleOrderVM.SaleOrderId = item.Id;
                    saleOrderVM.SaleOrderDetailVMs = item.OrderDetails.Select(od => new SaleOrderDetailVM()
                    {
                        ProductId = od.ProductId,
                        ProductName = od.ProductName,
                        UnitPrice = od.UnitPrice,
                        Quantity = od.Quantity,
                        TotalPrice = od.TotalPrice,
                    }).ToList();

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

                var saleOrderVM = _mapper.Map<SaleOrderVM>(saleOrder);
                saleOrderVM.PaymentStatus = Enum.GetName((PaymentStatus)saleOrder.PaymentStatus);
                saleOrderVM.OrderStatus = Enum.GetName((OrderStatus)saleOrder.OrderStatus);
                saleOrderVM.PaymentMethod = saleOrder.PaymentMethodId != null
                                ? Enum.GetName(typeof(OrderMethods), saleOrder.PaymentMethodId) : null;
                saleOrderVM.SaleOrderId = saleOrder.Id;
                saleOrderVM.SaleOrderDetailVMs = saleOrder.OrderDetails.Select(od => new SaleOrderDetailVM()
                {
                    ProductId = od.ProductId,
                    ProductName = od.ProductName,
                    UnitPrice = od.UnitPrice,
                    Quantity = od.Quantity,
                    TotalPrice = od.TotalPrice,
                }).ToList();

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
                var query = await _unitOfWork.SaleOrderRepository.GetAsync(o => o.UserId == userId, "User, SaleOrderDetails");
                if (query == null || !query.Any())
                {
                    response.IsSuccess = false;
                    response.Message = "SaleOrders are not found";
                    return response;
                }
                var saleOrderVMs = new List<SaleOrderVM>();

                foreach (var item in query)
                {

                    var saleOrderVM = _mapper.Map<SaleOrderVM>(item);
                    saleOrderVM.PaymentStatus = Enum.GetName((OrderStatus)item.PaymentStatus);
                    saleOrderVM.OrderStatus = Enum.GetName((OrderStatus)item.OrderStatus);
                    saleOrderVM.PaymentMethod = item.PaymentMethodId != null
                                 ? Enum.GetName(typeof(OrderMethods), item.PaymentMethodId) : null;

                    var saleOrderDetail = _unitOfWork.OrderDetailRepository.GetAsync(od => od.SaleOrderId == item.Id);
                    saleOrderVM.SaleOrderDetailVMs = item.OrderDetails.Select(od => new SaleOrderDetailVM()
                    {
                        ProductId = od.ProductId,
                        ProductName = od.ProductName,
                        UnitPrice = od.UnitPrice,
                        Quantity = od.Quantity,
                        TotalPrice = od.TotalPrice,
                    }).ToList();

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

                    var saleOrderVM = _mapper.Map<SaleOrderVM>(item);
                    saleOrderVM.PaymentStatus = Enum.GetName((OrderStatus)item.PaymentStatus);
                    saleOrderVM.OrderStatus = Enum.GetName((OrderStatus)item.OrderStatus);
                    saleOrderVM.PaymentMethod = item.PaymentMethodId != null
                         ? Enum.GetName(typeof(OrderMethods), item.PaymentMethodId) : null;

                    var saleOrderDetail = _unitOfWork.OrderDetailRepository.GetAsync(od => od.SaleOrderId == item.Id);
                    saleOrderVM.SaleOrderDetailVMs = item.OrderDetails.Select(od => new SaleOrderDetailVM()
                    {
                        ProductId = od.ProductId,
                        ProductName = od.ProductName,
                        UnitPrice = od.UnitPrice,
                        Quantity = od.Quantity,
                        TotalPrice = od.TotalPrice,
                    }).ToList();

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

                    var saleOrderVM = _mapper.Map<SaleOrderVM>(item);
                    saleOrderVM.PaymentStatus = Enum.GetName((OrderStatus)item.PaymentStatus);
                    saleOrderVM.OrderStatus = Enum.GetName((OrderStatus)item.OrderStatus);
                    saleOrderVM.PaymentMethod = item.PaymentMethodId != null
                         ? Enum.GetName(typeof(OrderMethods), item.PaymentMethodId) : null;
                    var saleOrderDetail = _unitOfWork.OrderDetailRepository.GetAsync(od => od.SaleOrderId == item.Id);
                    saleOrderVM.SaleOrderDetailVMs = item.OrderDetails.Select(od => new SaleOrderDetailVM()
                    {
                        ProductId = od.ProductId,
                        ProductName = od.ProductName,
                        UnitPrice = od.UnitPrice,
                        Quantity = od.Quantity,
                        TotalPrice = od.TotalPrice,
                    }).ToList();

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

                var result = _mapper.Map<SaleOrderVM>(saleOder);
                result.PaymentStatus = Enum.GetName((OrderStatus)saleOder.PaymentStatus);
                result.OrderStatus = Enum.GetName((OrderStatus)saleOder.OrderStatus);
                var saleOrderDetail = _unitOfWork.OrderDetailRepository.GetAsync(od => od.SaleOrderId == saleOder.Id);
                result.SaleOrderDetailVMs = saleOder.OrderDetails.Select(od => new SaleOrderDetailVM()
                {
                    ProductId = od.ProductId,
                    ProductName = od.ProductName,
                    UnitPrice = od.UnitPrice,
                    Quantity = od.Quantity,
                    TotalPrice = od.TotalPrice,
                }).ToList();

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
                            response.IsSuccess = false;
                            response.Message = $"User with Id = {saleOrderCM.UserID} is not found!";
                            return response;
                        }
                        else
                        {
                            shipmentDetail = await _unitOfWork.ShipmentDetailRepository
                                    .GetObjectAsync(s => s.Id == saleOrderCM.ShipmentDetailID && s.UserId == user.Id);
                            if (shipmentDetail == null)
                            {
                                response.IsSuccess = false;
                                response.Message = $"ShipmenDetail with Id = {saleOrderCM.ShipmentDetailID} is not found!";
                                return response;
                            }
                        }
                    }

                    string DeliveryMethod = saleOrderCM.DeliveryMethod;
                    if (string.IsNullOrEmpty(DeliveryMethod) || !_deliveryMethodService.IsValidMethod(DeliveryMethod))
                    {
                        response.IsSuccess = false;
                        response.Message = "Delivery Method is invalid";
                        return response;
                    }
                    //Den store lay thi phai co branchId
                    if (DeliveryMethod.Equals("STORE_PICKUP") && !saleOrderCM.BranchId.HasValue)
                    {
                        response.IsSuccess = false;
                        response.Message = "Branch is required!";
                        return response;
                    }
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
                    if(user != null && shipmentDetail != null)
                    {
                        toCreate.Address = shipmentDetail.Address;
                        toCreate.FullName = shipmentDetail.FullName;
                        toCreate.Email = shipmentDetail.Email;
                        toCreate.ContactPhone = shipmentDetail.PhoneNumber;
                        toCreate.UserId = user.Id;
                    }
                    if (DeliveryMethod.Equals("HOME_DELIVERY"))
                    {
                        toCreate.PaymentMethodId = (int)OrderMethods.COD;
                    }
                    await _unitOfWork.SaleOrderRepository.InsertAsync(toCreate);

                    decimal subTotal = 0;
                    foreach (var item in saleOrderCM.SaleOrderDetailCMs)
                    {
                        /*var stockInWarehouse = await _unitOfWork.WarehouseRepository
                                                .GetObjectAsync(p => p.Id == item.WarehouseId, new string[] { "Product, Branch" });

                        if (stockInWarehouse == null || stockInWarehouse.TotalQuantity < item.Quantity)
                        {
                            response.IsSuccess = false;
                            response.Message = $"Not enough stock for product {item.WarehouseId} at branch {stockInWarehouse.Branch.BranchName}";
                            return response;
                        }
                        //update available quantity in wareHouse
                        stockInWarehouse.AvailableQuantity -= item.Quantity;
                        await _unitOfWork.WarehouseRepository.UpdateAsync(stockInWarehouse);*/
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
                                response.IsSuccess = false;
                                response.Message = $"Failed to update stock for product {item.ProductName}";
                                await transaction.RollbackAsync();
                                return response;
                            }
                        }
                        var product = await _unitOfWork.ProductRepository.GetObjectAsync(p => p.Id == item.ProductId);
                        if (product != null)
                        {
                            detail.ProductId = product.Id;
                            detail.ProductName = product.ProductName;
                            detail.UnitPrice = product.Price;
                        }

                        await _unitOfWork.OrderDetailRepository.InsertAsync(detail);
                        subTotal += (decimal)(product.Price * item.Quantity);
                    }

                    toCreate.SubTotal = subTotal;
                    toCreate.TranSportFee = 0;
                    toCreate.TotalAmount = (decimal)(toCreate.SubTotal); // if we have coupon, applying to TotalAmount
                    await _unitOfWork.SaleOrderRepository.UpdateAsync(toCreate);

                    // Transaction submit
                    await transaction.CommitAsync();
                    //Return
                    var result = _mapper.Map<SaleOrderVM>(toCreate);
                    result.PaymentStatus = Enum.GetName((PaymentStatus)toCreate.PaymentStatus);
                    result.OrderStatus = Enum.GetName((OrderStatus)toCreate.OrderStatus);
                    result.PaymentMethod = Enum.GetName((OrderMethods)toCreate.PaymentMethodId);
                    result.SaleOrderId = toCreate.Id;
                    var saleOrderDetail = _unitOfWork.OrderDetailRepository.GetAsync(od => od.SaleOrderId == toCreate.Id);
                    result.SaleOrderDetailVMs = toCreate.OrderDetails.Select(od => new SaleOrderDetailVM()
                    {
                        ProductId = od.ProductId,
                        ProductName = od.ProductName,
                        UnitPrice = od.UnitPrice,
                        Quantity = od.Quantity,
                        TotalPrice = od.TotalPrice,
                    }).ToList();
                    response.IsSuccess = true;
                    response.Message = $"SaleOrder processed successfully";
                    response.Data = result;
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
        public async Task<ResponseDTO<SaleOrderVM>> UpdateSaleOrderAsync(int SaleOrderId, SaleOrderUM SaleOrderUM)
        {
            var response = new ResponseDTO<SaleOrderVM>();
            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    #region Validate Input
                    var toUpdate = await _unitOfWork.SaleOrderRepository.GetObjectAsync(o => o.Id == SaleOrderId, new string[] { "SaleOrderDetails" });
                    if (toUpdate == null)
                    {
                        response.IsSuccess = false;
                        response.Message = $"SaleOrder with id {SaleOrderId} not found!";
                        return response;
                    }
                    #endregion

                    #region UpdateOrderInfor
                    //Customer info
                    toUpdate.Email = SaleOrderUM.Email;
                    toUpdate.FullName = SaleOrderUM.FullName;
                    toUpdate.ContactPhone = SaleOrderUM.ContactPhone;
                    toUpdate.Address = SaleOrderUM.Address;
                    //Cost info
                    toUpdate.SubTotal = SaleOrderUM.SubTotal;
                    toUpdate.TranSportFee = SaleOrderUM.TransportFee;
                    toUpdate.TotalAmount = SaleOrderUM.TotalAmount;
                    //Status info
                    toUpdate.OrderStatus = SaleOrderUM.OrderStatus;
                    //Date info
                    toUpdate.DateOfReceipt = SaleOrderUM.DateOfReceipt;
                    toUpdate.UpdatedAt = DateTime.Now;
                    //Others info
                    toUpdate.Note = SaleOrderUM.Note;
                    toUpdate.DeliveryMethod = SaleOrderUM.DeliveryMethod;
                    toUpdate.BranchId = SaleOrderUM.BranchId;

                    await _unitOfWork.SaleOrderRepository.UpdateAsync(toUpdate);
                    #endregion
                    #region Update OrderDetail
                    foreach (var updatedItem in SaleOrderUM.SaleOrderDetailUMs)
                    {
                        var updateDetail = await UpdateOrderDetail(updatedItem, (int)toUpdate.BranchId);
                                  
                        if (!updateDetail)
                        {
                            response.IsSuccess = false;
                            response.Message = $"Failed to update order detail for product {updatedItem.ProductName}";
                            await transaction.RollbackAsync();
                            return response;
                        }

                    }
                    #endregion
                    await transaction.CommitAsync();
                    //Return
                    #region Result
                    var result = _mapper.Map<SaleOrderVM>(toUpdate);
                    result.PaymentStatus = Enum.GetName((OrderStatus)toUpdate.PaymentStatus);
                    result.OrderStatus = Enum.GetName((OrderStatus)toUpdate.OrderStatus);
                    var saleOrderDetail = _unitOfWork.OrderDetailRepository.GetAsync(od => od.SaleOrderId == toUpdate.Id);
                    result.SaleOrderDetailVMs = toUpdate.OrderDetails.Select(od => new SaleOrderDetailVM()
                    {
                        ProductId = od.ProductId,
                        ProductName = od.ProductName,
                        UnitPrice = od.UnitPrice,
                        Quantity = od.Quantity,
                        TotalPrice = od.TotalPrice,
                    }).ToList();
                    #endregion
                    response.IsSuccess = true;
                    response.Message = $"Update SaleOrder processed successfully";
                    response.Data = result;
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
       
        private async Task<bool> UpdateOrderDetail(SaleOrderDetailUM detailUM, int branchId)
        {
            var detail = await _unitOfWork.OrderDetailRepository
                                .GetObjectAsync(d => d.ProductId == detailUM.ProductId && d.ProductName == detailUM.ProductName);

            if (detail == null)
            {
                return false;
            }

            int difference = (int)(detailUM.Quantity - detail.Quantity);

            if (difference == 0)
            {
                return false;
            }

            detail.Quantity = detailUM.Quantity;

            // Cập nhật kho dựa trên sự chênh lệch
            bool isReturningStock = difference < 0;
            bool stockUpdated = await UpdateStock(new SaleOrderDetailUM { ProductId = detailUM.ProductId, Quantity = Math.Abs(difference) }, branchId, isReturningStock);

            if (!stockUpdated)
            {
                return false;
            }
            await _unitOfWork.OrderDetailRepository.UpdateAsync(detail);
            return true;
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

    }
}