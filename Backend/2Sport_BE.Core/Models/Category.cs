using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace _2Sport_BE.Repository.Models;

[Table("Categories")]
public class Category
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Column("CategoryName", TypeName = "nvarchar")]
    [MaxLength(500)]
    public string CategoryName { get; set; }

    [Column("Description", TypeName = "nvarchar")]
    [MaxLength(500)]
    public string Description { get; set; }

    [Column("CategoryImgPath", TypeName = "nvarchar")]
    [MaxLength(500)]
    public string? CategoryImgPath { get; set; }

    public int? Quantity { get; set; }
    public bool? Status { get; set; }
    public int? SportId { get; set; }
    public Sport Sport { get; set; }

    public virtual ICollection<BrandCategory> BrandCategories { get; set; }

    public virtual ICollection<Product> Products { get; set; }
}
