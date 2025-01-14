using _2Sport_BE.Enums;
using _2Sport_BE.Infrastructure.Services;
using _2Sport_BE.Repository.Models;
using _2Sport_BE.Service.Services;
using _2Sport_BE.Services;
using _2Sport_BE.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MimeKit.Cryptography;
using System.Security.Claims;

namespace _2Sport_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;
        private readonly IUserService _userService;
        private readonly IImageService _imageService;

        public ChatController(IChatService chatService, 
                              IUserService userService,
                              IImageService imageService)
        {
            _chatService = chatService;
            _userService = userService;
            _imageService = imageService;
        }

        [HttpPost]
        [Route("send-message")]
        public async Task<IActionResult> SendMessage(Guid chatSessionId, int branchId,
                                                     string senderRole, string receiverRole,
                                                     string message, IFormFile? imageFile)
        {
            try
            {
                if (senderRole == "Customer" && receiverRole == "Coordinator")
                {
                    int senderId = GetCurrentUserIdFromToken();
                    var coordinator = (await _userService.GetUserByRoleId((int)Roles.Coordinator))
                                            .FirstOrDefault();
                    int receiverId = coordinator.Id;

                    var imgUrl = "";
                    if (imageFile != null)
                    {
                        var uploadResult = await _imageService.UploadImageToCloudinaryAsync(imageFile);
                        if (uploadResult != null && uploadResult.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            imgUrl = uploadResult.SecureUrl.AbsoluteUri;
                        }
                        else
                        {
                            return BadRequest("Something wrong!");
                        }
                    }

                    await _chatService.SendMessageFromCustomerToCoordinator(senderId, receiverId,
                                                                            message, imgUrl);
                    return Ok("Send message from customer to coordinator successfully!");
                } 
                else if (senderRole == "Coordinator" && receiverRole == "Customer")
                {
                    int senderId = GetCurrentUserIdFromToken();
                    var customer = (await _userService.GetUserByRoleId((int)Roles.Customer))
                                            .FirstOrDefault();
                    int receiverId = customer.Id;

                    var imgUrl = "";
                    if (imageFile != null)
                    {
                        var uploadResult = await _imageService.UploadImageToCloudinaryAsync(imageFile);
                        if (uploadResult != null && uploadResult.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            imgUrl = uploadResult.SecureUrl.AbsoluteUri;
                        }
                        else
                        {
                            return BadRequest("Something wrong!");
                        }
                    }

                    await _chatService.SendMessageFromCoordinatorToCustomer(chatSessionId, senderId, receiverId,
                                                                            message, imgUrl);
                    return Ok("Send message from coordinator to customer successfully!");
                }
                else if (senderRole == "Coordinator" && receiverRole == "Manager")
                {
                    int senderId = GetCurrentUserIdFromToken();
                    var manager = (await _userService.GetUserByRoleIdAndBranchId((int)Roles.Customer,
                                                                                  branchId)).Data.FirstOrDefault();
                    int receiverId = manager.Id;

                    var imgUrl = "";
                    if (imageFile != null)
                    {
                        var uploadResult = await _imageService.UploadImageToCloudinaryAsync(imageFile);
                        if (uploadResult != null && uploadResult.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            imgUrl = uploadResult.SecureUrl.AbsoluteUri;
                        }
                        else
                        {
                            return BadRequest("Something wrong!");
                        }
                    }

                    await _chatService.SendMessageFromCoordinatorToManager(senderId, receiverId,
                                                                            message, imgUrl);
                    return Ok("Send message from coordinator to manager successfully!");
                }
                else
                {
                    int senderId = GetCurrentUserIdFromToken();
                    var coordinator = (await _userService.GetUserByRoleId((int)Roles.Coordinator))
                                                                .FirstOrDefault();
                    int receiverId = coordinator.Id;

                    var imgUrl = "";
                    if (imageFile != null)
                    {
                        var uploadResult = await _imageService.UploadImageToCloudinaryAsync(imageFile);
                        if (uploadResult != null && uploadResult.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            imgUrl = uploadResult.SecureUrl.AbsoluteUri;
                        }
                        else
                        {
                            return BadRequest("Something wrong!");
                        }
                    }

                    await _chatService.SendMessageFromManagerToCoordinator(chatSessionId, senderId, receiverId,
                                                                            message, imgUrl);
                    return Ok("Send message from manager to coordinator successfully!");
                }
            } catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }

        [HttpGet]
        [Route("get-messages-of-customer")]
        public async Task<IActionResult> GetMessagesOfCustomer()
        {
            try
            {
                var customerId = GetCurrentUserIdFromToken();
                if (customerId == 0)
                {
                    return Unauthorized();
                }
                var chat = await _chatService.GetMessagesOfCustomer(customerId);
                if (chat == null)
                {
                    return Ok("There is not message!");
                }
                return Ok(chat);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        [Route("get-all-chat-sessions-of-coordinator")]
        public async Task<IActionResult> GetAllChatSessionsOfCoordinator()
        {
            try
            {
                var coordinatorId = GetCurrentUserIdFromToken();
                if (coordinatorId == 0)
                {
                    return Unauthorized();
                }
                var chatSessions = await _chatService.GetAllChatSession();
                if (chatSessions is null)
                {
                    return Ok("There is not chat session!");
                }
                return Ok(chatSessions);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        [Route("get-all-messages-in-chat-sessions/{chatSessionId}")]
        public async Task<IActionResult> GetAllMessagesInChatSession(Guid chatSessionId)
        {
            try
            {
                var messages = await _chatService.GetAllMessagesInChatSession(chatSessionId);
                if (messages is null)
                {
                    return Ok("There is no message!");
                }
                return Ok(messages);
            }
            catch (Exception ex)
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
