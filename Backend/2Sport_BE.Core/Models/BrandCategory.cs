using System;
using System.Collections.Generic;

namespace _2Sport_BE.Repository.Models
{
    public partial class BrandCategory
    {
        public int Id { get; set; }
        public int? BrandId { get; set; }
        public int? CategoryId { get; set; }

        public virtual Brand Brand { get; set; }
        public virtual Category Category { get; set; }
    }
}
