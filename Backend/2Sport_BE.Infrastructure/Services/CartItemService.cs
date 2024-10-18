using _2Sport_BE.Repository.Data;
using _2Sport_BE.Repository.Interfaces;
using _2Sport_BE.Repository.Models;
using _2Sport_BE.Service.DTOs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vonage.Messages.Webhooks;

namespace _2Sport_BE.Service.Services
{
    public interface ICartItemService
    {
        Task<IQueryable<CartItem>> GetCartItems(int userId, int pageIndex, int pageSize);
        Task<CartItem> GetCartItemById(int cartItemId);

        Task<CartItem> AddCartItem(int userId, CartItem cartItem);
        Task DeleteCartItem(int cartItemId);
        Task ReduceCartItem(int cartItemId);
        Task UpdateQuantityOfCartItem(int cartItemId, int quantity);
        Task<bool> DeleteCartItem(Cart cart, List<OrderDetailCM> orderDetailCMs);
        Task<bool> DeleteCartItem(Cart cart, List<RentalOrderItems> rentalOrderItems);
    }
	public class CartItemService : ICartItemService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly TwoSportCapstoneDbContext _dbContext;
        private IGenericRepository<User> _userRepository;
        private IGenericRepository<Cart> _cartRepository;
        private IGenericRepository<CartItem> _cartItemRepository;
        private IGenericRepository<Product> _productRepository;
        private IGenericRepository<Warehouse> _warehouseRepository;

        public CartItemService(IUnitOfWork unitOfWork, TwoSportCapstoneDbContext dbContext)
        {
            _unitOfWork = unitOfWork;
            _dbContext = dbContext;
            _userRepository = _unitOfWork.UserRepository;
            _cartRepository = _unitOfWork.CartRepository;
            _cartItemRepository = _unitOfWork.CartItemRepository;
            _productRepository = _unitOfWork.ProductRepository;
            _warehouseRepository = _unitOfWork.WarehouseRepository;
        }

        public async Task<CartItem> AddCartItem(int userId, CartItem cartItem)
        {
            var user = (await _userRepository.GetAsync(_ => _.Id == userId)).FirstOrDefault();
            if (user == null)
            {
                return null;
            } else
            {
                var cart = (await _cartRepository.GetAsync(_ => _.UserId == userId)).FirstOrDefault();
                try
                {
                    if (cart != null)
                    {
                        cartItem = await AddCartItem(cart, cartItem);
                        return cartItem;
                    }
                    else
                    {
                        var newCart = new Cart()
                        {
                            UserId = userId
                        };
                        await _cartRepository.InsertAsync(newCart);
                        cartItem = await AddCartItem(newCart, cartItem);
                        return cartItem;
                    }

                }
                catch (Exception ex)
                {
                    return null;
                }
            }
            
            
        }

        public async Task<CartItem> AddCartItem(Cart cart, CartItem cartItem)
        {
            var currentItem = (await _cartItemRepository.GetAsync(_ => _.WarehouseId == cartItem.WarehouseId &&
                                                                        _.CartId == cart.CartId &&
                                                                        _.Status == true)).FirstOrDefault();
            var warehouse = (await _warehouseRepository.GetAsync(_ => _.Id == cartItem.WarehouseId && _.TotalQuantity > 0))
                                                                                .FirstOrDefault();
            if (warehouse == null)
            {
                return null;
            }

            var product = (await _productRepository.GetAsync(_ => _.Id == warehouse.ProductId && _.Status == true))
                                                                                        .FirstOrDefault();

            if (currentItem != null)
            {
                currentItem.Quantity += cartItem.Quantity;
                var totalPrice = product.Price * cartItem.Quantity;
                currentItem.TotalPrice += totalPrice;
                currentItem.CartId = cart.CartId;
                currentItem.Cart = cart;
                try
                {
                    await _cartItemRepository.UpdateAsync(currentItem);
                    return currentItem;
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
            else
            {
                cartItem.CartId = cart.CartId;
                cartItem.WarehouseId = warehouse.Id;
                var totalPrice = product.Price * cartItem.Quantity;
                cartItem.TotalPrice = totalPrice;
                cartItem.Status = true;
                try
                {
                    await _cartItemRepository.InsertAsync(cartItem);
                    return cartItem;
                }
                catch (Exception ex)
                {
                    return null;
                }
            }

        }

        public async Task<IQueryable<CartItem>> GetCartItems(int userId, int pageIndex, int pageSize)
        {
            var queryCart = await _cartRepository.GetAsync(_ => _.UserId == userId);
            var cart = queryCart.FirstOrDefault();
            if (cart != null)
            {
                var testCartItems = await _cartItemRepository.GetAllAsync();
                var cartItems = await _cartItemRepository.GetAsync(_ => _.CartId == cart.CartId && _.Status == true,
                                                                    null, "", pageIndex, pageSize);
                return cartItems.AsQueryable().Include(_ => _.Warehouse.Product);
            }
            else
            {
                return null;
            }
        }

		public async Task<CartItem> GetCartItemById(int cartItemId)
		{
            var queryCart = (await _cartItemRepository.GetAsync(_ => _.Status == true && _.Id == cartItemId)).FirstOrDefault();
			return queryCart;
		}

        public async Task DeleteCartItem(int cartItemId)
        {
            var deletedCartItem = await _cartItemRepository.FindAsync(cartItemId);
            if (deletedCartItem != null)
            {
                deletedCartItem.Status = false;
                await _unitOfWork.CartItemRepository.UpdateAsync(deletedCartItem);
			}
		}

        public async Task ReduceCartItem(int cartItemId)
        {
            var reducedCartItem = await _cartItemRepository.FindAsync(cartItemId);
            if (reducedCartItem != null)
            {
                var warehouse = await _warehouseRepository.FindAsync(reducedCartItem.WarehouseId);
                var product = (await _unitOfWork.ProductRepository.GetAsync(_ => _.Id == warehouse.ProductId)).FirstOrDefault();
                reducedCartItem.Quantity -= 1;
                reducedCartItem.TotalPrice -= product.Price;
                await _unitOfWork.CartItemRepository.UpdateAsync(reducedCartItem);
                if (reducedCartItem.Quantity == 0)
                {
                    DeleteCartItem(reducedCartItem.Id);
                }
            }
        }

        public async Task UpdateQuantityOfCartItem(int cartItemId, int quantity)
		{
			var updatedCartItem = await _cartItemRepository.FindAsync(cartItemId);
			if (updatedCartItem != null)
			{
				if (updatedCartItem.Quantity == 0)
				{
					DeleteCartItem(updatedCartItem.Id);
				}
				else
				{
					updatedCartItem.Quantity = quantity;
                    await _unitOfWork.CartItemRepository.UpdateAsync(updatedCartItem);
				}
			}
		}

        /*public async Task<bool> DeleteCartItem(Cart cart, int orderId, Warehouse warehouse)
        {
            var order = (await _unitOfWork.OrderRepository.GetAsync(_ => _.Id == orderId, "OrderDetails")).FirstOrDefault();
            var orderDetails = order.OrderDetails;
            if (orderDetails == null || !orderDetails.Any())
            {
                return false;
            }

            if (cart.CartItems.Count > 0)
            {
                bool allItemsDeleted = true;
                foreach (var orderDetail in orderDetails)
                {
                    if (warehouse != null && warehouse.Quantity >= orderDetail.Quantity)
                    {
                        var cartItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == orderDetail.ProductId && ci.Status == true);
                        if (cartItem != null)
                        {
                            await _cartItemService.DeleteCartItem(cartItem.Id);
                            wareHouse.Quantity -= orderDetail.Quantity;
                            await _warehouseService.UpdateWarehouseAsync(wareHouse);
                        }
                        else
                        {
                            allItemsDeleted = false;
                        }
                    }
                    else
                    {
                        allItemsDeleted = false;
                    }
                }

                if (allItemsDeleted)
                {
                    _unitOfWork.Save();
                    return true;
                }
            }
            return false;
        }*/

        public async Task<bool> DeleteCartItem(Cart cart, List<OrderDetailCM> orderDetailCMs)
        {
            if (orderDetailCMs == null || orderDetailCMs.Count < 1)
            {
                return false;
            }

            HashSet<int?> productIdsToDelete = new HashSet<int?>(orderDetailCMs.Select(od => od.WarehouseId));

            bool flag = false;
            List<CartItem> cartItems = cart.CartItems.ToList();
  
            List<CartItem> itemsToDelete = new List<CartItem>();

            foreach (var cartItem in cartItems)
            {
                if (productIdsToDelete.Contains(cartItem.WarehouseId))
                {
                    itemsToDelete.Add(cartItem);
                    flag = true;
                }
            }

            if (itemsToDelete.Count > 0)
            {
                foreach (var item in itemsToDelete)
                {
                    await _unitOfWork.CartItemRepository.DeleteAsync(item);
                }
            }

            return flag;
        }

        public async Task<bool> DeleteCartItem(Cart cart, List<RentalOrderItems> rentalOrderItems)
        {
            if (rentalOrderItems == null || rentalOrderItems.Count < 1)
            {
                return false;
            }

            HashSet<int?> productIdsToDelete = new HashSet<int?>(rentalOrderItems.Select(od => od.WarehouseId));

            bool flag = false;
            List<CartItem> cartItems = cart.CartItems.ToList();

            List<CartItem> itemsToDelete = new List<CartItem>();

            foreach (var cartItem in cartItems)
            {
                if (productIdsToDelete.Contains(cartItem.WarehouseId))
                {
                    itemsToDelete.Add(cartItem);
                    flag = true;
                }
            }

            if (itemsToDelete.Count > 0)
            {
                foreach (var item in itemsToDelete)
                {
                    await _unitOfWork.CartItemRepository.DeleteAsync(item);
                }
            }

            return flag;
        }
    }
}
