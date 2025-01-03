namespace _2Sport_BE.Infrastructure.Enums
{
    public enum PaymentStatus : int
    {
        PENDING = 1,
        PAID = 2,
        FAILED = 3,
        REFUNDED = 4,
        DEPOSIT = 5,
        CANCELED = 0
    }
}
