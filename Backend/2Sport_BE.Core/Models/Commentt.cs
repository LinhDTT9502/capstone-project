using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2Sport_BE.Repository.Models
{
    [Table("Comments")]
    public class Comment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int UserId { get; set; }
        public int? ParentCommentId { get; set; }

        [Column("ProductCode", TypeName = "nvarchar(25)")]
        public string ProductCode { get; set; }

        [Column("Content", TypeName = "nvarchar")]
        [MaxLength(200)]
        public string Content { get; set; }
        public User User { get; set; }
        public Comment ParentComment { get; set; }
        public DateTime CreatedAt { get; set; }


    }
}
