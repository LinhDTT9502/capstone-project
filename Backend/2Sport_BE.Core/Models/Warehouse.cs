using System;
using System.Collections.Generic;

namespace _2Sport_BE.Repository.Models
{
    public partial class Warehouse
    {
        public int Id { get; set; }
        public int? ProductId { get; set; }
        public int? Quantity { get; set; }

        public virtual Product? Product { get; set; }
    }
}
