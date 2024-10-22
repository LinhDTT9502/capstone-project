using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2Sport_BE.Repository.Models;

[Table("Comments")]
public class Comment
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public int ProductId { get; set; }

    public int? ParentCommentId { get; set; }

    [Column("Content", TypeName = "nvarchar")]
    [MaxLength]
    public string Content { get; set; }

    public Product Product { get; set; }
    public Comment ParentComment { get; set; }
    public DateTime CreatedAt { get; set; }


}

