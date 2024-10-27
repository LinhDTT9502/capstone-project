using _2Sport_BE.Infrastructure.Services;
using _2Sport_BE.Repository.Interfaces;
using _2Sport_BE.Repository.Models;
using _2Sport_BE.Infrastructure.DTOs;
using _2Sport_BE.Infrastructure.Enums;
using _2Sport_BE.Infrastructure.Helpers;


namespace _2Sport_BE.Infrastructure.Services
{
    public interface IRentalOrderService
    {
        Task<ResponseDTO<RentalOrderVM>> CreateRentalOrderAsync(RentalOrderCM rentalOrderCM);
        Task<ResponseDTO<RentalOrderVM>> UpdateRentalOrderAsync(int orderId, RentalOrderUM rentalOrderUM);
        Task<ResponseDTO<int>> DeleteRentalOrderAsync(int rentalOrderId);
    }
    public class RentalOrderService : IRentalOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMethodHelper _methodHelper;
        private readonly IWarehouseService _warehouseService;
        private readonly ICustomerService _customerService;

        public RentalOrderService(
            IUnitOfWork unitOfWork,
            IMethodHelper methodHelper,
            IWarehouseService warehouseService,
            ICustomerService customerService)
        {
            _unitOfWork = unitOfWork;
            _methodHelper = methodHelper;
            _warehouseService = warehouseService;
            _customerService = customerService;
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
        private decimal? CalculateSubTotal(decimal? rentPrice, decimal? quantity)
        {
            return rentPrice * quantity;
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
        /*public async Task<ResponseDTO<RentalOrderVM>> CreateRentalOrderAsync(RentalOrderCM rentalOrderCM)
        {
            var response = new ResponseDTO<RentalOrderVM>();
            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    if (rentalOrderCM.ShipmentDetailID.ToString() != "")
                    {
                        var shipmentDetail = await _unitOfWork.ShipmentDetailRepository
                                .GetObjectAsync(s => s.Id == rentalOrderCM.ShipmentDetailID && s.UserId == rentalOrderCM.CustomerInfo.UserID);

                        if (shipmentDetail == null)
                        {
                            response.IsSuccess = false;
                            response.Message = $"ShipmenDetail with Id = {rentalOrderCM.ShipmentDetailID} is not found!";
                            return response;

                        }
                        else
                        {
                            foreach (var item in rentalOrderCM.rentalOrderItems)
                            {
                                var productInWarehouse = await _unitOfWork.WarehouseRepository
                                                        .GetObjectAsync(p => p.Id == item.WarehouseId, new string[] {"Branch, Product"});

                                if (productInWarehouse == null || !(productInWarehouse.TotalQuantity >= item.Quantity && productInWarehouse.AvailableQuantity >= item.Quantity))
                                {
                                    response.IsSuccess = false;
                                    response.Message = $"Not enough stock in warehouse {item.WarehouseId}";
                                    return response;
                                }
                                else
                                {
                                    productInWarehouse.AvailableQuantity -= item.Quantity;
                                    await _unitOfWork.WarehouseRepository.UpdateAsync(productInWarehouse);

                                    var order = new RentalOrder
                                    {
                                        RentalOrderCode = _methodHelper.GenerateOrderCode(),
                                        Address = rentalOrderCM.CustomerInfo.Address,
                                        FullName = rentalOrderCM.CustomerInfo.FullName,
                                        ContactPhone = rentalOrderCM.CustomerInfo.FullName,
                                        Email = rentalOrderCM.CustomerInfo.Email,
                                        PaymentMethodId = rentalOrderCM.PaymentMethodID,
                                        UserId = rentalOrderCM.CustomerInfo.UserID,
                                        BranchId = productInWarehouse.BranchId,
                                        BranchName = productInWarehouse.Branch.BranchName,
                                        ImgAvatarPath = item.ImgAvatarPath ?? "",
                                        OrderStatus = (int)OrderStatus.PENDING,
                                        PaymentStatus = (int)PaymentStatus.IsWating,
                                        ProductId= productInWarehouse.ProductId,
                                        ProductName = productInWarehouse.Product.ProductName,
                                        RentPrice = productInWarehouse.Product.RentPrice,
                                        Quantity = item.Quantity,
                                        RentalStartDate = item.RentalStartDate,
                                        RentalEndDate = item.RentalEndDate,
                                        SubTotal = CalculateSubTotal(productInWarehouse.Product.RentPrice, item.Quantity),                                    
                                        Note = rentalOrderCM.Note,
                                        CreatedAt = DateTime.Now,
                                        
                                    };

                                    var dateValid = CheckValidOfRentalDate(order.RentalStartDate, order.RentalEndDate);
                                    if (!dateValid)
                                    {
                                        response.IsSuccess = false;
                                        response.Message = $"Rental dates are invalid!";
                                        return response;
                                    }
                                    await _unitOfWork.RentalOrderRepository.InsertAsync(order);

                                    // commit transaction
                                    await transaction.CommitAsync();
                                    //return
                                    var result = new RentalOrderVM()
                                    {
                                        RentalOrderID = order.Id,
                                        Note = order.Note,
                                        CreatedAt= order.CreatedAt,
                                        BranchId = order.BranchId,
                                        PaymentMethodID = order.PaymentMethodId,
                                        BranchName = order.BranchName,
                                        ProductId = order.ProductId,
                                        ProductName = order.ProductName,
                                        SubTotal = order.SubTotal,
                                        RentalOrderCode = order.RentalOrderCode,
                                        RentPrice = order.RentPrice,
                                        CustomerInfo = new CustomerInfo()
                                        {
                                            Address = order.Address,
                                            ContactPhone = order.ContactPhone,
                                            Email = order.Email,
                                            FullName = order.FullName,
                                            UserID = order.UserId ?? 0
                                        },
                                        
                                    };
                                    response.IsSuccess = true;
                                    response.Message = $"Rental Order processed successfully";
                                    response.Data = result;
                                    return response;
                                }
                            }
                        }
                        return response;
                    }
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    response.IsSuccess = false;
                    response.Message = ex.Message;
                    return response;
                }
            }

        }*/
        public async Task<ResponseDTO<RentalOrderVM>> CreateRentalOrderAsync(RentalOrderCM rentalOrderCM)
        {
            var response = new ResponseDTO<RentalOrderVM>();
            using var transaction = await _unitOfWork.BeginTransactionAsync();

            try
            {
                if (string.IsNullOrEmpty(rentalOrderCM.ShipmentDetailID.ToString()))
                {
                    response.IsSuccess = false;
                    response.Message = "ShipmentDetailID is required.";
                    return response;
                }

                var shipmentDetail = await _unitOfWork.ShipmentDetailRepository
                    .GetObjectAsync(s => s.Id == rentalOrderCM.ShipmentDetailID && s.UserId == rentalOrderCM.CustomerInfo.UserID);

                if (shipmentDetail == null)
                {
                    response.IsSuccess = false;
                    response.Message = $"ShipmentDetail with ID = {rentalOrderCM.ShipmentDetailID} is not found!";
                    return response;
                }

                foreach (var item in rentalOrderCM.rentalOrderItems)
                {
                    var productInWarehouse = await _unitOfWork.WarehouseRepository
                        .GetObjectAsync(p => p.Id == item.WarehouseId, new string[] { "Branch", "Product" });

                    if (!IsStockAvailable(productInWarehouse, item.Quantity))
                    {
                        response.IsSuccess = false;
                        response.Message = $"Not enough stock in warehouse {item.WarehouseId}";
                        return response;
                    }

                    // Reduce stock
                    await UpdateWarehouseStock(productInWarehouse, item.Quantity);

                    // Create the rental order
                    var order = CreateRentalOrder(rentalOrderCM, item, productInWarehouse);

                    if (!CheckValidOfRentalDate(order.RentalStartDate, order.RentalEndDate))
                    {
                        response.IsSuccess = false;
                        response.Message = "Rental dates are invalid!";
                        return response;
                    }

                    await _unitOfWork.RentalOrderRepository.InsertAsync(order);
                    await transaction.CommitAsync();

                    // Return success response
                    response = GenerateSuccessResponse(order);
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

        // Helper Methods
        private bool IsStockAvailable(Warehouse productInWarehouse, int quantity)
        {
            return productInWarehouse != null &&
                   productInWarehouse.TotalQuantity >= quantity &&
                   productInWarehouse.AvailableQuantity >= quantity;
        }

        private async Task UpdateWarehouseStock(Warehouse productInWarehouse, int quantity)
        {
            productInWarehouse.AvailableQuantity -= quantity;
            await _unitOfWork.WarehouseRepository.UpdateAsync(productInWarehouse);
        }

        private RentalOrder CreateRentalOrder(RentalOrderCM rentalOrderCM, RentalOrderItems item, Warehouse productInWarehouse)
        {
            return new RentalOrder
            {
                RentalOrderCode = _methodHelper.GenerateOrderCode(),
                Address = rentalOrderCM.CustomerInfo.Address,
                FullName = rentalOrderCM.CustomerInfo.FullName,
                ContactPhone = rentalOrderCM.CustomerInfo.ContactPhone,
                Email = rentalOrderCM.CustomerInfo.Email,
                PaymentMethodId = rentalOrderCM.PaymentMethodID,
                UserId = rentalOrderCM.CustomerInfo.UserID,
                BranchId = productInWarehouse.BranchId,
                BranchName = productInWarehouse.Branch.BranchName,
                ImgAvatarPath = item.ImgAvatarPath ?? "",
                OrderStatus = (int)OrderStatus.PENDING,
                PaymentStatus = (int)PaymentStatus.IsWating,
                ProductId = productInWarehouse.ProductId,
                ProductName = productInWarehouse.Product.ProductName,
                RentPrice = productInWarehouse.Product.RentPrice,
                Quantity = item.Quantity,
                RentalStartDate = item.RentalStartDate,
                RentalEndDate = item.RentalEndDate,
                SubTotal = CalculateSubTotal(productInWarehouse.Product.RentPrice, item.Quantity),
                Note = rentalOrderCM.Note,
                CreatedAt = DateTime.Now,
            };
        }

        private ResponseDTO<RentalOrderVM> GenerateSuccessResponse(RentalOrder order)
        {
            var result = new RentalOrderVM
            {
                RentalOrderID = order.Id,
                Note = order.Note,
                CreatedAt = order.CreatedAt,
                BranchId = order.BranchId,
                PaymentMethodID = order.PaymentMethodId,
                BranchName = order.BranchName,
                ProductId = order.ProductId,
                ProductName = order.ProductName,
                SubTotal = order.SubTotal,
                RentalOrderCode = order.RentalOrderCode,
                RentPrice = order.RentPrice,
                CustomerInfo = new CustomerInfo()
                {
                    Address = order.Address,
                    ContactPhone = order.ContactPhone,
                    Email = order.Email,
                    FullName = order.FullName,
                    UserID = order.UserId ?? 0
                },
            };

            return new ResponseDTO<RentalOrderVM>
            {
                IsSuccess = true,
                Message = "Rental Order processed successfully",
                Data = result
            };
        }

        public async Task<ResponseDTO<RentalOrderVM>> UpdateRentalOrderAsync(int orderId, RentalOrderUM rentalOrderUM)
        {
           

            var response = new ResponseDTO<RentalOrderVM>();
            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    /*var toUpdate = await _unitOfWork.RentalOrderRepository.GetObjectAsync(o => o.Id == orderId, new string[] { "OrderDetails" });
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
                            Quantity = (int)o.Quantity,
                            WarehouseId = GetWarehouseId(o.ProductId)
                        }).ToList(),
                        PaymentLink = ""
                    };
*/
                    response.IsSuccess = true;
                    response.Message = $"Order processed successfully";
                    response.Data = null;
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

        private int GetWarehouseId(int? productId)
        {
            var warehouse = _unitOfWork.WarehouseRepository.FindObject(o => o.ProductId == productId);
            return warehouse.Id;
        }
        public Task<ResponseDTO<int>> DeleteRentalOrderAsync(int rentalOrderId)
        {
            throw new NotImplementedException();
        }
        //Late or Damage Handling
        //Return Process
        //Verify phone send otp
        //Validate Dtos
        //GetOrderByType
        //Edit get order for 2 types
    }
}
