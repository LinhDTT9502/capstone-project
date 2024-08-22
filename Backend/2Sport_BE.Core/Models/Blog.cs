using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace _2Sport_BE.Repository.Models;

[Table("Blogs")]
public class Blog
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Column("BlogName", TypeName = "nvarchar")]
    [MaxLength(255)]
    public string BlogName { get; set; }

    [Column("Content", TypeName = "nvarchar")]
    [MaxLength]
    public string Content { get; set; }


    [DisplayFormat(DataFormatString = "{0:HH-mm-ss:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
    [DataType(DataType.DateTime)]
    public DateTime? CreateAt { get; set; }

    [Column("UserId")]
    public int UserId { get; set; }

    public virtual User User { get; set; }
    public virtual ICollection<Like> Likes { get; set; }

}
