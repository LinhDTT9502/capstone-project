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
    public class CommentController : ControllerBase
    {
        private readonly ICommentService _commentService;
        private readonly IUserService _userService;
        private readonly INotificationService _notificationService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CommentController(ICommentService commentService,
                                 IUserService userService,
                                 INotificationService notificationService,
                                 IUnitOfWork unitOfWork, IMapper mapper)
        {
            _userService = userService;
            _commentService = commentService;
            _notificationService = notificationService;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet]
        [Route("get-all-comments/{productId}")]
        public async Task<IActionResult> GetAllComments(int productId)
        {
            try
            {
                var allCommentInProduct = (await _commentService.GetAllComment(productId)).ToList();
                var result = _mapper.Map<List<CommentVM>>(allCommentInProduct);
                foreach (var item in result)
                {
                    var user = await _userService.GetUserById(item.UserId);
                    item.Username = user.UserName;
                }
                return Ok(new { total = allCommentInProduct.Count, data = result });
            } catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("comment/{productId}")]
        public async Task<IActionResult> Comment(int productId, CommentCM commentCM)
        {
            try
            {
                var currUserId = GetCurrentUserIdFromToken();
                if (currUserId == 0)
                {
                    return Unauthorized();
                }
                var comment = _mapper.Map<CommentCM, Comment>(commentCM);
                var isSuccess = await _commentService.AddComment(currUserId, productId, comment);


                if (isSuccess == 1)
                {
                    var product = (await _unitOfWork.ProductRepository.FindAsync(productId));
                    var isSuccessNotify = await _notificationService.NotifyForComment(currUserId, product);
                    if (!isSuccessNotify)
                    {
                        return StatusCode(500, "Notify to admin failed!");
                    }
                    return Ok("Add comment successfully!");
                }

                else if (isSuccess == -1) {
                    return StatusCode(500, "Something wrong!");
                } else
                {
                    return BadRequest("Add comment failed");
                }
            } catch (Exception ex)
            {
                return BadRequest("Add comment failed");
            }
            
        }

        [HttpPost]
        [Route("reply-comment/{productId}")]
        public async Task<IActionResult> ReplyComment(int productId, [FromQuery]int parentCommentId, CommentCM commentCM)
        {
            try
            {
                var currAdminId = GetCurrentUserIdFromToken();
                var comment = _mapper.Map<CommentCM, Comment>(commentCM);
                var isSuccess = await _commentService.ReplyComment(currAdminId, productId, parentCommentId, comment);
                var parentComment = await _unitOfWork.CommentRepository.FindAsync(parentCommentId);
                var currUserId = parentComment.UserId;
                if (isSuccess == 1)
                {
                    var product = (await _unitOfWork.ProductRepository.FindAsync(productId));
                    var isSuccessNotify = await _notificationService.NotifyForReplyComment(currAdminId, 
                                                                            currUserId.ToString(), product);
                    if (!isSuccessNotify)
                    {
                        return StatusCode(500, "Notify to admin failed!");
                    }
                    return Ok("Add comment successfully!");
                }
                else if (isSuccess == -1)
                {
                    return StatusCode(500, "Something wrong!");
                }
                else
                {
                    return BadRequest("Add comment failed");
                }
            }
            catch (Exception ex)
            {
                return BadRequest("Add comment failed");
            }
        }

        [HttpPut]
        [Route("update-comment/{commentId}")]
        public async Task<IActionResult> UpdateComment(int commentId, CommentUM commentUM)
        {
            try
            {
                var currUserId = GetCurrentUserIdFromToken();
                var comment = _mapper.Map<CommentUM, Comment>(commentUM);
                var response = await _commentService.UpdateComment(currUserId, commentId, comment);
                if (response.Data == -1)
                {
                    return StatusCode(500, response.Message);
                }
                if (response.Data == 0)
                {
                    return NotFound(response.Message);
                }
                if (response.Data == -2)
                {
                    return Unauthorized();
                }
                return Ok(response.Message);
            }
            catch (Exception ex)
            {
                return BadRequest("Remove comment failed!");
            }
        }


        [HttpDelete]
        [Route("remove-comment/{commentId}")]
        public async Task<IActionResult> RemoveComment(int commentId)
        {
            try
            {
                var currUserId = GetCurrentUserIdFromToken();
                var response = await _commentService.DeleteComment(currUserId, commentId);
                if (response.Data == -1)
                {
                    return StatusCode(500, response.Message);
                }
                if (response.Data == 0)
                {
                    return NotFound(response.Message);
                }
                if (response.Data == -2)
                {
                    return Unauthorized();
                }
                return Ok(response.Message);
            } catch (Exception ex)
            {
                return BadRequest("Remove comment failed!");
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
