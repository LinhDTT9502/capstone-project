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

    [Column("ProductId")]
    public int? ProductId { get; set; }
    [Column("ProductName")]
    public string? ProductName { get; set; }
    [Column("ProductCode")]
    public string? ProductCode { get; set; }

    [Column("UnitPrice", TypeName = "decimal")]
    public decimal? UnitPrice { get; set; }
    [Column("Quantity")]
    public int? Quantity { get; set; }

    [Column("BranchId")]
    public int? BranchId { get; set; }
    [Column("BranchName")]
    public string? BranchName { get; set; }

    [NotMapped]
    public decimal? TotalAmount => UnitPrice * Quantity;
    [Column("CreatedAt")]
    public DateTime CreatedAt { get; set; }

    [Column("UpdatedAt")]
    public DateTime? UpdatedAt { get; set; }

    [Column("ImgAvatarPath", TypeName = "varchar")]
    [MaxLength(500)]
    public string? ImgAvatarPath { get; set; } //Them anh cho detail

    public virtual SaleOrder SaleOrder { get; set; }

    public virtual Product Product { get; set; }

    public OrderDetail()
    {
        CreatedAt = DateTime.UtcNow;
    }
}

