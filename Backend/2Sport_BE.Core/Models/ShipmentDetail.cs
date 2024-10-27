using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace _2Sport_BE.Repository.Models;

[Table("ShipmentDetails")]
public class ShipmentDetail
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Column("UserId")]
    public int? UserId { get; set; }

    [Column("Address", TypeName = "nvarchar")]
    [MaxLength(255)]
    public string Address { get; set; }

    [Column("PhoneNumber", TypeName = "varchar")]
    [MaxLength(12)]
    public string PhoneNumber { get; set; }

    [Column("FullName", TypeName = "nvarchar")]
    [MaxLength(255)]
    public string FullName { get; set; }

    [Column("Email", TypeName = "nvarchar")]
    [MaxLength(255)]
    public string Email { get; set; }
    public virtual ICollection<SaleOrder> SaleOrders { get; set; }
    public virtual ICollection<RentalOrder> RentalOrders { get; set; }

    public virtual User User { get; set; }
}
