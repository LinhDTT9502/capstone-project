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
		Task<IQueryable<Review>> GetReviewsOfProduct(int productId);
		Task<IQueryable<Review>> GetAllReviews();

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

		public async Task<IQueryable<Review>> GetAllReviews()
		{
			return (await _unitOfWork.ReviewRepository.GetAllAsync()).AsQueryable();
		}

		public async Task<IQueryable<Review>> GetReviewsOfProduct(int productId)
		{
			try
			{
				var reviews = (await _unitOfWork.ReviewRepository.GetAsync(_ => _.ProductId == productId)).AsQueryable();
				return reviews;
			}
			catch (Exception ex)
			{
				return null;
			}
		}
	}
}
