using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace _2Sport_BE.Repository.Models;

[Table("Users")]
public class User
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Column("Username", TypeName = "varchar")]
    [MaxLength(255)]
    public string Username { get; set; }

    [Column("Password", TypeName = "nvarchar")]
    [MaxLength(100)]
    public string Password { get; set; }

    [Column("Email", TypeName = "varchar")]
    [MaxLength(100)]
    public string Email { get; set; }


    [Column("FullName", TypeName = "nvarchar")]
    [MaxLength(255)]
    public string FullName { get; set; }


    [Column("Gender", TypeName = "varchar")]
    [MaxLength(10)]
    public string Gender { get; set; }

    [Column("Phone", TypeName = "varchar")]
    [MaxLength(12)]
    public string Phone { get; set; }


    [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
    [DataType(DataType.Date)]
    public DateOnly? BirthDate { get; set; }

    [DisplayFormat(DataFormatString = "{0:HH-mm-ss:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
    [DataType(DataType.DateTime)]
    public DateTime? CreatedDate { get; set; }

    [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
    [DataType(DataType.DateTime)]
    public DateTime? LastUpdate { get; set; }

    public bool? IsActive { get; set; }

    [Column("RoleId")]
    public int? RoleId { get; set; }

    public virtual ICollection<Blog> Blogs { get; set; } = new List<Blog>();

    public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();

    public virtual ICollection<CustomerDetail> CustomerDetails { get; set; } = new List<CustomerDetail>();

    public virtual ICollection<EmployeeDetail> EmployeeDetailStaffs { get; set; } = new List<EmployeeDetail>();

    //public virtual ICollection<EmployeeDetail> EmployeeDetailSupervisors { get; set; } = new List<EmployeeDetail>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public virtual Role? Role { get; set; }

    public virtual ICollection<SalaryHistory> SalaryHistories { get; set; } = new List<SalaryHistory>();
    public virtual ICollection<ImportHistory> ImportHistories { get; set; }

    public virtual ICollection<Like> Likes { get; set; }
    public virtual ICollection<ShipmentDetail> ShipmentDetails { get; set; } = new List<ShipmentDetail>();
}
