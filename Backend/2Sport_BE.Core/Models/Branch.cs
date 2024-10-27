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

    public bool? Status { get; set; }
    public virtual ICollection<BrandBranch> BrandBranches { get; set; }

    public virtual ICollection<Staff> EmployeeDetails { get; set; }

    public virtual ICollection<Warehouse> Warehouses { get; set; }
    public virtual ICollection<SaleOrder> Orders { get; set; }
}
