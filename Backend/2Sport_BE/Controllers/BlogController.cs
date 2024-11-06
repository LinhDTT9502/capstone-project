using _2Sport_BE.Infrastructure.Services;
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
        private readonly IUserService _userService;

        public BlogController(IMapper mapper, IUnitOfWork unitOfWork, 
                              IBlogService blogService,
                              IUserService userService)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _blogService = blogService;
            _userService = userService;
        }

        [HttpPost]
        [Route("create-blog")]
        public async Task<IActionResult> CreateBlog(BlogCM blogCM)
        {
            try
            {
                var blog = _mapper.Map<Blog>(blogCM);
                var userId = GetCurrentUserIdFromToken();
                if (userId == 0)
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
                foreach (var blog in blogs)
                {
                    var createdStaffAccount = await _userService.GetUserById((int)blog.CreatedByStaff.UserId);
                    blog.CreatedByStaff.User = createdStaffAccount;
                    if (blog.EditedByStaff != null)
                    {
                        var editedStaffAccount = await _userService.GetUserById((int)blog.EditedByStaff.UserId);
                        blog.CreatedByStaff.User = editedStaffAccount;
                    }
                }
                var result = _mapper.Map<List<BlogVM>>(blogs.ToList());
                return Ok(new { total = result.Count, data = result });
            } catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        [Route("get-blog-by-id")]
        public async Task<IActionResult> GetBlogById(int id)
        {
            try
            {
                var blog = await _blogService.GetBlogById(id);
                var createdStaffAccount = await _userService.GetUserById((int)blog.CreatedByStaff.UserId);
                blog.CreatedByStaff.User = createdStaffAccount;
                if (blog.EditedByStaff != null)
                {
                    var editedStaffAccount = await _userService.GetUserById((int)blog.EditedByStaff.UserId);
                    blog.CreatedByStaff.User = editedStaffAccount;
                }
                var result = _mapper.Map<BlogVM>(blog);
                return Ok(result);
            }
            catch (Exception ex)
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
                var toEditeBlog = (await _blogService.GetBlogById(blogId));
                if (toEditeBlog == null)
                {
                    return NotFound($"Cannot find the blog with id: {blogId}");
                }
                toEditeBlog.Title = blogUM.Title;
                toEditeBlog.Content = blogUM.Content;
                var editedBlog = await _blogService.EditBlog(userId, toEditeBlog);
                if (editedBlog == null)
                {
                    return BadRequest("Edit blog failed!");
                }
                var result = _mapper.Map<BlogVM>(editedBlog);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }

        [HttpPut]
        [Route("hide-show-blog/{blogId}")]
        public async Task<IActionResult> HideShowBlog(int blogId)
        {
            try
            {
                var userId = GetCurrentUserIdFromToken();
                if (userId == null)
                {
                    return Unauthorized("You are not allowed to use this function!");
                }
                var toEditeBlog = (await _blogService.GetBlogById(blogId));
                if (toEditeBlog == null)
                {
                    return NotFound($"Cannot find the blog with id: {blogId}");
                }
                toEditeBlog.Status = !toEditeBlog.Status;
                var newBlog = _mapper.Map<Blog>(toEditeBlog);
                var editedBlog = await _blogService.HideShowBlog(newBlog);
                if (editedBlog == null)
                {
                    return BadRequest("Hide/show blog failed!");
                }
                else if (editedBlog.Status == false)
                {
                    return Ok("Hide blog successfully!");
                } else
                {
                    return Ok("Show blog successfully!");
                }
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
