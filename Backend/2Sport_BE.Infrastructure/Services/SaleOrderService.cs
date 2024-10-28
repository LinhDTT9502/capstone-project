using _2Sport_BE.Infrastructure.Helpers;
using _2Sport_BE.Repository.Interfaces;
using _2Sport_BE.Repository.Models;
using _2Sport_BE.Infrastructure.DTOs;
using _2Sport_BE.Infrastructure.Enums;

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
        #region Process_PayOS_Response
        Task<ResponseDTO<int>> ProcessCancelledSaleOrder(PaymentResponse paymentResponse);
        Task<ResponseDTO<int>> ProcessCompletedSaleOrder(PaymentResponse paymentResponse);
        #endregion
    }
    public class SaleOrderService : ISaleOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICustomerService _customerDetailService;
        private readonly IWarehouseService _warehouseService;
        private readonly IMethodHelper _methodHelper;
        public SaleOrderService(IUnitOfWork unitOfWork,
            ICustomerService customerDetailService,
            IWarehouseService warehouseService,
            IMethodHelper methodHelper)
        {
            _unitOfWork = unitOfWork;
            _customerDetailService = customerDetailService;
            _warehouseService = warehouseService;
            _methodHelper = methodHelper;
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
                var result = new List<SaleOrderVM>();

                foreach (var item in query)
                {
                    var SaleOrderVM = new SaleOrderVM()
                    {
                        SaleOrderID = item.Id,
                        SaleOrderCode = item.OrderCode,
                        UserID = item.UserId,
                        SubTotal = item.SubTotal,
                        TransportFee = item.TranSportFee,
                        TotalAmount = item.TotalAmount,
                        OrderStatus = Enum.GetName((OrderStatus)item.OrderStatus),
                        PaymentStatus = Enum.GetName((OrderStatus)item.PaymentStatus),
                        CreatedAt = item.CreatedAt,
                        PaymentMethodId = item.PaymentMethodId,
                        Email = item.Email,
                        Address = item.Address,
                        ContactPhone = item.ContactPhone,
                        FullName = item.FullName,
                        SaleOrderDetailVMs = new List<SaleOrderDetailVM>()
                    };

                    foreach (var detail in item.OrderDetails)
                    {

                        SaleOrderVM.SaleOrderDetailVMs.Add(new SaleOrderDetailVM()
                        {
                            Quantity = detail.Quantity,
                            TotalPrice = detail.TotalPrice,
                            Price = detail.Price,
                            BranchId = detail.BranchId,
                            ProductId = detail.ProductId,
                            CreatedAt = detail.CreatedAt,
                            BranchName = detail.BranchName,
                            ProductName = detail.ProductName,
                        });
                    }

                    result.Add(SaleOrderVM);
                }

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
        public async Task<ResponseDTO<SaleOrderVM>> GetSaleOrderDetailByIdAsync(int id)
        {
            var response = new ResponseDTO<SaleOrderVM>();
            try
            {
                var item = (await _unitOfWork.SaleOrderRepository.GetAsync(_ => _.Id == id, "User,SaleOrderDetails")).FirstOrDefault();
                if (item == null)
                {
                    response.IsSuccess = false;
                    response.Message = $"SaleOrder with id = {id} is not found";
                    return response;
                }

                var SaleOrderVM = new SaleOrderVM()
                {
                    SaleOrderID = item.Id,
                    SaleOrderCode = item.OrderCode,
                    UserID = item.UserId,
                    SubTotal = item.SubTotal,
                    TransportFee = item.TranSportFee,
                    TotalAmount = item.TotalAmount,
                    OrderStatus = Enum.GetName((OrderStatus)item.OrderStatus),
                    PaymentStatus = Enum.GetName((OrderStatus)item.PaymentStatus),
                    CreatedAt = item.CreatedAt,
                    PaymentMethodId = item.PaymentMethodId,
                    Email = item.Email,
                    Address = item.Address,
                    ContactPhone = item.ContactPhone,
                    FullName = item.FullName,
                    SaleOrderDetailVMs = new List<SaleOrderDetailVM>()
                };

                foreach (var detail in item.OrderDetails)
                {
                    var warehouse = (await _warehouseService.GetWarehouseByProductIdAndBranchId((int)detail.ProductId, detail.BranchId)).FirstOrDefault();
                    SaleOrderVM.SaleOrderDetailVMs.Add(new SaleOrderDetailVM()
                    {
                        Quantity = detail.Quantity,
                        TotalPrice = detail.TotalPrice,
                        Price = detail.Price,
                        BranchId = detail.BranchId,
                        ProductId = detail.ProductId,
                        CreatedAt = detail.CreatedAt,
                        BranchName = detail.BranchName,
                        ProductName = detail.ProductName,
                    });
                }

                response.IsSuccess = true;
                response.Message = "Query successfully";
                response.Data = SaleOrderVM;
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
                var query = await _unitOfWork.SaleOrderRepository.GetAsync(o => o.UserId == userId, "User,SaleOrderDetails");
                if (query == null || !query.Any())
                {
                    response.IsSuccess = false;
                    response.Message = "SaleOrders are not found";
                    return response;
                }
                var result = new List<SaleOrderVM>();
                foreach (var item in query)
                {
                    var SaleOrderVM = new SaleOrderVM()
                    {
                        SaleOrderID = item.Id,
                        SaleOrderCode = item.OrderCode,
                        UserID = item.UserId,
                        SubTotal = item.SubTotal,
                        TransportFee = item.TranSportFee,
                        TotalAmount = item.TotalAmount,
                        OrderStatus = Enum.GetName((OrderStatus)item.OrderStatus),
                        PaymentStatus = Enum.GetName((OrderStatus)item.PaymentStatus),
                        CreatedAt = item.CreatedAt,
                        PaymentMethodId = item.PaymentMethodId,
                        Email = item.Email,
                        Address = item.Address,
                        ContactPhone = item.ContactPhone,
                        FullName = item.FullName,
                        SaleOrderDetailVMs = new List<SaleOrderDetailVM>()
                    };

                    foreach (var detail in item.OrderDetails)
                    {
                        var warehouse = (await _warehouseService.GetWarehouseByProductIdAndBranchId((int)detail.ProductId, detail.BranchId)).FirstOrDefault();
                        SaleOrderVM.SaleOrderDetailVMs.Add(new SaleOrderDetailVM()
                        {
                            Quantity = detail.Quantity,
                            TotalPrice = detail.TotalPrice,
                            Price = detail.Price,
                            BranchId = detail.BranchId,
                            ProductId = detail.ProductId,
                            CreatedAt = detail.CreatedAt,
                            BranchName = detail.BranchName,
                            ProductName = detail.ProductName,
                        });
                    }

                    result.Add(SaleOrderVM);
                }

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
                var result = new List<SaleOrderVM>();
                foreach (var item in query)
                {
                    var SaleOrderVM = new SaleOrderVM()
                    {
                        SaleOrderID = item.Id,
                        SaleOrderCode = item.OrderCode,
                        UserID = item.UserId,
                        SubTotal = item.SubTotal,
                        TransportFee = item.TranSportFee,
                        TotalAmount = item.TotalAmount,
                        OrderStatus = Enum.GetName((OrderStatus)item.OrderStatus),
                        PaymentStatus = Enum.GetName((OrderStatus)item.PaymentStatus),
                        CreatedAt = item.CreatedAt,
                        PaymentMethodId = item.PaymentMethodId,
                        Email = item.Email,
                        Address = item.Address,
                        ContactPhone = item.ContactPhone,
                        FullName = item.FullName,
                        SaleOrderDetailVMs = new List<SaleOrderDetailVM>()
                    };

                    foreach (var detail in item.OrderDetails)
                    {
                        var warehouse = (await _warehouseService.GetWarehouseByProductIdAndBranchId((int)detail.ProductId, detail.BranchId)).FirstOrDefault();
                        SaleOrderVM.SaleOrderDetailVMs.Add(new SaleOrderDetailVM()
                        {
                            Quantity = detail.Quantity,
                            TotalPrice = detail.TotalPrice,
                            Price = detail.Price,
                            BranchId = detail.BranchId,
                            ProductId = detail.ProductId,
                            CreatedAt = detail.CreatedAt,
                            BranchName = detail.BranchName,
                            ProductName = detail.ProductName,
                        });
                    }
                    result.Add(SaleOrderVM);
                }

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

                var result = new List<SaleOrderVM>();
                foreach (var item in query)
                {
                    var SaleOrderVM = new SaleOrderVM()
                    {
                        SaleOrderID = item.Id,
                        SaleOrderCode = item.OrderCode,
                        UserID = item.UserId,
                        SubTotal = item.SubTotal,
                        TransportFee = item.TranSportFee,
                        TotalAmount = item.TotalAmount,
                        OrderStatus = Enum.GetName((OrderStatus)item.OrderStatus),
                        PaymentStatus = Enum.GetName((OrderStatus)item.PaymentStatus),
                        CreatedAt = item.CreatedAt,
                        PaymentMethodId = item.PaymentMethodId,
                        Email = item.Email,
                        Address = item.Address,
                        ContactPhone = item.ContactPhone,
                        FullName = item.FullName,
                        SaleOrderDetailVMs = new List<SaleOrderDetailVM>()
                    };

                    foreach (var detail in item.OrderDetails)
                    {
                        var warehouse = (await _warehouseService.GetWarehouseByProductIdAndBranchId((int)detail.ProductId, detail.BranchId)).FirstOrDefault();
                        SaleOrderVM.SaleOrderDetailVMs.Add(new SaleOrderDetailVM()
                        {
                            Quantity = detail.Quantity,
                            TotalPrice = detail.TotalPrice,
                            Price = detail.Price,
                            BranchId = detail.BranchId,
                            ProductId = detail.ProductId,
                            CreatedAt = detail.CreatedAt,
                            BranchName = detail.BranchName,
                            ProductName = detail.ProductName,
                        });
                    }

                    result.Add(SaleOrderVM);
                }

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
        public async Task<ResponseDTO<List<SaleOrderVM>>> GetSaleOrdersByDateRangeAndStatus(DateTime? fromDate, DateTime? toDate, int? orderStatus)
        {
            
            throw new NotImplementedException();
        }
        //============================
        public async Task<SaleOrder> GetSaleOrderByIdFromUserAsync(int SaleOrderId, int userId)
        {
            return await _unitOfWork.SaleOrderRepository.GetObjectAsync(_ => _.Id == SaleOrderId && _.UserId == userId);
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
        public async Task<IQueryable<SaleOrder>> GetAllSaleOrderQueryableAsync()
        {
            var query = await _unitOfWork.SaleOrderRepository.GetAllAsync();

            return query.AsQueryable();
        }
        public async Task<ResponseDTO<SaleOrderVM>> GetSaleOrderBySaleOrderCode(string SaleOrderCode)
        {
            var response = new ResponseDTO<SaleOrderVM>();
            try
            {
                var query = await _unitOfWork.SaleOrderRepository.GetAsync(o => o.OrderCode.Equals(SaleOrderCode), "User,OrderDetails");
                if (query == null || !query.Any())
                {
                    response.IsSuccess = false;
                    response.Message = "SaleOrders are not found";
                    return response;
                }

                var item = query.FirstOrDefault();
                var SaleOrderVM = new SaleOrderVM()
                {
                    SaleOrderID = item.Id,
                    SaleOrderCode = item.OrderCode,
                    UserID = item.UserId,
                    SubTotal = item.SubTotal,
                    TransportFee = item.TranSportFee,
                    TotalAmount = item.TotalAmount,
                    OrderStatus = Enum.GetName((OrderStatus)item.OrderStatus),
                    PaymentStatus = Enum.GetName((OrderStatus)item.PaymentStatus),
                    CreatedAt = item.CreatedAt,
                    PaymentMethodId = item.PaymentMethodId,
                    Email = item.Email,
                    Address = item.Address,
                    ContactPhone = item.ContactPhone,
                    FullName = item.FullName,
                    SaleOrderDetailVMs = new List<SaleOrderDetailVM>()
                };

                foreach (var detail in item.OrderDetails)
                {
                    var warehouse = (await _warehouseService.GetWarehouseByProductIdAndBranchId((int)detail.ProductId, detail.BranchId)).FirstOrDefault();
                    SaleOrderVM.SaleOrderDetailVMs.Add(new SaleOrderDetailVM()
                    {
                        Quantity = detail.Quantity,
                        TotalPrice = detail.TotalPrice,
                        Price = detail.Price,
                        BranchId = detail.BranchId,
                        ProductId = detail.ProductId,
                        CreatedAt = detail.CreatedAt,
                        BranchName = detail.BranchName,
                        ProductName = detail.ProductName,
                    });
                }

                response.IsSuccess = true;
                response.Message = "Query successfully";
                response.Data = SaleOrderVM;
                return response;

            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
                return response;
            }
        }
        /*public async Task<ResponseDTO<List<SaleOrderVM>>> GetSaleOrdersByMonthAndStatus(DateTime startDate, DateTime endDate, int status)
        {
            var response = new ResponseDTO<List<SaleOrderVM>>();
            try
            {
                var query = await _unitOfWork.SaleOrderRepository
                    .GetAsync(_ => _.ReceivedDate >= startDate &&
                                   _.ReceivedDate <= endDate &&
                                   _.Status == status, "User,SaleOrderDetails");

                if (query == null || !query.Any())
                {
                    response.IsSuccess = false;
                    response.Message = "SaleOrders are not found";
                    return response;
                }

                var result = new List<SaleOrderVM>();
                foreach (var item in query)
                {
                    var SaleOrderVM = new SaleOrderVM()
                    {
                        SaleOrderID = item.Id,
                        SaleOrderCode = item.OrderCode,
                        UserID = item.UserId,
                        SubTotal = item.SubTotal,
                        TransportFee = item.TranSportFee,
                        TotalAmount = item.TotalAmount,
                        OrderStatus = Enum.GetName((OrderStatus)item.OrderStatus),
                        PaymentStatus = Enum.GetName((OrderStatus)item.PaymentStatus),
                        CreatedAt = item.CreatedAt,
                        PaymentMethodId = item.PaymentMethodId,
                        Email = item.Email,
                        Address = item.Address,
                        ContactPhone = item.ContactPhone,
                        FullName = item.FullName,
                        SaleOrderDetailVMs = new List<SaleOrderDetailVM>()
                    };

                    foreach (var detail in item.SaleOrderDetails)
                    {
                        var warehouse = (await _warehouseService.GetWarehouseByProductIdAndBranchId((int)detail.ProductId, detail.BranchId)).FirstOrDefault();
                        SaleOrderVM.SaleOrderDetailVMs.Add(new SaleOrderDetailVM()
                        {
                            Quantity = detail.Quantity,
                            WarehouseId = warehouse?.Id ?? 0
                        });
                    }

                    result.Add(SaleOrderVM);
                }

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
        }*/

        #endregion
        #region Create_Update_Delete_SaleOrder
        public async Task<ResponseDTO<SaleOrderVM>> CreatetSaleOrderAsync(SaleOrderCM saleOrderCM)
        {
            var response = new ResponseDTO<SaleOrderVM>();
            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {

                    var user = await _unitOfWork.UserRepository
                        .GetObjectAsync(u => u.Id == saleOrderCM.UserID);
                    if (user == null)
                    {
                        response.IsSuccess = false;
                        response.Message = $"User with Id = {saleOrderCM.UserID} is not found!";
                        return response;
                    }

                    var shipmentDetail = await _unitOfWork.ShipmentDetailRepository
                        .GetObjectAsync(s => s.Id == saleOrderCM.ShipmentDetailID && s.UserId == user.Id);
                    if (shipmentDetail == null)
                    {
                        response.IsSuccess = false;
                        response.Message = $"ShipmenDetail with Id = {saleOrderCM.ShipmentDetailID} is not found!";
                        return response;
                    }
                    var paymentMethod = await _unitOfWork.PaymentMethodRepository
                                            .GetObjectAsync(p => p.Id == saleOrderCM.PaymentMethodID);
                    if (paymentMethod == null)
                    {
                        response.IsSuccess = false;
                        response.Message = $"Payment method id{saleOrderCM.PaymentMethodID} is invalid!";
                        return response;
                    }

                    var SaleOrderDetails = new List<OrderDetail>();
                    var toCreate = new SaleOrder
                    {
                        OrderCode = _methodHelper.GenerateOrderCode(),
                        OrderStatus = (int?)OrderStatus.PENDING,
                        PaymentStatus = (int)PaymentStatus.IsWating,
                        PaymentMethodId = saleOrderCM.PaymentMethodID,
                        Address = shipmentDetail.Address,
                        FullName = shipmentDetail.FullName,
                        Email = shipmentDetail.Email,
                        ContactPhone = shipmentDetail.PhoneNumber,
                        UserId = user.Id,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = null,
                        Note = saleOrderCM.Note,
                        OrderDetails = new List<OrderDetail>(),
                    };
                    await _unitOfWork.SaleOrderRepository.InsertAsync(toCreate);

                    decimal subTotal = 0;
                    foreach (var item in saleOrderCM.SaleOrderDetailCMs)
                    {
                        var stockInWarehouse = await _unitOfWork.WarehouseRepository
                                                .GetObjectAsync(p => p.Id == item.WarehouseId, new string[] { "Product, Branch" });

                        if (stockInWarehouse == null || stockInWarehouse.TotalQuantity < item.Quantity)
                        {
                            response.IsSuccess = false;
                            response.Message = $"Not enough stock for product {item.WarehouseId} at branch {stockInWarehouse.Branch.BranchName}";
                            return response;
                        }
                        //update available quantity in wareHouse
                        stockInWarehouse.AvailableQuantity -= item.Quantity;
                        await _unitOfWork.WarehouseRepository.UpdateAsync(stockInWarehouse);

                        var SaleOrderDetail = new OrderDetail
                        {
                            ProductId = stockInWarehouse.ProductId,
                            OrderId = toCreate.Id,
                            BranchId = stockInWarehouse.BranchId,
                            ProductName = stockInWarehouse.Product.ProductName,
                            BranchName = stockInWarehouse.Branch.BranchName,
                            Quantity = item.Quantity,
                            Price = stockInWarehouse.Product.Price,
                            CreatedAt = DateTime.Now,
                        };

                        await _unitOfWork.OrderDetailRepository.InsertAsync(SaleOrderDetail);
                        //toCreate.OrderDetails.Add(SaleOrderDetail);
                        subTotal += (decimal)(stockInWarehouse.Product.Price * item.Quantity);
                    }

                    toCreate.SubTotal = subTotal;
                    toCreate.TranSportFee = 0;
                    toCreate.TotalAmount = (decimal)(toCreate.SubTotal + toCreate.TranSportFee); // if we have coupon, applying to IntoMoney
                    await _unitOfWork.SaleOrderRepository.UpdateAsync(toCreate);

                    // Transaction submit
                    await transaction.CommitAsync();
                    //Return
                    var result = new SaleOrderVM()
                    {
                        SaleOrderID = toCreate.Id,
                        SaleOrderCode = toCreate.OrderCode,
                        UserID = toCreate.UserId,
                        SubTotal = toCreate.SubTotal,
                        TransportFee = toCreate.TranSportFee,
                        TotalAmount = toCreate.TotalAmount,
                        ContactPhone = toCreate.ContactPhone,
                        Address = toCreate.Address,
                        Email = toCreate.Email,
                        FullName = toCreate.FullName,
                        PaymentStatus = Enum.GetName((OrderStatus)toCreate.PaymentStatus),
                        CreatedAt = toCreate.CreatedAt,
                        OrderStatus = Enum.GetName((OrderStatus)toCreate.OrderStatus),
                        PaymentMethodId = toCreate.PaymentMethodId,
                        SaleOrderDetailVMs = toCreate.OrderDetails.Select(od => new SaleOrderDetailVM()
                        {
                            BranchId = od.BranchId,
                            BranchName = od.BranchName,
                            ProductId = od.ProductId,
                            ProductName = od.ProductName,
                            CreatedAt = od.CreatedAt,
                            Price = od.Price,
                            Quantity = od.Quantity,
                            TotalPrice = od.TotalPrice,
                        }).ToList(),
                        PaymentLink = ""
                    };
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

                    /*if (toUpdate.OrderStatus != (int)OrderStatus.PENDING)
                    {
                        response.IsSuccess = false;
                        response.Message = $"Your SaleOrder status has been {toUpdate.OrderStatus}. Only SaleOrders with a PENDING status will be updated.!";
                        return response;
                    }*/

                    var branch = await _unitOfWork.BranchRepository.GetObjectAsync(b => b.Id == SaleOrderUM.BranchId);
                    if (branch == null)
                    {
                        response.IsSuccess = false;
                        response.Message = $"Branch {SaleOrderUM.BranchId} not found!";
                        return response;
                    }
                    #endregion

                    #region UpdateOrderInfor
                    toUpdate.SubTotal = SaleOrderUM.SubTotal;
                    toUpdate.TranSportFee = SaleOrderUM.TransportFee;
                    toUpdate.TotalAmount = SaleOrderUM.TotalAmount;
                    toUpdate.PaymentStatus = SaleOrderUM.PaymentStatus;
                    toUpdate.OrderStatus = SaleOrderUM.OrderStatus;
                    toUpdate.Note = SaleOrderUM.Note;
                    toUpdate.Email = SaleOrderUM.Email;
                    toUpdate.FullName = SaleOrderUM.FullName;
                    toUpdate.ContactPhone = SaleOrderUM.ContactPhone;
                    toUpdate.Address = SaleOrderUM.Address;
                    toUpdate.BranchId = SaleOrderUM.BranchId;
                    toUpdate.PaymentMethodId = SaleOrderUM.PaymentMethodId;
                    await _unitOfWork.SaleOrderRepository.UpdateAsync(toUpdate);
                    #endregion
                    #region Update OrderDetail
                    foreach (var updatedItem in SaleOrderUM.SaleOrderDetailUMs)
                    {
                        var warehouse = await _unitOfWork.WarehouseRepository
                            .GetObjectAsync(w => w.Id == updatedItem.WarehouseId, new string[] { "Product" });

                        var SaleOrderDetail = await _unitOfWork.OrderDetailRepository
                            .GetObjectAsync(o => o.OrderId == toUpdate.Id && o.ProductId == warehouse.ProductId);

                        if (updatedItem.Quantity == 0)
                        {
                            if (SaleOrderDetail != null)
                            {
                                //restock quantity
                                AdjustStock(warehouse, (int)SaleOrderDetail.Quantity, true);

                                //Remove item in SaleOrder
                                await _unitOfWork.OrderDetailRepository.DeleteAsync(SaleOrderDetail);
                            }
                        }
                        else if (SaleOrderDetail != null)
                        {
                            int quantityDifference = (int)(updatedItem.Quantity - SaleOrderDetail.Quantity);

                            //Check warehouse, the availableQuan >= diffQuan
                            if (warehouse.AvailableQuantity >= quantityDifference)
                            {
                                AdjustStock(warehouse, quantityDifference, false);

                                SaleOrderDetail.Quantity = updatedItem.Quantity;
                                await _unitOfWork.OrderDetailRepository.UpdateAsync(SaleOrderDetail);
                            }
                            else
                            {
                                response.IsSuccess = false;
                                response.Message = $"Insufficient stock for product: {warehouse.Product.ProductName}";
                                return response;
                            }
                        }
                        else
                        {

                            // Add new item
                            if (warehouse.AvailableQuantity >= updatedItem.Quantity)
                            {
                                var newSaleOrderDetail = new OrderDetail
                                {
                                    ProductId = warehouse.ProductId,
                                    OrderId = toUpdate.Id,
                                    BranchId = warehouse.BranchId,
                                    ProductName = warehouse.Product.ProductName,
                                    BranchName = warehouse.Branch.BranchName,
                                    Quantity = updatedItem.Quantity,
                                    Price = warehouse.Product.Price,
                                    CreatedAt = DateTime.Now,
                                };

                                AdjustStock(warehouse, (int)updatedItem.Quantity, false);
                                await _unitOfWork.OrderDetailRepository.InsertAsync(newSaleOrderDetail);
                            }
                            else
                            {
                                response.IsSuccess = false;
                                response.Message = $"Insufficient stock for product: {warehouse.Product.ProductName}";
                                return response;
                            }
                        }
                        await _unitOfWork.WarehouseRepository.UpdateAsync(warehouse);
                    }
                    #endregion

                    await transaction.CommitAsync();
                    //Return
                    var SaleOrderDetails = await _unitOfWork.OrderDetailRepository.GetAsync(od => od.OrderId == toUpdate.Id);
                    #region Result
                    var result = new SaleOrderVM()
                    {
                        SaleOrderID = toUpdate.Id,
                        SaleOrderCode = toUpdate.OrderCode,
                        UserID = toUpdate.UserId,
                        SubTotal = toUpdate.SubTotal,
                        TransportFee = toUpdate.TranSportFee,
                        TotalAmount = toUpdate.TotalAmount,
                        ContactPhone = toUpdate.ContactPhone,
                        Address = toUpdate.Address,
                        Email = toUpdate.Email,
                        FullName = toUpdate.FullName,
                        PaymentStatus = Enum.GetName((OrderStatus)toUpdate.PaymentStatus),
                        CreatedAt = toUpdate.CreatedAt,
                        OrderStatus = Enum.GetName((OrderStatus)toUpdate.OrderStatus),
                        PaymentMethodId = toUpdate.PaymentMethodId,
                        SaleOrderDetailVMs = toUpdate.OrderDetails.Select(od => new SaleOrderDetailVM()
                        {
                            BranchId = od.BranchId,
                            BranchName = od.BranchName,
                            ProductId = od.ProductId,
                            ProductName = od.ProductName,
                            CreatedAt = od.CreatedAt,
                            Price = od.Price,
                            Quantity = od.Quantity,
                            TotalPrice = od.TotalPrice,
                        }).ToList(),
                        PaymentLink = ""
                    };
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
        #region Process_PayOs_Response
        public async Task<ResponseDTO<int>> ProcessCancelledSaleOrder(PaymentResponse paymentResponse)
        {
            var response = new ResponseDTO<int>();
            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    var saleOrder = await _unitOfWork.SaleOrderRepository.GetObjectAsync(o => o.OrderCode == paymentResponse.OrderCode);
                    if (saleOrder == null)
                    {
                        response.IsSuccess = false;
                        response.Message = $"SaleOrder with code {paymentResponse.OrderCode} is not found!";
                        response.Data = 0;
                        return response;
                    }
                    // Cập nhật trạng thái SaleOrder thành "Cancelled"
                    saleOrder.OrderStatus = (int)OrderStatus.CANCELLED;
                    saleOrder.PaymentStatus = (int)PaymentStatus.IsCanceled;

                    await _unitOfWork.SaleOrderRepository.UpdateAsync(saleOrder);

                    //Cập nhật lại số luọng sản phẩm của chi nhánh đó thông qua update lại warehouse
                    var SaleOrderDetails = await _unitOfWork.OrderDetailRepository
                                                        .GetAsync(od => od.OrderId == saleOrder.Id);

                    foreach (var item in SaleOrderDetails)
                    {
                        var productInWarehouse = await _unitOfWork.WarehouseRepository
                            .GetObjectAsync(wh => wh.ProductId == item.ProductId && wh.BranchId == item.BranchId);
                        if (productInWarehouse != null)
                        {
                            productInWarehouse.AvailableQuantity += item.Quantity;
                            await _unitOfWork.WarehouseRepository.UpdateAsync(productInWarehouse);
                        }
                    }

                    await transaction.CommitAsync();

                    response.IsSuccess = true;
                    response.Message = $"Cancelled SaleOrder with code {paymentResponse.OrderCode}";
                    response.Data = 1;
                    return response;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    response.IsSuccess = false;
                    response.Message = ex.Message;
                    response.Data = 0;
                    return response;
                }
            }
        }
        public async Task<ResponseDTO<int>> ProcessCompletedSaleOrder(PaymentResponse paymentResponse)
        {
            var response = new ResponseDTO<int>();
            try
            {
                var SaleOrder = await _unitOfWork.SaleOrderRepository.GetObjectAsync(o => o.OrderCode == paymentResponse.OrderCode);
                if (SaleOrder == null)
                {
                    response.IsSuccess = false;
                    response.Message = $"SaleOrder with code {paymentResponse.OrderCode} is not found!";
                    response.Data = 0;
                    return response;
                }
                // Cập nhật trạng thái SaleOrder thành "Completed"
                SaleOrder.PaymentStatus = (int)PaymentStatus.IsPaid;
                await _unitOfWork.SaleOrderRepository.UpdateAsync(SaleOrder);

                response.IsSuccess = true;
                response.Message = $"Completed SaleOrder with code {paymentResponse.OrderCode}";
                response.Data = 1;
                return response;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
                response.Data = 0;
                return response;
            }
        }
        #endregion
        #region Other_Methods
        private void AdjustStock(Warehouse warehouse, int quantity, bool isReturningStock)
        {
            if (isReturningStock)
            {
                warehouse.AvailableQuantity += quantity;
                warehouse.TotalQuantity += quantity;
            }
            else
            {
                warehouse.AvailableQuantity -= quantity;
                warehouse.TotalQuantity -= quantity;
            }
        }
        #endregion
    }
}
