using _2Sport_BE.Infrastructure.Services;
using _2Sport_BE.Repository.Interfaces;
using _2Sport_BE.Repository.Models;
using _2Sport_BE.Service.DTOs;
using _2Sport_BE.Service.Enums;


namespace _2Sport_BE.Service.Services
{
    public interface IRentalOrderService
    {
        Task<ResponseDTO<RentalOrderVM>> CreateRentalOrderForCustomer(RentalOrderCM rentalOrderCM);
        Task<ResponseDTO<GuestRentalOrderVM>> CreateRentalOrderForGuest(GuestRentalOrderCM guestRentalOrderCM);
        Task<ResponseDTO<RentalOrderVM>> UpdateRentailOrderForCustomerAsync(int orderId, RentalOrderUM rentalOrderUM);
        Task<ResponseDTO<RentalOrderVM>> UpdateRentailOrderForGuestAsync(int orderId, GuestRentalOrderUM guestRentalOrderUM);

    }
    public class RentalOrderService : IRentalOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICustomerDetailService _customerDetailService;
        private readonly IWarehouseService _warehouseService;
        public RentalOrderService(IUnitOfWork unitOfWork, ICustomerDetailService customerDetailService, IWarehouseService warehouseService)
        {
            _unitOfWork = unitOfWork;
            _customerDetailService = customerDetailService;
            _warehouseService = warehouseService;
        }
        public string GenerateOrderCode()
        {
            string datePart = DateTime.UtcNow.ToString("yyMMdd");

            Random random = new Random();
            string randomPart = random.Next(1000, 9999).ToString();

            string orderCode = $"{datePart}{randomPart}";

            return orderCode;
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
        private bool CheckValidOfRentalDate(DateTime? from, DateTime? to)
        {
            if (from == null || to == null) return false;

            if (from.Value.Date < DateTime.Now.Date || to.Value.Date < DateTime.Now.Date) return false;
            if (from.Value.Date > to.Value.Date) return false;

            return true;
        }
        private decimal RentalPriceCalculation(decimal price, decimal transportFee, decimal damageFee, decimal lateFee)
        {
            return price + transportFee + damageFee + lateFee;
        }
        public async Task<ResponseDTO<RentalOrderVM>> CreateRentalOrderForCustomer(RentalOrderCM rentalOrderCM)
        {
            var response = new ResponseDTO<RentalOrderVM>();
            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    var branch = await _unitOfWork.BranchRepository.GetObjectAsync(b => b.Id == rentalOrderCM.BranchId);
                    if (branch == null)
                    {
                        response.IsSuccess = false;
                        response.Message = "Branch not found!";
                        return response;
                    }

                    var user = await _unitOfWork.UserRepository
                        .GetObjectAsync(u => u.Id == rentalOrderCM.UserID);
                    if (user == null)
                    {
                        response.IsSuccess = false;
                        response.Message = $"User with Id = {rentalOrderCM.UserID} is not found!";
                        return response;
                    }

                    var shipmentDetail = await _unitOfWork.ShipmentDetailRepository
                        .GetObjectAsync(s => s.Id == rentalOrderCM.ShipmentDetailID && s.UserId == user.Id);
                    if (shipmentDetail == null)
                    {
                        response.IsSuccess = false;
                        response.Message = $"ShipmenDetail with Id = {rentalOrderCM.ShipmentDetailID} is not found!";
                        return response;
                    }
                    var dateValid = CheckValidOfRentalDate(rentalOrderCM.RentalStartDate, rentalOrderCM.RentalEndDate);
                    if (!dateValid)
                    {
                        response.IsSuccess = false;
                        response.Message = $"Rental dates are invalid!";
                        return response;
                    }
                    var orderDetails = new List<RentalOrderItems>();
                    var warehouseToUpdate = new List<Warehouse>();
                    foreach (var item in rentalOrderCM.rentalOrderItems)
                    {
                        var productInWarehouse = await _unitOfWork.WarehouseRepository
                                                .GetObjectAsync(p => p.Id == item.WarehouseId);

                         if (productInWarehouse == null || !(productInWarehouse.TotalQuantity >= item.Quantity && productInWarehouse.AvailableQuantity >= item.Quantity))
                        {
                            response.IsSuccess = false;
                            response.Message = $"Not enough stock for product {item.WarehouseId} at branch {branch.Id}";
                            return response;
                        }
                        else
                        {
                            productInWarehouse.AvailableQuantity -= item.Quantity;
                            warehouseToUpdate.Add(productInWarehouse);
                        }
                    }
                    await _unitOfWork.WarehouseRepository.UpdateRangeAsync(warehouseToUpdate);

                    var order = new Order
                    {
                        OrderCode = GenerateOrderCode(),
                        Status = (int?)OrderStatus.PENDING,
                        PaymentMethodId = rentalOrderCM.PaymentMethodID,
                        ShipmentDetailId = shipmentDetail.Id,
                        UserId = user.Id,
                        OrderDetails = new List<OrderDetail>(),
                        OrderType = rentalOrderCM.OrderType == (int)OrderType.Sale_Order ? (int)OrderType.Sale_Order : (int)OrderType.Rental_Order,
                        ReceivedDate = DateTime.UtcNow,
                        CreateAt = DateTime.UtcNow,
                        BranchId = branch.Id,
                        Note = rentalOrderCM.Note,
                    };

                    await _unitOfWork.OrderRepository.InsertAsync(order);

                    var rentalDetail = new RentalOrder()
                    {
                        OrderId = order.Id,
                        RentalEndDate = rentalOrderCM.RentalEndDate,
                        RentalStartDate = rentalOrderCM.RentalStartDate,
                        IsInspected = false,
                        IsRestocked = false,
                        LateFee = 0,
                        DamageFee = 0
                    };

                    await _unitOfWork.RentalOrderRepository.InsertAsync(rentalDetail);
                    // Calculate total price
                    decimal totalPrice = 0;
                    foreach (var item in rentalOrderCM.rentalOrderItems)
                    {
                        var productInWarehouse = await _unitOfWork.WarehouseRepository
                                                .GetObjectAsync(p => p.Id == item.WarehouseId, new string[] { "Product" });
                        if (productInWarehouse != null)
                        {
                            var orderDetail = new OrderDetail
                            {
                                ProductId = productInWarehouse.ProductId,
                                Quantity = item.Quantity,
                                Price = productInWarehouse.Product.RentPrice,
                                OrderId = order.Id,
                                BranchId = productInWarehouse.BranchId,
                                CreatedAt = DateTime.Now,
                            };

                            await _unitOfWork.OrderDetailRepository.InsertAsync(orderDetail);
                            order.OrderDetails.Add(orderDetail);
                            totalPrice += (decimal)(productInWarehouse.Product.RentPrice * item.Quantity);
                        }
                        else
                        {
                            response.IsSuccess = true;
                            response.Message = $"Stock is not found!";
                            return response;
                        }

                    }
                    order.TotalPrice = totalPrice;
                    order.TranSportFee = rentalOrderCM.TransportFee ?? 0;
                    order.IntoMoney = RentalPriceCalculation(order.TotalPrice, (decimal)order.TranSportFee, (decimal)rentalDetail.DamageFee, rentalDetail.LateFee); // if we have coupon, applying to IntoMoney
                    await _unitOfWork.OrderRepository.UpdateAsync(order);

                    // commit transaction
                    await transaction.CommitAsync();
                    //return
                    var result = new RentalOrderVM()
                    {
                        UserID = (int)order.UserId,
                        BranchId = (int)order.BranchId,
                        CreatedDate = order.CreateAt,
                        Note = order.Note,
                        PaymentMethodID = (int)order.PaymentMethodId,
                        RentalStartDate = rentalDetail.RentalStartDate,
                        RentalEndDate = rentalDetail.RentalEndDate,
                        ShipmentDetailID = (int)order.ShipmentDetailId,
                        rentalOrderItems = rentalOrderCM.rentalOrderItems,
                        OrderID = order.Id,
                        OrderCode = order.OrderCode,
                        IntoMoney = order.IntoMoney,
                       TotalPrice = order.TotalPrice,
                       TransportFee = order.TranSportFee,
                    };
                    response.IsSuccess = true;
                    response.Message = $"Rental Order processed successfully";
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
        public async Task<ResponseDTO<GuestRentalOrderVM>> CreateRentalOrderForGuest(GuestRentalOrderCM rentalOrderCM)
        {
            var response = new ResponseDTO<GuestRentalOrderVM>();
            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    var branch = await _unitOfWork.BranchRepository.GetObjectAsync(b => b.Id == rentalOrderCM.BranchId);
                    if (branch == null)
                    {
                        response.IsSuccess = false;
                        response.Message = "Branch not found!";
                        return response;
                    }
                    var dateValid = CheckValidOfRentalDate(rentalOrderCM.RentalStartDate, rentalOrderCM.RentalEndDate);
                    if (!dateValid)
                    {
                        response.IsSuccess = false;
                        response.Message = $"Rental dates are invalid!";
                        return response;
                    }

                    //Check stock
                    var orderDetails = new List<RentalOrderItems>();
                    foreach (var item in rentalOrderCM.rentalOrderItems)
                    {
                        var productInWarehouse = await _unitOfWork.WarehouseRepository
                                                .GetObjectAsync(p => p.Id == item.WarehouseId);

                        if (productInWarehouse == null || productInWarehouse.TotalQuantity < item.Quantity)
                        {
                            response.IsSuccess = false;
                            response.Message = $"Not enough stock for product {item.WarehouseId} at branch {branch.Id}";
                            return response;
                        }
                        productInWarehouse.AvailableQuantity -= item.Quantity;
                        await _unitOfWork.WarehouseRepository.UpdateAsync(productInWarehouse);
                    }

                    Guest guest = new Guest();
                    guest["Email"] = rentalOrderCM.guestCM.Email;
                    guest["Fullname"] = rentalOrderCM.guestCM.FullName;
                    guest["Address"] = rentalOrderCM.guestCM.Address;
                    guest["PhoneNumber"] = rentalOrderCM.guestCM.PhoneNumber;
                    await _unitOfWork.GuestRepository.InsertAsync(guest);

                    var order = new Order
                    {
                        OrderCode = GenerateOrderCode(),
                        Status = (int?)OrderStatus.PENDING,
                        PaymentMethodId = rentalOrderCM.PaymentMethodID,
                        GuestId = guest.Id,
                        OrderDetails = new List<OrderDetail>(),
                        OrderType = rentalOrderCM.OrderType == (int)OrderType.Sale_Order ? (int)OrderType.Sale_Order : (int)OrderType.Rental_Order,
                        ReceivedDate = DateTime.UtcNow,
                        CreateAt = DateTime.UtcNow,
                        BranchId = branch.Id,
                        Note = rentalOrderCM.Note,

                    };

                    await _unitOfWork.OrderRepository.InsertAsync(order);

                    var rentalDetail = new RentalOrder()
                    {
                        OrderId = order.Id,
                        RentalEndDate = rentalOrderCM.RentalEndDate,
                        RentalStartDate = rentalOrderCM.RentalStartDate,
                        IsInspected = false,
                        IsRestocked = false,
                        LateFee = 0,
                        DamageFee = 0
                    };

                    await _unitOfWork.RentalOrderRepository.InsertAsync(rentalDetail);
                    // Calculate total price
                    decimal totalPrice = 0;
                    foreach (var item in rentalOrderCM.rentalOrderItems)
                    {
                        var productInWarehouse = await _unitOfWork.WarehouseRepository
                                                .GetObjectAsync(p => p.Id == item.WarehouseId, new string[] { "Product" });
                        if (productInWarehouse != null)
                        {
                            var orderDetail = new OrderDetail
                            {
                                ProductId = productInWarehouse.ProductId,
                                Quantity = item.Quantity,
                                Price = productInWarehouse.Product.RentPrice,
                                OrderId = order.Id,
                                BranchId = productInWarehouse.BranchId,
                                CreatedAt = DateTime.Now,
                            };

                            await _unitOfWork.OrderDetailRepository.InsertAsync(orderDetail);
                            order.OrderDetails.Add(orderDetail);
                            totalPrice += (decimal)(productInWarehouse.Product.RentPrice * item.Quantity);
                        }
                        else
                        {
                            response.IsSuccess = true;
                            response.Message = $"Stock is not found!";
                            return response;
                        }

                    }
                    order.TotalPrice = totalPrice;
                    order.TranSportFee = rentalOrderCM.TransportFee ?? 0;
                    order.IntoMoney = (decimal)(totalPrice + order.TranSportFee); // if we have coupon, applying to IntoMoney
                    await _unitOfWork.OrderRepository.UpdateAsync(order);
                    //Transaction commit
                    await transaction.CommitAsync();
                    //return
                    var result = new GuestRentalOrderVM()
                    {
                        BranchId = (int)order.BranchId,
                        CreatedDate = order.CreateAt,
                        Note = order.Note,
                        PaymentMethodID = (int)order.PaymentMethodId,
                        RentalStartDate = rentalDetail.RentalStartDate,
                        RentalEndDate = rentalDetail.RentalEndDate,
                        rentalOrderItems = rentalOrderCM.rentalOrderItems,
                        OrderCode = order.OrderCode,
                        Address = guest.Address,
                        Email = guest.Email,
                        FullName = guest.FullName,
                        OrderID = order.Id,
                        PhoneNumber = guest.PhoneNumber,
                        IntoMoney = order.IntoMoney,
                        TotalPrice = order.TotalPrice,
                        TransportFee = order.TranSportFee,
                        PaymentLink = "",
                    };
                    response.IsSuccess = true;
                    response.Message = $"Rental Order processed successfully";
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
        public async Task<ResponseDTO<RentalOrderVM>> UpdateRentailOrderForCustomerAsync(int orderId, RentalOrderUM rentalOrderUM)
        {
            /* There are 3 fields to updating RentalOrder for Customer
             - Update items in order and restock if any
             - Update order infor
             - Update retal order detail
             */
            var response = new ResponseDTO<RentalOrderVM>();
            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    var toUpdate = await _unitOfWork.OrderRepository.GetObjectAsync(o => o.Id == orderId, new string[] { "OrderDetails" });
                    if (toUpdate == null)
                    {
                        response.IsSuccess = false;
                        response.Message = $"Order with id {orderId} not found!";
                        return response;
                    }
                    if (toUpdate.Status != (int)OrderStatus.PENDING && !(toUpdate.PaymentMethodId == 2 && toUpdate.Status == (int)OrderStatus.PAID))
                    {
                        response.IsSuccess = false;
                        response.Message = $"Your order status has been {toUpdate.Status}. Only orders with a PENDING status will be updated.!";
                        return response;
                    }
                    //Updating branch
                    var branch = await _unitOfWork.BranchRepository.GetObjectAsync(b => b.Id == rentalOrderUM.BranchId);
                    if (branch == null)
                    {
                        response.IsSuccess = false;
                        response.Message = "Branch not found!";
                        return response;
                    }
                    else
                    {

                        toUpdate.BranchId = rentalOrderUM.BranchId;
                    }
                    //Update shipment detail
                    var shipmentDetail = await _unitOfWork.ShipmentDetailRepository
                        .GetObjectAsync(s => s.Id == rentalOrderUM.ShipmentDetailID && s.UserId == toUpdate.UserId);
                    if (shipmentDetail == null)
                    {
                        response.IsSuccess = false;
                        response.Message = $"Shipment detail with Id {rentalOrderUM.ShipmentDetailID} not found!";
                        return response;
                    }
                    else
                    {
                        toUpdate.ShipmentDetailId = rentalOrderUM.ShipmentDetailID;
                    }

                    //Update warehouse and order detail
                    foreach (var updatedItem in rentalOrderUM.rentalOrderItems)
                    {
                        var warehouse = await _unitOfWork.WarehouseRepository
                            .GetObjectAsync(w => w.Id == updatedItem.WarehouseId, new string[] { "Product" });
                        var orderDetail = await _unitOfWork.OrderDetailRepository
                            .GetObjectAsync(o => o.OrderId == toUpdate.Id && o.ProductId == warehouse.ProductId);

                        if (updatedItem.Quantity == 0)
                        {
                            if (orderDetail != null)
                            {
                                //Remove item in order
                                AdjustStock(warehouse, (int)orderDetail.Quantity, true);
                                await _unitOfWork.OrderDetailRepository.DeleteAsync(orderDetail);
                            }
                        }
                        else if (orderDetail != null)
                        {
                            // Update quantity
                            int quantityDifference = (int)(updatedItem.Quantity - orderDetail.Quantity);

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
                        else
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
                    //Update order Infor
                    toUpdate.Status = rentalOrderUM.Status;
                    toUpdate.TotalPrice = (decimal)rentalOrderUM.TotalPrice;
                    toUpdate.TranSportFee = rentalOrderUM.TranSportFee;
                    toUpdate.IntoMoney = (decimal)rentalOrderUM.NewIntoMoney;
                    toUpdate.Note = rentalOrderUM.Note;
                    await _unitOfWork.OrderRepository.UpdateAsync(toUpdate);

                    //Update rental detail
                    var rentalOrder = await _unitOfWork.RentalOrderRepository.GetObjectAsync(o => o.OrderId == toUpdate.Id);
                    if (rentalOrder == null)
                    {
                        response.IsSuccess = false;
                        response.Message = $"Rental Order with id {orderId} not found!";
                        return response;
                    }
                    else
                    {
                        rentalOrder.LateFee = rentalOrderUM.rentalInfor.LateFee ?? 0;
                        rentalOrder.ReturnDate = rentalOrderUM.rentalInfor.ReturnDate;
                        rentalOrder.RentalStartDate = rentalOrderUM.rentalInfor.RentalStartDate;
                        rentalOrder.RentalEndDate = rentalOrderUM.rentalInfor.RentalEndDate;
                        rentalOrder.DamageFee = rentalOrderUM.rentalInfor.DamageFee;
                        rentalOrder.IsInspected = rentalOrderUM.rentalInfor.IsInspected;
                        rentalOrder.IsRestocked = rentalOrderUM.rentalInfor.IsRestocked;
                        // Update rental order details
                        await _unitOfWork.RentalOrderRepository.UpdateAsync(rentalOrder);

                    }

                    await transaction.CommitAsync();
                    //Return
                    var orderDetails = await _unitOfWork.OrderDetailRepository.GetAsync(od => od.OrderId == toUpdate.Id);
                    var result = new RentalOrderVM()
                    {
                        OrderID = toUpdate.Id,
                        OrderCode = toUpdate.OrderCode,
                        BranchId = toUpdate.BranchId ?? 0,
                        CreatedDate = toUpdate.CreateAt,
                        Note = toUpdate.Note,
                        PaymentMethodID = toUpdate.PaymentMethodId ?? 0,
                        UserID = toUpdate.UserId ?? 0,
                        RentalStartDate = rentalOrder.RentalStartDate,
                        RentalEndDate = rentalOrder.RentalEndDate,
                        ShipmentDetailID = toUpdate.ShipmentDetailId ?? 0,
                        IntoMoney = toUpdate.IntoMoney,
                        TransportFee = toUpdate.TranSportFee,
                        TotalPrice = toUpdate.TotalPrice,
                        rentalOrderItems = orderDetails.Select(o => new RentalOrderItems
                        {
                            Quantity = o.Quantity,
                            WarehouseId = GetWarehouseId(o.ProductId)
                        }).ToList(),
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
        public async Task<ResponseDTO<RentalOrderVM>> UpdateRentailOrderForGuestAsync(int orderId, GuestRentalOrderUM rentalOrderUM)
        {
            /* There are 4 fields to updating RentalOrder for Guest
             - Update Guest Info
             - Update items in order and restock if any
             - Update order infor
             - Update retal order detail
             */
            var response = new ResponseDTO<RentalOrderVM>();
            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    var toUpdate = await _unitOfWork.OrderRepository.GetObjectAsync(o => o.Id == orderId, new string[] { "OrderDetails" });
                    if (toUpdate == null)
                    {
                        response.IsSuccess = false;
                        response.Message = $"Order with id {orderId} not found!";
                        return response;
                    }
                    if (toUpdate.Status != (int)OrderStatus.PENDING && !(toUpdate.PaymentMethodId == 2 && toUpdate.Status == (int)OrderStatus.PAID))
                    {
                        response.IsSuccess = false;
                        response.Message = $"Your order status has been {toUpdate.Status}. Only orders with a PENDING status will be updated.!";
                        return response;
                    }
                    var branch = await _unitOfWork.BranchRepository.GetObjectAsync(b => b.Id == rentalOrderUM.BranchId);
                    if (branch == null)
                    {
                        response.IsSuccess = false;
                        response.Message = "Branch not found!";
                        return response;
                    }
                    //1.Update guest infor
                    var guest = await _unitOfWork.GuestRepository.GetObjectAsync(g => g.Id == toUpdate.GuestId);
                    if (guest == null)
                    {
                        response.IsSuccess = false;
                        response.Message = "Order and Guest Infomations are not matched!";
                    }
                    else
                    {
                        guest.FullName = rentalOrderUM.guestUM.FullName;
                        guest.Address = rentalOrderUM.guestUM.Address;
                        guest.PhoneNumber = rentalOrderUM.guestUM.PhoneNumber;
                        guest.Email = rentalOrderUM.guestUM.Email;
                        await _unitOfWork.GuestRepository.UpdateAsync(guest);
                    }
                    //2. Update warehouse and order detail
                    foreach (var updatedItem in rentalOrderUM.rentalOrderItems)
                    {
                        var warehouse = await _unitOfWork.WarehouseRepository
                            .GetObjectAsync(w => w.Id == updatedItem.WarehouseId, new string[] { "Product" });
                        var orderDetail = await _unitOfWork.OrderDetailRepository
                            .GetObjectAsync(o => o.OrderId == toUpdate.Id && o.ProductId == warehouse.ProductId);

                        if (updatedItem.Quantity == 0)
                        {
                            if (orderDetail != null)
                            {
                                //Remove item in order
                                AdjustStock(warehouse, (int)orderDetail.Quantity, true);
                                await _unitOfWork.OrderDetailRepository.DeleteAsync(orderDetail);
                            }
                        }
                        else if (orderDetail != null)
                        {
                            // Update quantity
                            int quantityDifference = (int)(updatedItem.Quantity - orderDetail.Quantity);

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
                        else
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

                    //3. Update order Infor
                    toUpdate.Status = rentalOrderUM.Status;
                    toUpdate.TotalPrice = (decimal)rentalOrderUM.TotalPrice;
                    toUpdate.TranSportFee = rentalOrderUM.TranSportFee;
                    toUpdate.IntoMoney = (decimal)rentalOrderUM.NewIntoMoney;
                    toUpdate.Note = rentalOrderUM.Note;
                    toUpdate.BranchId = rentalOrderUM.BranchId;

                    await _unitOfWork.OrderRepository.UpdateAsync(toUpdate);

                    //4. Update rental detail
                    var rentalOrder = await _unitOfWork.RentalOrderRepository.GetObjectAsync(o => o.OrderId == toUpdate.Id);
                    if (toUpdate == null)
                    {
                        response.IsSuccess = false;
                        response.Message = $"Order with id {orderId} not found!";
                        return response;
                    }
                    else
                    {
                        rentalOrder.LateFee = rentalOrderUM.rentalInfor.LateFee ?? 0;
                        rentalOrder.DamageFee = rentalOrderUM.rentalInfor.LateFee ?? 0;
                        rentalOrder.ReturnDate = rentalOrderUM.rentalInfor.ReturnDate;
                        rentalOrder.RentalEndDate = rentalOrderUM.rentalInfor.RentalEndDate;
                        rentalOrder.RentalStartDate = rentalOrderUM.rentalInfor.RentalStartDate;
                        rentalOrder.IsInspected = rentalOrderUM.rentalInfor.IsInspected;
                        rentalOrder.IsRestocked = rentalOrderUM.rentalInfor.IsRestocked;
                        await _unitOfWork.RentalOrderRepository.UpdateAsync(rentalOrder);

                    }

                    await transaction.CommitAsync();
                    var orderDetails = await _unitOfWork.OrderDetailRepository.GetAsync(od => od.OrderId == toUpdate.Id);
                    //Return
                    var result = new RentalOrderVM()
                    {
                        OrderID = toUpdate.Id,
                        OrderCode = toUpdate.OrderCode,
                        BranchId = toUpdate.BranchId ?? 0,
                        CreatedDate = toUpdate.CreateAt,
                        Note = toUpdate.Note,
                        PaymentMethodID = toUpdate.PaymentMethodId ?? 0,
                        UserID = toUpdate.UserId ?? 0,
                        RentalStartDate = rentalOrder.RentalStartDate,
                        RentalEndDate = rentalOrder.RentalEndDate,
                        ShipmentDetailID = toUpdate.ShipmentDetailId ?? 0,
                        PaymentLink = "",
                        IntoMoney = toUpdate.IntoMoney,
                        TotalPrice = toUpdate.TotalPrice,
                        TransportFee = toUpdate.TranSportFee ?? 0,
                        rentalOrderItems = orderDetails.Select(o => new RentalOrderItems()
                        {
                            Quantity = o.Quantity,
                            WarehouseId = GetWarehouseId(o.ProductId)
                        }).ToList(),
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
        //Late or Damage Handling
        //Return Process
        //Verify phone send otp
        //Validate Dtos
        //GetOrderByType
        //Edit get order for 2 types

        private int GetWarehouseId(int? productId)
        {
            var warehouse = _unitOfWork.WarehouseRepository.FindObject(o => o.ProductId == productId);
            return warehouse.Id;
        }
    }
}
