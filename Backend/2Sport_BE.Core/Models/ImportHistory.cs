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

    [Column("EmployeeId")]
    public int EmployeeId { get; set; }

    [Column("ProductId")]
    public int ProductId { get; set; }

    [Column("Content", TypeName = "nvarchar")]
    [MaxLength(500)]
    public string Content { get; set; }

    [DisplayFormat(DataFormatString = "{0:HH-mm-ss:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
    [DataType(DataType.DateTime)]
    public DateTime? ImportDate { get; set; }
    public int Quantity { get; set; }

    [Column("SupplierId")]
    public int? SupplierId { get; set; }

    [Column("LotCode", TypeName = "varchar")]
    [MaxLength(50)]
    public string LotCode { get; set; }

    public virtual Product Product { get; set; }
    public virtual Employee Employee { get; set; }
    public virtual Supplier Supplier { get; set; }
}