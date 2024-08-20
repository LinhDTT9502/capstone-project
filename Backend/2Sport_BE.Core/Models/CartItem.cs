using System;
using System.Collections.Generic;

namespace _2Sport_BE.Repository.Models
{
    public partial class CartItem
    {
        public int Id { get; set; }
        public int? ProductId { get; set; }
        public int? Quantity { get; set; }
        public decimal? TotalPrice { get; set; }
        public int? CartId { get; set; }
        public bool? Status { get; set; }

        public virtual Cart Cart { get; set; }
        public virtual Product Product { get; set; }
    }
}
