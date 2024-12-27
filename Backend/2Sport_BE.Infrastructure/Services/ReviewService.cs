using _2Sport_BE.Repository.Interfaces;
using _2Sport_BE.Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2Sport_BE.Service.Services
{
	public interface IReviewService
	{
		Task AddReview(Review review);
		Task AddReview(List<Review> reviews);
        Task<IQueryable<Review>> GetReviewsOfProduct(string productCode);
        Task<IQueryable<Review>> GetAllReviews();
        Task<bool> DeleteReview(int reviewId);
    }
    public class ReviewService : IReviewService
	{
		private readonly IUnitOfWork _unitOfWork;

		public ReviewService(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		public async Task AddReview(Review review)
		{
			await _unitOfWork.ReviewRepository.InsertAsync(review);
		}

        public async Task AddReview(List<Review> reviews)
        {
            await _unitOfWork.ReviewRepository.InsertRangeAsync(reviews);
        }

        public async Task<bool> DeleteReview(int reviewId)
        {
            try
			{
				await _unitOfWork.ReviewRepository.DeleteAsync(reviewId);
				return true;
			} catch (Exception ex)
			{
				return false;
				Console.WriteLine(ex.Message);
			}
        }

        public async Task<IQueryable<Review>> GetAllReviews()
		{
			return (await _unitOfWork.ReviewRepository.GetAllAsync()).AsQueryable();
		}

		public async Task<IQueryable<Review>> GetReviewsOfProduct(string productCode)
		{
			try
			{
				var reviews = (await _unitOfWork.ReviewRepository
								.GetAsync(_ => _.ProductCode.ToLower().Equals(productCode.ToLower()))).AsQueryable();
				return reviews;
			}
			catch (Exception ex)
			{
				return null;
			}
		}
    }
}
