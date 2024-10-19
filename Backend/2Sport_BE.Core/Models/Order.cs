using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace _2Sport_BE.Repository.Models;

[Table("Orders")]
public class Order
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Column("OrderCode", TypeName = "varchar")]
    [MaxLength(50)]
    public string? OrderCode { get; set; }
    [Column("OrderType")]
    public int? OrderType { get; set; } // 1: Bán, 2: Cho thuê

    [Column("BranchId")]
    public int? BranchId { get; set; }

    [Column("UserId")]
    public int? UserId { get; set; }

    public int? GuestId { get; set; }

    [Column("PaymentMethodId")]
    public int? PaymentMethodId { get; set; }

    [Column("ShipmentDetailId")]
    public int? ShipmentDetailId { get; set; }

    [Column("TotalPrice", TypeName = "decimal")]
    public decimal TotalPrice { get; set; }

    [Column("TranSportFee", TypeName = "decimal")]
    public decimal? TranSportFee { get; set; }

    [Column("IntoMoney", TypeName = "decimal")]
    public decimal IntoMoney { get; set; }

    [Column("Note", TypeName = "nvarchar")]
    [MaxLength(500)]
    public string? Note { get; set; }

    [Column("Status")]
    public int? Status { get; set; }

    [DisplayFormat(DataFormatString = "{0:HH-mm-ss:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
    [DataType(DataType.DateTime)]
    public DateTime? CreateAt { get; set; }

    [DisplayFormat(DataFormatString = "{0:HH-mm-ss:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
    [DataType(DataType.DateTime)]
    public DateTime? ReceivedDate { get; set; }

    [ForeignKey("GuestId")]
    public virtual Guest Guest { get; set; }
    public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    public virtual PaymentMethod PaymentMethod { get; set; }
    public virtual ShipmentDetail ShipmentDetail { get; set; }
    public virtual User User { get; set; }
    public virtual Branch Branch { get; set; }
}