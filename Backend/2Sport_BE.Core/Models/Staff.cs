using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace _2Sport_BE.Repository.Models
{
    [Table("Staffs")]
    public class Staff
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int StaffId { get; set; }

        [Column("UserId")]
        public int? UserId { get; set; }

        [Column("BranchId")]
        public int? BranchId { get; set; }

        [Column("ManagerId")]
        public int? ManagerId { get; set; }
        public virtual Manager Manager { get; set; }

        [Column("StartDate")]
        [DisplayFormat(DataFormatString = "{0:HH-mm-ss:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.DateTime)]
        public DateTime StartDate { get; set; }

        [Column("EndDate")]
        [DisplayFormat(DataFormatString = "{0:HH-mm-ss:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.DateTime)]
        public DateTime? EndDate { get; set; }

        [Column("Position", TypeName = "nvarchar")]
        [MaxLength(100)]
        public string Position { get; set; }

        [Column("IsActive")]
        public bool IsActive { get; set; } = true;

        public virtual Branch Branch { get; set; }
        public virtual User User { get; set; }

        public virtual ICollection<ImportHistory> ImportHistories { get; set; }

    }
}
