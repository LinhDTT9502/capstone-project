using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace _2Sport_BE.Repository.Models
{
    [Table("Blogs")]
    public class Blog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column("Title", TypeName = "nvarchar")]
        [MaxLength(255)]
        public string Title { get; set; }

        [Column("Content", TypeName = "nvarchar(MAX)")]
        public string Content { get; set; }

        [DisplayFormat(DataFormatString = "{0:HH-mm-ss:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.DateTime)]
        public DateTime? CreateAt { get; set; }

        [DisplayFormat(DataFormatString = "{0:HH-mm-ss:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.DateTime)]
        public DateTime? UpdatedAt { get; set; }

        [Column("CreatedStaffId")]
        public int CreatedStaffId { get; set; }

        [ForeignKey("CreatedStaffId")]
        public virtual Staff CreatedByStaff { get; set; }

        [Column("EditedByStaffId")]
        public int? EditedByStaffId { get; set; }

        [ForeignKey("EditedByStaffId")]
        public virtual Staff EditedByStaff { get; set; }

        public bool Status { get; set; }

    public virtual User User { get; set; }
}
