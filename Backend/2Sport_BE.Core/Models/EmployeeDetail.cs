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

    [Column("EmployeeId")]
    public int? EmployeeId { get; set; }

    [Column("BranchId")]
    public int? BranchId { get; set; }

    [DisplayFormat(DataFormatString = "{0:HH-mm-ss:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
    [DataType(DataType.DateTime)]
    public DateTime? HireDate { get; set; }

    [Column("Position", TypeName = "nvarchar")]
    [MaxLength(50)]
    public string Position { get; set; }

    [Column("SupervisorId")]
    public int? SupervisorId { get; set; }
    public virtual Branch Branch { get; set; }
    public virtual Employee Employee { get; set; }
    public virtual Employee Supervisor { get; set; }

}

