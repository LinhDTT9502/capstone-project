using _2Sport_BE.Helpers;
using _2Sport_BE.Infrastructure.Services;
using _2Sport_BE.Repository.Interfaces;
using _2Sport_BE.Repository.Models;
using _2Sport_BE.Service.DTOs;
using _2Sport_BE.Service.Enums;
using Microsoft.EntityFrameworkCore;
using System.Data.SqlTypes;

namespace _2Sport_BE.Service.Services
{
    public interface IOrderService
    {
        Task<Order> GetOrderByIdFromUserAsync(int orderId, int userId);
        Task<Order> AddOrderAsync(Order order);

        //Upgrade code 
        Task<ResponseDTO<OrderVM>> ProcessCreatetOrder(OrderCM orderCM);
        Task<ResponseDTO<GuestOrderVM>> ProcessCreatetOrderForGuest(GuestOrderCM guestOrderCM);
        Task<ResponseDTO<List<OrderVM>>> GetAllOrdersAsync();
        Task<ResponseDTO<OrderVM>> GetOrderByIdAsync(int id);
        Task<ResponseDTO<List<OrderVM>>> GetAllOrdersByUseIdAsync(int userId);
        Task<ResponseDTO<List<OrderVM>>> GetOrdersByStatus(int status);
        Task<ResponseDTO<OrderVM>> GetOrderByOrderCode(string orderCode);
        Task<ResponseDTO<List<OrderVM>>> GetOrdersByMonth(DateTime startDate, DateTime endDate);
        Task<ResponseDTO<List<OrderVM>>> GetOrdersByMonthAndStatus(DateTime startDate, DateTime endDate, int status);
        Task<ResponseDTO<string>> DeleteOrderAsync(int id);
        Task<ResponseDTO<string>> ChangeOrderStatusAsync(int id, int status);
        Task<ResponseDTO<OrderVM>> UpdateOrderOfCustomerAsync(int orderId, OrderUM orderUM);
        Task<ResponseDTO<OrderVM>> UpdateOrderOfGuestAsync(int orderId, GuestOrderUM orderUM);
        Task<IQueryable<Order>> GetAllOrderQueryableAsync();
        Task<ResponseDTO<int>> ProcessCancelledOrder(PaymentResponse paymentResponse);
        Task<ResponseDTO<int>> ProcessCompletedOrder(PaymentResponse paymentResponse);

        Task<ResponseDTO<RevenueVM>> GetOrdersRevenue(int? branchId, int? orderType, DateTime? from, DateTime? to, int? status);
    }
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICustomerDetailService _customerDetailService;
        private readonly IWarehouseService _warehouseService;
        private readonly IMethodHelper _methodHelper;
        public OrderService(IUnitOfWork unitOfWork,
            ICustomerDetailService customerDetailService,
            IWarehouseService warehouseService,
            IMethodHelper methodHelper)
        {
            _unitOfWork = unitOfWork;
            _customerDetailService = customerDetailService;
            _warehouseService = warehouseService;
            _methodHelper = methodHelper;
        }
        public string GenerateOrderCode()
        {
            string datePart = DateTime.UtcNow.ToString("yyMMdd");

            Random random = new Random();
            string randomPart = random.Next(1000, 9999).ToString();

            string orderCode = $"{datePart}{randomPart}";

            return orderCode;
        }
        public async Task<ResponseDTO<List<OrderVM>>> GetAllOrdersAsync()
        {
            var response = new ResponseDTO<List<OrderVM>>();
            try
            {
                var query = await _unitOfWork.OrderRepository.GetAllAsync(new string[] { "User", "OrderDetails" });
                if (query == null || !query.Any())
                {
                    response.IsSuccess = false;
                    response.Message = "Orders are not found";
                    return response;
                }
                var result = new List<OrderVM>();

                foreach (var item in query)
                {
                    var orderVM = new OrderVM()
                    {
                        CreateDate = item.CreateAt,
                        UserID = item.UserId,
                        OrderCode = item.OrderCode,
                        OrderID = item.Id,
                        Status = Enum.GetName(typeof(OrderStatus), item.Status)?.Replace('_', ' '),
                        IntoMoney = item.IntoMoney.ToString(),
                        TotalPrice = item.TotalPrice.ToString(),
                        PaymentMethodId = item.PaymentMethodId,
                        ShipmentDetailId = item.ShipmentDetailId,
                        orderDetailVMs = new List<OrderDetailVM>()
                    };

                    foreach (var detail in item.OrderDetails)
                    {
                        var warehouse = (await _warehouseService.GetWarehouseByProductIdAndBranchId((int)detail.ProductId, detail.BranchId)).FirstOrDefault();
                        orderVM.orderDetailVMs.Add(new OrderDetailVM()
                        {
                            Quantity = detail.Quantity,
                            WarehouseId = warehouse?.Id ?? 0
                        });
                    }

                    result.Add(orderVM);
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
        public async Task<ResponseDTO<OrderVM>> GetOrderByIdAsync(int id)
        {
            var response = new ResponseDTO<OrderVM>();
            try
            {
                var item = (await _unitOfWork.OrderRepository.GetAsync(_ => _.Id == id, "User,OrderDetails")).FirstOrDefault();
                if (item == null)
                {
                    response.IsSuccess = false;
                    response.Message = $"Order with id = {id} is not found";
                    return response;
                }

                var orderVM = new OrderVM()
                {
                    CreateDate = item.CreateAt,
                    UserID = item.UserId,
                    OrderCode = item.OrderCode,
                    OrderID = item.Id,
                    Status = Enum.GetName(typeof(OrderStatus), item.Status)?.Replace('_', ' '),
                    IntoMoney = item.IntoMoney.ToString(),
                    TotalPrice = item.TotalPrice.ToString(),
                    PaymentMethodId = item.PaymentMethodId,
                    ShipmentDetailId = item.ShipmentDetailId,
                    orderDetailVMs = new List<OrderDetailVM>()
                };

                foreach (var detail in item.OrderDetails)
                {
                    var warehouse = (await _warehouseService.GetWarehouseByProductIdAndBranchId((int)detail.ProductId, detail.BranchId)).FirstOrDefault();
                    orderVM.orderDetailVMs.Add(new OrderDetailVM()
                    {
                        Quantity = detail.Quantity,
                        WarehouseId = warehouse?.Id ?? 0
                    });
                }

                response.IsSuccess = true;
                response.Message = "Query successfully";
                response.Data = orderVM;
                return response;

            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
                return response;
            }
        }
        public async Task<ResponseDTO<List<OrderVM>>> GetAllOrdersByUseIdAsync(int userId)
        {
            var response = new ResponseDTO<List<OrderVM>>();
            try
            {
                var query = await _unitOfWork.OrderRepository.GetAsync(o => o.UserId == userId, "User,OrderDetails");
                if (query == null || !query.Any())
                {
                    response.IsSuccess = false;
                    response.Message = "Orders are not found";
                    return response;
                }
                var result = new List<OrderVM>();
                foreach (var item in query)
                {
                    var orderVM = new OrderVM()
                    {
                        CreateDate = item.CreateAt,
                        UserID = item.UserId,
                        OrderCode = item.OrderCode,
                        OrderID = item.Id,
                        Status = Enum.GetName(typeof(OrderStatus), item.Status)?.Replace('_', ' '),
                        IntoMoney = item.IntoMoney.ToString(),
                        TotalPrice = item.TotalPrice.ToString(),
                        PaymentMethodId = item.PaymentMethodId,
                        ShipmentDetailId = item.ShipmentDetailId,
                        orderDetailVMs = new List<OrderDetailVM>()
                    };

                    foreach (var detail in item.OrderDetails)
                    {
                        var warehouse = (await _warehouseService.GetWarehouseByProductIdAndBranchId((int)detail.ProductId, detail.BranchId)).FirstOrDefault();
                        orderVM.orderDetailVMs.Add(new OrderDetailVM()
                        {
                            Quantity = detail.Quantity,
                            WarehouseId = warehouse?.Id ?? 0
                        });
                    }

                    result.Add(orderVM);
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
        public async Task<ResponseDTO<List<OrderVM>>> GetOrdersByStatus(int status)
        {
            var response = new ResponseDTO<List<OrderVM>>();
            try
            {
                var query = await _unitOfWork.OrderRepository.GetAsync(o => o.Status == status, "User,OrderDetails");
                if (query == null || !query.Any())
                {
                    response.IsSuccess = false;
                    response.Message = "Orders are not found";
                    return response;
                }
                var result = new List<OrderVM>();
                foreach (var item in query)
                {
                    var orderVM = new OrderVM()
                    {
                        CreateDate = item.CreateAt,
                        UserID = item.UserId,
                        OrderCode = item.OrderCode,
                        OrderID = item.Id,
                        Status = Enum.GetName(typeof(OrderStatus), item.Status)?.Replace('_', ' '),
                        IntoMoney = item.IntoMoney.ToString(),
                        TotalPrice = item.TotalPrice.ToString(),
                        PaymentMethodId = item.PaymentMethodId,
                        ShipmentDetailId = item.ShipmentDetailId,
                        orderDetailVMs = new List<OrderDetailVM>()
                    };

                    foreach (var detail in item.OrderDetails)
                    {
                        var warehouse = (await _warehouseService.GetWarehouseByProductIdAndBranchId((int)detail.ProductId, detail.BranchId)).FirstOrDefault();
                        orderVM.orderDetailVMs.Add(new OrderDetailVM()
                        {
                            Quantity = detail.Quantity,
                            WarehouseId = warehouse?.Id ?? 0
                        });
                    }

                    result.Add(orderVM);
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
        public async Task<ResponseDTO<OrderVM>> GetOrderByOrderCode(string orderCode)
        {
            var response = new ResponseDTO<OrderVM>();
            try
            {
                var query = await _unitOfWork.OrderRepository.GetAsync(o => o.OrderCode.Equals(orderCode), "User,OrderDetails");
                if (query == null || !query.Any())
                {
                    response.IsSuccess = false;
                    response.Message = "Orders are not found";
                    return response;
                }

                var item = query.FirstOrDefault();
                var orderVM = new OrderVM()
                {
                    CreateDate = item.CreateAt,
                    UserID = item.UserId,
                    OrderCode = item.OrderCode,
                    OrderID = item.Id,
                    Status = Enum.GetName(typeof(OrderStatus), item.Status)?.Replace('_', ' '),
                    IntoMoney = item.IntoMoney.ToString(),
                    TotalPrice = item.TotalPrice.ToString(),
                    PaymentMethodId = item.PaymentMethodId,
                    ShipmentDetailId = item.ShipmentDetailId,
                };

                foreach (var detail in item.OrderDetails)
                {
                    var warehouse = (await _warehouseService.GetWarehouseByProductIdAndBranchId((int)detail.ProductId, detail.BranchId)).FirstOrDefault();
                    orderVM.orderDetailVMs.Add(new OrderDetailVM()
                    {
                        Quantity = detail.Quantity,
                        WarehouseId = warehouse?.Id ?? 0
                    });
                }

                response.IsSuccess = true;
                response.Message = "Query successfully";
                response.Data = orderVM;
                return response;

            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
                return response;
            }
        }
        public async Task<ResponseDTO<List<OrderVM>>> GetOrdersByMonth(DateTime startDate, DateTime endDate)
        {
            var response = new ResponseDTO<List<OrderVM>>();
            try
            {
                var query = await _unitOfWork.OrderRepository
                    .GetAsync(_ => _.ReceivedDate >= startDate && _.ReceivedDate <= endDate, "User,OrderDetails");
                if (query == null || !query.Any())
                {
                    response.IsSuccess = false;
                    response.Message = "Orders are not found";
                    return response;
                }

                var result = new List<OrderVM>();
                foreach (var item in query)
                {
                    var orderVM = new OrderVM()
                    {
                        CreateDate = item.CreateAt,
                        UserID = item.UserId,
                        OrderCode = item.OrderCode,
                        OrderID = item.Id,
                        Status = Enum.GetName(typeof(OrderStatus), item.Status)?.Replace('_', ' '),
                        IntoMoney = item.IntoMoney.ToString(),
                        TotalPrice = item.TotalPrice.ToString(),
                        PaymentMethodId = item.PaymentMethodId,
                        ShipmentDetailId = item.ShipmentDetailId,
                        orderDetailVMs = new List<OrderDetailVM>()
                    };

                    foreach (var detail in item.OrderDetails)
                    {
                        var warehouse = (await _warehouseService.GetWarehouseByProductIdAndBranchId((int)detail.ProductId, detail.BranchId)).FirstOrDefault();
                        orderVM.orderDetailVMs.Add(new OrderDetailVM()
                        {
                            Quantity = detail.Quantity,
                            WarehouseId = warehouse?.Id ?? 0
                        });
                    }

                    result.Add(orderVM);
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
        public async Task<ResponseDTO<List<OrderVM>>> GetOrdersByMonthAndStatus(DateTime startDate, DateTime endDate, int status)
        {
            var response = new ResponseDTO<List<OrderVM>>();
            try
            {
                var query = await _unitOfWork.OrderRepository
                    .GetAsync(_ => _.ReceivedDate >= startDate &&
                                   _.ReceivedDate <= endDate &&
                                   _.Status == status, "User,OrderDetails");

                if (query == null || !query.Any())
                {
                    response.IsSuccess = false;
                    response.Message = "Orders are not found";
                    return response;
                }

                var result = new List<OrderVM>();
                foreach (var item in query)
                {
                    var orderVM = new OrderVM()
                    {
                        CreateDate = item.CreateAt,
                        UserID = item.UserId,
                        OrderCode = item.OrderCode,
                        OrderID = item.Id,
                        Status = Enum.GetName(typeof(OrderStatus), item.Status)?.Replace('_', ' '),
                        IntoMoney = item.IntoMoney.ToString(),
                        TotalPrice = item.TotalPrice.ToString(),
                        PaymentMethodId = item.PaymentMethodId,
                        ShipmentDetailId = item.ShipmentDetailId,
                        orderDetailVMs = new List<OrderDetailVM>()
                    };

                    foreach (var detail in item.OrderDetails)
                    {
                        var warehouse = (await _warehouseService.GetWarehouseByProductIdAndBranchId((int)detail.ProductId, detail.BranchId)).FirstOrDefault();
                        orderVM.orderDetailVMs.Add(new OrderDetailVM()
                        {
                            Quantity = detail.Quantity,
                            WarehouseId = warehouse?.Id ?? 0
                        });
                    }

                    result.Add(orderVM);
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
        //CRUD
        public async Task<ResponseDTO<string>> DeleteOrderAsync(int id)
        {
            var response = new ResponseDTO<string>();
            try
            {
                var order = await _unitOfWork.OrderRepository.GetObjectAsync(o => o.Id == id);
                if (order == null)
                {
                    response.IsSuccess = false;
                    response.Message = $"Order with id = {id} is not found!";
                    response.Data = "No query";
                    return response;
                }

                await _unitOfWork.OrderRepository.DeleteAsync(id);

                response.IsSuccess = true;
                response.Message = $"Remove order with id = {id} successfully";
                response.Data = "Query Succesfully";
                return response;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
                return response;
            }
        }
        public async Task<ResponseDTO<string>> ChangeOrderStatusAsync(int id, int status)
        {
            var response = new ResponseDTO<string>();
            try
            {
                var order = await _unitOfWork.OrderRepository.GetObjectAsync(o => o.Id == id);
                if (order == null)
                {
                    response.IsSuccess = false;
                    response.Message = $"Order with id = {id} is not found!";
                    response.Data = "No query";
                    return response;
                }
                order.Status = status;
                if (status == (int)OrderStatus.COMPLETED)
                {
                    await _customerDetailService.UpdateLoyaltyPoints(order.Id);
                }
                await _unitOfWork.OrderRepository.UpdateAsync(order);

                response.IsSuccess = true;
                response.Message = $"Change status of order with id = {id} successfully";
                response.Data = "Query Succesfully";
                return response;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
                return response;
            }
        }
        //For logged-user
        public async Task<ResponseDTO<OrderVM>> ProcessCreatetOrder(OrderCM orderCM)
        {
            var response = new ResponseDTO<OrderVM>();
            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    var branch = await _unitOfWork.BranchRepository.GetObjectAsync(b => b.Id == orderCM.BranchId);
                    if (branch == null)
                    {
                        response.IsSuccess = false;
                        response.Message = "Branch not found!";
                        return response;
                    }

                    var user = await _unitOfWork.UserRepository
                        .GetObjectAsync(u => u.Id == orderCM.UserID);
                    if (user == null)
                    {
                        response.IsSuccess = false;
                        response.Message = $"User with Id = {orderCM.UserID} is not found!";
                        return response;
                    }

                    var shipmentDetail = await _unitOfWork.ShipmentDetailRepository
                        .GetObjectAsync(s => s.Id == orderCM.ShipmentDetailID && s.UserId == user.Id);
                    if (shipmentDetail == null)
                    {
                        response.IsSuccess = false;
                        response.Message = $"ShipmenDetail with Id = {orderCM.ShipmentDetailID} is not found!";
                        return response;
                    }
                    var orderDetails = new List<OrderDetail>();
                    foreach (var item in orderCM.orderDetailCMs)
                    {
                        var productInWarehouse = await _unitOfWork.WarehouseRepository
                                                .GetObjectAsync(p => p.Id == item.WarehouseId);

                        if (productInWarehouse == null || productInWarehouse.TotalQuantity < item.Quantity)
                        {
                            response.IsSuccess = false;
                            response.Message = $"Not enough stock for product {item.WarehouseId} at branch {branch.Id}";
                            return response;
                        }
                        productInWarehouse.TotalQuantity -= item.Quantity;
                        productInWarehouse.AvailableQuantity -= item.Quantity;
                        await _unitOfWork.WarehouseRepository.UpdateAsync(productInWarehouse);
                    }
                    var order = new Order
                    {
                        OrderCode = GenerateOrderCode(),
                        Status = (int?)OrderStatus.PENDING,
                        PaymentMethodId = orderCM.PaymentMethodID,
                        ShipmentDetailId = shipmentDetail.Id,
                        UserId = user.Id,
                        OrderDetails = new List<OrderDetail>(),
                        OrderType = orderCM.OrderType == (int)OrderType.Sale_Order ? (int)OrderType.Sale_Order : (int)OrderType.Rental_Order,
                        ReceivedDate = DateTime.UtcNow,
                        CreateAt = DateTime.UtcNow,
                        BranchId = branch.Id,
                        Note = orderCM.Note,
                    };
                    await _unitOfWork.OrderRepository.InsertAsync(order);

                    // Calculate total price
                    decimal totalPrice = 0;
                    foreach (var item in orderCM.orderDetailCMs)
                    {
                        var productInWarehouse = await _unitOfWork.WarehouseRepository
                                                .GetObjectAsync(p => p.Id == item.WarehouseId);
                        var orderDetail = new OrderDetail
                        {
                            ProductId = productInWarehouse.ProductId,
                            Quantity = item.Quantity,
                            Price = (int)item.Price,
                            OrderId = order.Id,
                            BranchId = productInWarehouse.BranchId,
                            CreatedAt = DateTime.Now,
                        };

                        await _unitOfWork.OrderDetailRepository.InsertAsync(orderDetail);
                        order.OrderDetails.Add(orderDetail);
                        totalPrice += (decimal)(item.Price * item.Quantity);
                    }
                    order.TotalPrice = totalPrice;
                    order.TranSportFee = 0;
                    order.IntoMoney = (decimal)(totalPrice + order.TranSportFee); // if we have coupon, applying to IntoMoney
                    await _unitOfWork.OrderRepository.UpdateAsync(order);
                    // Transaction submit
                    await transaction.CommitAsync();
                    //Return
                    var result = new OrderVM()
                    {
                        OrderID = order.Id,
                        OrderCode = order.OrderCode,
                        CreateDate = order.CreateAt,
                        UserID = order.UserId,
                        Status = Enum.GetName(typeof(OrderStatus), order.Status)?.Replace('_', ' '),
                        IntoMoney = order.IntoMoney.ToString(),
                        TotalPrice = order.TotalPrice.ToString(),
                        PaymentMethodId = order.PaymentMethodId,
                        ShipmentDetailId = order.ShipmentDetailId,
                        orderDetailVMs = orderCM.orderDetailCMs.Select(od => new OrderDetailVM()
                        {
                            WarehouseId = od.WarehouseId,
                            Quantity = od.Quantity,
                        }).ToList(),
                        TransportFee = order.TranSportFee.ToString(),
                        PaymentLink = ""
                    };
                    response.IsSuccess = true;
                    response.Message = $"Order processed successfully";
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
        //For guest
        public async Task<ResponseDTO<GuestOrderVM>> ProcessCreatetOrderForGuest(GuestOrderCM guestOrderCM)
        {
            var response = new ResponseDTO<GuestOrderVM>();
            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    var branch = await _unitOfWork.BranchRepository.GetObjectAsync(b => b.Id == guestOrderCM.BranchId);
                    if (branch == null)
                    {
                        response.IsSuccess = false;
                        response.Message = "Branch not found!";
                        return response;
                    }

                    //Check warehouse
                    var orderDetails = new List<OrderDetail>();
                    foreach (var item in guestOrderCM.orderDetailCMs)
                    {
                        var productInWarehouse = await _unitOfWork.WarehouseRepository
                            .GetObjectAsync(p => p.Id == item.WarehouseId);

                        if (productInWarehouse == null || productInWarehouse.TotalQuantity < item.Quantity)
                        {
                            response.IsSuccess = false;
                            response.Message = $"Not enough stock for product {item.WarehouseId} at branch {branch.Id}";
                            return response;
                        }
                        productInWarehouse.TotalQuantity -= item.Quantity;
                        productInWarehouse.AvailableQuantity -= item.Quantity;
                        await _unitOfWork.WarehouseRepository.UpdateAsync(productInWarehouse);
                    }

                    //Create guest
                    Guest guest = new Guest();
                    guest["Email"] = guestOrderCM.Email;
                    guest["Fullname"] = guestOrderCM.FullName;
                    guest["Address"] = guestOrderCM.Address;
                    guest["PhoneNumber"] = guestOrderCM.PhoneNumber;
                    await _unitOfWork.GuestRepository.InsertAsync(guest);
                    //Create order

                    var order = new Order
                    {
                        OrderCode = GenerateOrderCode(),
                        Status = (int?)OrderStatus.PENDING,
                        PaymentMethodId = guestOrderCM.PaymentMethodID,
                        GuestId = guest.Id,
                        OrderType = guestOrderCM.OrderType == (int)OrderType.Sale_Order ? (int)OrderType.Sale_Order : (int)OrderType.Rental_Order,
                        OrderDetails = new List<OrderDetail>(),
                        ReceivedDate = DateTime.UtcNow,
                        CreateAt = DateTime.UtcNow,
                        BranchId = branch.Id,
                        Note = guestOrderCM.Note,
                    };
                    await _unitOfWork.OrderRepository.InsertAsync(order);

                    // Calculate total price
                    decimal totalPrice = 0;
                    foreach (var item in guestOrderCM.orderDetailCMs)
                    {
                        var productInWarehouse = await _unitOfWork.WarehouseRepository
                            .GetObjectAsync(p => p.Id == item.WarehouseId);

                        var orderDetail = new OrderDetail
                        {
                            ProductId = productInWarehouse.ProductId,
                            Quantity = item.Quantity,
                            Price = (int)item.Price,
                            OrderId = order.Id,
                            BranchId = productInWarehouse.BranchId,
                            CreatedAt = DateTime.UtcNow,
                        };

                        await _unitOfWork.OrderDetailRepository.InsertAsync(orderDetail);
                        order.OrderDetails.Add(orderDetail);
                        totalPrice += (decimal)(item.Price * item.Quantity);
                    }
                    order.TotalPrice = totalPrice;
                    order.TranSportFee = 0;
                    order.IntoMoney = (decimal)(totalPrice + order.TranSportFee); // if we have coupon, applying to IntoMoney
                    await _unitOfWork.OrderRepository.UpdateAsync(order);

                    //Transaction commit
                    await transaction.CommitAsync();
                    //Return
                    var result = new GuestOrderVM()
                    {
                        OrderID = order.Id,
                        OrderCode = order.OrderCode,
                        CreateDate = order.CreateAt,
                        Status = Enum.GetName(typeof(OrderStatus), order.Status)?.Replace('_', ' '),
                        IntoMoney = order.IntoMoney.ToString(),
                        TotalPrice = order.TotalPrice.ToString(),
                        PaymentMethodId = order.PaymentMethodId,
                        orderDetailCMs = guestOrderCM.orderDetailCMs,
                        PaymentLink = "",
                        PhoneNumber = guest.PhoneNumber,
                        Address = guest.Address,
                        Email = guest.Email,
                        FullName = guest.FullName,
                        Note = order.Note,
                        TransportFee = order.TranSportFee.ToString()
                    };
                    response.IsSuccess = true;
                    response.Message = $"Order processed successfully";
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
        //================================
        public async Task<ResponseDTO<RevenueVM>> GetOrdersRevenue(int? branchId, int? orderType, DateTime? from, DateTime? to, int? status)
        {
            var response = new ResponseDTO<RevenueVM>();
            try
            {
                var ordersQuery = (await _unitOfWork.OrderRepository.GetAllAsync()).AsQueryable();
                if (branchId.HasValue)
                {
                    ordersQuery = ordersQuery.Where(o => o.BranchId == branchId.Value);
                }
                if (branchId.HasValue)
                {
                    ordersQuery = ordersQuery.Where(o => o.BranchId == branchId.Value);
                }
                if (from.HasValue)
                {
                    ordersQuery = ordersQuery.Where(o => o.CreateAt >= from.Value);
                }
                if (to.HasValue)
                {
                    ordersQuery = ordersQuery.Where(o => o.CreateAt <= to.Value);
                }

                if (status.HasValue)
                {
                    ordersQuery = ordersQuery.Where(o => o.Status == status.Value);
                }


                var orders = await ordersQuery.ToListAsync();


                var totalRevenue = orders.Sum(o => o.TotalPrice);

                response.IsSuccess = true;
                response.Message = "Query successfully!";
                response.Data = new RevenueVM()
                {
                    TotalOrders = orders.Count,
                    TotalPrice = totalRevenue.ToString()
                };

                return response;
            }
            catch (Exception ex)
            {

                response.IsSuccess = false;
                response.Message = ex.Message;
                return response;
            }
        }
        public async Task<ResponseDTO<int>> ProcessCancelledOrder(PaymentResponse paymentResponse)
        {
            var response = new ResponseDTO<int>();
            try
            {
                var order = await _unitOfWork.OrderRepository.GetObjectAsync(o => o.OrderCode == paymentResponse.OrderCode);
                if (order == null)
                {
                    response.IsSuccess = false;
                    response.Message = $"Order with code {paymentResponse.OrderCode} is not found!";
                    response.Data = 0;
                    return response;
                }
                // Cập nhật trạng thái Order thành "Cancelled"
                order.Status = (int)OrderStatus.CANCELLED;
                await _unitOfWork.OrderRepository.UpdateAsync(order);
                //Cập nhật lại số luọng sản phẩm của chi nhánh đó thông qua update lại warehouse
                var orderDetails = await _unitOfWork.OrderDetailRepository
                                                    .GetAsync(od => od.OrderId == order.Id);
                if (orderDetails == null)
                {
                    response.IsSuccess = false;
                    response.Message = $"Order Detail with Order code {paymentResponse.OrderCode} is not found!";
                    response.Data = 0;
                    return response;
                }
                foreach (var item in orderDetails)
                {
                    var productInWarehouse = await _unitOfWork.WarehouseRepository
                        .GetObjectAsync(wh => wh.ProductId == item.ProductId && wh.BranchId == item.BranchId);
                    if (productInWarehouse != null)
                    {
                        productInWarehouse.TotalQuantity += item.Quantity;
                        productInWarehouse.AvailableQuantity += item.Quantity;
                        await _unitOfWork.WarehouseRepository.UpdateAsync(productInWarehouse);
                    }
                }
                response.IsSuccess = true;
                response.Message = $"Cancelled Order with code {paymentResponse.OrderCode}";
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
        public async Task<ResponseDTO<int>> ProcessCompletedOrder(PaymentResponse paymentResponse)
        {
            var response = new ResponseDTO<int>();
            try
            {
                var order = await _unitOfWork.OrderRepository.GetObjectAsync(o => o.OrderCode == paymentResponse.OrderCode);
                if (order == null)
                {
                    response.IsSuccess = false;
                    response.Message = $"Order with code {paymentResponse.OrderCode} is not found!";
                    response.Data = 0;
                    return response;
                }
                // Cập nhật trạng thái Order thành "Completed"
                order.Status = (int)OrderStatus.PAID;
                await _unitOfWork.OrderRepository.UpdateAsync(order);

                response.IsSuccess = true;
                response.Message = $"Completed Order with code {paymentResponse.OrderCode}";
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
        //Update order for staff
        public async Task<ResponseDTO<OrderVM>> UpdateOrderOfCustomerAsync(int orderId, OrderUM orderUM)
        {
            var response = new ResponseDTO<OrderVM>();
            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    //Check the order is existed
                    var toUpdate = await _unitOfWork.OrderRepository.GetObjectAsync(o => o.Id == orderId, new string[] { "OrderDetails" });
                    if (toUpdate == null)
                    {
                        response.IsSuccess = false;
                        response.Message = $"Order with id {orderId} not found!";
                        return response;
                    }
                    //If isExisted == true, Check the status of Order, Only allowing 'Pending' and 'Paid' when they use PayOs
                    if (toUpdate.Status != (int)OrderStatus.PENDING && !(toUpdate.PaymentMethodId == 2 && toUpdate.Status == (int)OrderStatus.PAID))
                    {
                        response.IsSuccess = false;
                        response.Message = $"Your order status has been {toUpdate.Status}. Only orders with a PENDING status will be updated.!";
                        return response;
                    }
                    //If there are any changes about the branch, which revieved the order, check the branch is existed or not
                    var branch = await _unitOfWork.BranchRepository.GetObjectAsync(b => b.Id == orderUM.BranchId);
                    if (branch == null)
                    {
                        response.IsSuccess = false;
                        response.Message = "Branch not found!";
                        return response;
                    }
                    //The shipment detail of users need existed in List Available ShipmentDetails
                    var shipmentDetail = await _unitOfWork.ShipmentDetailRepository
                        .GetObjectAsync(s => s.Id == orderUM.ShipmentDetailID && s.UserId == toUpdate.UserId);
                    if (shipmentDetail == null)
                    {
                        response.IsSuccess = false;
                        response.Message = $"ShipmenDetail with Id = {orderUM.ShipmentDetailID} is not found!";
                        return response;
                    }
                    //Handling about updating orderDetailItems if there are any changes (update quantity, quan == 0 -> removing, adding more items)
                    foreach (var updatedItem in orderUM.orderDetailUMs)
                    {
                        //Finding the warehouse by productId, so we have a record productInWarehouse
                        var warehouse = await _unitOfWork.WarehouseRepository.GetObjectAsync(w => w.Id == updatedItem.WarehouseId, new string[] { "Product" });
                        //Next, Check for existence of items
                        var orderDetail = await _unitOfWork.OrderDetailRepository.GetObjectAsync(o => o.OrderId == toUpdate.Id && o.ProductId == warehouse.ProductId);

                        //The case: when the quantity are updated to ZERO, it means the item would be removed from order
                        if (updatedItem.Quantity == 0)
                        {
                            if (orderDetail != null)
                            {
                                //restock quantity
                                AdjustStock(warehouse, (int)orderDetail.Quantity, true);

                                //Remove item in order
                                await _unitOfWork.OrderDetailRepository.DeleteAsync(orderDetail);
                            }
                        }
                        else if (orderDetail != null) //The case: when quantity != ZERO and there is a diff between oldQuan and newQuan
                        {
                            int quantityDifference = (int)(updatedItem.Quantity - orderDetail.Quantity);

                            //Check warehouse, the availableQuan >= diffQuan
                            if (warehouse.AvailableQuantity >= quantityDifference)
                            {
                                AdjustStock(warehouse, quantityDifference, false);

                                orderDetail.Quantity = updatedItem.Quantity;
                                await _unitOfWork.OrderDetailRepository.UpdateAsync(orderDetail);
                            }
                            else
                            {
                                response.IsSuccess = false;
                                response.Message = $"Insufficient stock for product: {warehouse.Product.ProductName}";
                                return response;
                            }
                        }
                        else //The case of adding a new item into order
                        {
                            // Add new item
                            if (warehouse.AvailableQuantity >= updatedItem.Quantity)
                            {
                                var newOrderDetail = new OrderDetail
                                {
                                    OrderId = orderId,
                                    ProductId = warehouse.ProductId,
                                    Quantity = updatedItem.Quantity,
                                    BranchId = warehouse.BranchId,
                                    CreatedAt = DateTime.Now,
                                    UpdatedAt = DateTime.Now,
                                    Price = warehouse.Product.Price,
                                };

                                AdjustStock(warehouse, (int)updatedItem.Quantity, false);
                                await _unitOfWork.OrderDetailRepository.InsertAsync(newOrderDetail);
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

                    toUpdate.TotalPrice = orderUM.TotalPrice;
                    toUpdate.TranSportFee = orderUM.TranSportFee;
                    toUpdate.IntoMoney = orderUM.NewIntoMoney ?? toUpdate.TotalPrice;//into money
                    toUpdate.Status = orderUM.Status;
                    toUpdate.Note = orderUM.Note;
                    toUpdate.ShipmentDetailId = orderUM.ShipmentDetailID;
                    toUpdate.BranchId = orderUM.BranchId;
                    await _unitOfWork.OrderRepository.UpdateAsync(toUpdate);

                    await transaction.CommitAsync();
                    //Return
                    var orderDetails = await _unitOfWork.OrderDetailRepository.GetAsync(od => od.OrderId == toUpdate.Id);
                    var result = new OrderVM()
                    {
                        OrderID = toUpdate.Id,
                        OrderCode = toUpdate.OrderCode,
                        CreateDate = toUpdate.CreateAt,
                        UserID = toUpdate.UserId,
                        Status = toUpdate.Status.ToString(),
                        TotalPrice = toUpdate.TotalPrice.ToString(),
                        IntoMoney = toUpdate.IntoMoney.ToString(),
                        PaymentMethodId = toUpdate.PaymentMethodId,
                        ShipmentDetailId = toUpdate.ShipmentDetailId,
                        TransportFee = toUpdate.TranSportFee.ToString(),
                        orderDetailVMs = orderDetails.Select(o => new OrderDetailVM()
                        {
                            Quantity = o.Quantity,
                            WarehouseId = GetWarehouseId(o.ProductId)
                        }).ToList(),
                    };

                    response.IsSuccess = true;
                    response.Message = $"Update Order processed successfully";
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
        public async Task<ResponseDTO<OrderVM>> UpdateOrderOfGuestAsync(int orderId, GuestOrderUM orderUM)
        {
            var response = new ResponseDTO<OrderVM>();
            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    //Check the order is existed
                    var toUpdate = await _unitOfWork.OrderRepository.GetObjectAsync(o => o.Id == orderId, new string[] { "OrderDetails" });
                    if (toUpdate == null)
                    {
                        response.IsSuccess = false;
                        response.Message = $"Order with id {orderId} not found!";
                        return response;
                    }
                    //If isExisted == true, Check the status of Order, Only allowing 'Pending' and 'Paid' when they use PayOs
                    if (toUpdate.Status != (int)OrderStatus.PENDING && !(toUpdate.PaymentMethodId == 2 && toUpdate.Status == (int)OrderStatus.PAID))
                    {
                        response.IsSuccess = false;
                        response.Message = $"Your order status has been {toUpdate.Status}. Only orders with a PENDING status will be updated.!";
                        return response;
                    }
                    //If there are any changes about the branch, which revieved the order, check the branch is existed or not
                    var branch = await _unitOfWork.BranchRepository.GetObjectAsync(b => b.Id == orderUM.BranchId);
                    if (branch == null)
                    {
                        response.IsSuccess = false;
                        response.Message = "Branch not found!";
                        return response;
                    }
                    //Find the guest in database of updating order, if isExisted == true, updating with given infors
                    var guestOfOrder = await _unitOfWork.GuestRepository.GetObjectAsync(g => g.Id == toUpdate.GuestId);
                    if (guestOfOrder == null)
                    {
                        response.IsSuccess = false;
                        response.Message = $"Guest's information of the orderId = {toUpdate.Id} not found;";
                        return response;
                    }
                    else
                    {
                        guestOfOrder.Address = orderUM.guestUM.Address;
                        guestOfOrder.Email = orderUM.guestUM.Email;
                        guestOfOrder.FullName = orderUM.guestUM.FullName;
                        guestOfOrder.PhoneNumber = orderUM.guestUM.PhoneNumber;
                        await _unitOfWork.GuestRepository.UpdateAsync(guestOfOrder);
                    }
                    //Handling about updating orderDetailItems if there are any changes (update quantity, quan == 0 -> removing, adding more items)
                    foreach (var updatedItem in orderUM.orderDetailUMs)
                    {
                        //Finding the warehouse by productId, so we have a record productInWarehouse
                        var warehouse = await _unitOfWork.WarehouseRepository.GetObjectAsync(w => w.Id == updatedItem.WarehouseId, new string[] { "Product" });
                        //Next, Check for existence of items
                        var orderDetail = await _unitOfWork.OrderDetailRepository.GetObjectAsync(o => o.OrderId == toUpdate.Id && o.ProductId == warehouse.ProductId);

                        //The case: when the quantity are updated to ZERO, it means the item would be removed from order
                        if (updatedItem.Quantity == 0)
                        {
                            if (orderDetail != null)
                            {
                                //restock quantity
                                AdjustStock(warehouse, (int)orderDetail.Quantity, true);

                                //Remove item in order
                                await _unitOfWork.OrderDetailRepository.DeleteAsync(orderDetail);
                            }
                        }
                        else if (orderDetail != null) //The case: when quantity != ZERO and there is a diff between oldQuan and newQuan
                        {
                            int quantityDifference = (int)(updatedItem.Quantity - orderDetail.Quantity);

                            //Check warehouse, the availableQuan >= diffQuan
                            if (warehouse.AvailableQuantity >= quantityDifference)
                            {
                                AdjustStock(warehouse, quantityDifference, false);

                                orderDetail.Quantity = updatedItem.Quantity;
                                await _unitOfWork.OrderDetailRepository.UpdateAsync(orderDetail);
                            }
                            else
                            {
                                response.IsSuccess = false;
                                response.Message = $"Insufficient stock for product: {warehouse.Product.ProductName}";
                                return response;
                            }
                        }
                        else //The case of adding a new item into order
                        {
                            // Add new item
                            if (warehouse.AvailableQuantity >= updatedItem.Quantity)
                            {
                                var newOrderDetail = new OrderDetail
                                {
                                    OrderId = orderId,
                                    ProductId = warehouse.ProductId,
                                    Quantity = updatedItem.Quantity,
                                    BranchId = warehouse.BranchId,
                                    CreatedAt = DateTime.Now,
                                    UpdatedAt = DateTime.Now,
                                    Price = warehouse.Product.Price,
                                };

                                AdjustStock(warehouse, (int)updatedItem.Quantity, false);
                                await _unitOfWork.OrderDetailRepository.InsertAsync(newOrderDetail);
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

                    toUpdate.TotalPrice = orderUM.TotalPrice;
                    toUpdate.TranSportFee = orderUM.TranSportFee;
                    toUpdate.IntoMoney = orderUM.NewIntoMoney ?? toUpdate.TotalPrice;
                    toUpdate.Status = orderUM.Status;
                    toUpdate.Note = orderUM.Note;
                    toUpdate.BranchId = orderUM.BranchId;
                    await _unitOfWork.OrderRepository.UpdateAsync(toUpdate);

                    await transaction.CommitAsync();
                    //Return
                    var orderDetails = await _unitOfWork.OrderDetailRepository.GetAsync(od => od.OrderId == toUpdate.Id);
                    var result = new OrderVM()
                    {
                        OrderID = toUpdate.Id,
                        OrderCode = toUpdate.OrderCode,
                        CreateDate = toUpdate.CreateAt,
                        UserID = toUpdate.UserId,
                        Status = toUpdate.Status.ToString(),
                        IntoMoney = toUpdate.IntoMoney.ToString(),
                        TotalPrice = toUpdate.TotalPrice.ToString(),
                        PaymentMethodId = toUpdate.PaymentMethodId,
                        ShipmentDetailId = toUpdate.ShipmentDetailId,
                        TransportFee = toUpdate.TranSportFee.ToString(),
                        orderDetailVMs = orderDetails.Select(o => new OrderDetailVM()
                        {
                            Quantity = o.Quantity,
                            WarehouseId = GetWarehouseId(o.ProductId)
                        }).ToList()
                    };
                    response.IsSuccess = true;
                    response.Message = $"Order processed successfully";
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
        //--------------------------------------------------
        public async Task<Order> AddOrderAsync(Order order)
        {
            await _unitOfWork.OrderRepository.InsertAsync(order);
            return order;
        }
        public async Task<bool> UpdateOrderAsync(int orderId, int status)
        {
            var checkExist = await _unitOfWork.OrderRepository.GetObjectAsync(_ => _.Id == orderId);
            if (checkExist != null)
            {
                checkExist.Status = status;
                await _unitOfWork.OrderRepository.UpdateAsync(checkExist);
                return true;
            }
            return false;
        }
        public async Task<Order> GetOrderByIdFromUserAsync(int orderId, int userId)
        {
            return await _unitOfWork.OrderRepository.GetObjectAsync(_ => _.Id == orderId && _.UserId == userId);
        }
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
        public async Task<IQueryable<Order>> GetAllOrderQueryableAsync()
        {
            var query = await _unitOfWork.OrderRepository.GetAllAsync();

            return query.AsQueryable();
        }
        private int GetWarehouseId(int? productId)
        {
            var warehouse = _unitOfWork.WarehouseRepository.FindObject(o => o.ProductId == productId);
            return warehouse.Id;
        }
    }

}
