using _2Sport_BE.Repository.Data;
using _2Sport_BE.Repository.Interfaces;
using _2Sport_BE.Repository.Models;
using _2Sport_BE.Service.DTOs;
using _2Sport_BE.Service.Enums;
using Microsoft.CodeAnalysis.Semantics;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Net.payOS;
using Net.payOS.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vonage.Users;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
        Task<ResponseDTO<string>> UpdateOrderAsync(int orderId, OrderUM orderUM);
        Task<IQueryable<Order>> GetAllOrderQueryableAsync();
        Task<ResponseDTO<int>> ProcessCancelledOrder(PaymentResponse paymentResponse);
        Task<ResponseDTO<int>> ProcessCompletedOrder(PaymentResponse paymentResponse);

        Task<ResponseDTO<RevenueVM>> GetOrdersRevenue(int? branchId, int? orderType, DateTime? from, DateTime? to, int? status);
    }
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICustomerDetailService _customerDetailService;

        public OrderService(IUnitOfWork unitOfWork, ICustomerDetailService customerDetailService)
        {
            _unitOfWork = unitOfWork;
            _customerDetailService = customerDetailService;
        }
        public string GenerateOrderCode()
        {
            string datePart = DateTime.UtcNow.ToString("yyyyMMdd");

            Random random = new Random();
            string randomPart = random.Next(100000, 999999).ToString();

            string orderCode = $"{datePart}_{randomPart}";

            return orderCode;
        }
        public async Task<ResponseDTO<List<OrderVM>>> GetAllOrdersAsync()
        {
            var response = new ResponseDTO<List<OrderVM>>();
            try
            {
                var query = await _unitOfWork.OrderRepository.GetAllAsync("User", "OrderDetails");
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
                        CustomerName = item.User.FullName,
                        OrderCode = item.OrderCode,
                        OrderID = item.Id,
                        Status = Enum.GetName(typeof(OrderStatus), item.Status)?.Replace('_', ' '),
                        IntoMoney = item.IntoMoney.ToString(),
                        TotalPrice = item.TotalPrice.ToString(),
                        PaymentMethodId = item.PaymentMethodId,
                        ShipmentDetailId = item.ShipmentDetailId,
                        orderDetailCMs = (List<OrderDetailCM>)item.OrderDetails.Select(_ => new OrderDetailCM()
                        {
                            Price = (decimal)_.Price,
                            ProductID = _.ProductId,
                            Quantity = _.Quantity
                        })
                    };
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
                var item = (await _unitOfWork.OrderRepository.GetAsync(_ => _.Id == id, "User, OrderDetails")).FirstOrDefault();
                if (item == null)
                {
                    response.IsSuccess = false;
                    response.Message = $"Order with id = {id} is not found";
                    return response;
                }
                var result = new OrderVM()
                {
                    CreateDate = item.CreateAt,
                    CustomerName = item.User.FullName,
                    OrderCode = item.OrderCode,
                    OrderID = item.Id,
                    Status = Enum.GetName(typeof(OrderStatus), item.Status)?.Replace('_', ' '),
                    IntoMoney = item.IntoMoney.ToString(),
                    TotalPrice = item.TotalPrice.ToString(),
                    PaymentMethodId = item.PaymentMethodId,
                    ShipmentDetailId = item.ShipmentDetailId,
                    orderDetailCMs = (List<OrderDetailCM>)item.OrderDetails.Select(_ => new OrderDetailCM()
                    {
                        Price = (decimal)_.Price,
                        ProductID = _.ProductId,
                        Quantity = _.Quantity
                    })
                };

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
                        CustomerName = item.User.FullName,
                        OrderCode = item.OrderCode,
                        OrderID = item.Id,
                        Status = Enum.GetName(typeof(OrderStatus), item.Status)?.Replace('_', ' '),
                        IntoMoney = item.IntoMoney.ToString(),
                        TotalPrice = item.TotalPrice.ToString(),
                        PaymentMethodId = item.PaymentMethodId,
                        ShipmentDetailId = item.ShipmentDetailId,
                        orderDetailCMs = (List<OrderDetailCM>)item.OrderDetails.Select(_ => new OrderDetailCM()
                        {
                            Price = (decimal)_.Price,
                            ProductID = _.ProductId,
                            Quantity = _.Quantity
                        })
                    };
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
                        CustomerName = item.User.FullName,
                        OrderCode = item.OrderCode,
                        OrderID = item.Id,
                        Status = Enum.GetName(typeof(OrderStatus), item.Status)?.Replace('_', ' '),
                        IntoMoney = item.IntoMoney.ToString(),
                        TotalPrice = item.TotalPrice.ToString(),
                        PaymentMethodId = item.PaymentMethodId,
                        ShipmentDetailId = item.ShipmentDetailId,
                        orderDetailCMs = (List<OrderDetailCM>)item.OrderDetails.Select(_ => new OrderDetailCM()
                        {
                            Price = (decimal)_.Price,
                            ProductID = _.ProductId,
                            Quantity = _.Quantity
                        })
                    };
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
                var result = new OrderVM()
                {
                    CreateDate = item.CreateAt,
                    CustomerName = item.User.FullName,
                    OrderCode = item.OrderCode,
                    OrderID = item.Id,
                    Status = Enum.GetName(typeof(OrderStatus), item.Status)?.Replace('_', ' '),
                    IntoMoney = item.IntoMoney.ToString(),
                    TotalPrice = item.TotalPrice.ToString(),
                    PaymentMethodId = item.PaymentMethodId,
                    ShipmentDetailId = item.ShipmentDetailId,
                    orderDetailCMs = (List<OrderDetailCM>)item.OrderDetails.Select(_ => new OrderDetailCM()
                    {
                        Price = (decimal)_.Price,
                        ProductID = _.ProductId,
                        Quantity = _.Quantity
                    })
                };


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
                        CustomerName = item.User.FullName,
                        OrderCode = item.OrderCode,
                        OrderID = item.Id,
                        Status = Enum.GetName(typeof(OrderStatus), item.Status)?.Replace('_', ' '),
                        IntoMoney = item.IntoMoney.ToString(),
                        TotalPrice = item.TotalPrice.ToString(),
                        PaymentMethodId = item.PaymentMethodId,
                        ShipmentDetailId = item.ShipmentDetailId,
                        orderDetailCMs = (List<OrderDetailCM>)item.OrderDetails.Select(_ => new OrderDetailCM()
                        {
                            Price = (decimal)_.Price,
                            ProductID = _.ProductId,
                            Quantity = _.Quantity
                        })
                    };
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
                        CustomerName = item.User.FullName,
                        OrderCode = item.OrderCode,
                        OrderID = item.Id,
                        Status = Enum.GetName(typeof(OrderStatus), item.Status)?.Replace('_', ' '),
                        IntoMoney = item.IntoMoney.ToString(),
                        TotalPrice = item.TotalPrice.ToString(),
                        PaymentMethodId = item.PaymentMethodId,
                        ShipmentDetailId = item.ShipmentDetailId,
                        orderDetailCMs = (List<OrderDetailCM>)item.OrderDetails.Select(_ => new OrderDetailCM()
                        {
                            Price = (decimal)_.Price,
                            ProductID = _.ProductId,
                            Quantity = _.Quantity
                        })
                    };
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
        public async Task<ResponseDTO<string>> ChangeOrderStatusAsync(int id ,int status)
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
                if(status == (int)OrderStatus.COMPLETED)
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
        //For logged-user checkout
        public async Task<ResponseDTO<OrderVM>> ProcessCreatetOrder(OrderCM orderCM)
        {
            var response = new ResponseDTO<OrderVM>();
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
                                            .GetObjectAsync(p => p.ProductId == item.ProductID 
                                                         && p.BranchId == item.BranchId);

                    if (productInWarehouse == null || productInWarehouse.Quantity < item.Quantity)
                    {
                        response.IsSuccess = false;
                        response.Message = $"Not enough stock for product {item.ProductID} at branch {branch.Id}";
                        return response;
                    }
                    productInWarehouse.Quantity -= item.Quantity;
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
                   
                    var orderDetail = new OrderDetail
                    {
                        ProductId = item.ProductID,
                        Quantity = item.Quantity,
                        Price = (int)item.Price,
                        OrderId = order.Id
                    };

                    await _unitOfWork.OrderDetailRepository.InsertAsync(orderDetail);
                    order.OrderDetails.Add(orderDetail);
                    totalPrice += (decimal)(item.Price * item.Quantity);
                }
                order.TotalPrice = totalPrice;
                order.IntoMoney = (decimal)(totalPrice); // if we have coupon, applying to IntoMoney
                order.TranSportFee = 0;
                await _unitOfWork.OrderRepository.UpdateAsync(order);
                //Return
                var result = new OrderVM()
                {
                    OrderID = order.Id,
                    OrderCode = order.OrderCode,
                    CreateDate = order.CreateAt,
                    CustomerName = order.User.FullName,
                    Status = Enum.GetName(typeof(OrderStatus), order.Status)?.Replace('_', ' '),
                    IntoMoney = order.IntoMoney.ToString(),
                    TotalPrice = order.TotalPrice.ToString(),
                    PaymentMethodId = order.PaymentMethodId,
                    ShipmentDetailId = order.ShipmentDetailId,
                    orderDetailCMs = orderCM.orderDetailCMs,
                    PaymentLink = ""
                };
                response.IsSuccess = true;
                response.Message = $"Order processed successfully";
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
        //For guest checkout
        public async Task<ResponseDTO<GuestOrderVM>> ProcessCreatetOrderForGuest(GuestOrderCM guestOrderCM)
        {
            var response = new ResponseDTO<GuestOrderVM>();
            try
            {
                var branch = await _unitOfWork.BranchRepository.GetObjectAsync(b => b.Id == guestOrderCM.BranchId);
                if (branch == null)
                {
                    response.IsSuccess = false;
                    response.Message = "Branch not found!";
                    return response;
                }

                //Create guest
                Guest guest = new Guest();
                guest["Email"] = guestOrderCM.Email;
                guest["Fullname"] = guestOrderCM.FullName;
                guest["Address"] = guestOrderCM.Address;
                guest["PhoneNumber"] = guestOrderCM.PhoneNumber;
                await _unitOfWork.GuestRepository.InsertAsync(guest);

                //Check warehouse
                var orderDetails = new List<OrderDetail>();
                foreach (var item in guestOrderCM.orderDetailCMs)
                {
                    var productInWarehouse = await _unitOfWork.WarehouseRepository
                                            .GetObjectAsync(p => p.ProductId == item.ProductID
                                                         && p.BranchId == item.BranchId);

                    if (productInWarehouse == null || productInWarehouse.Quantity < item.Quantity)
                    {
                        response.IsSuccess = false;
                        response.Message = $"Not enough stock for product {item.ProductID} at branch {branch.Id}";
                        return response;
                    }
                    productInWarehouse.Quantity -= item.Quantity;
                    await _unitOfWork.WarehouseRepository.UpdateAsync(productInWarehouse);
                }
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

                    var orderDetail = new OrderDetail
                    {
                        ProductId = item.ProductID,
                        Quantity = item.Quantity,
                        Price = (int)item.Price,
                        OrderId = order.Id
                    };

                    await _unitOfWork.OrderDetailRepository.InsertAsync(orderDetail);
                    order.OrderDetails.Add(orderDetail);
                    totalPrice += (decimal)(item.Price * item.Quantity);
                }
                order.TotalPrice = totalPrice;
                order.IntoMoney = (decimal)(totalPrice); // if we have coupon, applying to IntoMoney
                order.TranSportFee = 0;
                await _unitOfWork.OrderRepository.UpdateAsync(order);
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
                    Note = order.Note
                };
                response.IsSuccess = true;
                response.Message = $"Order processed successfully";
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
                        productInWarehouse.Quantity += item.Quantity;
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
        public Task<ResponseDTO<string>> UpdateOrderAsync(int orderId, OrderUM orderUM)
        {
            throw new NotImplementedException();
        }
        public async Task<IQueryable<Order>> GetAllOrderQueryableAsync()
        {
            var query = await _unitOfWork.OrderRepository.GetAllAsync();

            return query.AsQueryable();
        }
    }

}
