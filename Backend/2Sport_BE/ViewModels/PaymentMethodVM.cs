
namespace _2Sport_BE.ViewModels
{
    public class PaymentMethodDTO
    {
        public string Name { get; set; }
    }
    public class PaymentMethodCM : PaymentMethodDTO
    {
    }
    public class PaymentMethodUM : PaymentMethodDTO
    {
    }
    public class PaymentMethodVM : PaymentMethodDTO
    {
        public int Id { get; set; }
    }
}
