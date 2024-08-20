using System;
using System.Collections.Generic;

namespace _2Sport_BE.Repository.Models
{
    public partial class PaymentMethod
    {
        public PaymentMethod()
        {
            Orders = new HashSet<Order>();
        }

        public int Id { get; set; }
        public string PaymentMethodName { get; set; }

        public virtual ICollection<Order> Orders { get; set; }
    }
}
