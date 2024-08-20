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
    public interface ICartService
    {
        Task<Cart> GetCartByUserId(int userId);
        Task AddNewCart(Cart cart);
    }
    public class CartService : ICartService
    {
        private readonly IUnitOfWork _unitOfWork;
        private TwoSportDBContext _twoSportDBContext;
        public CartService(IUnitOfWork unitOfWork,
            TwoSportDBContext twoSportDBContext)
        {
            this._unitOfWork = unitOfWork;
            _twoSportDBContext = twoSportDBContext;
        }

        public async Task<Cart> GetCartByUserId(int userId)
        {
            var test = await _unitOfWork.CartRepository.GetAsync(_ => _.User.Id == userId, "CartItems");

            if (test != null)
            {
                return test.FirstOrDefault();
            }
            return null;
        }
        public async Task AddNewCart(Cart cart)
        {
            await _unitOfWork.CartRepository.InsertAsync(cart);
        }
    }
}
