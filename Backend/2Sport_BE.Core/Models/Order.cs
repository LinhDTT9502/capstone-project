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

    [Column("UserId")]
    public int UserId { get; set; }

    [Column("IntoMoney", TypeName = "decimal")]
    public decimal IntoMoney { get; set; }

    [Column("TotalPrice", TypeName = "decimal")]
    public decimal TotalPrice { get; set; }

    [Column("OrderCode", TypeName = "varchar")]
    [MaxLength(50)]
    public string OrderCode { get; set; }

    [Column("PaymentMethodId")]
    public int? PaymentMethodId { get; set; }

    [Column("ShipmentDetailId")]
    public int? ShipmentDetailId { get; set; }

    public int? Status { get; set; }

    [Column("TranSportFee", TypeName = "decimal")]
    public decimal? TranSportFee { get; set; }

    [DisplayFormat(DataFormatString = "{0:HH-mm-ss:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
    [DataType(DataType.DateTime)]
    public DateTime? CreateAt { get; set; }

    [DisplayFormat(DataFormatString = "{0:HH-mm-ss:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
    [DataType(DataType.DateTime)]
    public DateTime? ReceivedDate { get; set; }

    public virtual ICollection<OrderDetail> OrderDetails { get; set; }

    public virtual PaymentMethod PaymentMethod { get; set; }

    public virtual ShipmentDetail ShipmentDetail { get; set; }

    public virtual User User { get; set; }
}