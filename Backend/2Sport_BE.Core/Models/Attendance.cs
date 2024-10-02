using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2Sport_BE.Repository.Models
{
    [Table("Attendances")]
    public class Attendance
    {
        [Key]
        public int AttendanceId { get; set; }

        [Required]
        public int EmployeeId { get; set; }
        public virtual Employee Employee { get; set; }

        [Required]
        public int BranchId { get; set; }
        public virtual Branch Branch { get; set; }

        [Required]
        public DateTime AttendanceDate { get; set; }

        [Required]
        [Column("Status", TypeName = "nvarchar")]
        [MaxLength(50)]
        public string Status { get; set; }
        [Column("Reason", TypeName = "nvarchar")]
        [MaxLength(500)]
        public string? Reason { get; set; }

        [Required]
        public int CheckedBy { get; set; }
        public virtual Employee Manager { get; set; }
    }
}
