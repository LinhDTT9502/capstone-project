using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace _2Sport_BE.Infrastructure.DTOs
{
    public class RentalOrderItems
    {
        [Required]
        public int WarehouseId { get; set; }
        [Required]
        public int Quantity { get; set; }
        [Required]
        public DateTime? RentalStartDate { get; set; }
        [Required]
        public DateTime? RentalEndDate { get; set; }

        public string? ImgAvatarPath { get; set; }
    }
    public class RentalOrderDTO
    {
        [Required]
        public CustomerInfo CustomerInfo { get; set; }
    }
    public class CustomerInfo
    {
        public int UserID { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string ContactPhone { get; set; }
        public string Address { get; set; }
    }
    public class RentalOrderCM : RentalOrderDTO
    {
        public int ShipmentDetailID { get; set; }
        [Required]
        public int PaymentMethodID { get; set; }
        public string? DiscountCode { get; set; } // Option
        public string? Note { get; set; }
        [Required]
        public List<RentalOrderItems> rentalOrderItems { get; set; }
    }
    public class RentalInfor
    {
        public DateTime? ReturnDate { get; set; }
        public bool IsRestocked { get; set; }
        public bool IsInspected { get; set; }
        public decimal? LateFee { get; set; }
        public decimal? DamageFee { get; set; }
    }
    public class RentalOrderUM : RentalOrderDTO
    {

        [Required]
        public int PaymentMethodID { get; set; }
        public string? DiscountCode { get; set; } // Option
        public string? Note { get; set; }
        [Required]
        public List<RentalOrderItems> rentalOrderItems { get; set; }

        [Required(ErrorMessage = "Rental Information is required")]
        public RentalInfor rentalInfor { get; set; }
    }
    public class RentalOrderVM : RentalOrderDTO
    {
        public int RentalOrderID { get; set; }
        public string? RentalOrderCode { get; set; }
        public int? PaymentMethodID { get; set; }
        public int? BranchId { get; set; } //Branch nao nhan order
        public string? BranchName { get; set; }
        public int? ProductId { get; set; }
        public string? ProductName { get; set; }
        public decimal? RentPrice { get; set; }
        public decimal? SubTotal { get; set; }
        public decimal? TransportFee { get; set; }
        public decimal? TotalAmount { get; set; }
        public string? Note { get; set; }
        public string? PaymentLink { get; set; }
        public RentalInfor rentalInfor { get; set; }
        public DateTime? CreatedAt { get; set; }

    }

}
