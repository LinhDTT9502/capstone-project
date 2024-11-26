using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace _2Sport_BE.Repository.Models;

[Table("SaleOrders")]
public class SaleOrder
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("SaleOrderId")]
    public int Id { get; set; }

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

    #region BranchInformation
    [Column("BranchId")]
    public int? BranchId { get; set; }

    [Column("BranchName")]
    public string? BranchName { get; set; }

    #endregion

    #region OrderInformation
    [Column("SaleOrderCode", TypeName = "varchar")]
    [MaxLength(50)]
    public string? SaleOrderCode { get; set; }

    [DisplayFormat(DataFormatString = "{0:HH-mm-ss:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
    [DataType(DataType.DateTime)]
    [Column("DateOfReceipt")]
    public DateTime? DateOfReceipt { get; set; }

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
    #endregion

    #region PaymentInformation

    [Column("PaymentMethodId")]
    public int? PaymentMethodId { get; set; }

    [Column("PaymentStatus")]
    public int? PaymentStatus { get; set; }

    #endregion

    #region AuditInformation
    [DisplayFormat(DataFormatString = "{0:HH-mm-ss:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
    [DataType(DataType.DateTime)]
    [Column("CreatedAt")]
    public DateTime? CreatedAt { get; set; }

    [DisplayFormat(DataFormatString = "{0:HH-mm-ss:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
    [DataType(DataType.DateTime)]
    [Column("UpdatedAt")]
    public DateTime? UpdatedAt { get; set; }
    #endregion

    #region NavigationProperties
    public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    public virtual PaymentMethod PaymentMethod { get; set; }
    public virtual User User { get; set; }
    public virtual Branch Branch { get; set; }
    public virtual ICollection<RefundRequest> Refunds { get; set; }
    #endregion
}