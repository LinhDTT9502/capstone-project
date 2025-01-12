using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
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

    public class NotificationHub : Hub
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public NotificationHub(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        private string GetCurrentUserBranchFromToken()
        {
            var identity = Context.User?.Identity as ClaimsIdentity;
            var roleClaim = identity?.FindFirst("BranchId");
            return roleClaim?.Value ?? string.Empty;
        }
        private string GetCurrentUserRoleFromToken()
        {
            var identity = Context.User?.Identity as ClaimsIdentity;
            var roleClaim = identity?.FindFirst(ClaimTypes.Role);
            return roleClaim?.Value ?? string.Empty;
        }
        private string  GetCurrentUserIdFromToken()
        {
            var identity = Context.User?.Identity as ClaimsIdentity;
            var userId = identity?.FindFirst("UserId");
            return userId.Value ?? string.Empty;
        }
        public override async Task OnConnectedAsync()
        {
            string userRole = GetCurrentUserRoleFromToken();
            string branchId = GetCurrentUserBranchFromToken();
            string userId = GetCurrentUserIdFromToken();

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
            else if (userRole == "Admin")
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, "Admin");
            }
            else
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, userId);
            }

            await base.OnConnectedAsync();
        }
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            string userRole = GetCurrentUserRoleFromToken();
            string branchId = GetCurrentUserBranchFromToken();
            string userId = GetCurrentUserIdFromToken();

            // Xử lý dựa trên vai trò người dùng để xóa khỏi các nhóm phù hợp
            if (userRole == "Order Coordinator")
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, "Coordinator");
            }
            else if (userRole == "Manager" || userRole == "Staff")
            {
                if (!string.IsNullOrEmpty(branchId))
                {
                    string branchGroupName = $"Branch_{branchId}";
                    await Groups.RemoveFromGroupAsync(Context.ConnectionId, branchGroupName);
                }
                else
                {
                    await Groups.RemoveFromGroupAsync(Context.ConnectionId, "NoBranch");
                }
            }
            else if (userRole == "Admin")
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, "Admin");
            }
            else
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, userId);
            }

            await base.OnDisconnectedAsync(exception);
        }

        
    }
    public class NotificationHubService : INotificationHub
    {
        private readonly IHubContext<NotificationHub> _hubContext;

        public NotificationHubService(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task SendMessageToGroup(string groupName, string message)
        {
            await _hubContext.Clients.Group(groupName).SendAsync("ReceiveMessage", message);
        }

        public async Task SendNotificationToCustomer(string userId, string message)
        {
            await _hubContext.Clients.Group(userId).SendAsync("ReceiveNotification", message);
        }

    }
}
