using _2Sport_BE.Repository.Data;
using _2Sport_BE.Repository.Interfaces;
using _2Sport_BE.Repository.Models;
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
        Task<CartItem> GetCartItemById(int cartItemId);

        Task<CartItem> AddNewCartItem(int userId, CartItem cartItem);
        Task DeleteCartItem(int cartItemId);
        Task ReduceCartItem(int cartItemId);
        Task UpdateQuantityOfCartItem(int cartItemId, int quantity);
        Task<CartItem> AddExistedCartItem(CartItem newCartItem);
    }
    public class CartItemService : ICartItemService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly TwoSportCapstoneDbContext _dbContext;
        private IGenericRepository<User> _userRepository;
        private IGenericRepository<Cart> _cartRepository;
        private IGenericRepository<CartItem> _cartItemRepository;
        private IGenericRepository<Product> _productRepository;

        public CartItemService(IUnitOfWork unitOfWork, TwoSportCapstoneDbContext dbContext)
        {
            _unitOfWork = unitOfWork;
            _dbContext = dbContext;
            _userRepository = _unitOfWork.UserRepository;
            _cartRepository = _unitOfWork.CartRepository;
            _cartItemRepository = _unitOfWork.CartItemRepository;
            _productRepository = _unitOfWork.ProductRepository;
        }

        public async Task<CartItem> AddNewCartItem(int userId, CartItem cartItem)
        {

            var cart = (await _cartRepository.GetAsync(_ => _.UserId == userId)).FirstOrDefault();
            try
            {
                if (cart != null)
                {
                    var product = (await _productRepository.GetAsync(_ => _.Id == cartItem.ProductId && 
                                                                    _.Status == true))
                                                           .FirstOrDefault();
                    cartItem.CartId = cart.Id;
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
            var queryCart = await _cartRepository.GetAsync(_ => _.UserId == userId);
            var cart = queryCart.FirstOrDefault();
            if (cart != null)
            {
                var testCartItems = await _cartItemRepository.GetAllAsync();
                var cartItems = await _cartItemRepository.GetAsync(_ => _.CartId == cart.Id,
                                                                    null, "", pageIndex, pageSize);
                return cartItems.AsQueryable();
            }
            else
            {
                return null;
            }
        }

        public async Task<CartItem> GetCartItemById(int cartItemId)
        {
            var queryCart = (await _cartItemRepository.GetAsync(_ => _.Id == cartItemId)).FirstOrDefault();
            return queryCart;
        }

        public async Task DeleteCartItem(int cartItemId)
        {
            try
            {
                await _cartItemRepository.DeleteAsync(cartItemId);
            } catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public async Task ReduceCartItem(int cartItemId)
        {
            var reducedCartItem = await _cartItemRepository.FindAsync(cartItemId);
            if (reducedCartItem != null)
            {
                var product = (await _unitOfWork.ProductRepository.GetAsync(_ => _.Id == reducedCartItem.ProductId)).FirstOrDefault();
                reducedCartItem.Quantity -= 1;
                reducedCartItem.Price -= product.Price;
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
                var product = await _productRepository.FindAsync(updatedCartItem.ProductId);
                if (updatedCartItem.Quantity == 0)
                {
                    DeleteCartItem(updatedCartItem.Id);
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
            var cart = (await _cartRepository.GetAsync(_ => _.UserId == userId)).FirstOrDefault();
            var queryCartItem = (await _cartItemRepository.GetAsync(_ => _.CartId == cart.Id && _.ProductId == productId))
                                                      .AsQueryable()
                                                      .AsNoTracking()
                                                      .FirstOrDefault();
            return queryCartItem;

        }

    }
}
