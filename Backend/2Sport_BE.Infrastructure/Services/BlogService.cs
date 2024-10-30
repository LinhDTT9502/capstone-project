using _2Sport_BE.Repository.Interfaces;
using _2Sport_BE.Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2Sport_BE.Service.Services
{
    public interface IBlogService
    {
        Task<int> CreateBlog(int userId, Blog blog);
        Task UpdateBlog(int userId, Blog blog);
        Task DeleteBlog(Blog blog);
        Task<IQueryable<Blog>> GetAllBlogs();
        Task<IQueryable<Blog>> GetAllOwnerBlogs(int userId);
    }
    public class BlogService : IBlogService
    {
        private readonly IUnitOfWork _unitOfWork;

        public BlogService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<int> CreateBlog(int userId, Blog blog)
        {
            var user = (await _unitOfWork.UserRepository.GetAsync(_ => _.Id == userId)).FirstOrDefault();
            if (user == null)
            {
                return -1;
            }
            blog.UserId = userId;
            blog.User = user;
            await _unitOfWork.BlogRepository.InsertAsync(blog);
            return 1;
   
        }

        public Task DeleteBlog(Blog blog)
        {
            throw new NotImplementedException();
        }

        public async Task<IQueryable<Blog>> GetAllBlogs()
        {
            return (await _unitOfWork.BlogRepository.GetAllAsync()).AsQueryable();
        }

        public async Task<IQueryable<Blog>> GetAllOwnerBlogs(int userId)
        {
            return (await _unitOfWork.BlogRepository.GetAsync(_ => _.UserId == userId)).AsQueryable();
        }

        public Task UpdateBlog(int userId, Blog blog)
        {
            throw new NotImplementedException();
        }
    }
}
