using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace _2Sport_BE.Repository.Models;

[Table("Customers")]
public class Customer
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Column("UserId")]
    public int? UserId { get; set; }
    public virtual User User { get; set; }

    [Column("LoyaltyPoints")]
    public int? LoyaltyPoints { get; set; }

    [Column("MembershipLevel", TypeName = "nvarchar")]
    [MaxLength(50)]
    public string MembershipLevel { get; set; }

    [DisplayFormat(DataFormatString = "{0:HH-mm-ss:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
    [DataType(DataType.DateTime)]
    public DateTime? JoinedAt { get; set; }
}
