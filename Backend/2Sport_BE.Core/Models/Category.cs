using System;
using System.Collections.Generic;

namespace _2Sport_BE.Repository.Models
{
    public partial class Category
    {
        public Category()
        {
            BrandCategories = new HashSet<BrandCategory>();
            Products = new HashSet<Product>();
        }

        public int Id { get; set; }
        public string CategoryName { get; set; }
        public int? Quantity { get; set; }
        public bool? Status { get; set; }

        public virtual ICollection<BrandCategory> BrandCategories { get; set; }
        public virtual ICollection<Product> Products { get; set; }
    }
}
