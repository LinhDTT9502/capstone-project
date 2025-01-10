using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace _2Sport_BE.Repository.Models;

[Table("OrderDetails")]
public class OrderDetail
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }


    [Column("SaleOrderId")]
    public int? SaleOrderId { get; set; }

    [Column("SaleOrderCode", TypeName = "varchar")]
    [MaxLength(50)]
    public string? SaleOrderCode { get; set; }

    #region BranchInformation
    [Column("BranchId")]
    public int? BranchId { get; set; }

    [Column("BranchName")]
    public string? BranchName { get; set; }

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

    [Column("UnitPrice", TypeName = "decimal")]
    public decimal? UnitPrice { get; set; }

    [Column("ImgAvatarPath", TypeName = "nvarchar")]
    [MaxLength(500)]
    public string? ImgAvatarPath { get; set; }

    [Column("Quantity")]
    public int? Quantity { get; set; }
    #endregion

    [Column("TotalAmount")]
    public decimal? TotalAmount => UnitPrice * Quantity;

    #region AuditInformation
    [DisplayFormat(DataFormatString = "{0:HH-mm-ss:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
    [DataType(DataType.DateTime)]
    [Column("CreatedAt")]
    public DateTime? CreatedAt { get; set; } = DateTime.Now;

    [DisplayFormat(DataFormatString = "{0:HH-mm-ss:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
    [DataType(DataType.DateTime)]
    [Column("UpdatedAt")]
    public DateTime? UpdatedAt { get; set; } = null;
    #endregion

    #region NavigationProperties
    public virtual SaleOrder SaleOrder { get; set; }

    #endregion
}

