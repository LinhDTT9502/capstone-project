using _2Sport_BE.Repository.Interfaces;
using _2Sport_BE.Repository.Models;
using _2Sport_BE.Service.Services;
using _2Sport_BE.ViewModels;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace _2Sport_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBlogService _blogService;

        public BlogController(IMapper mapper, IUnitOfWork unitOfWork, IBlogService blogService)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _blogService = blogService;
        }

        [HttpPost]
        [Route("create-blog")]
        public async Task<IActionResult> CreateBlog(BlogCM blogCM)
        {
            try
            {
                var blog = _mapper.Map<Blog>(blogCM);
                var userId = GetCurrentUserIdFromToken();
                if (userId == null)
                {
                    return Unauthorized("You are not allowed to do this method!");
                }
                var isSuccess = await _blogService.CreateBlog(userId, blog);
                if (isSuccess == -1)
                {
                    return NotFound("Cannot find user");
                }
                return Ok(blog);
            } catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
            
        }

        [HttpGet]
        [Route("get-all-blogs")]
        public async Task<IActionResult> GetAllBlogs()
        {
            try
            {
                var blogs = await _blogService.GetAllBlogs();
                var result = _mapper.Map<List<BlogVM>>(blogs.ToList());
                return Ok(new { total = result.Count, data = result });
            } catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut]
        [Route("edit-blog/{blogId}")]
        public async Task<IActionResult> EditBlog(int blogId, BlogUM blogUM)
        {
            try
            {
                var userId = GetCurrentUserIdFromToken();
                if (userId == null)
                {
                    return Unauthorized("You are not allowed to use this function!");
                }
                var newBlog = _mapper.Map<Blog>(blogUM);
                var toEditeBlog = (await _blogService.GetBlogById(blogId)).FirstOrDefault();
                if (toEditeBlog == null)
                {
                    return NotFound($"Cannot find the blog with id: {blogId}");
                }
                
                var editedBlog = await _blogService.EditBlog(userId, newBlog);
                if (editedBlog == null)
                {
                    return BadRequest("Staff is not existed!");
                }
                var result = _mapper.Map<BlogVM>(editedBlog);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }

        [HttpDelete]
        [Route("delete-blog/{blogId}")]
        public async Task<IActionResult> DeleteBlog(int blogId)
        {
            try
            {
                int userId = GetCurrentUserIdFromToken();
                if (userId == null)
                {
                    return Unauthorized("You are not allowed to do this function!");
                }
                await _blogService.DeleteBlog(blogId);
                return Ok("Delete blog successfully!");
            } catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
            
        }

        protected int GetCurrentUserIdFromToken()
        {
            int UserId = 0;
            try
            {
                if (HttpContext.User.Identity.IsAuthenticated)
                {
                    var identity = HttpContext.User.Identity as ClaimsIdentity;
                    if (identity != null)
                    {
                        IEnumerable<Claim> claims = identity.Claims;
                        string strUserId = identity.FindFirst("UserId").Value;
                        int.TryParse(strUserId, out UserId);

                    }
                }
                return UserId;
            }
            catch
            {
                return UserId;
            }
        }
    }
}
