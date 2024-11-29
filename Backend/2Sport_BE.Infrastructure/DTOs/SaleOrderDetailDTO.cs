namespace _2Sport_BE.Infrastructure.DTOs
{
    public class SaleOrderDetailDTO
    {
        public int? ProductId { get; set; }
        public string? ProductName { get; set; }
        public string? ProductCode { get; set; }
        public string? Size { get; set; }
        public string? Color { get; set; }
        public int? Condition { get; set; }
        public string? ImgAvatarPath { get; set; }
        public decimal? UnitPrice { get; set; }
        public int? Quantity { get; set; }
        public decimal? TotalAmount {  get; set; }
    }
    public class SaleOrderDetailCM : SaleOrderDetailDTO
    {
    }
    public class SaleOrderDetailUM : SaleOrderDetailDTO
    {
    }
    public class SaleOrderDetailVM : SaleOrderDetailDTO
    {

    }
}
