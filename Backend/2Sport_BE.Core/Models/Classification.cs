using System;
using System.Collections.Generic;

namespace _2Sport_BE.Repository.Models
{
    public partial class Classification
    {
        public Classification()
        {
            Products = new HashSet<Product>();
        }

        public int Id { get; set; }
        public string ClassificationName { get; set; }
        public int? Quantity { get; set; }

        public virtual ICollection<Product> Products { get; set; }
    }
}
