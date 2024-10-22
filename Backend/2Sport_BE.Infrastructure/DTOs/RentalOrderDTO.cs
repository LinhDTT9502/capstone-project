using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2Sport_BE.Service.DTOs
{
    public class RentalOrderItems
    {
        [Required]
        public int WarehouseId { get; set; }
        [Required]
        public int Quantity { get; set; }
    }
    public class RentalOrderDTO
    {
        public List<RentalOrderItems> rentalOrderItems { get; set; }
    }
    public class RentalInfor
    {
        public DateTime? RentalStartDate { get; set; }
        public DateTime? RentalEndDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public bool IsRestocked { get; set; }
        public bool IsInspected { get; set; }
        public decimal? LateFee { get; set; }
        public decimal? DamageFee { get; set; }
    }
    public class RentalOrderCM : RentalOrderDTO
    {
        [Required]
        public int UserID { get; set; }
        [Required]
        public int ShipmentDetailID { get; set; }
        [Required]
        public int PaymentMethodID { get; set; }
        [Required]
        public int OrderType { get; set; }
        [Required]
        public string? DiscountCode { get; set; } // Option
        [Required]
        public int BranchId { get; set; } //Branch nao nhan order
        [Required]
        public DateTime? RentalStartDate { get; set; }
        [Required]
        public DateTime? RentalEndDate { get; set; }
        [Required]
        public decimal? TransportFee { get; set; }
        public string? Note { get; set; }
    }
    public class GuestRentalOrderCM : RentalOrderDTO
    {
        [Required]
        public GuestCM guestCM { get; set; }
        [Required]
        public int PaymentMethodID { get; set; }
        [Required]
        public int OrderType { get; set; }
        [Required]
        public string? DiscountCode { get; set; }
        [Required]
        public int BranchId { get; set; }
        [Required]
        public DateTime? RentalStartDate { get; set; }
        [Required]
        public DateTime? RentalEndDate { get; set; }
        [Required]
        public decimal? TransportFee { get; set; }
        public string? Note { get; set; }
    }
    public class RentalOrderVM : RentalOrderDTO
    {
        public int OrderID { get; set; }
        public string? OrderCode { get; set; }
        public int UserID { get; set; }
        public int ShipmentDetailID { get; set; }
        public int PaymentMethodID { get; set; }
        public int BranchId { get; set; } //Branch nao nhan order
        public DateTime? RentalStartDate { get; set; }
        public DateTime? RentalEndDate { get; set; }
        public DateTime? CreatedDate { get; set; }
        public decimal? TotalPrice { get; set; }
        public decimal? TransportFee { get; set; }
        public decimal? IntoMoney { get; set; }
        public string? Note { get; set; }
        public string? PaymentLink { get; set; }
    }
    public class GuestRentalOrderVM : RentalOrderDTO
    {
        public int OrderID { get; set; }
        public string? OrderCode { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public decimal? TotalPrice { get; set; }
        public decimal? TransportFee { get; set; }
        public decimal? IntoMoney { get; set; }
        public string? PaymentLink { get; set; }
        public int PaymentMethodID { get; set; }
        public int BranchId { get; set; }
        public DateTime? RentalStartDate { get; set; }
        public DateTime? RentalEndDate { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? Note { get; set; }
    }
    public class RentalOrderUM : RentalOrderDTO
    {
        [Required(ErrorMessage = "BranchId is required")]
        [Range(1, int.MaxValue, ErrorMessage = "BranchId must be a positive integer")]
        public int BranchId { get; set; }

        [Required(ErrorMessage = "ShipmentDetailID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "ShipmentDetailID must be a positive integer")]
        public int ShipmentDetailID { get; set; }

        [Required(ErrorMessage = "TotalPrice is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "TotalPrice must be greater than zero")]
        public decimal TotalPrice { get; set; }

        [Required(ErrorMessage = "TranSportFee is required")]
        [Range(0, double.MaxValue, ErrorMessage = "TranSportFee must be a positive value or zero")]
        public decimal TranSportFee { get; set; }

        [Required(ErrorMessage = "NewIntoMoney is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "NewIntoMoney must be greater than zero")]
        public decimal NewIntoMoney { get; set; }

        [Required(ErrorMessage = "Status is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Status must be a valid integer")]
        public int Status { get; set; }

        [Required(ErrorMessage = "Note is required")]
        [StringLength(500, ErrorMessage = "Note cannot exceed 500 characters")]
        public string Note { get; set; }

        [Required(ErrorMessage = "At least one Rental Order Item is required")]
        [MinLength(1, ErrorMessage = "At least one Rental Order Item is required")]
        public List<RentalOrderItems> rentalOrderItems { get; set; }

        [Required(ErrorMessage = "Rental Information is required")]
        public RentalInfor rentalInfor { get; set; }
    }

    public class GuestRentalOrderUM : RentalOrderDTO
    {
        // Guest Information (must be required if it's crucial for guest orders)
        [Required(ErrorMessage = "Guest information is required")]
        public GuestUM guestUM { get; set; }

        // Order Info
        [Required(ErrorMessage = "BranchId is required")]
        [Range(1, int.MaxValue, ErrorMessage = "BranchId must be a positive integer")]
        public int? BranchId { get; set; }

        [Required(ErrorMessage = "TotalPrice is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "TotalPrice must be greater than zero")]
        public decimal TotalPrice { get; set; }

        [Required(ErrorMessage = "TranSportFee is required")]
        [Range(0, double.MaxValue, ErrorMessage = "TranSportFee must be a positive value or zero")]
        public decimal? TranSportFee { get; set; }

        [Required(ErrorMessage = "NewIntoMoney is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "NewIntoMoney must be greater than zero")]
        public decimal? NewIntoMoney { get; set; }

        [Required(ErrorMessage = "Status is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Status must be a valid integer")]
        public int? Status { get; set; }

        [Required(ErrorMessage = "Note is required")]
        [StringLength(500, ErrorMessage = "Note cannot exceed 500 characters")]
        public string? Note { get; set; }

        [Required(ErrorMessage = "At least one Rental Order Item is required")]
        [MinLength(1, ErrorMessage = "At least one Rental Order Item is required")]
        public List<RentalOrderItems>? rentalOrderItems { get; set; }

        [Required(ErrorMessage = "Rental information is required")]
        public RentalInfor rentalInfor { get; set; }
    }

}
