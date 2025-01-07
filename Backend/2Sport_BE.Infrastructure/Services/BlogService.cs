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
        Task<Blog> HideShowBlog(Blog blog);
        Task DeleteBlog(int blogId);
        Task<IQueryable<Blog>> GetAllBlogs();
        Task<Blog> GetBlogById(int blogId);
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
            try
            {
                var user = (await _unitOfWork.UserRepository.GetAsync(_ => _.Id == userId)).FirstOrDefault();
                if (user == null)
                {
                    return -1;
                }
                blog.CreateAt = DateTime.Now;
                blog.CreatedStaffId = user.Id;
                blog.UpdatedAt = null;
                blog.EditedByStaffId = null;
                blog.Status = true;
                await _unitOfWork.BlogRepository.InsertAsync(blog);
                return 1;
            } catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return 0;
            }
            
   
        }

        public async Task DeleteBlog(int blogId)
        {
            try
            {
                await _unitOfWork.BlogRepository.DeleteAsync(blogId);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public async Task<IQueryable<Blog>> GetAllBlogs()
        {
            var blogs = (await _unitOfWork.BlogRepository.GetAllAsync()).AsQueryable().Include(_ => _.CreatedByStaff)
                                                                                      .Include(_ => _.EditedByStaff);
            foreach (var blog in blogs)
            {
                var createdStaff = await _unitOfWork.UserRepository.FindAsync(blog.CreatedStaffId);
                if (createdStaff != null)
                {
                    blog.CreatedByStaff = createdStaff;
                }

                var editedStaff = await _unitOfWork.UserRepository.FindAsync(blog.EditedByStaffId);
                if (editedStaff != null)
                {
                    blog.EditedByStaff = editedStaff;
                }
            }
            return blogs;
        }

        public async Task<Blog> EditBlog(int userId, Blog blog)
        {
            try
            {
                var editedByStaff = await _unitOfWork.UserRepository.FindAsync(userId);
                if (editedByStaff == null)
                {
                    return null;
                }
                blog.UpdatedAt = DateTime.Now;
                blog.EditedByStaffId = editedByStaff.Id;
                await _unitOfWork.BlogRepository.UpdateAsync(blog);
                return blog;
            } catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public async Task<Blog> GetBlogById(int blogId)
        {
            var blog = await _unitOfWork.BlogRepository.FindAsync(blogId);
            var createdStaff = await _unitOfWork.UserRepository.FindAsync(blog.CreatedStaffId);
            if (createdStaff != null)
            {
                blog.CreatedByStaff = createdStaff;
            }

            var editedStaff = await _unitOfWork.UserRepository.FindAsync(blog.EditedByStaffId);
            if (editedStaff != null)
            {
                blog.EditedByStaff = editedStaff;
            }
            return blog;
        }

        public async Task<Blog> HideShowBlog(Blog blog)
        {
            try
            {
                await _unitOfWork.BlogRepository.UpdateAsync(blog);
                return blog;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
    }
}
