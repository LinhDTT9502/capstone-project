namespace _2Sport_BE.ViewModels
{
    public class PaymentResponse
    {
        public string? Status { get; set; }
        public string? Code { get; set; }
        public string? Id { get; set; }
        public bool Cancel { get; set; }
        public string? OrderCode { get; set; }
    }
}
