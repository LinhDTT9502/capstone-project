using _2Sport_BE.Repository.Interfaces;
using _2Sport_BE.Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2Sport_BE.Service.Services
{
	public interface ILikeService
	{
		Task AddLike(Like like);
		Task<int> CountLikeOfProduct(int productId);
		Task<IQueryable<Like>> GetLikes();
	}

	public class LikeService : ILikeService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly TwoSportDBContext _context;

		public LikeService(IUnitOfWork unitOfWork, TwoSportDBContext context)
		{
			_unitOfWork = unitOfWork;
			_context = context;
		}

		public async Task<IQueryable<Like>> GetLikes()
		{
			return (await _unitOfWork.LikeRepository.GetAllAsync()).ToList().AsQueryable();
		}

		public async Task AddLike(Like like)
		{
			var liked = (await _unitOfWork.LikeRepository.GetAsync(_ => _.UserId == like.UserId 
															&& _.ProductId == like.ProductId)).FirstOrDefault();
			if (liked != null)
			{
				await _unitOfWork.LikeRepository.DeleteAsync(liked);
			}
			else
			{
				await _unitOfWork.LikeRepository.InsertAsync(like);
			}
		}

		public async Task<int> CountLikeOfProduct(int productId)
		{
			try
			{
				var likes = (await _unitOfWork.LikeRepository.GetAsync(_ => _.ProductId == productId)).ToList();
				var numOfLikes = likes.Count();
				return numOfLikes;
			} catch (Exception ex)
			{
				return 0;
			}
		}
	}


}
