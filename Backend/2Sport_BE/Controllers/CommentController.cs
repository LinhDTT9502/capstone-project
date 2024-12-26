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
        private readonly IProductService _productService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CommentController(ICommentService commentService, 
                                 IUserService userService, 
                                 INotificationService notificationService, 
                                 IProductService productService, 
                                 IUnitOfWork unitOfWork, 
                                 IMapper mapper)
        {
            _commentService = commentService;
            _userService = userService;
            _notificationService = notificationService;
            _productService = productService;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet]
        [Route("get-all-comments")]
        public async Task<IActionResult> GetAllComments()
        {
            try
            {
                var allCommentInProduct = (await _commentService.GetAllComments()).ToList();
                var result = _mapper.Map<List<CommentVM>>(allCommentInProduct);
                foreach (var item in result)
                {
                    var product = await _productService.GetProductByProductCode(item.ProductCode);
                    var user = await _userService.GetUserById(item.UserId);
                    item.ProductName = product.ProductName ?? "";
                    item.FullName = user.FullName ?? "";
                    item.Email = user.Email ?? "";
                }
                return Ok(new { total = allCommentInProduct.Count, data = result });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("get-all-comments/{productCode}")]
        public async Task<IActionResult> GetAllComments(string productCode)
        {
            try
            {
                var allCommentInProduct = (await _commentService.GetAllComment(productCode)).ToList();
                var result = _mapper.Map<List<CommentVM>>(allCommentInProduct);
                foreach (var item in result)
                {
                    var product = await _productService.GetProductByProductCode(item.ProductCode);
                    var user = await _userService.GetUserById(item.UserId);
                    item.ProductName = product.ProductName ?? "";
                    item.FullName = user.FullName ?? "";
                    item.Email = user.Email ?? "";
                }
                return Ok(new { total = allCommentInProduct.Count, data = result });
            } catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("get-comment-by-id/{commentId}")]
        public async Task<IActionResult> GetCommentById(int commentId)
        {
            try
            {
                var allCommentInProduct = (await _commentService.GetCommentById(commentId));
                var result = _mapper.Map<CommentVM>(allCommentInProduct);

                var product = await _productService.GetProductByProductCode(result.ProductCode);
                var user = await _userService.GetUserById(result.UserId);
                result.ProductName = product.ProductName ?? "";
                result.FullName = user.FullName ?? "";
                result.Email = user.Email ?? "";
                return Ok(new { data = result });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("get-child-comments/{parentCommentId}")]
        public async Task<IActionResult> GetChildComments(int parentCommentId)
        {
            try
            {
                var allChildComments= (await _commentService.GetChildComments(parentCommentId));
                var result = _mapper.Map<List<CommentVM>>(allChildComments.ToList());

                foreach (var item in result)
                {
                    var product = await _productService.GetProductByProductCode(item.ProductCode);
                    //var user = await _userService.GetUserById(result.UserId);
                    item.ProductName = product.ProductName ?? "";
                    //result.FullName = user.FullName ?? "";
                    //result.Email = user.Email ?? "";
                }

                return Ok(new { data = result });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("comment/{productCode}")]
        public async Task<IActionResult> Comment(string productCode, CommentCM commentCM)
        {
            try
            {
                var currUserId = GetCurrentUserIdFromToken();
                if (currUserId == 0)
                {
                    return Unauthorized();
                }
                var comment = _mapper.Map<CommentCM, Comment>(commentCM);
                var isSuccess = await _commentService.AddComment(currUserId, productCode, comment);

                var coordinators = await _userService.GetUserWithConditionAsync(_ => _.RoleId == 16);
                if (isSuccess == 1)
                {
                    var product = (await _unitOfWork.ProductRepository.GetAsync(_ => _.ProductCode.ToLower()
                                                                                    .Equals(productCode.ToLower())))
                                                                                    .FirstOrDefault();
                    var isSuccessNotify = await _notificationService.NotifyForComment(currUserId, 
                                                                    coordinators.ToList(), product);
                    if (!isSuccessNotify)
                    {
                        return StatusCode(500, "Notify to coordinator failed!");
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
        [Route("reply-comment/{productCode}")]
        public async Task<IActionResult> ReplyComment(string productCode, [FromQuery]int parentCommentId, 
                                                            CommentCM commentCM)
        {
            try
            {
                var currAdminId = GetCurrentUserIdFromToken();
                var comment = _mapper.Map<CommentCM, Comment>(commentCM);
                var isSuccess = await _commentService.ReplyComment(currAdminId, productCode, parentCommentId, comment);
                var parentComment = await _unitOfWork.CommentRepository.FindAsync(parentCommentId);
                var currUserId = parentComment.UserId;
                if (isSuccess == 1)
                {
                    var product = (await _unitOfWork.ProductRepository.GetAsync(_ => _.ProductCode.ToLower()
                                                                                    .Equals(productCode.ToLower())))
                                                                                    .FirstOrDefault();
                    var isSuccessNotify = await _notificationService
                                            .NotifyForReplyComment(currUserId.ToString(), product);
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
