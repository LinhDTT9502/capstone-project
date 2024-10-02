using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace _2Sport_BE.Repository.Models;

[Table("Reviews")]
public class Review
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Column("Star", TypeName = "decimal")]
    public decimal? Star { get; set; }

    [Column("ReviewContent", TypeName = "nvarchar")]
    [MaxLength]
    public string ReviewContent { get; set; }

    public bool? Status { get; set; }

    [Column("UserId")]
    public int? UserId { get; set; }

    [Column("ProductId")]
    public int? ProductId { get; set; }

    public virtual Product Product { get; set; }

    public virtual User User { get; set; }
}