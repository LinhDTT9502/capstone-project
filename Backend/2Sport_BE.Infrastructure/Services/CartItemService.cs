using _2Sport_BE.Repository.Data;
using _2Sport_BE.Repository.Interfaces;
using _2Sport_BE.Repository.Models;
using Hangfire.States;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2Sport_BE.Service.Services
{
    public interface ICartItemService
    {
        Task<IQueryable<CartItem>> GetCartItems(int userId, int pageIndex, int pageSize);
        Task<CartItem> GetCartItemByUserIdAndProductId(int userId, int productId);
        Task<CartItem> GetCartItemById(Guid cartItemId);

        Task<CartItem> AddNewCartItem(int userId, CartItem cartItem);
        Task DeleteCartItem(Guid cartItemId);
        Task ReduceCartItem(Guid cartItemId);
        Task UpdateQuantityOfCartItem(Guid cartItemId, int quantity);
        Task<CartItem> AddExistedCartItem(CartItem newCartItem);
    }
    public class CartItemService : ICartItemService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly TwoSportCapstoneDbContext _dbContext;
        private IGenericRepository<User> _userRepository;
        private IGenericRepository<CartItem> _cartItemRepository;
        private IGenericRepository<Product> _productRepository;

        public CartItemService(IUnitOfWork unitOfWork, TwoSportCapstoneDbContext dbContext)
        {
            _unitOfWork = unitOfWork;
            _dbContext = dbContext;
            _userRepository = _unitOfWork.UserRepository;
            _cartItemRepository = _unitOfWork.CartItemRepository;
            _productRepository = _unitOfWork.ProductRepository;
        }

        public async Task<CartItem> AddNewCartItem(int userId, CartItem cartItem)
        {

            try
            {
                var user = await _unitOfWork.UserRepository.FindAsync(userId);
                if (user != null)
                {
                    var product = (await _productRepository.GetAsync(_ => _.Id == cartItem.ProductId && 
                                                                    _.Status == true))
                                                           .FirstOrDefault();
                    cartItem.UserId = userId;
                    cartItem.ProductId = product.Id;
                    var totalPrice = product.Price * cartItem.Quantity;
                    cartItem.Price = totalPrice;
                    await _cartItemRepository.InsertAsync(cartItem);
                    return cartItem;
                }
                else
                {
                    return null;
                }

            }
            catch (Exception ex)
            {
                return null;
            }
        }


        public async Task<CartItem> AddExistedCartItem(CartItem newCartItem)
        {
            try
            {
                await _cartItemRepository.UpdateAsync(newCartItem);
                return newCartItem;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public async Task<IQueryable<CartItem>> GetCartItems(int userId, int pageIndex, int pageSize)
        {
            var user = await _unitOfWork.UserRepository.FindAsync(userId);
            if (user != null)
            {
                var testCartItems = await _cartItemRepository.GetAllAsync();
                var cartItems = await _cartItemRepository.GetAsync(_ => _.UserId == userId,
                                                                    null, "", pageIndex, pageSize);
                return cartItems.AsQueryable();
            }
            else
            {
                return null;
            }
        }

        public async Task<CartItem> GetCartItemById(Guid cartItemId)
        {
            var queryCart = (await _cartItemRepository.GetAsync(_ => _.CartItemId.Equals(cartItemId))).FirstOrDefault();
            return queryCart;
        }

        public async Task DeleteCartItem(Guid cartItemId)
        {
            try
            {
                var deletedCartItem = (await _unitOfWork.CartItemRepository
                        .GetAsync(_ => _.CartItemId.Equals(cartItemId))).FirstOrDefault();
                if (deletedCartItem is not null)
                {
                    await _cartItemRepository.DeleteAsync(deletedCartItem);
                } else
                {
                    Console.WriteLine("Remove cart item failed!");
                }
            } catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public async Task ReduceCartItem(Guid cartItemId)
        {
            var reducedCartItem = (await _cartItemRepository.GetAsync(_ => _.CartItemId.Equals(cartItemId)))
                                                            .FirstOrDefault();
            if (reducedCartItem != null)
            {
                var product = (await _unitOfWork.ProductRepository.GetAsync(_ => _.Id == reducedCartItem.ProductId)).FirstOrDefault();
                reducedCartItem.Quantity -= 1;
                reducedCartItem.Price -= product.Price;
                if (reducedCartItem.Quantity == 0)
                {
                    DeleteCartItem(reducedCartItem.CartItemId);
                }
                await _unitOfWork.CartItemRepository.UpdateAsync(reducedCartItem);
                
            }
        }

        public async Task UpdateQuantityOfCartItem(Guid cartItemId, int quantity)
        {
            var updatedCartItem = (await _cartItemRepository.
                                                    GetAsync(_ => _.CartItemId.Equals(cartItemId))).FirstOrDefault();
            if (updatedCartItem != null)
            {
                var product = await _productRepository.FindAsync(updatedCartItem.ProductId);
                if (updatedCartItem.Quantity == 0)
                {
                    DeleteCartItem(updatedCartItem.CartItemId);
                }
                else
                {
                    updatedCartItem.Quantity = quantity;
                    updatedCartItem.Price = quantity * product.Price;
                    await _unitOfWork.CartItemRepository.UpdateAsync(updatedCartItem);
                }
            }
        }

        public async Task<CartItem> GetCartItemByUserIdAndProductId(int userId, int productId)
        {
            var queryCartItem = (await _cartItemRepository
                                .GetObjectAsync(_ => _.UserId == userId && _.ProductId == productId));
            return queryCartItem;

        }

    }
}
