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
        [Column("RentalOrderId")]
        public int Id { get; set; }

        [Column("OrderCode", TypeName = "varchar")]
        [MaxLength(50)]
        public string? OrderCode { get; set; }

        [Column("UserId")]
        public int? UserId { get; set; }

        [Column("Email")]
        public string Email { get; set; }

        [Column("ContactPhone")]
        public string ContactPhone { get; set; }

        [Column("FullName")]
        public string FullName { get; set; }

        [Column("Address")]
        public string Address { get; set; }

        [Column("BranchId")]
        public int? BranchId { get; set; }

        [Column("PaymentMethodId")]
        public int? PaymentMethodId { get; set; }


        [Column("SubTotal", TypeName = "decimal")]
        public decimal? SubTotal { get; set; }


        [Column("TranSportFee", TypeName = "decimal")]
        public decimal? TranSportFee { get; set; }


        [Column("TotalAmount", TypeName = "decimal")]
        public decimal TotalAmount { get; set; }


        [Column("Note", TypeName = "nvarchar")]
        [MaxLength(500)]
        public string? Note { get; set; }


        [Column("OrderStatus")]
        public int? OrderStatus { get; set; }


        [Column("PaymentStatus")]
        public string? PaymentStatus { get; set; }


        [DisplayFormat(DataFormatString = "{0:HH-mm-ss:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.DateTime)]
        [Column("CreatedAt")]
        public DateTime? CreatedAt { get; set; }


        [DisplayFormat(DataFormatString = "{0:HH-mm-ss:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.DateTime)]
        [Column("UpdatedAt")]
        public DateTime? UpdatedAt { get; set; }


        [DisplayFormat(DataFormatString = "{0:HH-mm-ss:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.DateTime)]
        public DateTime? RentalStartDate { get; set; }


        [DisplayFormat(DataFormatString = "{0:HH-mm-ss:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.DateTime)]
        public DateTime? RentalEndDate { get; set; }


        [DisplayFormat(DataFormatString = "{0:HH-mm-ss:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.DateTime)]
        public DateTime? ReturnDate { get; set; }


        [Column("LateFee", TypeName = "decimal")]
        public decimal LateFee { get; set; }

        public bool IsRestocked { get; set; } // Track if the product has been restocked
        public bool IsInspected { get; set; } // Track if the product is inspected upon return

        [Column("DamageFee", TypeName = "decimal")]
        public decimal? DamageFee { get; set; }

        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
        public virtual PaymentMethod PaymentMethod { get; set; }
        public virtual User User { get; set; }
        public virtual Branch Branch { get; set; }
    }

}
