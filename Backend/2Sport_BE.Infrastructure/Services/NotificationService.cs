using _2Sport_BE.Infrastructure.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace _2Sport_BE.Infrastructure.Services
{
    public interface INotificationService
    {
        Task NotifyForCreatingNewOrderAsync(string orderCode);
        Task SendRentalOrderExpirationNotificationAsync(string customerId, string orderId, DateTime rentalEndDate);
    }
    public class NotificationService : INotificationService
    {
        private readonly IHubContext<NotificationHub> _notificationHubContext;
        public NotificationService(IHubContext<NotificationHub> hubContext)
        {
            _notificationHubContext = hubContext;
        }
        public async Task NotifyForCreatingNewOrderAsync(string orderCode)
        {
            var message = $"A new order has been created with order code: {orderCode}";
            await _notificationHubContext.Clients.Group("Coordinator").SendAsync("ReceiveOrderCreated", message);
        }

        public async Task SendRentalOrderExpirationNotificationAsync(string customerId, string orderId, DateTime rentalEndDate)
        {
            var message = $"Your rental order #{orderId} is about to expire on {rentalEndDate.ToShortDateString()}";
            await _notificationHubContext.Clients.User(customerId)
                .SendAsync("ReceiveNotification", message);
        }
    }
}
