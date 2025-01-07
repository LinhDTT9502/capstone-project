using _2Sport_BE.Repository.Interfaces;
using _2Sport_BE.Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Twilio.Rest.Numbers.V2.RegulatoryCompliance;

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
			try
			{
                return (await _unitOfWork.ReviewRepository.GetAndIncludeAsync(_ => _.Id > 0,
                                                new string[] { "Product", "User" })).AsQueryable();
            } catch (Exception ex)
            {
                return null;
            }
        }

		public async Task<IQueryable<Review>> GetReviewsOfProduct(string productCode)
		{
			try
			{
				var reviews = (await _unitOfWork.ReviewRepository
								.GetAndIncludeAsync(_ => _.ProductCode.ToLower()
								.Equals(productCode.ToLower()),
                                                new string[] { "Product", "User" }));
				return reviews.AsQueryable();
			}
			catch (Exception ex)
			{
				return null;
			}
		}
    }
}
