using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace _2Sport_BE.Infrastructure.DTOs
{
    public class CustomerInformation
    {
        public int? UserId { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string Gender { get; set; }
        public string ContactPhone { get; set; }
        public string Address { get; set; }
    }
    public class ProductInformation
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
        public decimal? RentPrice { get; set; }
        public string? ImgAvatarPath { get; set; }
        public int? Quantity { get; set; }
        public RentalDates RentalDates { get; set; }
        public RentalCosts RentalCosts { get; set; }
    }
    public class RentalDates
    {
        public DateTime DateOfReceipt { get; set; }
        public DateTime RentalStartDate { get; set; }
        public DateTime RentalEndDate { get; set; }
        public int? RentalDays { get; set; }
    }
    public class RentalCosts
    {
        public decimal? SubTotal { get; set; }
        public decimal? TranSportFee { get; set; }
        public decimal? TotalAmount { get; set; }
    }
    public class ChildOrderReturnModel
    {
        public int OrderId { get; set; }           // ID của child order
        public DateTime ReturnDate { get; set; }   // Ngày trả hàng
        public decimal LateFee { get; set; }       // Phí trễ hạn
        public decimal DamageFee { get; set; }     // Phí hư hỏng
        public bool IsRestocked { get; set; }      // Đã nhập kho lại chưa
        public bool IsInspected { get; set; }      // Đã kiểm tra chưa
    }
    public class ParentOrderReturnModel
    {
        public int ParentOrderId { get; set; }            // ID của parent order
        public List<ChildOrderReturnModel> ChildOrders { get; set; }  // Danh sách child orders

        // Thông tin tổng hợp
        public decimal TotalLateFee { get; set; }         // Tổng phí trễ hạn
        public decimal TotalDamageFee { get; set; }       // Tổng phí hư hỏng
        public bool IsRestocked { get; set; }             // Tổng trạng thái nhập kho
        public bool IsInspected { get; set; }             // Tổng trạng thái kiểm tra
    }
    public class RentalOrderCM
    {
        public CustomerInformation CustomerInformation { get; set; }
        public string? Note { get; set; }
        public string DeliveryMethod { get; set; }
        public int? BranchId { get; set; }
        public List<ProductInformation> ProductInformations { get; set; }
    }
    public class RentalOrderUM
    {
        public CustomerInformation CustomerInformation { get; set; }

        public int? PaymentMethodID { get; set; }
        public string DeliveryMethod { get; set; }
        public int? PaymentStatus { get; set; }
        public string? Note { get; set; }

        public decimal? ParentSubTotal { get; set; }
        public decimal? ParentTranSportFee { get; set; }
        public decimal ParentTotalAmount { get; set; }
        public int BranchId { get; set; }


        [Required]
        public List<ProductInformation> ProductInformations { get; set; }
    }
    public class RentalOrderVM
    {
        public int Id { get; set; }
        public string? RentalOrderCode { get; set; }
        public string? ParentOrderCode { get; set; }
        public string Gender { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string ContactPhone { get; set; }
        public string Address { get; set; }
        public int? BranchId { get; set; }
        public string? BranchName { get; set; }
        public int? ProductId { get; set; }
        public string? ProductName { get; set; }
        public decimal? RentPrice { get; set; }
        public int? Quantity { get; set; }
        public DateTime? RentalStartDate { get; set; }
        public DateTime? RentalEndDate { get; set; }
        public int RentalDays { get; set; }
        public decimal? SubTotal { get; set; }
        public decimal? TranSportFee { get; set; }
        public decimal? TotalAmount { get; set; }
        public string? DepositStatus { get; set; }
        public decimal? DepositAmount { get; set; }
        public string? DeliveryMethod { get; set; }
        public string? PaymentMethod { get; set; }
        public string? Note { get; set; }
        public string? OrderStatus { get; set; }
        public string? PaymentStatus { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string? ImgAvatarPath { get; set; }
        public string? PaymentLink { get; set; }
        public bool? IsExtended { get; set; } = false;
        public int? ExtensionDays { get; set; } = 0;
        public decimal? ExtensionCost { get; set; } = decimal.Zero;

        public List<RentalOrderVM>? listChild { get; set; }
    }
    public class ExtendRentalModel
    {
        public int ChildOrderId { get; set; }
        public int ExtensionDays { get; set; }
    }
}
