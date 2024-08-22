using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace _2Sport_BE.Repository.Models;

[Table("Warehouses")]
public class Warehouse
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Column("BranchId")]
    public int? BranchId { get; set; }

    [Column("ProductId")]
    public int? ProductId { get; set; }

    public int? Quantity { get; set; }

    public virtual Product Product { get; set; }

    public virtual Branch Branch { get; set; }

}

