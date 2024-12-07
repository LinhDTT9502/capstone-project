namespace _2Sport_BE.Infrastructure.Enums
{
    public enum OrderStatus : int
    {
        PENDING = 1,
        CONFIRMED = 2,
        AWAITING_PAYMENT = 3,
        PROCESSING = 4,
        SHIPPED = 5,
        DELIVERED = 6,
        COMPLETED = 7,
        DECLINED = 8,
        REFUNDED = 9,
        ON_HOLD = 10,
        AWAITING_PICKUP = 11,
        RETURN_REQUESTED = 12,
        RETURNED = 13,
        FAILED = 14,
        CANCELLED = 0
    }
}
