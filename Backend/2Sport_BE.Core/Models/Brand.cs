using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace _2Sport_BE.Repository.Models;

[Table("Brands")]
public class Brand
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Column("BrandName", TypeName = "varchar")]
    [MaxLength(50)]
    public string BrandName { get; set; }

    [Column("Logo", TypeName = "varchar")]
    [MaxLength(500)]
    public string Logo { get; set; }

    public int? Quantity { get; set; }

    public bool? Status { get; set; }

    public virtual ICollection<Product> Products { get; set; }

}
