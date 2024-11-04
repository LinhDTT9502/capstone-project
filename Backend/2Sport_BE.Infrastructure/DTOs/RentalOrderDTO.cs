using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace _2Sport_BE.Infrastructure.DTOs
{
    public class RentalOrderItems
    {
        public int? ProductId { get; set; }
        public string? ProductName { get; set; }
        public int? Quantity { get; set; }
        public decimal? UnitPrice { get; set; }
        public DateTime RentalStartDate { get; set; }
        public DateTime RentalEndDate { get; set; }
        public string? ImgAvatarPath { get; set; }
    }
    public class RentalOrderDTO
    {
        public int? UserID { get; set; }
        public int? ShipmentDetailID { get; set; }
        public string Gender { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string ContactPhone { get; set; }
        public string Address { get; set; }
    }

    public class RentalInfor
    {
        public DateTime? ReturnDate { get; set; }
        public bool IsRestocked { get; set; }
        public bool IsInspected { get; set; }
        public decimal? LateFee { get; set; }
        public decimal? DamageFee { get; set; }
    }
    public class RentalOrderCM : RentalOrderDTO
    {

        public string DeliveryMethod { get; set; }
        public int? BranchId { get; set; }
        public string? DiscountCode { get; set; } // Option
        public string? Note { get; set; }
        public DateTime? DateOfReceipt { get; set; } //NGAY NHAN HANG 

        [Required]
        public List<RentalOrderItems> rentalOrderItems { get; set; }
    }
    public class RentalOrderUM : RentalOrderDTO
    {
        public int PaymentMethodID { get; set; }
        public string? DiscountCode { get; set; } // Option
        public string? Note { get; set; }
        public decimal? SubTotal { get; set; }
        public decimal? TranSportFee { get; set; }
        public decimal TotalAmount { get; set; }
        public int? OrderStatus { get; set; }
        public int? PaymentStatus { get; set; }

        [Required]
        public RentalOrderItems rentalOrderItem { get; set; }

        [Required(ErrorMessage = "Rental Information is required")]
        public RentalInfor rentalInfor { get; set; }
    }
    public class RentalOrderVM : RentalOrderDTO
    {
        public int Id { get; set; }
        public string? RentalOrderCode { get; set; }
        public string? ParentOrderCode { get; set; }
        public int? BranchId { get; set; }
        public string? BranchName { get; set; }
        public int? ProductId { get; set; }
        public string? ProductName { get; set; }
        public decimal? RentPrice { get; set; }
        public int? Quantity { get; set; }
        public DateTime? RentalStartDate { get; set; }
        public DateTime? RentalEndDate { get; set; }
        public decimal? SubTotal { get; set; }
        public decimal? TranSportFee { get; set; }
        public decimal? TotalAmount { get; set; }
        public string? DeliveryMethod { get; set; }
        public int? PaymentMethodId { get; set; }
        public string? Note { get; set; }
        public string? OrderStatus { get; set; }
        public string? PaymentStatus { get; set; }
        public DateTime? ReturnDate { get; set; }
        public decimal LateFee { get; set; }
        public bool IsRestocked { get; set; } // Track if the product has been restocked
        public bool? IsInspected { get; set; } // Track if the product is inspected upon return
        public decimal? DamageFee { get; set; }
        public string? ImgAvatarPath { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? PaymentLink { get; set; }
    }

}
