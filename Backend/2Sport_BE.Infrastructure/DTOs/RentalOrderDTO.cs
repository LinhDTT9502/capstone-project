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
        public int? WarehouseId { get; set; }
        public int? Quantity { get; set; }
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
        public string? TotalPrice { get; set; }
        public string? TransportFee { get; set; }
        public string? IntoMoney { get; set; }
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
        [Required]
        public int BranchId { get; set; }
        [Required]
        public int ShipmentDetailID { get; set; }
        [Required]
        public decimal TotalPrice { get; set; }
        [Required]
        public decimal TranSportFee { get; set; }
        [Required]
        public decimal NewIntoMoney { get; set; }
        [Required]
        public int Status { get; set; }
        [Required]
        public string Note { get; set; }
        [Required]
        public List<RentalOrderItems>? RentalOrderItems { get; set; }
        [Required]
        public RentalInfor rentalInfor { get; set; }

    }
    public class GuestRentalOrderUM : RentalOrderDTO
    {
        //Guest Infor UM
        public GuestUM guestUM { get; set; }
        //Order Info UM
        [Required]
        public int? BranchId { get; set; }
        [Required]
        public decimal TotalPrice { get; set; }
        [Required]
        public decimal? TranSportFee { get; set; }
        [Required]
        public int? Status { get; set; }
        [Required]
        public string? Note { get; set; }
        [Required]
        public List<RentalOrderItems>? RentalOrderItems { get; set; }
        [Required]
        public RentalInfor rentalInfor { get; set; }
    }
}
