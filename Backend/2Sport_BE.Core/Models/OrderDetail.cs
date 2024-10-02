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

    [Column("ProductId")]
    public int? ProductId { get; set; }

    [Column("OrderId")]
    public int? OrderId { get; set; }

    [Column("Price", TypeName = "decimal")]
    public decimal? Price { get; set; }

    public int? Quantity { get; set; }

    public virtual Order Order { get; set; }

    public virtual Product Product { get; set; }
}

