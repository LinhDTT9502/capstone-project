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
            else if (userRole == "Admin")
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, "Admin");
            }
            await base.OnConnectedAsync();
        }

        public async Task SendMessageToGroup(string groupName, string message)
        {
            await Clients.Group(groupName).SendAsync("ReceiveMessage", message);
        }

        public async Task SendNotificationToCustomer(string userId, string message)
        {
            var user = Clients.User(userId); // Kiểm tra user connection

            if (user != null)
            {
                await user.SendAsync("ReceiveNotification", message);
            }
            else
            {
                // Log thông báo hoặc xử lý khi user không được kết nối
                Console.WriteLine("User not connected");
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
                var user = _hubContext.Clients.User(userId);
                if (user != null)
                {
                    await user.SendAsync("ReceiveNotification", message);
                }
                else
                {
                    Console.WriteLine("User not connected or does not exist.");
                }
            }
        }
    }

}
