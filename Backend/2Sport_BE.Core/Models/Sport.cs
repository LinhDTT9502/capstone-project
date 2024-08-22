using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2Sport_BE.Repository.Models;

[Table("Sports")]
public class Sport
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Column("Name", TypeName = "nvarchar")]
    [MaxLength(50)]
    public string Name { get; set; }

    public virtual ICollection<Product> Products { get; set; }
}
