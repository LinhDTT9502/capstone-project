using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace _2Sport_BE.Repository.Models;

[Table("ImagesVideos")]
public class ImagesVideo
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Column("ImageUrl", TypeName = "varchar")]
    [MaxLength(500)]
    public string? ImageUrl { get; set; }

    [Column("VideoUrl", TypeName = "varchar")]
    [MaxLength(500)]
    public string? VideoUrl { get; set; }

    [DisplayFormat(DataFormatString = "{0:HH-mm-ss:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
    [DataType(DataType.DateTime)]
    public DateTime? CreateAt { get; set; }

    [Column("ProductId")]
    public int? ProductId { get; set; }

    public virtual Product Product { get; set; }
}
