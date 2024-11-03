using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace _2Sport_BE.Infrastructure.DTOs
{
    public class SaleOrderDTO
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string ContactPhone { get; set; }
        public string Address { get; set; }
    }

    public class SaleOrderCM : SaleOrderDTO
    {
        public int? UserID { get; set; }
        public int? ShipmentDetailID { get; set; }
        public string DeliveryMethod { get; set; }
        public string Gender { get; set; }
        public int? BranchId { get; set; }
        public DateTime? DateOfReceipt { get; set; } //NGAY NHAN HANG 
        public string? DiscountCode { get; set; } // Option
        public string? Note { get; set; }
        public List<SaleOrderDetailCM> SaleOrderDetailCMs { get; set; }
    }
    public class SaleOrderUM : SaleOrderDTO
    {
        public string? DeliveryMethod { get; set; }
        public int? BranchId { get; set; }
        public DateTime? DateOfReceipt { get; set; } //NGAY NHAN HANG 
        public decimal SubTotal { get; set; }
        public decimal TransportFee { get; set; }
        public decimal TotalAmount { get; set; }
        public int? PaymentStatus { get; set; }
        public int? OrderStatus { get; set; }
        public int? PaymentMethodId { get; set; }
        public string? Note { get; set; }
        public List<SaleOrderDetailUM>? SaleOrderDetailUMs { get; set; }

    }
    public class SaleOrderVM 
    {
        public int SaleOrderId { get; set; }
        public string? OrderCode { get; set; }
        public int? UserId { get; set; }
        public string FullName { get; set; }
        public string Gender { get; set; }//THEM GENDER
        public string Email { get; set; }
        public string ContactPhone { get; set; }
        public string Address { get; set; }
        public string DeliveryMethod { get; set; } //PHUONG THUC NHAN HANG
        public string? PaymentMethod { get; set; }
        public int? BranchId { get; set; }
        public decimal? SubTotal { get; set; }
        public decimal? TranSportFee { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime? DateOfReceipt { get; set; } //NGAY NHAN HANG 
        public string? Note { get; set; }
        public string? OrderStatus { get; set; }
        public string? PaymentStatus { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string? PaymentLink  { get; set; }
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
    public class CheckoutModel
    {
        public int PaymentMethodID { get; set; }
        public int OrderID { get; set; }
        public string OrderCode { get; set; }
        public int UserId { get; set; }
    }
}
