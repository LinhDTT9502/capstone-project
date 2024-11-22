using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2Sport_BE.Repository.Models;
[Table("ImportHistories")]
public class ImportHistory
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Column("ManagerId")]
    public int ManagerId { get; set; }

    [Column("Action", TypeName = "nvarchar")]
    [MaxLength(500)]
    public string Action { get; set; }

    [Column("ProductId")]
    public int ProductId { get; set; }

    [Column("ProductName", TypeName = "nvarchar")]
    [MaxLength(255)]
    public string ProductName { get; set; } = null!;

    [Column("ProductCode", TypeName = "nvarchar")]
    [MaxLength(255)]
    public string ProductCode { get; set; }

    [Column("Price")]
    public decimal? Price { get; set; }

    [Column("RentPrice")]
    public decimal? RentPrice { get; set; }

    [Column("Size", TypeName = "nvarchar")]
    [MaxLength(50)]
    public string? Size { get; set; }

    [Column("Color", TypeName = "nvarchar")]
    [MaxLength(255)]
    public string? Color { get; set; }

    [Column("Condition")]
    public int? Condition { get; set; }

    public int? Quantity { get; set; }

    [DisplayFormat(DataFormatString = "{0:HH-mm-ss:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
    [DataType(DataType.DateTime)]
    public DateTime? ImportDate { get; set; }

    public virtual Product Product { get; set; }
    public virtual Manager Manager { get; set; }
}