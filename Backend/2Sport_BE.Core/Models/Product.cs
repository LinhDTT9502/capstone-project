using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace _2Sport_BE.Repository.Models;
[Table("Products")]
public class Product
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Column("ProductName", TypeName = "nvarchar")]
    [MaxLength(255)]
    public string ProductName { get; set; } = null!;

    [Column("ListedPrice", TypeName = "decimal")]
    public decimal? ListedPrice { get; set; }

    [Column("Price", TypeName = "decimal")]
    public decimal? Price { get; set; }

    [Column("Size", TypeName = "nvarchar")]
    [MaxLength(50)]
    public string? Size { get; set; }

    [Column("Description", TypeName = "text")]
    public string? Description { get; set; }

    public bool? Status { get; set; }

    [Column("Color", TypeName = "nvarchar")]
    [MaxLength(255)]
    public string? Color { get; set; }

    [Column("Condition")]
    public int? Condition { get; set; }

    [Column("Offers", TypeName = "nvarchar")]
    [MaxLength]
    public string? Offers { get; set; }

    //public int? ReviewId { get; set; }
    [Column("CategoryId")]
    public int CategoryId { get; set; }

    [Column("BrandId")]
    public int BrandId { get; set; }

    [Column("ProductCode", TypeName = "nvarchar")]
    [MaxLength(255)]
    public string ProductCode { get; set; }

    [Column("SportId")]
    public int SportId { get; set; }

    [DisplayFormat(DataFormatString = "{0:HH-mm-ss:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
    [DataType(DataType.DateTime)]
    public DateTime? CreateAt { get; set; }

    [Column("RentPrice", TypeName = "decimal")]
    public decimal? RentPrice { get; set; }
    public bool IsRent { get; set; }
    [Column("ImgAvatarPath", TypeName = "varchar")]
    [MaxLength(500)]
    public string ImgAvatarPath { get; set; }

    [Column("ImgAvatarName", TypeName = "varchar")]
    [MaxLength(500)]
    public string ImgAvatarName { get; set; }

    public virtual Sport? Sport { get; set; }
    public virtual Brand? Brand { get; set; }

    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

    public virtual Category? Category { get; set; }

    public virtual ICollection<ImagesVideo> ImagesVideos { get; set; } = new List<ImagesVideo>();

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
    public virtual ICollection<Like> Likes { get; set; }
    public virtual ICollection<ImportHistory> ImportHistories { get; set; }
    public virtual ICollection<Warehouse> Warehouses { get; set; }
}
