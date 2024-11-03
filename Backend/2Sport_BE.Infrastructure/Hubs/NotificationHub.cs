using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace _2Sport_BE.Infrastructure.Hubs
{
    public interface INotificationMethod
    {

    }
    public class NotificationHub : Hub
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public NotificationHub(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
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
            if (userRole == "Order Coordinator")
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, "Coordinator");
            }
            else
            {
                return;
            }
            await base.OnConnectedAsync();
        }

        public async Task SendMessageToGroup(string orderCode)
        {
            await Clients.Group("Coordinator").SendAsync("ReceiveOrderCreated", $"New order created with code: {orderCode}");
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
