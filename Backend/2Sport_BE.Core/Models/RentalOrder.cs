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

        #region BranchInformation
        [Column("BranchId")]
        public int? BranchId { get; set; }

        [Column("BranchName")]
        public string? BranchName { get; set; }

        #endregion

        #region CustomerInformation
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
        #endregion

        #region ProductInformation
        [Column("ProductId")]
        public int? ProductId { get; set; }

        [Column("ProductName")]
        public string? ProductName { get; set; }

        [Column("ProductCode")]
        public string? ProductCode { get; set; }

        [Column("Size")]
        public string? Size { get; set; }

        [Column("Color")]
        public string? Color { get; set; }

        [Column("Condition")]
        public int? Condition { get; set; }

        [Column("RentPrice", TypeName = "decimal")]
        public decimal? RentPrice { get; set; }

        [Column("ImgAvatarPath", TypeName = "varchar")]
        [MaxLength(500)]
        public string? ImgAvatarPath { get; set; }

        [Column("Quantity")]
        public int? Quantity { get; set; }
        #endregion

        #region OrderInformation
        [Column("RentalOrderCode", TypeName = "varchar")]
        [MaxLength(50)]
        public string? RentalOrderCode { get; set; }

        [Column("ParentOrderCode", TypeName = "varchar")]
        [MaxLength(50)]
        public string? ParentOrderCode { get; set; }

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

        [DisplayFormat(DataFormatString = "{0:HH-mm-ss:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.DateTime)]
        public DateTime? DepositDate { get; set; }

        [Column("RentalDays")]
        public int RentalDays { get; set; }

        [Column("SubTotal", TypeName = "decimal")]
        public decimal? SubTotal { get; set; }

        [Column("TranSportFee", TypeName = "decimal")]
        public decimal? TranSportFee { get; set; }

        [Column("TotalAmount", TypeName = "decimal")]
        public decimal TotalAmount { get; set; }

        [Column("DeliveryMethod")]
        [MaxLength(500)]
        public string DeliveryMethod { get; set; }

        [Column("OrderStatus")]
        public int? OrderStatus { get; set; }
        [Column("Note", TypeName = "nvarchar")]
        [MaxLength(500)]
        public string? Note { get; set; }
        [Column("Reason", TypeName = "nvarchar")]
        [MaxLength(500)]
        public string? Reason { get; set; }

        [Column("TransactionId", TypeName = "nvarchar")]
        [MaxLength(100)]
        public string? TransactionId { get; set; }
        [Column("OrderImage", TypeName = "nvarchar")]
        [MaxLength(500)]
        public string? OrderImage { get; set; }
        #endregion

        #region PaymentInformation
        [Column("PaymentMethodId")]
        public int? PaymentMethodId { get; set; }

        [Column("PaymentStatus")]
        public int? PaymentStatus { get; set; }

        [Column("DepositStatus")]
        public int? DepositStatus { get; set; }

        [Column("DepositAmount", TypeName = "decimal")]
        public decimal? DepositAmount { get; set; } = decimal.Zero;
        #endregion

        #region ReturnInformation
        [DisplayFormat(DataFormatString = "{0:HH-mm-ss:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.DateTime)]
        public DateTime? ReturnDate { get; set; } = null;

        [Column("LateFee", TypeName = "decimal")]
        public decimal? LateFee { get; set; } = decimal.Zero;

        [Column("DamageFee", TypeName = "decimal")]
        public decimal? DamageFee { get; set; } = decimal.Zero;

        [Column("IsRestocked")]
        public bool? IsRestocked { get; set; } = false;

        [Column("IsInspected")]
        public bool? IsInspected { get; set; } = false;

        [Column("ExtensionStatus")]
        public int? ExtensionStatus { get; set; } 

        [Column("IsExtended")]
        public bool? IsExtended { get; set; } = false;

        [Column("ExtensionDays")]
        public int? ExtensionDays { get; set; } = 0;

        [Column("ExtensionCost")]
        public decimal? ExtensionCost { get; set; } = decimal.Zero;

        [DisplayFormat(DataFormatString = "{dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.DateTime)]
        [Column("ExtendedDueDate")]
        public DateTime? ExtendedDueDate { get; set; }
        #endregion

        #region AuditInformation
        [DisplayFormat(DataFormatString = "{dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.DateTime)]
        [Column("CreatedAt")]
        public DateTime? CreatedAt { get; set; }

        [DisplayFormat(DataFormatString = "{dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.DateTime)]
        [Column("UpdatedAt")]
        public DateTime? UpdatedAt { get; set; }
        #endregion

        #region NavigationProperties
        public virtual PaymentMethod PaymentMethod { get; set; }
        public virtual User User { get; set; }
        public virtual Branch Branch { get; set; }
        public virtual ICollection<RefundRequest> RefundRequests { get; set; }
        #endregion
    }


}
