using _2Sport_BE.Repository.Data;
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
		Task LikeProduct(Like like);
		Task<int> CountLikesOfProduct(int productId);
        Task LikeBlog(Like like);
        Task<int> CountLikesOfBLog(int blogId);
        Task<IQueryable<Like>> GetLikesOfProduct();
		Task<IQueryable<Like>> GetLikesOfBlog();
		Task DeleteLikes(IEnumerable<Like> likes);
    }

	public class LikeService : ILikeService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly TwoSportCapstoneDbContext _context;

		public LikeService(IUnitOfWork unitOfWork, TwoSportCapstoneDbContext context)
		{
			_unitOfWork = unitOfWork;
			_context = context;
		}

		public async Task<IQueryable<Like>> GetLikesOfProduct()
		{
			return (await _unitOfWork.LikeRepository.GetAsync(_ => _.ProductId > 0)).ToList().AsQueryable();
		}

        public async Task<IQueryable<Like>> GetLikesOfBlog()
        {
            return (await _unitOfWork.LikeRepository.GetAsync(_ => _.BlogId > 0)).ToList().AsQueryable();
        }

        public async Task LikeProduct(Like like)
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

        public async Task LikeBlog(Like like)
        {
            var liked = (await _unitOfWork.LikeRepository.GetAsync(_ => _.UserId == like.UserId
                                                            && _.BlogId == like.BlogId)).FirstOrDefault();
            if (liked != null)
            {
                await _unitOfWork.LikeRepository.DeleteAsync(liked);
            }
            else
            {
                await _unitOfWork.LikeRepository.InsertAsync(like);
            }
        }

        public async Task<int> CountLikesOfProduct(int productId)
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

        public async Task<int> CountLikesOfBLog(int blogId)
        {
            try
            {
                var likes = (await _unitOfWork.LikeRepository.GetAsync(_ => _.BlogId == blogId)).ToList();
                var numOfLikes = likes.Count();
                return numOfLikes;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public async Task DeleteLikes(IEnumerable<Like> likes)
        {
			try
			{
				await _unitOfWork.LikeRepository.DeleteRangeAsync(likes);
			} catch (Exception e)
			{
				Console.WriteLine(e.Message);
			}
        }
    }


}
