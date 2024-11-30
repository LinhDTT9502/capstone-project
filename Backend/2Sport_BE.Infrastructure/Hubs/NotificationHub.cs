using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace _2Sport_BE.Infrastructure.Hubs
{
    public interface INotificationHub
    {
        Task SendMessageToGroup(string groupName, string message);
        Task SendNotificationToCustomer(string userId, string message);
    }
    public class NotificationHub : Hub, INotificationHub
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public NotificationHub(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        private string GetCurrentUserBranchFromToken()
        {
            string userbranchId = string.Empty;
            try
            {
                if (_httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
                {
                    var identity = _httpContextAccessor.HttpContext.User.Identity as ClaimsIdentity;
                    var branchClaim = identity?.FindFirst("BranchId");
                    userbranchId = branchClaim?.Value ?? string.Empty;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return userbranchId;
        }
        private string GetCurrentUserRoleFromToken()
        {
            string userRole = string.Empty;
            try
            {
                if (_httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
                {
                    var identity = _httpContextAccessor.HttpContext.User.Identity as ClaimsIdentity;
                    var roleClaim = identity?.FindFirst(ClaimTypes.Role);
                    userRole = roleClaim?.Value ?? string.Empty;
                }
            }
            catch (Exception ex)
            {
                // Log the exception (consider using a logging framework)
                Console.WriteLine(ex.Message);
            }
            return userRole;
        }

        public override async Task OnConnectedAsync()
        {
            string userRole = GetCurrentUserRoleFromToken();
            string branchId = GetCurrentUserBranchFromToken();

            if (userRole == "Order Coordinator")
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, "Coordinator");
            }
            else if (userRole == "Manager" || userRole == "Staff")
            {
                if (!string.IsNullOrEmpty(branchId))
                {
                    // Thêm user vào nhóm chi nhánh tương ứng
                    string branchGroupName = $"Branch_{branchId}";
                    await Groups.AddToGroupAsync(Context.ConnectionId, branchGroupName);
                }
            }
            await base.OnConnectedAsync();
        }

        public async Task SendMessageToGroup(string groupName, string message)
        {
            await Clients.Group(groupName).SendAsync("ReceiveMessage", message);
        }

        public async Task SendNotificationToCustomer(string userId, string message)
        {
            if (Clients.User(userId) != null)
            {
                await Clients.User(userId).SendAsync("ReceiveNotification", message);
            }
            else
            {
                return;
            }
        }
    }

}
