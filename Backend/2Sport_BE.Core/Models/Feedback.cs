using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2Sport_BE.Repository.Models
{
    [Table("Feedbacks")]
    public class Feedback
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int? UserId { get; set; }

        [Column("UserName", TypeName = "nvarchar(50)")]
        public string? UserName { get; set; }

        [Column("FullName", TypeName = "nvarchar(50)")]
        public string FullName { get; set; }

        [Column("Email", TypeName = "nvarchar(50)")]
        public string Email { get; set; }

        [Column("PhoneNumber", TypeName = "nvarchar(12)")]
        public string? PhoneNumber { get; set; }

        [Column("Content", TypeName = "nvarchar(500)")]
        public string Content { get; set; }

        [Column("CreatedAt")]
        public DateTime CreatedAt { get; set; }
        public virtual User? User { get; set; }
    }
}
