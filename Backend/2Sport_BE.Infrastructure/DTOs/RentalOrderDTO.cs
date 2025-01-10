using Microsoft.AspNetCore.Http;
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

        #region BranchInformation
        public int? BranchId { get; set; }

        public string? BranchName { get; set; }

        #endregion

        #region CustomerInformation
        public int? UserId { get; set; }

        public string Email { get; set; }

        public string FullName { get; set; }

        public string Gender { get; set; }

        public string ContactPhone { get; set; }

        public string Address { get; set; }
        #endregion

        #region ProductInformation
        public int? ProductId { get; set; }

        public string? ProductName { get; set; }

        public string? ProductCode { get; set; }

        public string? Size { get; set; }

        public string? Color { get; set; }

        public int? Condition { get; set; }

        public decimal? RentPrice { get; set; }

        public string? ImgAvatarPath { get; set; }

        public int? Quantity { get; set; }
        #endregion

        #region OrderInformation
        public string? RentalOrderCode { get; set; }

        public string? ParentOrderCode { get; set; }

        public DateTime? DateOfReceipt { get; set; }

        public DateTime? RentalStartDate { get; set; }

        public DateTime? RentalEndDate { get; set; }

        public int RentalDays { get; set; }

        public decimal? SubTotal { get; set; }

        public decimal? TranSportFee { get; set; }

        public decimal TotalAmount { get; set; }

        public string DeliveryMethod { get; set; }

        public int? OrderStatusId {  get; set; }
        public string? OrderStatus { get; set; }

        public string? Note { get; set; }
        #endregion

        #region PaymentInformation
        public string? PaymentMethod { get; set; }

        public string? PaymentStatus { get; set; }

        public string? DepositStatus { get; set; }

        public decimal? DepositAmount { get; set; }

        public DateTime? DepositDate { get; set; }

        public DateTime? PaymentDate { get; set; }
        #endregion

        #region ReturnInformation
        public DateTime? ReturnDate { get; set; }

        public decimal? LateFee { get; set; }

        public decimal? DamageFee { get; set; }

        public bool? IsRestocked { get; set; }

        public bool? IsInspected { get; set; }

        public bool? IsExtended { get; set; }

        public int? ExtensionDays { get; set; }

        public decimal? ExtensionCost { get; set; }
        public string? ExtensionStatus { get; set; }
        #endregion

        public string? Reason { get; set; }
        public string? TransactionId { get; set; }
        public string? OrderImage { get; set; }

        #region AuditInformation
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        #endregion
        public string? PaymentLink { get; set; }
        public List<RentalOrderVM>? childOrders { get; set; }
    }
    public class ExtensionRequestModel
    {
        public int ParentOrderId { get; set; }
        public int? ChildOrderId { get; set; }
        public int ExtensionDays { get; set; }

    }
    public class ReturnRequestModel
    {
        public int SelectedReturnOrderId { get; set; }
        //public int ParentOrderId { get; set; }
        public DateTime? RequestTimestamp { get; set; }
    }

    public class ReturnRequestModelUM
    {
        public int SelectedReturnOrderId { get; set; }
        public bool IsRestocked { get; set; }      // Đã nhập kho lại chưa
        public bool IsInspected { get; set; }      // Đã kiểm tra chưa
        public decimal? LateFee { get; set; }       // Phí trễ hạn
        public decimal? DamageFee { get; set; }     // Phí hư hỏng
    }

    public class RentalOrderImageModel
    {
        public int parentOrderId { get; set; }
        public IFormFile OrderImage { get; set; }
    }
}
