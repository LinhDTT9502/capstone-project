using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2Sport_BE.Repository.Models;

[Table("Likes")]
public class Like
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Column("UserId")]
    public int? UserId { get; set; }

    [Column("ProductId")]
    public int? ProductId { get; set; }

    public virtual Product Product { get; set; }
    public virtual User User { get; set; }
}
