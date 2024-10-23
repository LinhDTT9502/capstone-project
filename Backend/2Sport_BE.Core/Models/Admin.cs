using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2Sport_BE.Repository.Models
{
    [Table("Admins")]
    public class Admin
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AdminId { get; set; }
        [Required]
        [Column("Username", TypeName = "varchar")]
        [MaxLength(100)]
        public string UserName { get; set; }

        [Required]
        [Column("Username", TypeName = "varchar")]
        [MaxLength(100)]
        public string HashPassword { get; set; }

        public string Description { get; set; }
        
        public int  RoleId { get; set; }

        [ForeignKey(nameof(RoleId))]
        public virtual Role Role { get; set; }
    }
}
