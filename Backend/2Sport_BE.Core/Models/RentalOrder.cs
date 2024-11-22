using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2Sport_BE.Repository.Models
{
    [Table("RentalOrders")]
    public class RentalOrder
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("Id")]
        public int Id { get; set; }

        [Column("RentalOrderCode", TypeName = "varchar")]
        [MaxLength(50)]
        public string? RentalOrderCode { get; set; }
        [Column("ParentOrderCode", TypeName = "varchar")]
        [MaxLength(50)]
        public string? ParentOrderCode { get; set; }
        [Column("UserId")]
        public int? UserId { get; set; }

        [Column("Email")]
        public string Email { get; set; }

        [Column("FullName")]
        public string FullName { get; set; }

        [Column("Gender")]
        public string Gender { get; set; }

        [Column("ContactPhone")]
        public string ContactPhone { get; set; }

        [Column("Address")]
        public string Address { get; set; }

        [Column("DeliveryMethod")]
        [MaxLength(500)]
        public string DeliveryMethod { get; set; }

        [Column("BranchId")]
        public int? BranchId { get; set; }
        [Column("BranchName")]
        public string? BranchName { get; set; }

        [Column("ProductId")]
        public int? ProductId { get; set; }
        [Column("ProductName")]
        public string? ProductName { get; set; }

        [Column("ProductCode")]
        public string? ProductCode { get; set; }

        [Column("RentPrice", TypeName = "decimal")]
        public decimal? RentPrice { get; set; }

        [Column("Quantity")]
        public int? Quantity { get; set; }

        [DisplayFormat(DataFormatString = "{0:HH-mm-ss:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.DateTime)]
        [Column("DateOfReceipt")]
        public DateTime? DateOfReceipt { get; set; }

        [DisplayFormat(DataFormatString = "{0:HH-mm-ss:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.DateTime)]
        public DateTime? RentalStartDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:HH-mm-ss:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.DateTime)]
        public DateTime? RentalEndDate { get; set; }

        [Column("RentalDays")]
        public int RentalDays { get; set; }

        [Column("SubTotal", TypeName = "decimal")]
        public decimal? SubTotal { get; set; }

        [Column("TranSportFee", TypeName = "decimal")]
        public decimal? TranSportFee { get; set; }

        [Column("TotalAmount", TypeName = "decimal")]
        public decimal TotalAmount { get; set; }

        [Column("PaymentMethodId")]
        public int? PaymentMethodId { get; set; }

        [Column("Note", TypeName = "nvarchar")]
        [MaxLength(500)]
        public string? Note { get; set; }

        [Column("OrderStatus")]
        public int? OrderStatus { get; set; }

        [Column("PaymentStatus")]
        public int? PaymentStatus { get; set; }

        [Column("DepositStatus")]
        public int? DepositStatus { get; set; }

        [Column("DepositAmount", TypeName = "decimal")]
        public decimal? DepositAmount { get; set; } = decimal.Zero;

        [DisplayFormat(DataFormatString = "{0:HH-mm-ss:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.DateTime)]
        public DateTime? ReturnDate { get; set; } = null;

        [Column("LateFee", TypeName = "decimal")]
        public decimal? LateFee { get; set; } = decimal.Zero;

        [Column("IsRestocked")]
        public bool? IsRestocked { get; set; } = false;

        [Column("IsInspected")]
        public bool? IsInspected { get; set; } = false;

        [Column("DamageFee", TypeName = "decimal")]
        public decimal? DamageFee { get; set; } = decimal.Zero;

        [Column("ImgAvatarPath", TypeName = "varchar")]
        [MaxLength(500)]
        public string? ImgAvatarPath { get; set; }

        [DisplayFormat(DataFormatString = "{0:HH-mm-ss:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.DateTime)]
        [Column("CreatedAt")]
        public DateTime? CreatedAt { get; set; }

        [DisplayFormat(DataFormatString = "{0:HH-mm-ss:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.DateTime)]
        [Column("UpdatedAt")]
        public DateTime? UpdatedAt { get; set; }

        [Column("IsExtended")]
        public bool? IsExtended { get; set; } = false;

        [Column("ExtensionDays")]
        public int? ExtensionDays { get; set; } = 0;
        [Column("ExtensionCost")]
        public decimal? ExtensionCost {  get; set; } = decimal.Zero;

        public virtual PaymentMethod PaymentMethod { get; set; }
        public virtual User User { get; set; }
        public virtual Branch Branch { get; set; }
    }

}
