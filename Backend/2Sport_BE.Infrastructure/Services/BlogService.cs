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
    public interface IBlogService
    {
        Task<int> CreateBlog(int userId, Blog blog);
        Task<Blog> EditBlog(int userId, Blog blog);
        Task DeleteBlog(int blogId);
        Task<IQueryable<Blog>> GetAllBlogs();
        Task<IQueryable<Blog>> GetBlogById(int blogId);
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
            var staff = (await _unitOfWork.StaffRepository.GetAsync(_ => _.UserId == userId)).FirstOrDefault();
            if (staff == null)
            {
                return -1;
            }
            blog.CreateAt = DateTime.Now;
            blog.CreatedStaffId = userId;
            blog.CreatedByStaff = staff;
            blog.UpdatedAt = null;
            blog.EditedByStaff = null;
            blog.EditedByStaffId = null;
            await _unitOfWork.BlogRepository.InsertAsync(blog);
            return 1;
   
        }

        public async Task DeleteBlog(int blogId)
        {
            await _unitOfWork.BlogRepository.DeleteAsync(blogId);
        }

        public async Task<IQueryable<Blog>> GetAllBlogs()
        {
            return (await _unitOfWork.BlogRepository.GetAllAsync()).AsQueryable().Include(_ => _.CreatedByStaff)
                                                                                 .Include(_ => _.EditedByStaff);
        }

        public async Task<Blog> EditBlog(int userId, Blog blog)
        {
            var editedByStaff = (await _unitOfWork.StaffRepository.GetAsync(_ => _.UserId == userId)).FirstOrDefault();
            if (editedByStaff == null)
            {
                return null;
            }
            blog.UpdatedAt = DateTime.Now;
            blog.EditedByStaff = editedByStaff;
            await _unitOfWork.BlogRepository.UpdateAsync(blog);
            return blog;
        }

        public async Task<IQueryable<Blog>> GetBlogById(int blogId)
        {
            return (await _unitOfWork.BlogRepository.GetAsync(_ => _.Id == blogId)).AsQueryable();
        }
    }
}
