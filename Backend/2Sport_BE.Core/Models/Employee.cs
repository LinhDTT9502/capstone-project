/*using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2Sport_BE.Repository.Models
{
    [Table("Employees")]
    public class Employee
    {
        public Employee()
        {
            RefreshTokens = new HashSet<RefreshToken>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EmployeeId { get; set; }

        [Required]
        [Column("Username", TypeName = "varchar")]
        [MaxLength(100)]
        public string UserName { get; set; }

        [Required]
        [Column("HashPassword", TypeName = "nvarchar")]
        [MaxLength(500)]
        public string Password { get; set; }
        [Required]
        [Column("Email", TypeName = "varchar")]
        [MaxLength(100)]
        public string Email { get; set; }

        [Column("FullName", TypeName = "nvarchar")]
        [MaxLength(100)]
        public string? FullName { get; set; }

        [Column("Gender", TypeName = "nvarchar")]
        [MaxLength(50)]
        public string? Gender { get; set; }

        [Column("PhoneNumber", TypeName = "varchar")]
        [MaxLength(50)]
        public string? Phone { get; set; }

        [Column("Address", TypeName = "nvarchar")]
        [MaxLength(500)]
        public string? Address { get; set; }
        [Column("ImgAvatarPath", TypeName = "varchar")]
        [MaxLength(500)]
        public string? AvatarUrl { get; set; }

        [DisplayFormat(DataFormatString = "{0:HH-mm-ss:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.DateTime)]
        public DateTime? DOB { get; set; }
        [DisplayFormat(DataFormatString = "{0:HH-mm-ss:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.DateTime)]
        public DateTime CreatedDate { get; set; }
        [DisplayFormat(DataFormatString = "{0:HH-mm-ss:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.DateTime)]
        public DateTime? LastUpdate { get; set; }
        public bool IsActive { get; set; }

        public int? RoleId { get; set; }

        // Thêm column mới
        [MaxLength(100)]
        public string? PasswordResetToken { get; set; }

        public virtual Role? Role { get; set; }
        public virtual ICollection<RefreshToken> RefreshTokens { get; set; }
        public virtual ICollection<Attendance> Attendances { get; set; }
    }

}
*/