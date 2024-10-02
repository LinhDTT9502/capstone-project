using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2Sport_BE.Repository.Models;

[Table("Suppliers")]
public class Supplier
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Column("SupplierName", TypeName = "nvarchar")]
    [MaxLength(255)]
    public string SupplierName { get; set; }

    [Column("Location", TypeName = "nvarchar")]
    [MaxLength(255)]
    public string Location { get; set; }

    public virtual ICollection<ImportHistory> ImportHistories { get; set; }
}
