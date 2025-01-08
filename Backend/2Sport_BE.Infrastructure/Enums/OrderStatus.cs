namespace _2Sport_BE.Infrastructure.Enums
{
    public enum OrderStatus : int
    {
        CANCELLED = 0,            // Đã hủy
        PENDING = 1,              // Chờ xử lý 
        CONFIRMED = 2,            // Đã xác nhận
        PROCESSING = 3,           // Đang xử lý
        SHIPPED = 4,              // Đã giao cho đơn vị vận chuyển
        DELIVERED = 5,            // Đã giao hàng
        DECLINED = 6,
        RETURN_PROCESSING = 7,
        RETURNED = 8,             // Đã trả hàng
        REFUND_PROCESSING = 9,
        REFUNDED = 10,             // Đã hoàn tiền
        COMPLETED = 11,              // Đã hoàn thành
        AWAITING_PICKUP = 12,    // Chờ khách nhận hàng
    }
}
