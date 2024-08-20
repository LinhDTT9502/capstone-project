using System;
using System.Collections.Generic;

namespace _2Sport_BE.Repository.Models
{
    public partial class Sport
    {
        public Sport()
        {
            Blogs = new HashSet<Blog>();
            Products = new HashSet<Product>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Blog> Blogs { get; set; }
        public virtual ICollection<Product> Products { get; set; }
    }
}
