namespace _2Sport_BE.ViewModels
{
    public class OrderDTO
    {
        public decimal? TransportFee { get; set; }
        public DateTime? ReceivedDate { get; set; }
        public List<OrderDetailRequest> OrderDetails { get; set; }
    }
    public class OrderCM : OrderDTO
    {
        public int? ShipmentDetailId { get; set; }
    }
    public class OrderUM : OrderDTO
    {
    }
    public class OrderVM : OrderDTO
    {
        public int? id { get; set; }
        public int? Status { get; set; }
        public decimal? IntoMoney { get; set; }
        public string? PaymentMethod { get; set; }
        public int? ShipmentDetailId { get; set; }
        public string? PaymentLink { get; set; }
    }
    public class OrderDetailRequest
    {
        public int? ProductId { get; set; }
        public int? Quantity { get; set; }
        public decimal Price { get; set; }
    }
    public class CancelPaymentRequest
    {
        public int OrderId { get; set; }
        public string Reason { get; set; }
    }
    public class OrdersSales
    {
        public int TotalOrders { get; set; }
        public decimal? TotalIntoMoney { get; set; }
        public double orderGrowthRatio { get; set; }
        public bool IsIncrease { get; set; }
    }
    public class OrderResponse
    {
        public int Id { get; set; }
        public string? OrderCode { get; set; }
        public string? CreateDate { get; set; }
        public string? CustomerName { get; set; }
        public string? Status { get; set; }
        public string? Amount {  get; set; }
        public List<OrderDetailResponse> OrderDetails { get; set; }

    }
    public class OrderDetailResponse
    {
        public string? ProductName { get; set; }
        public int? Quantity { get; set; }
        public string TotalPrice { get; set; }
    }
}
