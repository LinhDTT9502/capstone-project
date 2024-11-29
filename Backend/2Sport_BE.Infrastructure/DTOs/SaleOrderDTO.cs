using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace _2Sport_BE.Infrastructure.DTOs
{
    public class ProductInfor
    {
        public Guid? CartItemId { get; set; }
        [Required]
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        [Required]
        public string ProductCode { get; set; }
        public string? Size { get; set; }
        public string? Color { get; set; }
        public int? Condition { get; set; }
        public decimal? UnitPrice { get; set; }
        public string? ImgAvatarPath { get; set; }
        public int? Quantity { get; set; }
    }
    public class SaleCosts
    {
        public decimal? SubTotal { get; set; }
        public decimal? TranSportFee { get; set; }
        public decimal? TotalAmount { get; set; }
    }
    public class SaleOrderCM
    {
        public CustomerInformation CustomerInformation { get; set; }

        public string DeliveryMethod { get; set; }
        public int? BranchId { get; set; }
        public DateTime? DateOfReceipt { get; set; }
        public string? Note { get; set; }
        public List<ProductInfor> ProductInformations { get; set; }
        public RentalCosts SaleCosts { get; set; }

    }
    public class SaleOrderUM
    {
        public CustomerInformation CustomerInformation { get; set; }
        public string? DeliveryMethod { get; set; }
        public int? BranchId { get; set; }
        public DateTime? DateOfReceipt { get; set; }
        public int? PaymentMethodId { get; set; }
        public int? PaymentStatus { get; set; }
        public string? Note { get; set; }
        public List<ProductInfor> ProductInformations { get; set; }
        public RentalCosts SaleCosts { get; set; }

    }
    public class SaleOrderVM
    {
        public int Id { get; set; }

        #region CustomerInformation
        public int? UserId { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string Gender { get; set; }
        public string ContactPhone { get; set; }
        public string Address { get; set; }
        #endregion

        #region BranchInformation
        public int? BranchId { get; set; }
        public string? BranchName { get; set; }

        #endregion

        #region OrderInformation
        public string? SaleOrderCode { get; set; }
        public DateTime? DateOfReceipt { get; set; }
        public decimal? SubTotal { get; set; }
        public decimal? TranSportFee { get; set; }
        public decimal TotalAmount { get; set; }
        public string DeliveryMethod { get; set; }
        public string? OrderStatus { get; set; }
        public string? Note { get; set; }
        #endregion

        #region PaymentInformation
        public int? PaymentMethodId { get; set; }
        public string? PaymentStatus { get; set; }

        #endregion

        #region AuditInformation
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        #endregion  
        public string? PaymentMethod { get; set; }
        public string? PaymentLink { get; set; }
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
        public string? TransactionType { get; set; }
    }
}
