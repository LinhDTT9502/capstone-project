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

    [Column("OrderId")]
    public int? OrderId { get; set; }
    [Column("ProductId")]
    public int? ProductId { get; set; }
    [Column("Price", TypeName = "decimal")]
    public decimal? Price { get; set; }
    [Column("Quantity")]
    public int? Quantity { get; set; }

    [Column("BranchId")]
    public int? BranchId { get; set; }
    [NotMapped]
    public decimal? TotalPrice => Price * Quantity;
    [Column("CreatedAt")]
    public DateTime CreatedAt { get; set; }

    [Column("UpdatedAt")]
    public DateTime? UpdatedAt { get; set; }

    public virtual Order Order { get; set; }

    public virtual Product Product { get; set; }

    public OrderDetail()
    {
        CreatedAt = DateTime.UtcNow;
    }
}

