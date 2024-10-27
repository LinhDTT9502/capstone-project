namespace _2Sport_BE.Infrastructure.Enums
{
    public enum OrderStatus : int
    {
        CANCELLED = 0,
        PENDING = 1,
        CONFIRMED = 2,
        PAID = 3,
        PROCESSING = 4,
        SHIPPED = 5,
        DELAYED = 6,
        COMPLETED = 7,
    }
}
