using System.ComponentModel.DataAnnotations;

namespace _2Sport_BE.Infrastructure.DTOs
{
    public class SaleOrderDTO
    {


    }
    public class SaleOrderCM : SaleOrderDTO
    {
        public int UserID { get; set; }
        public int ShipmentDetailID { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string ContactPhone { get; set; }
        public string Address { get; set; }
        public int PaymentMethodID { get; set; }
        public string? DiscountCode { get; set; } // Option
        public string? Note { get; set; }
        public List<SaleOrderDetailCM> SaleOrderDetailCMs { get; set; }
    }
    public class SaleOrderUM
    {
        [Required]
        public int BranchId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string ContactPhone { get; set; }
        public string Address { get; set; }
        public decimal SubTotal { get; set; }
        public decimal TransportFee { get; set; }
        public decimal TotalAmount { get; set; }
        public int PaymentStatus { get; set; }
        public int OrderStatus { get; set; }
        public int PaymentMethodId { get; set; }
        public string Note { get; set; }
        public List<SaleOrderDetailUM> SaleOrderDetailUMs { get; set; }

    }
    public class SaleOrderVM
    {
        public int SaleOrderID { get; set; }
        public string? SaleOrderCode { get; set; }
        public int? UserID { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string ContactPhone { get; set; }
        public string Address { get; set; }
        public int? PaymentMethodId { get; set; }
        public decimal? SubTotal { get; set; }
        public decimal? TransportFee { get; set; }
        public decimal? TotalAmount { get; set; }
        public string? OrderStatus { get; set; }
        public string? PaymentStatus { get; set; }
        public string? PaymentLink { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string? Note { get; set; }
        public List<SaleOrderDetailVM> SaleOrderDetailVMs { get; set; }
    }
    public class RevenueVM
    {
        public int TotalSaleOrders { get; set; }
        public string SubTotal { get; set; }
    }
    public class SaleOrdersSales
    {
        public int TotalSaleOrders { get; set; }
        public decimal TotalIntoMoney { get; set; }
        public int SaleOrderGrowthRatio { get; set; }
        public bool IsIncrease { get; set; }
    }
}
