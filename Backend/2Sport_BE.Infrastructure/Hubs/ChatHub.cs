using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace _2Sport_BE.Infrastructure.Hubs
{
    public interface IChatHub
    {
        Task SendMessageToGroup(string groupName, string message);
        Task SendMessageToSpecificUser(string userId, string message);
    }
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ChatHub : Hub
    {
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
        private string GetCurrentUserIdFromToken()
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

            if (userRole == "Coordinator")
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, "Coordinator");
            }
            else if (userRole == "Manager")
            {
                if (!string.IsNullOrEmpty(branchId))
                {
                    // Thêm user vào nhóm chi nhánh tương ứng
                    string branchGroupName = $"Branch_{branchId}";
                    await Groups.AddToGroupAsync(Context.ConnectionId, branchGroupName);
                }
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
            if (userRole == "Coordinator")
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, "Coordinator");
            }
            else if (userRole == "Manager")
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
                if (!string.IsNullOrWhiteSpace(userId))
                {
                    await Groups.RemoveFromGroupAsync(Context.ConnectionId, userId);
                }
            }

            await base.OnDisconnectedAsync(exception);
        }

        
    }
    public class ChatHubService : IChatHub
    {
        private readonly IHubContext<ChatHub> _hubContext;

        public ChatHubService(IHubContext<ChatHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task SendMessageToGroup(string groupName, string message)
        {
            await _hubContext.Clients.Group(groupName).SendAsync("ReceiveMessage", message);
        }

        public async Task SendMessageToSpecificUser(string userId, string message)
        {
            await _hubContext.Clients.Group(userId).SendAsync("ReceiveMessage", message);
        }

    }
}
