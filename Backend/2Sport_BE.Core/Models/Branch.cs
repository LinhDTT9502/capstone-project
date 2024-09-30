using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace _2Sport_BE.Repository.Models;


[Table("Branches")]
public class Branch
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Column("BranchName", TypeName = "nvarchar")]
    [MaxLength(255)]
    public string BranchName { get; set; }

    [Column("Location", TypeName = "nvarchar")]
    [MaxLength(255)]
    public string Location { get; set; }

    [Column("Hotline", TypeName = "varchar")]
    [MaxLength(12)]
    public string Hotline { get; set; }

    [Column("ImgAvatarPath", TypeName = "varchar")]
    [MaxLength(500)]
    public string ImgAvatarPath { get; set; }

    [Column("ImgAvatarName", TypeName = "varchar")]
    [MaxLength(500)]
    public string ImgAvatarName { get; set; }

    public bool? Status { get; set; }

    public virtual ICollection<EmployeeDetail> EmployeeDetails { get; set; }

    public virtual ICollection<Warehouse> Warehouses { get; set; }
    public virtual ICollection<Attendance> Attendances { get; set; }
    public virtual ICollection<Order> Orders { get; set; }
}
