using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace _2Sport_BE.Repository.Models;


[Table("PaymentMethods")]
public class PaymentMethod
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Column("PaymentMethodName", TypeName = "nvarchar")]
    [MaxLength(50)]
    public string PaymentMethodName { get; set; }

    public virtual ICollection<Order> Orders { get; set; }
}