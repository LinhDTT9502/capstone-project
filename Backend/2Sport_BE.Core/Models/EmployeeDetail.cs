using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace _2Sport_BE.Repository.Models;

[Table("EmployeeDetails")]
public class EmployeeDetail
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Column("StaffId")]
    public int? StaffId { get; set; }

    [Column("BranchId")]
    public int? BranchId { get; set; }

    [DisplayFormat(DataFormatString = "{0:HH-mm-ss:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
    [DataType(DataType.DateTime)]
    public DateOnly? HireDate { get; set; }

    //[Column("SupervisorId")]
    //public int? SupervisorId { get; set; }

    [Column("Position", TypeName = "nvarchar")]
    [MaxLength(50)]
    public string Position { get; set; }

    public virtual Branch Branch { get; set; }

    public virtual User Staff { get; set; }

    //public virtual User Supervisor { get; set; }
}
