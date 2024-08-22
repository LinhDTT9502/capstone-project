using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace _2Sport_BE.Repository.Models;

[Table("BrandCategories")]
public class BrandCategory
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Column("CategoryId")]
    public int? CategoryId { get; set; }

    [Column("BrandId")]
    public int? BrandId { get; set; }

    public virtual Brand Brand { get; set; }

    public virtual Category Category { get; set; }
}
