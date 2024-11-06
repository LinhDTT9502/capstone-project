namespace _2Sport_BE.Infrastructure.DTOs
{
    public class SaleOrderDetailDTO
    {
        public int? ProductId { get; set; }
        public string? ProductName { get; set; }
        public string? ProductCode { get; set; }
        public int? Quantity { get; set; }
        public decimal? UnitPrice { get; set; }
    }
    public class SaleOrderDetailCM : SaleOrderDetailDTO
    {
    }
    public class SaleOrderDetailUM : SaleOrderDetailDTO
    {
        public string? ImgAvatarPath { get; set; }
    }
    public class SaleOrderDetailVM : SaleOrderDetailDTO
    {
        public decimal? TotalPrice { get; set; }
    }
}
