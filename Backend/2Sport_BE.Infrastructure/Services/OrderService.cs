using _2Sport_BE.Repository.Data;
using _2Sport_BE.Repository.Interfaces;
using _2Sport_BE.Repository.Models;
using _2Sport_BE.Service.DTOs;
using _2Sport_BE.Service.Enums;
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

namespace _2Sport_BE.Service.Services
{
    public interface IOrderService
    {
        Task<Order> GetOrderByIdFromUserAsync(int orderId, int userId);
        Task<Order> AddOrderAsync(Order order);
       
        //Upgrade code 
        Task<ResponseDTO<int>> ProcessCheckoutOrder(OrderCM orderCM);
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
    }
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;

        public OrderService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
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
        public async Task<ResponseDTO<int>> ProcessCheckoutOrder(OrderCM orderCM)
        {
            var response = new ResponseDTO<int>();
            try
            {
                var branch = await _unitOfWork.BranchRepository.GetObjectAsync(b => b.Id == orderCM.BranchId);
                if (branch == null)
                {
                    response.IsSuccess = false;
                    response.Message = "Branch not found!";
                    return response;
                }

                if (orderCM.orderDetailCMs == null || !orderCM.orderDetailCMs.Any())
                {
                    response.IsSuccess = false;
                    response.Message = "Order details are empty.";
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

                var order = new Order
                {
                    OrderCode = GenerateOrderCode(),
                    Status = (int?)OrderStatus.PENDING,
                    PaymentMethodId = orderCM.PaymentMethodID,
                    ShipmentDetailId = shipmentDetail.Id,
                    UserId = user.Id,
                    OrderDetails = new List<OrderDetail>(),
                    ReceivedDate = DateTime.UtcNow,
                    CreateAt = DateTime.UtcNow,

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

                response.IsSuccess = true;
                response.Message = $"Order processed successfully";
                response.Data = order.Id;
                return response;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
                return response;
            }
        }

        //--------------------------------------------------
        public async Task<decimal> GetOrdersRevenueByMonth(int month)
        {

            var ordersInMonth = await _unitOfWork.OrderRepository.GetAsync(_ => _.ReceivedDate.Value.Month == month);

            var totalOrdersInMonth = ordersInMonth.ToList().Sum(_ => _.IntoMoney);

            return (decimal)totalOrdersInMonth;
        }
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
